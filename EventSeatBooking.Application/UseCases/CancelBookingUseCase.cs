using EventSeatBooking.Application.Interfaces;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.Interfaces;

namespace EventSeatBooking.Application.UseCases
{
    public class CancelBookingUseCase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScreeningRepository _screeningRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelBookingUseCase(
            IBookingRepository bookingRepository,
            IScreeningRepository screeningRepository,
            IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _screeningRepository = screeningRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking is null)
                throw new DomainException($"Booking with id = {bookingId} not found");

            var screening = await _screeningRepository.GetByIdAsync(booking.ScreeningId);
            if (screening is null)
                throw new DomainException($"Screening with id = {booking.ScreeningId} not found");

            // Cancel the Booking first — if this throws (already cancelled), the seats
            // stay Reserved rather than being released for a cancellation that never happened.
            booking.Cancel();

            // Only after Booking confirms the cancellation is valid, release each seat back to Available.
            foreach (var bookedSeat in booking.Seats)
                screening.ReleaseSeat(bookedSeat.SeatNumber);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}