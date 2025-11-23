using backend.Models;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Event
{
    public class EventUpdateDTO
    {
        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public EventType Type { get; set; }
    }
}
