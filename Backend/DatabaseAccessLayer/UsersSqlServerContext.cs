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

        public DbSet<User> Users { get; set; }
    }
}
