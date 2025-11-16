using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace backend.Services
{
    public class OtpCleanupServiceImpl : IOtpCleanupService
    {
        private readonly CaroDbContext _context;

        public OtpCleanupServiceImpl(CaroDbContext context)
        {
            _context = context;
        }

        public async Task CleanupExpiredOtpsAsync(CancellationToken cancellationToken = default)
        {
            var expiredOtps = await _context.ProfileUpdateOtps
                .Where(o => o.Expiration < DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            if (expiredOtps.Any())
            {
                _context.ProfileUpdateOtps.RemoveRange(expiredOtps);
                await _context.SaveChangesAsync(cancellationToken);
                Console.WriteLine($"{expiredOtps.Count} OTP hết hạn đã được xóa lúc {DateTime.UtcNow}");
            }
        }

        // Mới: Lấy toàn bộ OTP
        public async Task<List<ProfileUpdateOtp>> GetAllOtpsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ProfileUpdateOtps
                .OrderByDescending(o => o.Expiration)
                .ToListAsync(cancellationToken);
        }
    }
}
