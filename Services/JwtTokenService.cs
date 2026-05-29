using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using com.zameen.Models;
using com.zameen.Models.Dto.Response;
using com.zameen.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace com.zameen.Services;

public class JwtTokenService(
    UserManager<ApplicationUser> userManager,
    IRefreshTokenRepository repo,
    IConfiguration configuration
)
{
    public async Task<string> GenerateAccessToken(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new("fullName", user.FullName ?? ""),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(configuration["Jwt:AccessTokenExpirationMinutes"])
            ),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshToken(ApplicationUser user, string? ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(
                Convert.ToDouble(configuration["Jwt:RefreshTokenExpirationDays"])
            ),
            UserId = user.Id,
            CreatedByIp = ipAddress,
            CreatedAt = DateTime.UtcNow,
        };
        await repo.AddAsync(refreshToken);
        return refreshToken.Token;
    }

    public async Task<TokenResponse> GenerateTokensAsync(
        ApplicationUser user,
        string? ipAddress = null
    )
    {
        var accessToken = await GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshToken(user, ipAddress);
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(configuration["Jwt:AccessTokenExpirationMinutes"])
            ),
        };
    }
}
