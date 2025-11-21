using backend.DTOs.Game;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameStatsController : ControllerBase
    {
        private readonly IGameStatsService _service;

        public GameStatsController(IGameStatsService service)
        {
            _service = service;
        }

        // Lấy recent games của user
        [HttpGet("recent/{userId}")]
        public async Task<ActionResult<List<RecentGameDTO>>> GetRecentGames(int userId)
        {
            var games = await _service.GetRecentGamesAsync(userId);
            return Ok(games);
        }

        // Lấy leaderboard theo loại game
        [HttpGet("leaderboard/{gameType}")]
        public async Task<ActionResult<List<LeaderboardEntryDTO>>> GetLeaderboard(string gameType)
        {
            var leaderboard = await _service.GetLeaderboardAsync(gameType);
            return Ok(leaderboard);
        }
    }
}
