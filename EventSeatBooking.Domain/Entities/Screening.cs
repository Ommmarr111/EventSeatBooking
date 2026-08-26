using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Entities
{
    public class Screening
    {
        private readonly List<Seat> _seats = new();

        public int Id { get; private set; }
        public string Title { get; private set; }
        public DateTime ShowTime { get; private set; }
        public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();

        private Screening(string title, DateTime showTime)
        {
            Title = title;
            ShowTime = showTime;
        }

        public static Screening Create(string title, DateTime showTime, IEnumerable<SeatNumber> seatNumbers)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Screening title cannot be empty.");

            if (showTime <= DateTime.UtcNow)
                throw new DomainException("Screening time must be in the future.");

            var screening = new Screening(title, showTime);

            foreach (var seatNumber in seatNumbers)
                screening._seats.Add(Seat.Create(seatNumber));

            return screening;
        }

        public Seat GetSeat(SeatNumber seatNumber)
        {
            var seat = _seats.FirstOrDefault(s => s.SeatNumber.Equals(seatNumber));

            if (seat is null)
                throw new DomainException($"Seat {seatNumber} does not exist for this screening.");

            return seat;
        }

        public void ReserveSeat(SeatNumber seatNumber) => GetSeat(seatNumber).Reserve();

        public void ReleaseSeat(SeatNumber seatNumber) => GetSeat(seatNumber).Release();

        public void ConfirmSeatBooking(SeatNumber seatNumber) => GetSeat(seatNumber).ConfirmBooking();
    }
}