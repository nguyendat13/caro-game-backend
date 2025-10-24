using backend.DTOs.Auth;
using backend.DTOs.User;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly CaroDbContext _context;
        private readonly IConfiguration _config;

        public AuthServiceImpl(CaroDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var claims = new List<Claim>
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? "")
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
                .FirstOrDefaultAsync(u => u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

            if (user == null)
                throw new Exception("Không tìm thấy tài khoản với thông tin đã nhập.");

            var hashedPassword = HashPassword(dto.Password);
            if (user.PasswordHash != hashedPassword)
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
                    user.Phone
                }
            };
        }

        public async Task<LoginResponseDTO> RegisterAsync(UserRegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                throw new Exception("Vui lòng nhập đầy đủ thông tin đăng ký.");

            // Kiểm tra username hoặc email đã tồn tại
            bool exists = await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (exists)
                throw new Exception("Tên người dùng hoặc email đã được sử dụng.");

            // Hash mật khẩu
            string hashedPassword = HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

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
                    user.Phone
                }
            };
        }


        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new Exception("Email không tồn tại trong hệ thống.");

            // Tạo mật khẩu tạm thời (8 ký tự)
            var tempPassword = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);

            // Hash lại mật khẩu
            user.PasswordHash = HashPassword(tempPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Gửi email thông báo
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
                var emailService = new EmailService(_config);
                emailService.SendEmail(user.Email, "Yêu cầu đặt lại mật khẩu", emailBody);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể gửi email. " + ex.Message);
            }
        }
    }
}
