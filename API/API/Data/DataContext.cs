using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<BusRoute> BusRoutes { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<CoDriver> CoDrivers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<New> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bus -> BusRoute
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.BusRoute)
                .WithMany(br => br.Buses)
                .HasForeignKey(b => b.BusRouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trip -> BusRoute
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.BusRoute)
                .WithMany(br => br.Trips)
                .HasForeignKey(t => t.BusRouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trip -> Bus
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Bus)
                .WithMany(br => br.Trips)
                .HasForeignKey(t => t.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Driver -> BusRoute
            modelBuilder.Entity<Driver>()
                .HasOne(t => t.BusRoute)
                .WithMany(br => br.Drivers)
                .HasForeignKey(t => t.BusRouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // CoDriver -> BusRoute
            modelBuilder.Entity<CoDriver>()
                .HasOne(t => t.BusRoute)
                .WithMany(br => br.CoDrivers)
                .HasForeignKey(t => t.BusRouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trip -> CoDriver
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.CoDriver)
                .WithMany(br => br.Trips)
                .HasForeignKey(t => t.CoDriverId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trip -> Driver
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Driver)
                .WithMany(br => br.Trips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Cascade);
            // Ticket -> Trip
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Trip)
                .WithMany(br => br.Tickets)
                .HasForeignKey(t => t.TripId)
                .OnDelete(DeleteBehavior.Cascade);
            // Seat -> Trip
            modelBuilder.Entity<Seat>()
                .HasOne(t => t.Trip)
                .WithMany(br => br.Seats)
                .HasForeignKey(t => t.TripId)
                .OnDelete(DeleteBehavior.Cascade);
            // Seat -> Ticket
            modelBuilder.Entity<Seat>()
                .HasOne(t => t.Ticket)
                .WithMany(br => br.Seats)
                .HasForeignKey(t => t.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
            // Account -> Staff 
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Staff) 
                .WithOne(s => s.Account)
                .HasForeignKey<Account>(a => a.StaffId) 
                .OnDelete(DeleteBehavior.Cascade);
            // Staff -> Account
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Account)
                .WithOne(a => a.Staff)
                .HasForeignKey<Staff>(s => s.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}