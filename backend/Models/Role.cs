        namespace backend.Models
    {
        public class Role
        {
            public int RoleId { get; set; }
        public string RoleName { get; set; } = null!; // null-forgiving vì EF sẽ gán giá trị khi seed
        public ICollection<User> Users { get; set; } = new List<User>();

        }
    }
