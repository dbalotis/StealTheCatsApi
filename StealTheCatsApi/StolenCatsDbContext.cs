using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StolenCatsData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StolenCatsData
{
    public class StolenCatsDbContext(DbContextOptions<StolenCatsDbContext> options) : DbContext(options)
    {
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cat>()
                .HasIndex(c => c.CatId)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
