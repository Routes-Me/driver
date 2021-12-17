using Microsoft.EntityFrameworkCore;

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
        public virtual DbSet<Phone> Phone { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Phone>(entity =>
            {
                entity.HasKey(e => e.PhoneId)
                    .HasName("PRIMARY");

                entity.ToTable("phone");

                entity.HasIndex(e => e.DriverId)
                    .HasName("driver_id");

                entity.Property(e => e.PhoneId).HasColumnName("phone_id");

                entity.Property(e => e.Number)
                    .HasColumnName("number")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");

            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.DriverId)
                    .HasName("PRIMARY");

                entity.ToTable("drivers");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(40)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}

