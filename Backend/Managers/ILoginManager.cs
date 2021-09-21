using System.Threading.Tasks;

namespace Backend.Managers
{
    public interface ILoginManager
    {
        Task<string> CreatePlayerAsync(string name);
        Task<string> LoginPlayerByNameAsync(string name);
        Task LogoutPlayerAsync(string uuid);
    }
}