namespace EventSeatBooking.Domain.Events
{
    public record SeatAdded(int BookingId, string SeatNumber) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }

    public record BookingConfirmed(int BookingId, int CustomerId, int SeatCount) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }

    public record BookingCancelled(int BookingId) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}