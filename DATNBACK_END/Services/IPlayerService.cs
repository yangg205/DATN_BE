using DATNBACK_END.Models;

namespace DATNBACK_END.Services;
    public interface IPlayerService{
        Task<Player> AddPlayer(string email,string name,string Password);
        Task<List<Player>> GetPlayers();
    }