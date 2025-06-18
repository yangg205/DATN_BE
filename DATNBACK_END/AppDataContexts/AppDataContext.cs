using DATNBACK_END.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DATNBACK_END.AppDataContexts;

public class AppDataContext : DbContext
{
    private DbSetting _DbSetting;

    public AppDataContext(IOptions<DbSetting> DbSetting)
    {
        _DbSetting = DbSetting.Value;
    }

    public DbSet<Player> Player { get; set; }
    public DbSet<PendingPlayer> PendingPlayer { get; set; }
    public DbSet<Characters> Characters { get; set; }
    public DbSet<Player_Characters> Player_Characters { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<Transactions> Transactions { get; set; }
    public DbSet<Achievement> Achievement { get; set; }
    public DbSet<Skill_tree> Skill_tree { get; set; }
    public DbSet<Player_Character_Skill> Player_Character_Skill { get; set; }
    public DbSet<Boss> Boss { get; set; }
    public DbSet<Player_Character_Battle> Player_Character_Battle { get; set; }

    //ket noi voi database
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_DbSetting.ConnectionString);
    }

    //thuc hien cau hinh cac quy tac
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().ToTable("Player").HasKey(x => x.Player_id);
        modelBuilder.Entity<Player_Character_Skill>().ToTable("Player_Character_Skill").HasKey(x => x.Player_Skill_id);
        modelBuilder.Entity<Characters>().ToTable("Characters").HasKey(x => x.Characters_id);
        modelBuilder.Entity<Player_Characters>().ToTable("Player_Characters").HasKey(x => x.Player_Character_id);
        modelBuilder.Entity<Item>().ToTable("Item").HasKey(x => x.Item_id);
        modelBuilder.Entity<Transactions>().ToTable("Transactions").HasKey(x => x.Transaction_id);
        modelBuilder.Entity<Achievement>().ToTable("Achievement").HasKey(x => x.Achievement_id);
        modelBuilder.Entity<Skill_tree>().ToTable("Skill_tree").HasKey(x => x.Skill_tree_id);
        modelBuilder.Entity<Boss>().ToTable("Boss").HasKey(x => x.Boss_id);
        modelBuilder.Entity<Player_Character_Battle>().ToTable("Player_Character_Battle").HasKey(x => x.Player_battle_id);
        modelBuilder.Entity<PendingPlayer>().ToTable("PendingPlayer").HasKey(x => x.PendingPlayer_id);
    }

    internal class AppDataContexts { }
}
