namespace backend.DTOs.User
{
    public class UpdateProfileRequestDTO
    {
        public int UserId { get; set; }
        public string? NewUsername { get; set; }
        public string? NewEmail { get; set; }
    }
}
