using backend.Models;
using System.ComponentModel.DataAnnotations;

public class ChatChannel : Event
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsPrivate { get; set; }
    public bool VoiceEnabled { get; set; }
    public int MaxMembers { get; set; }
}
