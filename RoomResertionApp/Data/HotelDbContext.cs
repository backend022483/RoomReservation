using Microsoft.EntityFrameworkCore;
using RoomResertionApp.Models;

namespace RoomResertionApp.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<LoginActivity> LoginActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20).HasConversion<string>();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.FailedLoginAttempts).IsRequired();
                entity.Property(e => e.LockoutEndDate);
            });

            // Configure Room entity
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PricePerNight).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.Capacity).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsAvailable).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(255);
            });

            // Configure Guest entity
            modelBuilder.Entity<Guest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

            // Configure Reservation entity
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CheckInDate).IsRequired();
                entity.Property(e => e.CheckOutDate).IsRequired();
                entity.Property(e => e.NumberOfGuests).IsRequired();
                entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BookingDate).IsRequired();
                entity.Property(e => e.ConfirmationNumber).HasMaxLength(50);

                entity.HasOne(e => e.Room)
                    .WithMany()
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Guest)
                    .WithMany()
                    .HasForeignKey(e => e.GuestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure LoginActivity entity
            modelBuilder.Entity<LoginActivity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Success).IsRequired();
                entity.Property(e => e.ErrorMessage).HasMaxLength(255);
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.UserRole).HasMaxLength(20);
            });
        }
    }
}
