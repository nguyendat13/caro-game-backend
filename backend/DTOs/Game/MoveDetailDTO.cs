namespace backend.DTOs.Game
{
    public class MoveDetailDTO
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public string PlayerType { get; set; } // "Player" hoặc "Bot"

        public MoveDetailDTO(byte x, byte y, string playerType)
        {
            X = x;
            Y = y;
            PlayerType = playerType;
        }
    }
}
