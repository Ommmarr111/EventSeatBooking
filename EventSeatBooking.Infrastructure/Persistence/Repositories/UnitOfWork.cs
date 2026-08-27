using EventSeatBooking.Application.Interfaces;

namespace EventSeatBooking.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventSeatBookingDbContext _context;

        public UnitOfWork(EventSeatBookingDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}