using System.Linq;
using System.Threading.Tasks;
using Backend.DatabaseAccessLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.DatabaseAccessLayer.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private const int BeginningRankingScore = 1000;
        
        private readonly UsersSqlServerContext _context;

        public PlayerRepository(UsersSqlServerContext context)
        {
            _context = context;
        }

        public async Task<Player?> GetPlayerByNameAsync(string name)
        {
            return await _context.Players.FirstOrDefaultAsync(player => player.Name == name);
        }

        public async Task<Player> AddUser(string name)
        {
            var newPlayer = new Player { Name = name, RankingPoints = BeginningRankingScore };
            await _context.Players.AddAsync(newPlayer);
            await _context.SaveChangesAsync();
            return newPlayer;
        }
    }
}