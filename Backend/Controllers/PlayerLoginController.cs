using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerLoginController : ControllerBase
    {
        private readonly ILogger<PlayerLoginController> _logger;
        private readonly ILoginManager _loginManager;  

        public PlayerLoginController(ILogger<PlayerLoginController> logger, ILoginManager loginManager)
        {
            _logger = logger;
            _loginManager = loginManager;
        }
        
        [HttpPost("login")]
        public async Task<string> Login(PlayerNameRequestDto dto)
        {
            return await _loginManager.LoginPlayerByNameAsync(dto.Name);
        }
        
        [HttpPost("logout")]
        public async Task Logout(PlayerSessionIdRequestDto dto)
        {
            await _loginManager.LogoutPlayerAsync(dto.PlayerSessionId);
        }
        
        [HttpPost("register")]
        public async Task<string> Register(PlayerNameRequestDto dto)
        {
            return await _loginManager.CreatePlayerAsync(dto.Name);
        }
    }
}