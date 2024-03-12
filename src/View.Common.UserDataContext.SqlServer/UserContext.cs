using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace View.Shared
{
    public partial class UserContext : IdentityDbContext<ApplicationUser>
    {
        public UserContext() : base()
        {
            // Ensure the database is created. This is only used during development
            // and not suitable for production. In a production environment, use
            // migrations to create the database.
            Database.EnsureCreated();
        }

        public UserContext(DbContextOptions options)
            : base(options)
        {
            // Ensure the database is created. This is only used during development
            // and not suitable for production. In a production environment, use
            // migrations to create the database.
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Users;Integrated Security=true;Encrypt=false;");
            }
        }
    }
}