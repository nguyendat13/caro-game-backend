using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public enum EventType
    {
        Tournament,
        CommunityEvent,
        ClanRecruit,
        ChatChannel,
        Article,
        Announcement
    }

    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public EventType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
