using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpTestController : ControllerBase
    {
        private readonly IOtpCleanupService _otpService;

        public OtpTestController(IOtpCleanupService otpService)
        {
            _otpService = otpService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOtps()
        {
            var otps = await _otpService.GetAllOtpsAsync();
            return Ok(otps);
        }

        [HttpDelete("cleanup")]
        public async Task<IActionResult> CleanupExpiredOtps()
        {
            await _otpService.CleanupExpiredOtpsAsync();
            return Ok(new { message = "Đã xóa OTP hết hạn." });
        }
    }
}
