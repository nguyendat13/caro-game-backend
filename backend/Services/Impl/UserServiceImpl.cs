using backend.DTOs.Game;
using backend.DTOs.User;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserServiceImpl : IUserService
    {
        private readonly CaroDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly EmailService _emailService;

        public UserServiceImpl(CaroDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        private string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        private bool VerifyPassword(User user, string password, string storedHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, storedHash, password);
            return result == PasswordVerificationResult.Success;
        }

        // ✅ Đăng ký người dùng (mặc định RoleId = 3 → user)
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
                RoleId = dto.RoleId > 0 ? dto.RoleId : 3, // 1=superadmin, 2=admin, 3=user
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user.PasswordHash = HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId
            };
        }

        // ✅ Xóa người dùng (chỉ admin/superadmin mới được xóa)
        public async Task<bool> DeleteAsync(int userId, int currentUserRoleId)
        {
            if (currentUserRoleId != 1 && currentUserRoleId != 2)
                throw new Exception("Bạn không có quyền xóa người dùng.");

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
            return await _context.Users
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    RoleId = u.RoleId
                })
                .ToListAsync();
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
                Phone = user.Phone,
                RoleId = user.RoleId
            };
        }

        // ✅ Cập nhật thông tin người dùng (có thể thay RoleId nếu là admin/superadmin)
        public async Task<UserResponseDTO> UpdateAsync(UserUpdateDTO dto, int currentUserRoleId)
        {
            if (currentUserRoleId != 1 && currentUserRoleId != 2)
                throw new Exception("Bạn không có quyền cập nhật Role.");

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
                user.PasswordHash = HashPassword(user, dto.Password);

            // Chỉ admin/superadmin mới có thể cập nhật Role
            if (currentUserRoleId == 1 || currentUserRoleId == 2)
                user.RoleId = dto.RoleId;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId
            };
        }

        // ✅ Update profile cá nhân (không cho thay Role)
        public async Task<UserResponseDTO> UpdateProfileAsync(UpdateProfileDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng.");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.UserId != dto.UserId))
                throw new Exception("Username đã được sử dụng bởi người dùng khác.");
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserId != dto.UserId))
                throw new Exception("Email đã được sử dụng bởi người dùng khác.");

            user.Username = dto.Username;
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId
            };
        }

        // ✅ Đổi mật khẩu
        public async Task<bool> ChangePasswordAsync(ChangePasswordDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng.");

            if (!VerifyPassword(user, dto.CurrentPassword, user.PasswordHash))
                throw new Exception("Mật khẩu hiện tại không đúng.");
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                throw new Exception("Mật khẩu mới không được để trống.");

            user.PasswordHash = HashPassword(user, dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task RequestProfileUpdateAsync(UpdateProfileRequestDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng.");

            bool isChangingUsername = dto.NewUsername != null && dto.NewUsername != user.Username;
            bool isChangingEmail = dto.NewEmail != null && dto.NewEmail != user.Email;

            // ❗ Nếu không thay đổi gì thì báo lỗi
            if (!isChangingUsername && !isChangingEmail)
                throw new Exception("Bạn không thay đổi thông tin nào.");

            // Check username trùng
            if (isChangingUsername)
            {
                if (await _context.Users.AnyAsync(u => u.Username == dto.NewUsername))
                    throw new Exception("Username đã tồn tại.");
            }

            // Check email trùng
            if (isChangingEmail)
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.NewEmail))
                    throw new Exception("Email đã tồn tại.");
            }

            // Tạo OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            var otpRecord = new ProfileUpdateOtp
            {
                UserId = user.UserId,
                NewUsername = isChangingUsername ? dto.NewUsername : null,
                NewEmail = isChangingEmail ? dto.NewEmail : null,
                OtpCode = otpCode,
                Expiration = DateTime.UtcNow.AddMinutes(5),
            };

            _context.ProfileUpdateOtps.Add(otpRecord);
            await _context.SaveChangesAsync();

            // Gửi về email cũ trước khi thay đổi
            _emailService.SendEmail(user.Email, "Xác nhận cập nhật hồ sơ - Caro Game",
    $@"
    <div style='font-family: Arial, sans-serif; color: #333; line-height: 1.5;'>
        <h2 style='color: #8c6746;'>Caro Game</h2>
        <p>Xin chào <strong>{user.FullName}</strong>,</p>
        <p>Bạn vừa yêu cầu cập nhật thông tin hồ sơ của mình trên <strong>Caro Game</strong>.</p>
        <p>Mã xác nhận của bạn là:</p>
        <p style='font-size: 24px; font-weight: bold; color: #2d6cdf;'>{otpCode}</p>
        <p>Mã này có hiệu lực trong <strong>5 phút</strong>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
        <hr style='border:none; border-top:1px solid #ccc;'/>
        <p style='font-size:12px; color:#777;'>Nếu bạn không yêu cầu cập nhật, vui lòng bỏ qua email này.</p>
    </div>
    "
);

        }


        public async Task<UserResponseDTO> ConfirmProfileUpdateAsync(ConfirmUpdateProfileDTO dto)
        {
            var otpRecord = await _context.ProfileUpdateOtps
     .Where(o => o.UserId == dto.UserId)
     .Where(o => (o.NewUsername == null && dto.NewUsername == null) || o.NewUsername == dto.NewUsername)
     .Where(o => (o.NewEmail == null && dto.NewEmail == null) || o.NewEmail == dto.NewEmail)
     .OrderByDescending(o => o.Expiration)
     .FirstOrDefaultAsync();


            if (otpRecord == null)
                throw new Exception("Yêu cầu cập nhật không tồn tại.");

            if (otpRecord.Expiration < DateTime.UtcNow)
                throw new Exception("OTP đã hết hạn.");

            if (otpRecord.OtpCode != dto.OtpCode)
                throw new Exception("OTP không đúng.");

            // Cập nhật vào User
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng.");

            if (!string.IsNullOrWhiteSpace(dto.NewUsername))
                user.Username = dto.NewUsername;

            if (!string.IsNullOrWhiteSpace(dto.NewEmail))
                user.Email = dto.NewEmail;

            user.UpdatedAt = DateTime.UtcNow;

            // Xóa bản ghi OTP đã dùng
            _context.ProfileUpdateOtps.Remove(otpRecord);

            await _context.SaveChangesAsync();

            return new UserResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId
            };
        }


        public async Task<UserStatsDTO> GetUserStatsAsync(int userId)
        {
            // Lấy tất cả các game user tham gia
            var allGames = await _context.Games
                .Where(g => g.PlayerXId == userId || g.PlayerOId == userId)
                .Where(g => g.Status == GameStatus.Finished)
                .ToListAsync();

            int totalGames = allGames.Count;
            int totalWins = allGames.Count(g => g.WinnerId == userId);
            int totalLosses = totalGames - totalWins;
            double totalWinRate = totalGames > 0 ? Math.Round((double)totalWins / totalGames * 100, 2) : 0;

            // Thống kê theo từng game
            var groupedByGame = allGames
                  .GroupBy(g => g.Type)
                  .Select(g => new GameStatsDTO
                  {
                      GameName = g.Key.ToString(),
                      GamesPlayed = g.Count(),
                      Wins = g.Count(x => x.WinnerId == userId),
                      Losses = g.Count(x => x.WinnerId != userId && x.WinnerId != null),
                      WinRate = g.Count() > 0
                          ? Math.Round((double)g.Count(x => x.WinnerId == userId) / g.Count() * 100, 2)
                          : 0
                  })
                  .ToList();


            return new UserStatsDTO
            {
                TotalGamesPlayed = totalGames,
                TotalWins = totalWins,
                TotalLosses = totalLosses,
                TotalWinRate = totalWinRate,
                GamesByType = groupedByGame
            };
        }

    }
}
