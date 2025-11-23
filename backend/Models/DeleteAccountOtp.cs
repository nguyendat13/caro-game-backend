namespace backend.Models
{
    public class DeleteAccountOtp
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OtpCode { get; set; }
        public DateTime Expiration { get; set; }
    }
}
