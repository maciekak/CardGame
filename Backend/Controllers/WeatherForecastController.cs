using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DatabaseAccessLayer;
using Backend.DatabaseAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UsersSqlServerContext _context;  

        public WeatherForecastController(ILogger<WeatherForecastController> logger, UsersSqlServerContext context, IDistributedCache cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            await _context.Users.AddAsync(new User { Password = "123", Username = "username" });
            await _context.SaveChangesAsync();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }

        [HttpGet("user")]
        public async Task<IEnumerable<User>> GetUser()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("redis/save")]
        public async Task GetRedisSave()
        {
            await _cache.SetStringAsync("myName", "naveen", new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)});
        }

        [HttpGet("redis/read")]
        public async Task<string> GetRedisRead()
        {
            var result = await _cache.GetStringAsync("myName");
            return result;
        }
    }
}