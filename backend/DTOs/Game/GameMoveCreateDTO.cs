namespace backend.DTOs.Game
{
    public class GameMoveCreateDTO
    {
        public int GameId { get; set; }
        public int? PlayerId { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
    }
}
