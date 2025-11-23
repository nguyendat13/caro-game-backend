using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // 7. Announcement.cs
    [Table("Announcements")]
    public class Announcement : Event
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        public string Audience { get; set; } = "all"; // "all", "admin", "vip"

        public bool IsPinned { get; set; } = false;
        public DateTime? ExpiresAt { get; set; }
    }

}
