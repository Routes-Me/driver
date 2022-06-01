using driver_service.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace driver_service.Models
{
    public partial class DriverContext : DbContext
    {
        public DriverContext(DbContextOptions<DriverContext> options) : base(options)
        {

        }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<DriverVehicle> DriverVehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.DriverId)
                    .HasName("PRIMARY");

                entity.ToTable("drivers");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.InvitationId).HasColumnName("invitation_id");

                entity.Property(e => e.AvatarUrl)
                    .HasColumnName("avatar_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                //entity.HasMany(d=>d.DriverVehicles)
                //.WithOne(d=>d.Driver)


            });
            modelBuilder.Entity<DriverVehicle>(entity =>
            {
                entity.HasKey(e => e.DriverVehicleId)
                    .HasName("PRIMARY");

                entity.ToTable("driverVehicles");
                entity.HasIndex(e => e.DriverId)
                   .HasName("driver_id");


                entity.Property(e => e.DriverId).HasColumnName("driver_id");
                entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

                entity.HasOne(d => d.Driver)
                .WithMany(d => d.DriverVehicles)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("driver_id_ibfk_1")
                .OnDelete(DeleteBehavior.Cascade);

            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
