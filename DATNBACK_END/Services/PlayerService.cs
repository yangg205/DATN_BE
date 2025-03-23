using DATNBACK_END.Helpers;
using DATNBACK_END.Models;
using Microsoft.AspNetCore.Identity;

namespace DATNBACK_END.Services;
public class PlayerService : IPlayerService
{
    private readonly AppDataContexts.AppDataContext _context;
    public PlayerService(AppDataContexts.AppDataContext context)
    {
        _context = context;
    }
    public Task<ReturnPlayer> Register( string email,string Password, string Repassword)
    {
        try
        {
            System.Console.WriteLine($"{email} {Password}");    
            var Player = _context.Player.FirstOrDefault(x => x.Email == email);
            if (Player != null)
                return Task.FromResult(new ReturnPlayer{
                    status = false,
                    message = "Email đã tồn tại"
                });
            if(Repassword != Password){
                return Task.FromResult(new ReturnPlayer{
                    status = false,
                    message = "Mật khẩu không trùng khớp"
                });
            }
            //thuc hien goi ma hoa password
            PasswordHasher.CreatePasswordHash(Password,out var passwordHash, out var passwordSalt);
            var newPlayer = new Player{
                Email = email,
                Password = Convert.ToBase64String(passwordHash),
                Repassword = Convert.ToBase64String(passwordHash),
                Password_Salt = Convert.ToBase64String(passwordSalt),
                Name = $"Player {_context.Player.Count()}",
                Total_Money = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Player.Add(newPlayer);
            _context.SaveChanges();
            var obj = new ReturnPlayer{
                status = true,
                message = "Đăng ký thành công",
                Player = new Player{
                    Email = newPlayer.Email,
                    Name = newPlayer.Name,
                    Total_Money = newPlayer.Total_Money
                }
            };
            return Task.FromResult(obj);
        }
        catch (Exception e)
        {
            return Task.FromResult(new ReturnPlayer{
                status = false,
                message = "Đăng ký thất bại"
            });
            throw new Exception(e.Message);
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
