namespace backend.DTOs.User
{
    public class UserUpdateDTO
    {
        public int UserId { get; set; }        // Bắt buộc khi cập nhật
        public string Username { get; set; }   // Có thể đổi tên
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Password { get; set; }  // Có thể null nếu không đổi mật khẩu
        public int RoleId { get; set; }
    }
}
