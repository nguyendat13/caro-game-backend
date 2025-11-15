using backend.DTOs.Game;
using backend.Models;

namespace backend.Services
{
    public interface IGameOfflineService
    {
        Task<List<GameDTO>> GetAllGamesAsync();

        Task<GameDTO> CreateOfflineGameAsync(int playerId);
        Task<GameMoveDTO> MakeMoveAsync(GameMoveCreateDTO moveDto);
        Task<GameDTO?> GetGameAsync(int gameId);
        Task<bool> DeleteGameAsync(int gameId);

    }
}
