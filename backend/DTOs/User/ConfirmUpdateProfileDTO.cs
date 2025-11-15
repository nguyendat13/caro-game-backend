namespace backend.DTOs.User
{
    public class ConfirmUpdateProfileDTO
    {
        public int UserId { get; set; }
        public string? NewUsername { get; set; }
        public string? NewEmail { get; set; }
        public string OtpCode { get; set; }
    }
}
