using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // 7. Announcement.cs
    [Table("Announcements")]
    public class Announcement 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Audience { get; set; } = "all"; // "all", "admin", "vip"

        public bool IsPinned { get; set; } = false;
        public DateTime? ExpiresAt { get; set; }
        // Navigation property
        public int? EventRefId { get; set; }
        public Event? Event { get; set; }
    }

}
