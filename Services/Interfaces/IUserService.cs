using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;

namespace com.zameen.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<PagedResult<UserResponse>>> GetAllUsersAsync(
        int page,
        int size,
        AccountStatus? accountStatus
    );
    Task<ApiResponse<UserResponse>> GetUserByIdAsync(string userId);
    Task<ApiResponse> UpdateUserRolesAsync(UpdateUserRolesDto dto);
    Task<ApiResponse> ChangeUserAccountStatusAsync(ChangeUserStatusDto dto);
    Task<ApiResponse> DeleteUserAsync(string userId);
    Task<ApiResponse<UserResponse>> GetProfileAsync(string userId);
    Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
