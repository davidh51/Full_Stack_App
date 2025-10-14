using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace api.Data
{
    //public class ApplicationDBContext : DbContext  //Without Identity
    public class ApplicationDBContext : IdentityDbContext<AppUser> //With Identity
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Portfolio>(x => x.HasKey(p => new { p.AppUserId, p.StockId })); //Foreign key creation

            builder.Entity<Portfolio>()
                .HasOne(u => u.AppUser)
                .WithMany(p => p.Portfolios)
                .HasForeignKey(u => u.AppUserId); //Navigation property configuration

            builder.Entity<Portfolio>()
                .HasOne(s => s.Stock)
                .WithMany(p => p.Portfolios)
                .HasForeignKey(s => s.StockId); //Navigation property configuration

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "1f8d9f93-b275-4212-bc8e-194657cfd568", // static id avoid db EF issues and duplicate roles
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2c9e1e3a-1d3b-4f5a-9c4e-2b6f8e9d7a12", // static id avoid db EF issues and duplicate roles
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}