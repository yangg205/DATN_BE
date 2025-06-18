using DATNBACK_END.Models;
using DATNBACK_END.Models.modelTemp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace DATNBACK_END.Services;

public class PlayerCharacterService : IPlayerCharacterService
{
    private readonly AppDataContexts.AppDataContext _context;
    public PlayerCharacterService(AppDataContexts.AppDataContext context)
    {
        _context = context;
    }
    public async Task<ReturnPlayerCharacter> SaveGame(Player_Characters player_Characters)
    {
        try
        {
            var PC = await _context.Player_Characters.FirstOrDefaultAsync(x => x.Player_id == player_Characters.Player_id && x.Characters_id == player_Characters.Characters_id);
            if (PC != null)
            {
                PC.Current_hp = player_Characters.Current_hp;
                PC.Current_exp = player_Characters.Current_exp;
                PC.level = player_Characters.level;
                PC.Total_point = player_Characters.Total_point;
                PC.Total_coin = player_Characters.Total_coin;
                PC.Skill_Point = player_Characters.Skill_Point;
                PC.Position_x = player_Characters.Position_x;
                PC.Position_y = player_Characters.Position_y;
                PC.Position_z = player_Characters.Position_z;
                _context.Player_Characters.Update(PC);
            }
            else
            {
                await _context.Player_Characters.AddAsync(player_Characters);
            }
            await _context.SaveChangesAsync();
            return await Task.FromResult(new ReturnPlayerCharacter
            {
                status = true,
                message = "Save game thành công"
            });

        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.InnerException);
            return await Task.FromResult(new ReturnPlayerCharacter
            {
                status = false,
                message = "Lỗi hệ thống khi SAVE"
            });
        }
    }
    public async Task<ReturnPlayerCharacter> ContinueGame(int playerId, int characterId)
    {
        try
        {
            var PC = await _context.Player_Characters.FirstOrDefaultAsync(x => x.Player_id == playerId && x.Characters_id == characterId);
            if (PC == null)
            {
                return await Task.FromResult(new ReturnPlayerCharacter
                {
                    status = false,
                    message = "Không có dữ liệu game"
                });
            }
            return await Task.FromResult(new ReturnPlayerCharacter
            {
                status = true,
                message = "Dữ liệu đã load thành công",
                player_Characters = new Player_Characters
                {
                    Player_Character_id = PC.Player_Character_id,
                    Player_id = PC.Player_id,
                    Characters_id = PC.Characters_id,
                    Current_hp = PC.Current_hp,
                    Current_exp = PC.Current_exp,
                    level = PC.level,
                    Total_point = PC.Total_point,
                    Total_coin = PC.Total_coin,
                    Skill_Point = PC.Skill_Point,
                    Position_x = PC.Position_x,
                    Position_y = PC.Position_y,
                    Position_z = PC.Position_z
                }
            });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return await Task.FromResult(new ReturnPlayerCharacter
            {
                status = false,
                message = "Lỗi hệ thống, không thể load game"
            });
        }
        ;
    }
    public async Task<ReturnPlayerCharacter> SelectCharacter(int Player_id, int Character_id)
    {
        try
        {
            System.Console.WriteLine("Player_id: " + Player_id, " Character_id: " + Character_id);
            var player = await _context.Player
                .FirstOrDefaultAsync(p => p.Player_id == Player_id);

            if (player == null)
            {
                return await Task.FromResult(new ReturnPlayerCharacter
                {
                    status = false,
                    message = "Player không tồn tại"
                });
            }
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Characters_id == Character_id);

            if (character == null)
            {
                return await Task.FromResult(new ReturnPlayerCharacter
                {
                    status = false,
                    message = "Nhân vật không tồn tại"
                });
            }
            var existingSelection = await _context.Player_Characters
                .FirstOrDefaultAsync(pc => pc.Player_id == Player_id && pc.Characters_id == Character_id);

            if (existingSelection != null)
            {
                return await Task.FromResult(new ReturnPlayerCharacter
                {
                    status = false,
                    message = "Nhân vật này đã được chọn"
                });
            }

            // Tạo bản ghi mới trong Player_Characters
            var playerCharacter = new Player_Characters
            {
                Player_id = Player_id,
                Characters_id = Character_id,
                Ownershipdate = DateTime.UtcNow,
                Current_hp = character.Hp, // Máu hiện tại bằng máu của nhân vật
                Current_exp = 0,
                level = 1,
                Total_point = 0,
                Total_coin = 0,
                Skill_Point = 0,
                Position_x = 0f, // Vị trí mặc định
                Position_y = 0f,
                Position_z = 0f,
                Datesave = DateTime.UtcNow
            };

            _context.Player_Characters.Add(playerCharacter);
            await _context.SaveChangesAsync();

            return await Task.FromResult(new ReturnPlayerCharacter
            {
                status = true,
                message = "Chọn nhân vật thành công",
                player_Characters = playerCharacter
            });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return await Task.FromResult(new ReturnPlayerCharacter
            {
                status = false,
                message = "Lỗi khi chọn nhân vật"
            });
        }
    }
    public async Task<bool> DeleteAllPlayerCharacter()
    {
        try
        {
            var playerCharacters = await _context.Player_Characters.ToListAsync();
            if (playerCharacters != null && playerCharacters.Count > 0)
            {
                _context.Player_Characters.RemoveRange(playerCharacters);
                await _context.SaveChangesAsync();
            }
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
    }
    public async Task<List<Player_Characters>> GetAllPlayerCharacter()
    {
        var playerCharacters = await _context.Player_Characters.ToListAsync();
        return playerCharacters;
    }
    public async Task<bool> UpdateSkillPoint(int playerCharacterId, int Point)
    {
        try
        {
            var playerCharacter = await _context.Player_Characters
                .FirstOrDefaultAsync(pc => pc.Player_Character_id == playerCharacterId);
            if (playerCharacter == null)
            {
                return false;
            }
            playerCharacter.Skill_Point += Point;
            _context.Player_Characters.Update(playerCharacter);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            return false;
        }
    }
    public async Task<List<Ranking>> RankingPlayerCharacter(int Character_id)
    {
        try
        {
            // Lấy dữ liệu từ bảng Player_Characters
            var query = _context.Player_Characters.AsQueryable();

            // Nếu Character_id khác 0, lọc theo Character_id
            if (Character_id != 0)
            {
                query = query.Where(pc => pc.Characters_id == Character_id);
            }

            // Thực hiện join với bảng Player và sắp xếp theo Total_point
            var rankings = await query
                .Join(
                    _context.Player,
                    pc => pc.Player_id, // Khóa liên kết từ Player_Characters
                    p => p.Player_id,   // Khóa liên kết từ Player
                    (pc, p) => new
                    {
                        PlayerName = p.Name,               // Tên người chơi
                        Total_point = pc.Total_point       // Điểm tổng
                    }
                )
                .OrderByDescending(r => r.Total_point)    // Sắp xếp theo điểm tổng
                .ToListAsync();

            // Gán thứ hạng
            var rankedResult = rankings
                .Select((r, index) => new Ranking
                {
                    PlayerName = r.PlayerName,
                    Total_point = (int?)r.Total_point,
                    Rank = index + 1 // Thứ hạng bắt đầu từ 1
                })
                .ToList();

            return rankedResult;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            // Trả về thông báo lỗi nếu có vấn đề xảy ra
            return new List<Ranking>
        {
            new Ranking
            {
                PlayerName = "Error",
                Total_point = 0,
                Rank = -1
            }
        };
        }
    }

    public async Task<PlayerCharacterSkillPoint> GetSkillPoint(int Player_Character_id)
    {
        try
        {
            var playerCharacter = await _context.Player_Characters
                .FirstOrDefaultAsync(pc => pc.Player_Character_id == Player_Character_id);
            if (playerCharacter == null)
            {
                throw new Exception("Player_Character không tồn tại");
            }
            return new PlayerCharacterSkillPoint
            {
                PlayerCharacterId = playerCharacter.Player_Character_id,
                SkillPoint = playerCharacter.Skill_Point
            };

        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Lỗi hệ thống khi lấy Skill Point");
        }
    }
    public async Task<ReturnPlayerCharacter> SelectedCharacter(int Player_id, int Character_id)
    {
        try
        {
            var playercharacter = await _context.Player_Characters.FirstOrDefaultAsync(p => p.Player_id == Player_id && p.Characters_id == Character_id);
            if (playercharacter == null)
            {
                return await Task.FromResult(new ReturnPlayerCharacter
                {
                    status = false,
                    message = "Người chơi chưa chọn nhân vật này"
                });
            }
            return await Task.FromResult(new ReturnPlayerCharacter
            {
                status = true,
                message = $"bạn đã chọn nhân vật có id là {Character_id}"
            });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return await Task.FromResult(new ReturnPlayerCharacter
            {
                status = false,
                message = "Lỗi hệ thống"
            });
        }
    }

}