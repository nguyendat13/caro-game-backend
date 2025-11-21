// backend/DTOs/Game/LeaderboardEntryDTO.cs

namespace backend.DTOs.Game;

public class LeaderboardEntryDTO
{
    public int Rank { get; set; }                    // Hạng (sẽ được gán ở service)
    public string UserId { get; set; } = string.Empty; // ID người chơi (rất cần cho future feature)
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }            // Tên đầy đủ (có thể null)

    public int TotalGames { get; set; }              // Tổng số trận đã chơi
    public int Wins { get; set; }                    // Số trận thắng
    public int Losses { get; set; }                  // Số trận thua
    public int Draws { get; set; }                   // Số trận hòa (nếu có)

    public double WinRate { get; set; }              // Tỷ lệ thắng (%) – đã tính sẵn, ví dụ: 68.5

    // Tùy chọn: nếu bạn vẫn muốn giữ "Points" để làm Elo hoặc ranking point sau này
    // public int Points { get; set; } = 0;
}