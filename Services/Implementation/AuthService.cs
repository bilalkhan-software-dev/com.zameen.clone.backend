using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using com.zameen.Data;
using com.zameen.Exceptions;
using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Repositories.Interfaces;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace com.zameen.Services.Implementation;

public class AuthService(
    UserManager<ApplicationUser> _userManager,
    SignInManager<ApplicationUser> _signInManager,
    IRefreshTokenRepository _refreshTokenRepo,
    IConfiguration _configuration,
    ILogger<AuthService> _logger,
    JwtTokenService jwtService,
    ApplicationDbContext _dbContext
) : IAuthService
{
    public async Task<ApiResponse<TokenResponse>> RegisterAsync(RegisterRequest dto)
    {
        _logger.LogInformation("Registration attempt for {Email}", dto.Email);

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new ResourceAlreadyExistsException("Email already registered.");

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Registration failed for {Email}: {Errors}",
                    dto.Email,
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
                return ApiResponse<TokenResponse>.Fail(
                    "User creation failed.",
                    result.Errors.Select(e => e.Description)
                );
            }

            await _userManager.AddToRoleAsync(user, "User");

            if (dto.IsAgency)
            {
                await _userManager.AddToRoleAsync(user, "Agent");

                var agent = new Agent
                {
                    UserId = user.Id.ToString(),
                    AgencyName = dto.AgencyName ?? "Unknown Agency",
                    Bio = dto.Bio,
                    ProfilePic = dto.ProfilePic ?? "",
                };

                // Directly add to DbContext (no SaveChangesAsync here)
                await _dbContext.Set<Agent>().AddAsync(agent);
            }

            // At this point, both user and agent are tracked but not committed.
            // SaveChangesAsync will persist both inside the transaction.
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            var tokens = await jwtService.GenerateTokensAsync(user);
            _logger.LogInformation("User {Email} registered successfully", dto.Email);
            return ApiResponse<TokenResponse>.Ok(tokens, "Registration successful.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest dto, string? ipAddress)
    {
        _logger.LogInformation("Login attempt for {Email}", dto.Email);
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new UnauthorizedException("No account registered with this email.");

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            lockoutOnFailure: false
        );
        if (!signInResult.Succeeded)
        {
            _logger.LogWarning("Invalid password for {Email}", dto.Email);
            throw new UnauthorizedException("Invalid Credentials.");
        }

        var tokens = await jwtService.GenerateTokensAsync(user, ipAddress);
        _logger.LogInformation("User {Email} logged in successfully", dto.Email);
        return ApiResponse<TokenResponse>.Ok(tokens, "Login successful.");
    }

    public async Task<ApiResponse<TokenResponse>> RefreshTokenAsync(
        RefreshTokenDto dto,
        string? ipAddress
    )
    {
        _logger.LogInformation("Refresh token request received");
        var principal = GetPrincipalFromExpiredToken(dto.AccessToken);
        if (principal == null)
            throw new UnauthorizedException("Invalid access token.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Invalid token claims.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new ResourceNotFoundException("User not found.");

        var storedRefreshToken = await _refreshTokenRepo.GetByTokenAsync(dto.RefreshToken);
        if (
            storedRefreshToken == null
            || storedRefreshToken.UserId != Guid.Parse(userId)
            || !storedRefreshToken.IsActive
        )
            throw new UnauthorizedException("Invalid or expired refresh token.");

        // Revoke old refresh token
        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.RevokedByIp = ipAddress;
        storedRefreshToken.ReplacedByToken = null; // will be replaced below
        _refreshTokenRepo.Update(storedRefreshToken);

        var newTokens = await jwtService.GenerateTokensAsync(user, ipAddress);
        storedRefreshToken.ReplacedByToken = newTokens.RefreshToken;
        await _refreshTokenRepo.SaveChangesAsync();

        _logger.LogInformation("Refresh token rotated for user {UserId}", userId);
        return ApiResponse<TokenResponse>.Ok(newTokens, "Token refreshed successfully.");
    }

    public async Task RevokeRefreshTokenAsync(
        RevokeRequest request,
        string userId,
        string? ipAddress
    )
    {
        var storedToken = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken);

        if (storedToken != null && storedToken.IsActive)
        {
            storedToken.IsRevoked = true;
            storedToken.RevokedByIp = ipAddress;
            _refreshTokenRepo.Update(storedToken);
            await _refreshTokenRepo.SaveChangesAsync();
            _logger.LogInformation("Refresh token revoked for user {UserId}", userId);
        }
    }

    public async Task<ApiResponse<TokenResponse>> GoogleLoginCallbackAsync(
        ExternalLoginInfo info,
        string? ipAddress
    )
    {
        if (info == null)
            return ApiResponse<TokenResponse>.Fail("External login info not available.");

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return ApiResponse<TokenResponse>.Fail("Email not received from Google.");

        _logger.LogInformation("Google callback for email: {Email}", email);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return ApiResponse<TokenResponse>.Fail(
                    "User creation failed.",
                    createResult.Errors.Select(e => e.Description)
                );
            await _userManager.AddToRoleAsync(user, "User");
            _logger.LogInformation("New user created via Google: {Email}", email);
        }

        // Add login if not already linked
        var existingLogins = await _userManager.GetLoginsAsync(user);
        if (
            !existingLogins.Any(l =>
                l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey
            )
        )
        {
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
                return ApiResponse<TokenResponse>.Fail("Failed to add external login.");
        }

        var tokens = await jwtService.GenerateTokensAsync(user, ipAddress);
        return ApiResponse<TokenResponse>.Ok(tokens, "Google login successful.");
    }

    // ─── Private helpers ────────────────────────
    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // we don’t care about the token’s expiration
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            ),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out SecurityToken securityToken
        );
        if (
            securityToken is not JwtSecurityToken jwtToken
            || !jwtToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            )
        )
            return null;

        return principal;
    }
}
