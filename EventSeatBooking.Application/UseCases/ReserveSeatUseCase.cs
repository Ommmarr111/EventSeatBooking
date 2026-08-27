using EventSeatBooking.Application.Interfaces;
using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.Interfaces;
using EventSeatBooking.Domain.Services;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Application.UseCases
{
    public class ReserveSeatUseCase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScreeningRepository _screeningRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SeatAvailabilityService _seatAvailabilityService;

        public ReserveSeatUseCase(
            IBookingRepository bookingRepository,
            IScreeningRepository screeningRepository,
            IUnitOfWork unitOfWork,
            SeatAvailabilityService seatAvailabilityService)
        {
            _bookingRepository = bookingRepository;
            _screeningRepository = screeningRepository;
            _unitOfWork = unitOfWork;
            _seatAvailabilityService = seatAvailabilityService;
        }

        public async Task<int> ExecuteAsync(int screeningId, int customerId, string seatRow, int seatColumnNumber)
        {
            // 1. Load the Screening aggregate
            var screening = await _screeningRepository.GetByIdAsync(screeningId);
            if (screening is null)
                throw new DomainException($"Screening with id = {screeningId} not found");

            // 2. Create a new Booking for this customer + screening.
            //    (Simplification, stated plainly: this always creates a fresh Booking per reservation
            //    call, rather than checking for an existing Pending booking to add to. Extending that
            //    is straightforward but out of scope for what this project needs to demonstrate.)
            var booking = Booking.Create(customerId, screeningId);

            var seatNumber = SeatNumber.Of(seatRow, seatColumnNumber);

            // 3. Domain Service coordinates both aggregates in the correct order —
            //    if the seat isn't available, this throws and nothing below runs.
            _seatAvailabilityService.ReserveSeatForBooking(screening, booking, seatNumber);

            // 4. Only now, after the domain operation succeeded, persist the new Booking.
            await _bookingRepository.AddAsync(booking);

            // 5. One commit for both aggregates — Screening's seat status change (tracked already
            //    since it was loaded from this same DbContext) and the new Booking, together.
            await _unitOfWork.SaveChangesAsync();

            return booking.Id;
        }
    }
}