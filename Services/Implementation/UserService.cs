using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Services.Implementation;

public class UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
    : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<ApiResponse<IEnumerable<UserResponse>>> GetAllUsersAsync()
    {
        _logger.LogInformation("Admin fetching all users");
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(
                new UserResponse
                {
                    Id = user.Id.ToString(),
                    Email = user.Email!,
                    FullName = user.FullName ?? "",
                    AccountStatus = user.AccountStatus,
                    Roles = roles,
                }
            );
        }

        return ApiResponse<IEnumerable<UserResponse>>.Ok(userDtos);
    }

    public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return ApiResponse<UserResponse>.Fail("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = new UserResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email!,
            FullName = user.FullName ?? "",
            AccountStatus = user.AccountStatus,
            Roles = roles,
        };
        return ApiResponse<UserResponse>.Ok(userDto);
    }

    public async Task<ApiResponse> UpdateUserRolesAsync(UpdateUserRolesDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return ApiResponse.Fail("User not found.");

        _logger.LogInformation(
            "Updating roles for user {UserId} to {Roles}",
            dto.UserId,
            string.Join(",", dto.Roles)
        );

        var existingRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        if (!removeResult.Succeeded)
            return ApiResponse.Fail(
                "Failed to remove existing roles.",
                removeResult.Errors.Select(e => e.Description)
            );

        var addResult = await _userManager.AddToRolesAsync(user, dto.Roles);
        if (!addResult.Succeeded)
            return ApiResponse.Fail(
                "Failed to add new roles.",
                addResult.Errors.Select(e => e.Description)
            );

        return ApiResponse.Ok("User roles updated successfully.");
    }

    public async Task<ApiResponse> ChangeUserAccountStatusAsync(ChangeUserStatusDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return ApiResponse.Fail("User not found.");

        _logger.LogInformation(
            "Changing account status of user {UserId} to {Status}",
            dto.UserId,
            dto.NewStatus
        );
        user.AccountStatus = dto.NewStatus;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return ApiResponse.Fail(
                "Failed to update user status.",
                result.Errors.Select(e => e.Description)
            );

        return ApiResponse.Ok("User account status updated.");
    }

    public async Task<ApiResponse> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return ApiResponse.Fail("User not found.");

        _logger.LogWarning("Deleting user {UserId}", userId);
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return ApiResponse.Fail(
                "Failed to delete user.",
                result.Errors.Select(e => e.Description)
            );

        return ApiResponse.Ok("User deleted successfully.");
    }

    public async Task<ApiResponse<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return ApiResponse<UserProfileResponse>.Fail("User not found.");

        var profile = new UserProfileResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email!,
            FullName = user.FullName ?? "",
            PhoneNumber = user.PhoneNumber,
            UserName = user.UserName!,
        };
        return ApiResponse<UserProfileResponse>.Ok(profile);
    }

    public async Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return ApiResponse.Fail("User not found.");

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword
        );
        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Password change failed for {UserId}: {Errors}",
                userId,
                result.Errors
            );
            return ApiResponse.Fail(
                "Password change failed.",
                result.Errors.Select(e => e.Description)
            );
        }

        _logger.LogInformation("Password changed for user {UserId}", userId);
        return ApiResponse.Ok("Password changed successfully.");
    }
}
