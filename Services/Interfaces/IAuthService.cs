using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using Microsoft.AspNetCore.Identity;

namespace com.zameen.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<TokenResponse>> RegisterAsync(RegisterRequest dto);
    Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest dto, string? ipAddress);
    Task<ApiResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenDto dto, string? ipAddress);
    Task RevokeRefreshTokenAsync(RevokeRequest token, string userId, string? ipAddress);
    Task<ApiResponse<TokenResponse>> GoogleLoginCallbackAsync(
        ExternalLoginInfo info,
        string? ipAddress
    );
}
