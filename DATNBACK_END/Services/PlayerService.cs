using DATNBACK_END.Helpers;
using DATNBACK_END.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DATNBACK_END.Services;
public class PlayerService : IPlayerService
{
    private readonly AppDataContexts.AppDataContext _context;
    private readonly IEmailService _emailservice;
    public PlayerService(AppDataContexts.AppDataContext context, IEmailService emailService)
    {
        _context = context;
        _emailservice = emailService;
    }
    public string GenerateNumberOTP(){
        Random random = new Random();
        string otp = random.Next(100000, 999999).ToString();
        return otp;
    }
    public async Task<ReturnPlayer> RegisterPendingPlayer( string email,string Password, string Repassword)
    {
        try
        {
            System.Console.WriteLine($"{email} {Password}");    
            var Player = _context.Player.FirstOrDefault(x => x.Email == email);
            if (Player != null)
                return await Task.FromResult(new ReturnPlayer{
                    status = false,
                    message = "Email đã tồn tại"
                });
            if(Repassword != Password){
                return await Task.FromResult(new ReturnPlayer{
                    status = false,
                    message = "Mật khẩu không trùng khớp"
                });
            }
            var OTP = GenerateNumberOTP();
            //thuc hien goi ma hoa password
            PasswordHasher.CreatePasswordHash(Password,out var passwordHash, out var passwordSalt);
            var newPendingPlayer = new PendingPlayer{
                Email = email,
                Password = Convert.ToBase64String(passwordHash),
                Password_Salt = Convert.ToBase64String(passwordSalt),
                Name = $"Player {_context.Player.Count() + 1}",
                OTP = OTP,
                CreatedAt = DateTime.Now,
            };
            //thuc hien check ma otp roi moi them tai khoan
            _context.PendingPlayer.Add(newPendingPlayer);
           await _context.SaveChangesAsync();
           //thuc hien gui mail
            _emailservice.SendOTPEmail(email,"⚠️OTP Đăng Ký⚠️", OTP);
            return new ReturnPlayer{
                status = true,
                message = "Vui lòng nhập OTP gửi qua email để hoàn tất đăng ký tài khoản"
            };
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.InnerException);
            return await Task.FromResult(new ReturnPlayer{
                status = false,
                message = "Đăng ký thất bại"
            });
        }
    }
    public async Task<ReturnPlayer> VerifyOtpAndRegister(string email,string OTP){
        try{
            var checkPendingPlayer = await _context.PendingPlayer.FirstOrDefaultAsync(x=>x.Email==email&&x.OTP==OTP);
            if(checkPendingPlayer==null){
                return new ReturnPlayer{
                    status = false,
                    message = "OTP không đúng, vui lòng nhập lại"
                };
            }
            if(checkPendingPlayer.CreatedAt<DateTime.Now.AddMinutes(-5)){
                _context.PendingPlayer.Remove(checkPendingPlayer);
                await _context.SaveChangesAsync();
                return new ReturnPlayer{
                    status = false,
                    message = "OTP của bạn đã hết hạn"
                };
            }
            
            //neu nhap dung thi gan tu pending vao player chinh
            var newPlayer = new Player{
                Email = checkPendingPlayer.Email,
                Password = checkPendingPlayer.Password,
                Password_Salt = checkPendingPlayer.Password_Salt,
                Name = checkPendingPlayer.Name,
                Total_Money = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Player.Add(newPlayer);
            _context.PendingPlayer.Remove(checkPendingPlayer);
            await _context.SaveChangesAsync();
            return new ReturnPlayer{
                status = true,
                message = "Đăng ký thành công"
            };
        }
        catch(Exception e){
            System.Console.WriteLine(e);
            return await Task.FromResult(new ReturnPlayer{
                status = false,
                message = "Lỗi hệ thống"
            });
            
        }
    }
    public Task<List<Player>> GetPlayers()
    {
        try
        {
            return Task.FromResult(_context.Player.ToList());
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
