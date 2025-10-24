using backend.DTOs.Auth;
using backend.DTOs.User;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginDTO dto);
        Task<LoginResponseDTO> RegisterAsync(UserRegisterDTO dto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDTO dto);

    }
}
