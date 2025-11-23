using backend.Models;
using System.ComponentModel.DataAnnotations;

// 4. ClanRecruit.cs
public class ClanRecruit : Event
{
    [Required]
    public string ClanName { get; set; } = string.Empty;

    public string? RequiredRank { get; set; } // "Master", "Expert",...

    public string? PositionNeeded { get; set; } // "Carry", "Support",...

    public string? Contact { get; set; } // Discord, Zalo,...
}
