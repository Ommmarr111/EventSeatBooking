using EventSeatBooking.Domain.Enums;
using EventSeatBooking.Domain.Events;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Entities
{
    public class Booking
    {
        private const int MaxSeatsPerBooking = 6;
        private readonly List<BookedSeat> _seats = new();
        private readonly List<IDomainEvent> _domainEvents = new();

        public int Id { get; private set; }

        public int ScreeningId { get; private set; }

        public int CustomerId { get; private set; }
        public BookingStatus Status { get; private set; }
        public IReadOnlyCollection<BookedSeat> Seats => _seats.AsReadOnly();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private Booking()
        {

        }
        private Booking(int customerId, int screeningId)
        {
            CustomerId = customerId;
            Status = BookingStatus.Pending;
            ScreeningId = screeningId;
        }

        public static Booking Create(int customerId, int screeningId)
        {
            if (customerId <= 0)
                throw new DomainException("Invalid customer.");
            return new Booking(customerId, screeningId);
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
            _domainEvents.Add(new SeatAdded(Id, seatNumber.ToString()));
        }

        public void Confirm()
        {
            if (Status == BookingStatus.Cancelled)
                throw new DomainException("Cancelled booking cannot be confirmed.");

            if (!_seats.Any())
                throw new DomainException("Cannot confirm a booking with no seats.");

            Status = BookingStatus.Confirmed;
            _domainEvents.Add(new BookingConfirmed(Id, CustomerId, _seats.Count));
        }

        public void Cancel()
        {
            if (Status == BookingStatus.Cancelled)
                throw new DomainException("Booking is already cancelled.");

            Status = BookingStatus.Cancelled;
            _domainEvents.Add(new BookingCancelled(Id));
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}