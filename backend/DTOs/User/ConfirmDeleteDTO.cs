namespace backend.DTOs.User
{
    public class ConfirmDeleteDTO
    {
        public int UserId { get; set; }
        public string OtpCode { get; set; } = null!;
    }
}
