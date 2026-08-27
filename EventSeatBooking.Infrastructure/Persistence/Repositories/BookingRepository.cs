using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventSeatBooking.Infrastructure.Persistence.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly EventSeatBookingDbContext _context;

        public BookingRepository(EventSeatBookingDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Seats)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

    }
}