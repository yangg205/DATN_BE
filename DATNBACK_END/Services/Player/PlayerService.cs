using System.Diagnostics;
using DATNBACK_END.Helpers;
using DATNBACK_END.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

    public string GenerateNumberOTP()
    {
        Random random = new Random();
        string otp = random.Next(100000, 999999).ToString();
        return otp;
    }

    public async Task<ReturnPlayer> RegisterPendingPlayer(string email, string password, string repassword)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(repassword))
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Dữ liệu đầu vào không hợp lệ" });
            }
            if (password.Length < 8)
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Mật khẩu phải hơn 8 ký tự" });
            }
            System.Console.WriteLine($"{email} {password}");
            var player = _context.Player.FirstOrDefault(x => x.Email == email);
            if (player != null)
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Email đã tồn tại" });

            if (repassword != password)
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Mật khẩu không trùng khớp" });
            }

            var otp = GenerateNumberOTP();
            // Thực hiện gọi mã hóa password
            PasswordHasher.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            // Ensure passwordHash and passwordSalt are not null
            if (passwordHash == null || passwordSalt == null)
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Lỗi khi mã hóa mật khẩu" });
            }

            var newPendingPlayer = new PendingPlayer
            {
                Email = email,
                Password = Convert.ToBase64String(passwordHash),
                Password_Salt = Convert.ToBase64String(passwordSalt),
                Name = $"Player {_context.Player.Count() + 1}",
                OTP = otp,
                CreatedAt = DateTime.Now,
            };

            _context.PendingPlayer.Add(newPendingPlayer);
            await _context.SaveChangesAsync();

            // Thực hiện gửi mail
            _emailservice.SendOTPEmail(email, "⚠️OTP Đăng Ký⚠️", otp);
            return await Task.FromResult(new ReturnPlayer
            {
                status = true,
                message = "Vui lòng kiểm tra email để xác thực tài khoản bằng mã OTP",
            });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.InnerException);
            return await Task.FromResult(new ReturnPlayer { status = false, message = "Đăng ký thất bại" });
        }
    }

    public async Task<ReturnPlayer> VerifyOtpAndRegister(string email, string otp)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Dữ liệu đầu vào không hợp lệ" });
            }

            var checkPendingPlayer = await _context.PendingPlayer.FirstOrDefaultAsync(x => x.Email == email && x.OTP == otp);
            if (checkPendingPlayer == null)
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "OTP không đúng, vui lòng nhập lại" });
            }

            if (checkPendingPlayer.CreatedAt < DateTime.Now.AddMinutes(-5))
            {
                _context.PendingPlayer.Remove(checkPendingPlayer);
                await _context.SaveChangesAsync();
                return await Task.FromResult(new ReturnPlayer { status = false, message = "OTP của bạn đã hết hạn" });
            }

            var newPlayer = new Player
            {
                Email = checkPendingPlayer.Email,
                Password = checkPendingPlayer.Password,
                Password_Salt = checkPendingPlayer.Password_Salt,
                Name = checkPendingPlayer.Name,
                Total_Money = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.Player.Add(newPlayer);
            _context.PendingPlayer.Remove(checkPendingPlayer);
            await _context.SaveChangesAsync();
            return await Task.FromResult(new ReturnPlayer { status = true, message = "Đăng ký thành công" });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return await Task.FromResult(new ReturnPlayer { status = false, message = "Lỗi hệ thống" });
        }
    }

    public async Task<ReturnPlayer> Login(string email, string password)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Dữ liệu đầu vào không hợp lệ" });
            }
            var checkEmail = await _context.Player.FirstOrDefaultAsync(x => x.Email == email);
            if (checkEmail == null)
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Email hoặc mật khẩu không hợp lệ" });
            }
            var checkPass = PasswordHasher.VerifyPasswordHash(
                password,
                Convert.FromBase64String(checkEmail.Password),
                Convert.FromBase64String(checkEmail.Password_Salt)
            );
            if (!checkPass)
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Email hoặc mật khẩu không hợp lệ" });
            }

            var obj = new Player
            {
                Player_id = checkEmail.Player_id,
                Email = checkEmail.Email,
                Name = checkEmail.Name,
                Total_Money = checkEmail.Total_Money,
            };
            return await Task.FromResult(new ReturnPlayer { status = true, message = "Đăng nhập thành công", Player = obj });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return await Task.FromResult(new ReturnPlayer { status = false, message = "Lỗi hệ thống khi đăng nhập" });
        }
    }

    public async Task<ReturnPlayer> LoginWithGoogle(string email, string name)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrEmpty(email))
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Email không hợp lệ" });
            }

            var existingPlayer = await _context.Player.FirstOrDefaultAsync(x => x.Email == email);
            if (existingPlayer != null)
            {
                var player = new Player
                {
                    Email = existingPlayer.Email,
                    Name = existingPlayer.Name,
                    Total_Money = existingPlayer.Total_Money,
                };
                return await Task.FromResult(new ReturnPlayer { status = true, message = "Đăng nhập bằng Google thành công", Player = player });
            }

            var newPlayer = new Player
            {
                Email = email,
                Name = name ?? $"Player {_context.Player.Count() + 1}",
                Password = null,
                Password_Salt = null,
                Total_Money = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.Player.Add(newPlayer);
            await _context.SaveChangesAsync();
            Console.WriteLine("Saved player to DB");

            var returnPlayer = new Player
            {
                Email = newPlayer.Email,
                Name = newPlayer.Name,
                Total_Money = newPlayer.Total_Money,
            };

            return await Task.FromResult(new ReturnPlayer { status = true, message = "Đăng nhập và đăng ký bằng Google thành công", Player = returnPlayer });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return await Task.FromResult(new ReturnPlayer { status = false, message = "Lỗi hệ thống khi đăng nhập bằng Google" });
        }
    }

    public async Task<ReturnPlayer> LoginWithGoogleToken(string idToken)
    {
        try
        {
            // Validate input
            if (string.IsNullOrEmpty(idToken))
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Token không hợp lệ" });
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            var email = payload.Email;
            var name = payload.Name;

            return await LoginWithGoogle(email, name);
        }
        catch (InvalidJwtException)
        {
            return await Task.FromResult(new ReturnPlayer { status = false, message = "Token Google không hợp lệ" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(new ReturnPlayer { status = false, message = "Lỗi hệ thống khi xác thực Google" });
        }
    }
    public async Task<Player> GetPlayersByEmail(string email)
    {
        try
        {
            var existingPlayer = _context.Player.FirstOrDefault(x => x.Email == email);
            if (existingPlayer == null)
            {
                return await Task.FromResult(new Player { });
            }
            var player = new Player
            {
                Player_id = existingPlayer.Player_id,
                Email = existingPlayer.Email,
                Name = existingPlayer.Name,
                Total_Money = existingPlayer.Total_Money,
            };
            return await Task.FromResult(player);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    public async Task<bool> deletePlayer(string email)
    {
        try
        {
            var player = await _context.Player.FirstOrDefaultAsync(x => x.Email == email);
            if (player == null)
            {
                return false;
            }
            _context.Player.Remove(player);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
    }
    public async Task<ReturnPlayer> Updatemoney(string email, decimal amount)
    {
        try
        {
            var player = _context.Player.FirstOrDefault(x => x.Email == email);
            if (player == null)
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Người chơi không tồn tại" });
            }
            player.Total_Money += amount;
            _context.Player.Update(player);
            _context.SaveChanges();
            var obj = new Player
            {
                Email = player.Email,
                Total_Money = player.Total_Money
            };
            return await Task.FromResult(new ReturnPlayer { status = true, message = "Cập nhật tiền thành công", Player = obj });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return await Task.FromResult(new ReturnPlayer { status = false, message = "Lỗi hệ thống khi cập nhật tiền" });
        }
    }
    public async Task<ReturnPlayer> ResetPassword(string email, string password, string newpassword, string renewpassword)
    {
        try
        {
            var player = await _context.Player.FirstOrDefaultAsync(p => p.Email == email);
            if (player is null)
            {
                return await Task.FromResult(new ReturnPlayer
                {
                    status = false,
                    message = "người chơi không tồn tại"
                });
            }
            if (newpassword.Length < 8)
            {
                return await Task.FromResult(new ReturnPlayer
                {
                    status = false,
                    message = "Mật khẩu phải hơn 8 ký tự"
                });
            }
            if (renewpassword != newpassword)
            {
                return await Task.FromResult(new ReturnPlayer
                {
                    status = false,
                    message = "Mật khẩu không khớp"
                });
            }

            var checkpass = PasswordHasher.VerifyPasswordHash(password, Convert.FromBase64String(player.Password), Convert.FromBase64String(player.Password_Salt));
            if (!checkpass)
            {
                return await Task.FromResult(new ReturnPlayer
                {
                    status = false,
                    message = "Mật khẩu cũ không khớp"
                });
            }
            PasswordHasher.CreatePasswordHash(newpassword, out var passwordHash, out var passwordSalt);
            if (passwordHash == null || passwordSalt == null)
            {
                return await Task.FromResult(new ReturnPlayer { status = false, message = "Lỗi khi mã hóa mật khẩu" });
            }

            player.Password = Convert.ToBase64String(passwordHash);
            player.Password_Salt = Convert.ToBase64String(passwordSalt);
            await _context.SaveChangesAsync();

            return await Task.FromResult(new ReturnPlayer
            {
                status = true,
                message = "Thay đổi mật khẩu thành công",
            });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.InnerException);
            return await Task.FromResult(new ReturnPlayer
            {
                status = false,
                message = "Lỗi khi đổi mật khẩu"
            });
        }
    }
    public async Task<ReturnPlayer> UpdateName(string email, string newname)
    {
        try
        {
            var player = await _context.Player.FirstOrDefaultAsync(p => p.Email == email);
            if (player == null)
            {
                return await Task.FromResult(new ReturnPlayer
                {
                    status = false,
                    message = "Người chơi không tồn tại"
                });
            }
            var playername = new Player
            {
                Name = newname
            };
            player.Name = newname;
            if (player.Name.Contains("Player"))
            {
                return await Task.FromResult(new ReturnPlayer
                {
                    status = true,
                    message = $"Xin chào {newname}",
                    Player = playername
                });
            }
            await _context.SaveChangesAsync();
            return await Task.FromResult(new ReturnPlayer
            {
                status = true,
                message = "Đổi tên thành công",
                Player = playername
            });

        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return await Task.FromResult(new ReturnPlayer
            {
                status = false,
                message = "Lỗi hệ thống khi đổi tên"
            });
        }
    }
}