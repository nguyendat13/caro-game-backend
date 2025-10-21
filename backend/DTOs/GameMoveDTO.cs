namespace backend.DTOs
{
    public class GameMoveDTO
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public int MoveOrder { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
