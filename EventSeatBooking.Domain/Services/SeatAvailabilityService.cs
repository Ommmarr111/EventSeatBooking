using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Services
{
    public class SeatAvailabilityService
    {
        /// <summary>
        /// Attempts to reserve a seat on the given screening and add it to the booking.
        /// If the seat isn't available, the screening throws and the booking is left untouched —
        /// this method doesn't catch that exception, it lets the caller decide how to handle it.
        /// </summary>
        public void ReserveSeatForBooking(Screening screening, Booking booking, SeatNumber seatNumber)
        {
            // 1. Ask the Screening aggregate to reserve the seat.
            //    This throws a DomainException if the seat isn't Available.
            screening.ReserveSeat(seatNumber);

            // 2. Only if that succeeded, add it to the Booking aggregate.
            booking.AddSeat(seatNumber);
        }
    }
}