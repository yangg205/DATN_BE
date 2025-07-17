namespace DATNBACK_END.Services;
using DATNBACK_END.Models;
using DATNBACK_END.Models.DTO.Transaction;

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
    Task<ReturnPlayer> ResetPassword(string email, string password, string newpassword, string renewpassword);
    Task<ReturnPlayer> UpdateName(string email, string newname);


        // Lấy Player Model để truy cập Total_Money
    Task<Player?> GetPlayerByIdAsync(int playerId);



    // Xử lý đổi tiền (orchestrates calls to IPlayerCharacterService if involves Total_coin)
    Task<ExchangeCurrencyResponseDTO> ExchangeCurrencyAsync(ExchangeCurrencyRequestDTO request);
}
