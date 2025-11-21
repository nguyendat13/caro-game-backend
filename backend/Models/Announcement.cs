using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Announcements")]
    public class Announcement : Event
    {
        public string Content { get; set; }
        public string Audience { get; set; } // "all", "admins", "users"
    }

}
