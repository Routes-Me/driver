using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DriverService.Models.DBModels
{
    public partial class DriversServiceContext : DbContext
    {
        public DriversServiceContext()
        {
        }

        public DriversServiceContext(DbContextOptions<DriversServiceContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Driver> Drivers { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@Directory.GetCurrentDirectory() + "/../driver-service/appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseMySql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.DriverId)
                    .HasName("PRIMARY");

                entity.ToTable("drivers");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.InstitutionId).HasColumnName("institution_id");

                entity.Property(e => e.AvatarUrl)
                    .HasColumnName("avatar_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");


            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}

