using Backend.DatabaseAccessLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.DatabaseAccessLayer
{
    public sealed class UsersSqlServerContext : DbContext
    {

        public UsersSqlServerContext(DbContextOptions<UsersSqlServerContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
    }
}
