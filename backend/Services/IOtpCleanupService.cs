using backend.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface IOtpCleanupService
    {
        Task CleanupExpiredOtpsAsync(CancellationToken cancellationToken = default);

        // Mới: lấy danh sách OTP (dùng cho kiểm tra)
        Task<List<ProfileUpdateOtp>> GetAllOtpsAsync(CancellationToken cancellationToken = default);
    }
}
