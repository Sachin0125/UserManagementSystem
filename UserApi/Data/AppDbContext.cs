using System;
using UserApi.Modals;
using Microsoft.EntityFrameworkCore;
namespace UserApi.Data
{
    public class AppDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        //public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
