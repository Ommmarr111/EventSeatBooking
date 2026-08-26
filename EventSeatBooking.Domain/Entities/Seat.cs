using EventSeatBooking.Domain.Enums;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Entities
{
    public class Seat
    {
        public SeatNumber SeatNumber { get; private set; }
        public SeatStatus Status { get; private set; }

        private Seat(SeatNumber seatNumber)
        {
            SeatNumber = seatNumber;
            Status = SeatStatus.Available;
        }

        public static Seat Create(SeatNumber seatNumber) => new Seat(seatNumber);

        public void Reserve()
        {
            if (Status != SeatStatus.Available)
                throw new Exceptions.DomainException($"Seat {SeatNumber} is not available.");

            Status = SeatStatus.Reserved;
        }

        public void Release()
        {
            Status = SeatStatus.Available;
        }

        public void ConfirmBooking()
        {
            if (Status != SeatStatus.Reserved)
                throw new Exceptions.DomainException($"Seat {SeatNumber} must be reserved before it can be booked.");

            Status = SeatStatus.Booked;
        }
    }
}