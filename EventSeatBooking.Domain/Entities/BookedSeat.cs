using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Entities
{
    public class BookedSeat
    {
        public int Id { get; private set; }
        public SeatNumber SeatNumber { get; private set; }

        private BookedSeat(SeatNumber seatNumber)
        {
            SeatNumber = seatNumber;
        }

        public static BookedSeat Create(SeatNumber seatNumber) =>
            new BookedSeat(seatNumber);
    }
}