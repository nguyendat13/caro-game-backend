using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // 3. CommunityEvent.cs
    [Table("CommunityEvents")]
    public class CommunityEvent 
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string? Location { get; set; } // Online / Offline
        public string? StreamLink { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }

        public string? Host { get; set; }
        // Navigation property
        public int? EventRefId { get; set; }
        public Event? Event { get; set; }
    }

}
