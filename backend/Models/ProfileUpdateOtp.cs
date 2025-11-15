namespace backend.Models
{
    public class ProfileUpdateOtp
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? NewUsername { get; set; }
        public string? NewEmail { get; set; }
        public string OtpCode { get; set; }
        public DateTime Expiration { get; set; }
    }
}
