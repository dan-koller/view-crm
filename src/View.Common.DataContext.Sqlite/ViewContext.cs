using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace View.Shared
{
    public partial class ViewContext : DbContext
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
                string dir = Environment.CurrentDirectory;
                string path = string.Empty;

                if (dir.EndsWith("net7.0"))
                {
                    // Running in the <project>\bin\<Debug|Release>\net7.0 directory.
                    path = Path.Combine("..", "..", "..", "..", "View.db");
                }
                else
                {
                    // Running in the <project> directory.
                    path = Path.Combine("..", "View.db");
                }

                optionsBuilder.UseSqlite($"Filename={path}");
            }
        }
    }
}