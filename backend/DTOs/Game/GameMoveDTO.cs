namespace backend.DTOs.Game
{
    public class GameMoveDTO
    {
        public MoveDetailDTO PlayerMove { get; set; }
        public MoveDetailDTO BotMove { get; set; }
    }
}
