﻿namespace backend.DTOs.User
{
    public class UserRegisterDTO
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Password { get; set; }
    }
}
