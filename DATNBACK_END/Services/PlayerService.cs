using DATNBACK_END.Models;

namespace DATNBACK_END.Services;
public class PlayerService : IPlayerService
{
    private readonly AppDataContexts.AppDataContext _context;
    public PlayerService(AppDataContexts.AppDataContext context)
    {
        _context = context;
    }
    public Task<Player> AddPlayer( string email,string name,string Password)
    {
        try
        {

            var Player = _context.Player.FirstOrDefault(x => x.Name == name);
            if (Player != null)
            {
                throw new Exception("Player is already exist");
            }
            var PLayerResult = new Player
            {
                Email = email,
                Name = name,
                Password = Password,
                Repassword = Password,
                Password_Salt = Password,
                Total_coin = 0,
                Total_Money = 0,
            };
            _context.Player.Add(PLayerResult);
            _context.SaveChanges();
            return Task.FromResult(PLayerResult);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException.Message);
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
