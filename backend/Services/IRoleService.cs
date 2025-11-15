using backend.DTOs.Role;
using backend.Models;

namespace backend.Services
{
    public interface IRoleService
    {
        Task<RoleResponseDTO> CreateAsync(RoleCreateDTO dto);
        Task<RoleResponseDTO> UpdateAsync(RoleUpdateDTO dto);
        Task<bool> DeleteAsync(int roleId);
        Task<IEnumerable<RoleResponseDTO>> GetAllAsync();
        Task<RoleResponseDTO> GetByIdAsync(int roleId);
    }
}
