using backend.DTOs.Game;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/offline-game")]
    [ApiController]
    public class GameOfflineController : ControllerBase
    {
        private readonly IGameOfflineService _gameService;

        public GameOfflineController(IGameOfflineService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromQuery] int playerId)
        {
            var game = await _gameService.CreateOfflineGameAsync(playerId);
            return Ok(game);
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove([FromBody] GameMoveCreateDTO moveDto)
        {
            var result = await _gameService.MakeMoveAsync(moveDto);
            return Ok(result);
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGame(int gameId)
        {
            var game = await _gameService.GetGameAsync(gameId);
            if (game == null) return NotFound();
            return Ok(game);
        }
        [HttpDelete("delete-game/{gameId}")]
        public async Task<IActionResult> DeleteGame(int gameId)
        {
            var success = await _gameService.DeleteGameAsync(gameId);
            if (!success)
                return NotFound(new { message = $"Không tìm thấy ván cờ có ID {gameId}." });

            return Ok(new { message = "Xóa ván cờ thành công." });
        }

    }
}
