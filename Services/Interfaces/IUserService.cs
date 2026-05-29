using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;

namespace com.zameen.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<IEnumerable<UserResponse>>> GetAllUsersAsync();
    Task<ApiResponse<UserResponse>> GetUserByIdAsync(string userId);
    Task<ApiResponse> UpdateUserRolesAsync(UpdateUserRolesDto dto);
    Task<ApiResponse> ChangeUserAccountStatusAsync(ChangeUserStatusDto dto);
    Task<ApiResponse> DeleteUserAsync(string userId);
    Task<ApiResponse<UserProfileResponse>> GetProfileAsync(string userId);
    Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
