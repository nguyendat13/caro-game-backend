using backend.DTOs;
using backend.DTOs.User;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        private int GetCurrentUserRoleId()
        {
            // Lấy roleId từ JWT claim, mặc định = 3 (user) nếu không tìm thấy
            var roleClaim = User.FindFirst("roleId")?.Value;
            return int.TryParse(roleClaim, out var roleId) ? roleId : 3;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRegisterDTO dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = created.UserId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDTO dto)
        {
            if (id != dto.UserId) return BadRequest();

            try
            {
                int currentUserRoleId = GetCurrentUserRoleId();
                var updated = await _service.UpdateAsync(dto, currentUserRoleId);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int currentUserRoleId = GetCurrentUserRoleId();
                var result = await _service.DeleteAsync(id, currentUserRoleId);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                var result = await _service.ChangePasswordAsync(dto);
                return Ok(new { message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            try
            {
                var updatedUser = await _service.UpdateProfileAsync(dto);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================
        // 1) YÊU CẦU CẬP NHẬT PROFILE (GỬI OTP)
        // ============================================
        [HttpPost("request-profile-update")]
        public async Task<IActionResult> RequestProfileUpdate([FromBody] UpdateProfileRequestDTO dto)
        {
            try
            {
                await _service.RequestProfileUpdateAsync(dto); // ❌ không gán vào biến
                return Ok(new { success = true, message = "OTP đã gửi vào email cũ." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        // ============================================
        // 2) XÁC NHẬN OTP ĐỂ CẬP NHẬT PROFILE
        // ============================================
        [HttpPost("confirm-profile-update")]
        public async Task<IActionResult> ConfirmProfileUpdate([FromBody] ConfirmUpdateProfileDTO dto)
        {
            try
            {
                var updatedUser = await _service.ConfirmProfileUpdateAsync(dto);
                return Ok(new { success = true, data = updatedUser, message = "Cập nhật thông tin thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{userId}/stats")]
        public async Task<IActionResult> GetStats(int userId)
        {
            var stats = await _service.GetUserStatsAsync(userId);
            return Ok(stats);
        }

    }
}
