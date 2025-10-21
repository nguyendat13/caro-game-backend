using backend.DTOs.User;

namespace backend.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> CreateAsync(UserRegisterDTO dto);
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO> GetByIdAsync(int userId);
        Task<UserResponseDTO> UpdateAsync(UserUpdateDTO dto);
        Task<bool> DeleteAsync(int userId);
    }
}
