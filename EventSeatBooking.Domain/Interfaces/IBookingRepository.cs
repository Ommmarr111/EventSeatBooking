using EventSeatBooking.Domain.Entities;

namespace EventSeatBooking.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int id);
        Task AddAsync(Booking booking);
        Task SaveChangesAsync();
    }
}