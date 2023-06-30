using MAuth.AspNetCore.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.MySql
{
    public class MAuthDbContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {

        public virtual DbSet<Carousel> Carousels { get; set; }

        public virtual DbSet<Category> Categories { get; set; }
        public MAuthDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Meeting>(build =>
            //{
            //    build.Property(p => p.ConcurrencyToken).IsConcurrencyToken();
            //});
            modelBuilder.Entity<User>(build =>
            {
                build.HasIndex(p => p.PhoneNumber);
                //build.HasMany(p => p.Articles)
                //.WithOne(p => p.User)
                //.HasForeignKey(p => p.UserId);
                //build.HasOne(p => p.DoctorCetificate)
                //.WithOne(p => p.User)
                //.HasForeignKey(p => p.);
                //build.HasMany(x => x.Articles)
                //.WithOne(x => x.User)
                //.HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
