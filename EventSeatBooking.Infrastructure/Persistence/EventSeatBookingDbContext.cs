using EventSeatBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSeatBooking.Infrastructure.Persistence
{
    public class EventSeatBookingDbContext : DbContext
    {
        public EventSeatBookingDbContext(DbContextOptions<EventSeatBookingDbContext> options)
            : base(options) { }

        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Screening> Screenings => Set<Screening>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventSeatBookingDbContext).Assembly);
        }
    }
}