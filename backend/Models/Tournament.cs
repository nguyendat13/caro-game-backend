using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // 2. Tournament.cs
    [Table("Tournaments")]
    public class Tournament 
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }

        public string Name { get; set; }
        [Required]
        public string Game { get; set; } = string.Empty; // "caro", "chess",...

        [Required]
        public string Format { get; set; } = string.Empty; // "1v1", "2v2", "swiss",...

        public int MaxPlayers { get; set; } = 64;

        public string? Prize { get; set; }

        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }

        public string? Rules { get; set; }

        public bool IsRegistrationOpen { get; set; } = true;

        // Navigation property
        public int? EventRefId { get; set; }
        public Event? Event { get; set; }
    }

}
