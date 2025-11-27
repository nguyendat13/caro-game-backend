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
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public EventType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<EventFeature> Features { get; set; } = new List<EventFeature>();
        // Collections
        public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
        public ICollection<CommunityEvent> CommunityEvents { get; set; } = new List<CommunityEvent>();
        public ICollection<ClanRecruit> ClanRecruits { get; set; } = new List<ClanRecruit>();
        public ICollection<Article> Articles { get; set; } = new List<Article>();
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public ICollection<ChatChannel> ChatChannels { get; set; } = new List<ChatChannel>();

    }
}
