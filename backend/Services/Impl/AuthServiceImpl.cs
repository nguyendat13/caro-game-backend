using backend.DTOs.Auth;
using backend.DTOs.User;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly CaroDbContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly EmailService _emailService;

        public AuthServiceImpl(CaroDbContext context, IConfiguration config, EmailService emailService)
        {
            _context = context;
            _config = config;
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

        private string GenerateToken(User user)
        {
            var keyString = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new Exception("JWT Key is not configured.");

            var key = Encoding.UTF8.GetBytes(keyString);

            var claims = new List<Claim>
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("roleId", user.RoleId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"] ?? "60")),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
                throw new Exception("Vui lòng nhập đầy đủ tài khoản/email và mật khẩu.");

            var user = await _context.Users
                .Include(u => u.Role) // Include Role
                .FirstOrDefaultAsync(u => u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

            if (user == null)
                throw new Exception("Không tìm thấy tài khoản với thông tin đã nhập.");

            if (!VerifyPassword(user, dto.Password, user.PasswordHash))
                throw new Exception("Mật khẩu không chính xác.");

            string token = GenerateToken(user);

            return new LoginResponseDTO
            {
                Message = "Đăng nhập thành công!",
                Token = token,
                User = new
                {
                    user.UserId,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.Phone,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.RoleName
                }
            };
        }

        public async Task<LoginResponseDTO> RegisterAsync(UserRegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                throw new Exception("Vui lòng nhập đầy đủ thông tin đăng ký.");

            bool exists = await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (exists)
                throw new Exception("Tên người dùng hoặc email đã được sử dụng.");

            var user = new User
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                RoleId = dto.RoleId > 0 ? dto.RoleId : 3 // default = 3 (user)
            };

            user.PasswordHash = HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Lấy role để trả về
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            string token = GenerateToken(user);

            return new LoginResponseDTO
            {
                Message = "Đăng ký thành công!",
                Token = token,
                User = new
                {
                    user.UserId,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.Phone,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.RoleName
                }
            };
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new Exception("Email không tồn tại trong hệ thống.");

            var tempPassword = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

            user.PasswordHash = HashPassword(user, tempPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var emailBody = $@"
            <p>Xin chào {user.FullName ?? user.Username},</p>
            <p>Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản của mình.</p>
            <p><strong>Mật khẩu tạm thời:</strong> {tempPassword}</p>
            <p>Vui lòng đăng nhập và đổi mật khẩu ngay lập tức để bảo mật tài khoản của bạn.</p>
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
            <p>Trân trọng,<br/>{_config["EmailSettings:SenderName"]}</p>
        ";

            try
            {
                // ❌ Không new EmailService nữa
               await _emailService.SendEmailAsync(user.Email, "Yêu cầu đặt lại mật khẩu", emailBody);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể gửi email. " + ex.Message);
            }
        }
    }
}
