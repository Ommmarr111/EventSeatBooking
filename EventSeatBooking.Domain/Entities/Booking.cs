using EventSeatBooking.Domain.Enums;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Entities
{
    public class Booking
    {
        private const int MaxSeatsPerBooking = 6;
        private readonly List<BookedSeat> _seats = new();

        public int Id { get; private set; }
        public int CustomerId { get; private set; }
        public BookingStatus Status { get; private set; }
        public IReadOnlyCollection<BookedSeat> Seats => _seats.AsReadOnly();

        private Booking(int customerId)
        {
            CustomerId = customerId;
            Status = BookingStatus.Pending;
        }

        public static Booking Create(int customerId)
        {
            if (customerId <= 0)
                throw new DomainException("Invalid customer.");
            return new Booking(customerId);
        }

        public void AddSeat(SeatNumber seatNumber)
        {
            if (Status != BookingStatus.Pending)
                throw new DomainException("Seats can only be added to a pending booking.");

            if (_seats.Count >= MaxSeatsPerBooking)
                throw new DomainException($"Cannot book more than {MaxSeatsPerBooking} seats.");

            if (_seats.Any(s => s.SeatNumber.Equals(seatNumber)))
                throw new DomainException($"Seat {seatNumber} is already added to this booking.");

            _seats.Add(BookedSeat.Create(seatNumber));
        }

        public void Confirm()
        {
            if (Status == BookingStatus.Cancelled)
                throw new DomainException("Cancelled booking cannot be confirmed.");

            if (!_seats.Any())
                throw new DomainException("Cannot confirm a booking with no seats.");

            Status = BookingStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == BookingStatus.Cancelled)
                throw new DomainException("Booking is already cancelled.");

            Status = BookingStatus.Cancelled;
        }
    }
}