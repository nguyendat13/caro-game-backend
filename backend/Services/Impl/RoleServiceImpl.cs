using backend.DTOs.Role;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class RoleServiceImpl : IRoleService
    {
        private readonly CaroDbContext _context;

        public RoleServiceImpl(CaroDbContext context)
        {
            _context = context;
        }

        public async Task<RoleResponseDTO> CreateAsync(RoleCreateDTO dto)
        {
            var role = new Role { RoleName = dto.RoleName };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return new RoleResponseDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }

        public async Task<RoleResponseDTO> UpdateAsync(RoleUpdateDTO dto)
        {
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null) throw new Exception("Không tìm thấy Role.");
            role.RoleName = dto.RoleName;
            await _context.SaveChangesAsync();

            return new RoleResponseDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }

        public async Task<bool> DeleteAsync(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) throw new Exception("Không tìm thấy Role.");
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoleResponseDTO>> GetAllAsync()
        {
            return await _context.Roles
                .Select(r => new RoleResponseDTO
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToListAsync();
        }

        public async Task<RoleResponseDTO> GetByIdAsync(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) throw new Exception("Không tìm thấy Role.");
            return new RoleResponseDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }
    }
}
