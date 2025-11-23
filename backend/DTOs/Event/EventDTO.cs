using backend.Models;

namespace backend.DTOs.Event
{
    public class EventDTO
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EventType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Features { get; set; } = new();


    }
}
