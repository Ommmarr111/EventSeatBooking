using EventSeatBooking.Application.Interfaces;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.Interfaces;

namespace EventSeatBooking.Application.UseCases
{
    public class ConfirmBookingUseCase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmBookingUseCase(IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking is null)
                throw new DomainException($"Booking with id = {bookingId} not found");

            booking.Confirm(); // throws DomainException if invariants aren't met (no seats, already cancelled)

            await _unitOfWork.SaveChangesAsync();
        }
    }
}