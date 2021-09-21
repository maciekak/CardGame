using System;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.DatabaseAccessLayer.Model;
using Backend.DatabaseAccessLayer.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Backend.Managers
{
    public class LoginManager : ILoginManager
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IDistributedCache _cache;

        public LoginManager(IPlayerRepository playerRepository, IDistributedCache cache)
        {
            _playerRepository = playerRepository;
            _cache = cache;
        }

        public async Task<string> CreatePlayerAsync(string name)
        {
            var player = await _playerRepository.AddUser(name);
            var playerSessionId = await SetPlayerSessionId(player.Id.ToString());
            return await AddPlayerToCache(player, playerSessionId);
        }

        public async Task<string> LoginPlayerByNameAsync(string name)
        {
            var player = await _playerRepository.GetPlayerByNameAsync(name)
                         ?? throw new ArgumentException("User not found");

            var id = player.Id.ToString();
            var playerSessionId = await GetAndRenewPlayerSessionId(id);
            if (playerSessionId != null)
            {
                return playerSessionId;
            }
            
            playerSessionId = await SetPlayerSessionId(id);
            return await AddPlayerToCache(player, playerSessionId);
        }

        public async Task LogoutPlayerAsync(string uuid)
        {
            var player = await GetPlayerFromCache(uuid);
            await Task.WhenAll(_cache.RemoveAsync(uuid), _cache.RemoveAsync(player.Id.ToString()));
        }

        private async Task<string> SetPlayerSessionId(string id)
        {
            var playerSessionId = Guid.NewGuid().ToString();
            await _cache.SetStringAsync(
                id,
                playerSessionId,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });
            return playerSessionId;
        }

        private async Task<string?> GetAndRenewPlayerSessionId(string id)
        {
            var currentId = await _cache.GetStringAsync(id);
            if (currentId == null)
            {
                return null;
            }

            await _cache.SetStringAsync(
                id,
                currentId,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });
            return currentId;
        }

        private async Task<string> AddPlayerToCache(Player player, string playerSessionId)
        {
            var serializedPlayer = JsonSerializer.Serialize(player);
            await _cache.SetStringAsync(playerSessionId, serializedPlayer);
            return playerSessionId;
        }

        private async Task<Player> GetPlayerFromCache(string playerSessionId)
        {
            var value = await _cache.GetStringAsync(playerSessionId)
                ?? throw new ArgumentException("Incorrect player session id");

            return JsonSerializer.Deserialize<Player>(value) ?? throw new ArgumentException("Incorrect player session id");
        }
    }
}