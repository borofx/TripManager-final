using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TripManager.Models;

namespace TripManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {        

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Landmark> Landmarks { get; set; }
        public DbSet<TourLandmark> TourLandmarks { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TourLandmark>()
                .HasKey(tl => new { tl.TourId, tl.LandmarkId });

            builder.Entity<TourLandmark>()
                .HasOne(tl => tl.Tour)
                .WithMany(t => t.TourLandmarks)
                .HasForeignKey(tl => tl.TourId);

            builder.Entity<TourLandmark>()
                .HasOne(tl => tl.Landmark)
                .WithMany()
                .HasForeignKey(tl => tl.LandmarkId);
        }
    
    }
}
