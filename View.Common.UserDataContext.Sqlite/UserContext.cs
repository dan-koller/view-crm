﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using View.Shared; // ApplicationUser

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
                string dir = Environment.CurrentDirectory;
                string path = string.Empty;

                if (dir.EndsWith("net7.0"))
                {
                    // Running in the <project>\bin\<Debug|Release>\net7.0 directory.
                    path = Path.Combine("..", "..", "..", "..", "Users.db");
                }
                else
                {
                    // Running in the <project> directory.
                    path = Path.Combine("..", "Users.db");
                }

                optionsBuilder.UseSqlite($"Filename={path}");
            }
        }
    }
}