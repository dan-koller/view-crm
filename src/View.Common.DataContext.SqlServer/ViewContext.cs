﻿using Microsoft.EntityFrameworkCore;

namespace View.Shared
{
    public class ViewContext : DbContext
    {
        public ViewContext()
        {
            // Ensure the database is created. This is only used during development
            // and not suitable for production. In a production environment, use
            // migrations to create the database.
            Database.EnsureCreated();
        }

        public ViewContext(DbContextOptions<ViewContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Ticket> Tickets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Tickets;Integrated Security=true;Encrypt=false;");
            }
        }
    }
}