using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventSeatBooking.Infrastructure.Persistence.Repositories
{
    public class ScreeningRepository : IScreeningRepository
    {
        private readonly EventSeatBookingDbContext _context;

        public ScreeningRepository(EventSeatBookingDbContext context)
        {
            _context = context;
        }

        public async Task<Screening?> GetByIdAsync(int id)
        {
            return await _context.Screenings
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Screening screening)
        {
            await _context.Screenings.AddAsync(screening);
        }

    }
}