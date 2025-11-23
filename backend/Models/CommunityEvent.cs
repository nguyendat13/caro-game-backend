using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // 3. CommunityEvent.cs
    [Table("CommunityEvents")]
    public class CommunityEvent : Event
    {
        public string Name { get; set; }
        public string? Location { get; set; } // Online / Offline
        public string? StreamLink { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }

        public string? Host { get; set; }
    }

}
