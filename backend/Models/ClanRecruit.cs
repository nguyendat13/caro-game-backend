using backend.Models;
using System.ComponentModel.DataAnnotations;

// 4. ClanRecruit.cs
public class ClanRecruit 
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ClanName { get; set; } = string.Empty;

    public string? RequiredRank { get; set; } // "Master", "Expert",...

    public string? PositionNeeded { get; set; } // "Carry", "Support",...

    public string? Contact { get; set; } // Discord, Zalo,...
                                         // Navigation property
    public int? EventRefId { get; set; }
    public Event? Event { get; set; }
}
