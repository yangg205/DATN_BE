using DATNBACK_END.Models;

namespace DATNBACK_END.Services;
    public interface IPlayerService{
        Task<ReturnPlayer> RegisterPendingPlayer(string email,string Password,string Repasswords);
        Task<ReturnPlayer> VerifyOtpAndRegister(string email,string OTP);
        Task<List<Player>> GetPlayers();
    }