using DATNBACK_END.Models;

namespace DATNBACK_END.Services;
    public interface IPlayerService{
        Task<ReturnPlayer> Register(string email,string Password,string Repassword);
        Task<List<Player>> GetPlayers();
    }