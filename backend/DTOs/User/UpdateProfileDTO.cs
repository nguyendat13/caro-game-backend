﻿namespace backend.DTOs.User
{
    public class UpdateProfileDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }=string.Empty;   // Có thể đổi tên

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
