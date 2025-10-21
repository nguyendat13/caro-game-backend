using backend.DTOs.Auth;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginDTO dto);

    }
}
