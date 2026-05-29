using System.Security.Claims;
using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration,
    ILogger<AuthController> logger
) : ControllerBase
{
    private readonly string _frontendBaseUrl =
        configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await authService.LoginAsync(request, ip);
        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await authService.RefreshTokenAsync(request, ip);
        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>
    /// What Revoke Does
    /// When a user logs out (especially from a mobile app or a public computer), you want to invalidate their refresh token so it can never be used again.
    /// Without revoking, the refresh token remains valid until it expires – meaning an attacker who stole it could still get new access tokens.
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Revoke([FromBody] RevokeRequest revokeRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Fail("User not authenticated."));

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        await authService.RevokeRefreshTokenAsync(revokeRequest, userId, ip);
        return Ok(ApiResponse.Ok("Token revoked successfully."));
    }

    // ─── Google OAuth 2.0 (challenge flow) ──────────────────────

    /// <summary>
    /// Starts the Google login challenge. The user will be redirected to Google.
    /// </summary>
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth");
        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            "Google",
            redirectUrl
        );
        return Challenge(properties, "Google");
    }

    /// <summary>
    /// Callback endpoint that Google redirects to after authentication.
    /// Processes the external login info and redirects the user to the frontend
    /// with the JWT tokens in the query string.
    /// </summary>
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback(string? remoteError = null)
    {
        if (remoteError != null)
        {
            logger.LogWarning("Google login failed with remote error: {Error}", remoteError);
            return Redirect($"{_frontendBaseUrl}/auth-callback?error=google_login_failed");
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            logger.LogWarning("External login info not available.");
            return Redirect($"{_frontendBaseUrl}/auth-callback?error=no_login_info");
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await authService.GoogleLoginCallbackAsync(info, ip);

        if (!result.Success)
        {
            logger.LogWarning("Google login processing failed: {Message}", result.Message);
            return Redirect(
                $"{_frontendBaseUrl}/auth-callback?error={Uri.EscapeDataString(result.Message)}"
            );
        }

        var tokens = result.Data;
        if (tokens == null)
        {
            logger.LogWarning("Tokens data is null despite successful login.");
            return Redirect($"{_frontendBaseUrl}/auth-callback?error=token_generation_failed");
        }

        // Build redirect URL with tokens
        var redirectUrl =
            $"{_frontendBaseUrl}/auth-callback"
            + $"?accessToken={Uri.EscapeDataString(tokens.AccessToken)}"
            + $"&refreshToken={Uri.EscapeDataString(tokens.RefreshToken)}"
            + $"&expiresAt={Uri.EscapeDataString(tokens.ExpiresAt.ToString("o"))}";

        logger.LogInformation(
            "Redirecting to frontend with tokens for user {Email}",
            info.Principal.FindFirstValue(ClaimTypes.Email)
        );

        return Redirect(redirectUrl);
    }
}
