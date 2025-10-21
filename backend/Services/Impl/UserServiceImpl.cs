using backend.DTOs.User;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class UserServiceImpl : IUserService
    {
        private readonly CaroDbContext _context;

        public UserServiceImpl(CaroDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // ✅ Đăng ký người dùng
        public async Task<UserResponseDTO> CreateAsync(UserRegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new Exception("Tên đăng nhập không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new Exception("Email không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new Exception("Mật khẩu không được để trống.");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email đã tồn tại, vui lòng sử dụng email khác.");

            var user = new User
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };
        }

        // ✅ Xóa người dùng
        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng cần xóa.");

            bool hasOngoingGame = await _context.Games
                .AnyAsync(g => (g.PlayerXId == userId || g.PlayerOId == userId) && g.Status == GameStatus.Ongoing);

            if (hasOngoingGame)
                throw new Exception("Người dùng hiện đang trong trận đấu, không thể xóa.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Lấy tất cả người dùng
        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            var users = await _context.Users
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone
                })
                .ToListAsync();

            if (!users.Any())
                throw new Exception("Không có người dùng nào trong hệ thống.");

            return users;
        }

        // ✅ Lấy người dùng theo ID
        public async Task<UserResponseDTO> GetByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng.");

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };
        }

        // ✅ Cập nhật thông tin người dùng
        public async Task<UserResponseDTO> UpdateAsync(UserUpdateDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng để cập nhật.");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.UserId != dto.UserId))
                throw new Exception("Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserId != dto.UserId))
                throw new Exception("Email đã tồn tại, vui lòng chọn email khác.");

            user.Username = dto.Username;
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = HashPassword(dto.Password);

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };
        }
    }
}
