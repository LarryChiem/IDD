using AdminUI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminUI.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
