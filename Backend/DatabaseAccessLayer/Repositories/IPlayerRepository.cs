using System.Threading.Tasks;
using Backend.DatabaseAccessLayer.Model;

namespace Backend.DatabaseAccessLayer.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player?> GetPlayerByNameAsync(string name);
        Task<Player> AddUser(string name);
    }
}