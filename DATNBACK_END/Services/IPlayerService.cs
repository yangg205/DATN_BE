using DATNBACK_END.Models;

namespace DATNBACK_END.Services;

public interface IPlayerService
{
    Task<ReturnPlayer> RegisterPendingPlayer(string email, string Password, string Repasswords);
    Task<ReturnPlayer> VerifyOtpAndRegister(string email, string OTP);
    Task<ReturnPlayer> Login(string email, string password);
    Task<ReturnPlayer> LoginWithGoogle(string email, string password);
    Task<ReturnPlayer> LoginWithGoogleToken(string Token);
    Task<Player> GetPlayersByEmail(string email);
    Task<bool> deletePlayer(string email);
    Task<ReturnPlayer> Updatemoney(string email, decimal amount);
}
