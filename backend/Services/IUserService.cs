using backend.DTOs.User;

namespace backend.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> CreateAsync(UserRegisterDTO dto);
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO> GetByIdAsync(int userId);

        // Update người dùng (có RoleId) chỉ admin/superadmin mới gọi
        Task<UserResponseDTO> UpdateRoleAsync(UpdateUserRoleDTO dto, int currentUserRoleId, int currentUserId);

        // Xóa người dùng (chỉ admin/superadmin)
        Task<bool> DeleteAsync(int userId, int currentUserRoleId, int currentUserId);
        Task<bool> ConfirmDeleteAsync(int userId, string otpCode);
        Task<bool> ChangePasswordAsync(ChangePasswordDTO dto);

        // Update profile cá nhân bình thường (không cho RoleId)
        Task<UserResponseDTO> UpdateProfileAsync(UpdateProfileDTO dto);
        Task RequestProfileUpdateAsync(UpdateProfileRequestDTO dto);
        Task<UserResponseDTO> ConfirmProfileUpdateAsync(ConfirmUpdateProfileDTO dto);
        Task<UserStatsDTO> GetUserStatsAsync(int userId);
    }
}
