using DATNBACK_END.Models;
using DATNBACK_END.Models.modelTemp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DATNBACK_END.Services;

public class PlayerCharacterBattleService : IPlayerCharacterBattleService
{
    private readonly AppDataContexts.AppDataContext _context;
    public PlayerCharacterBattleService(AppDataContexts.AppDataContext context)
    {
        _context = context;
    }
    public async Task<Battle> Battle(int Player_Characters_id, int Boss_id, int Death, double maxtime, double realtime)
    {
        try
        {
            var player = await _context.Player_Characters.FirstOrDefaultAsync(pc => pc.Player_Character_id == Player_Characters_id);
            var Boss = await _context.Boss.FirstOrDefaultAsync(b => b.Boss_id == Boss_id);
            if (player == null || Boss == null)
            {
                throw new Exception("Nguoi choi hoac Boss khong ton tai");
            }
            string text = "";
            var point = (100 * Boss.Level * Boss.Difficult) - (Death * 10) + (maxtime - realtime) / 5;
            if (point < 10)
            {
                point = 0;
                text = "D";
            }
            //chinh logic lai
            if (Boss.Level == 1)
            {
                if (point >= 100)
                {
                    text = "S";
                }
                else if (point >= 75 && point < 100)
                {
                    text = "A";
                }
                else if (point >= 50 && point < 75)
                {
                    text = "B";
                }
                else if (point >= 10 && point < 50)
                {
                    text = "C";
                }
            }
            else if (Boss.Level == 2)
            {
                //boss level 2 > 200
                if (point >= 200)
                {
                    text = "S";
                }
                else if (point >= 150 && point < 200)
                {
                    text = "A";
                }
                else if (point >= 100 && point < 150)
                {
                    text = "B";
                }
                else if (point >= 10 && point < 100)
                {
                    text = "C";
                }
            }
            else if (Boss.Level == 3)
            {
                //boss level 3 > 300
                if (point >= 300)
                {
                    text = "S";
                }
                else if (point >= 225 && point < 300)
                {
                    text = "A";
                }
                else if (point >= 150 && point < 225)
                {
                    text = "B";
                }
                else if (point >= 10 && point < 150)
                {
                    text = "C";
                }
            }
            else if (Boss.Level == 4)
            {
                //boss level 4 > 400
                if (point >= 400)
                {
                    text = "S";
                }
                else if (point >= 300 && point < 400)
                {
                    text = "A";
                }
                else if (point >= 200 && point < 300)
                {
                    text = "B";
                }
                else if (point >= 10 && point < 200)
                {
                    text = "C";
                }
            }
            else if (Boss.Level == 5)
            {
                //boss level 5 > 500
                if (point >= 500)
                {
                    text = "S";
                }
                else if (point >= 375 && point < 500)
                {
                    text = "A";
                }
                else if (point >= 250 && point < 375)
                {
                    text = "B";
                }
                else if (point >= 10 && point < 250)
                {
                    text = "C";
                }
            }
            //moi level se xep loai theo cap so cong 25-50-75-100
            else
            {
                throw new Exception("Level Boss khong hop le");
            }
            player.Total_point += (int?)point;
            _context.Player_Characters.Update(player);
            await _context.SaveChangesAsync();
            return new Battle
            {
                point = (int?)point,
                text = text,
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Loi o server" + ex.Message);
        }
    }
    public async Task<Boss> createBoss(int Boss_id, string Name, int Level, int Hp, int Dame, int Def, int Location_x, int Location_y, int Location_z, double Difficult)
    {
        try
        {
            var boss = new Boss
            {
                Boss_id = Boss_id,
                Name = Name,
                Level = Level,
                Hp = Hp,
                Dame = Dame,
                Def = Def,
                Location_x = Location_x,
                Location_y = Location_y,
                Location_z = Location_z,
                Difficult = Difficult
            };
            _context.Boss.Add(boss);
            await _context.SaveChangesAsync();
            return boss;
        }
        catch (Exception ex)
        {
            throw new Exception("Loi khi tao Boss: " + ex.Message);
        }
    }
    public async Task<List<Boss>> GetAllBoss()
{
    try
    {
        var bosses = await _context.Boss.ToListAsync(); // Ensure the context property is named correctly (e.g., Bosses, not Boss)
        if (!bosses.Any())
        {
            throw new Exception("Không có Boss nào tồn tại");
        }
        return bosses; // Return the full list
    }
    catch (Exception ex)
    {
        throw new Exception("Lỗi khi lấy danh sách Boss: " + ex.Message);
    }
}
}