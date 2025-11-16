namespace backend.DTOs.Game
{
    public class GameMoveDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public MoveDetailDTO? PlayerMove { get; set; }
        public MoveDetailDTO? BotMove { get; set; }

        // ====== Factory Methods ======

        public static GameMoveDTO Fail(string msg)
            => new GameMoveDTO
            {
                Success = false,
                Message = msg
            };

        public static GameMoveDTO SuccessMove(byte px, byte py, byte bx, byte by)
            => new GameMoveDTO
            {
                Success = true,
                Message = "Tiếp tục",
                PlayerMove = new MoveDetailDTO(px, py, "Player"),
                BotMove = new MoveDetailDTO(bx, by, "Bot")
            };

        public static GameMoveDTO SuccessPlayerWin(byte px, byte py)
            => new GameMoveDTO
            {
                Success = true,
                Message = "🎉 Bạn đã chiến thắng!",
                PlayerMove = new MoveDetailDTO(px, py, "Player"),
                BotMove = null
            };

        public static GameMoveDTO SuccessBotWin(byte px, byte py, byte bx, byte by)
            => new GameMoveDTO
            {
                Success = true,
                Message = "🤖 Bot đã chiến thắng!",
                PlayerMove = new MoveDetailDTO(px, py, "Player"),
                BotMove = new MoveDetailDTO(bx, by, "Bot")
            };

        public static GameMoveDTO Draw(byte px, byte py)
            => new GameMoveDTO
            {
                Success = true,
                Message = "Ván cờ hòa.",
                PlayerMove = new MoveDetailDTO(px, py, "Player"),
                BotMove = null
            };
    }
}
