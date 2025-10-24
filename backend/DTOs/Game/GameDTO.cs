namespace backend.DTOs.Game
{
    public class GameDTO
    {

        public int GameId { get; set; }
        public int? PlayerXId { get; set; }
        public string Status { get; set; }  // Ongoing / Finished
        public int? WinnerId { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<MoveDetailDTO>? Moves { get; set; } // để xem lại các nước đi
    }
}
