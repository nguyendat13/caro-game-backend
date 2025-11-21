using backend.Models;
using System.ComponentModel.DataAnnotations;

public class ClanRecruit : Event
{
    [Required]
    public string ClanName { get; set; }
    public string Description { get; set; }
    public string RequiredRank { get; set; }
    public string Position { get; set; }
}
