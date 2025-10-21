namespace backend.DTOs.Auth
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Message { get; set; }

        public object User { get; set; }
    }
}
