using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class VoiceSettings
{
    [Key]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string? InputDeviceId { get; set; }
    public string? OutputDeviceId { get; set; }
    public decimal InputVolume { get; set; } = 100;
    public decimal OutputVolume { get; set; } = 100;
    public bool PushToTalk { get; set; } = false;
    public string? PushToTalkKey { get; set; }

    public bool VoiceActivation { get; set; } = true;
    public decimal VoiceActivationThreshold { get; set; } = -40;
}
