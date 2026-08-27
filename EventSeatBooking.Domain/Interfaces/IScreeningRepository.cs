using EventSeatBooking.Domain.Entities;

namespace EventSeatBooking.Domain.Interfaces
{
    public interface IScreeningRepository
    {
        Task<Screening?> GetByIdAsync(int id);
        Task AddAsync(Screening screening);
    }
}