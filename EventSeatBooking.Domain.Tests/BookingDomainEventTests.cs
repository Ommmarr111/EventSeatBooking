using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.Events;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Tests
{
    public class BookingDomainEventTests
    {
        [Fact]
        public void AddSeat_ShouldRaiseSeatAdded()
        {
            // Arrange
            var booking = Booking.Create(1);

            // Act
            booking.AddSeat(SeatNumber.Of("A", 1));

            // Assert
            var domainEvent = Assert.Single(booking.DomainEvents);
            var seatAdded = Assert.IsType<SeatAdded>(domainEvent);
            Assert.Equal("A1", seatAdded.SeatNumber);
        }

        [Fact]
        public void AddSeat_WhenSeatLimitExceeded_ShouldNotRaiseAnAdditionalEvent()
        {
            // Arrange — fill up to the limit (6 events raised)
            var booking = Booking.Create(1);
            for (int i = 1; i <= 6; i++)
                booking.AddSeat(SeatNumber.Of("A", i));

            // Act — the 7th call should throw and add nothing
            Assert.Throws<DomainException>(() => booking.AddSeat(SeatNumber.Of("A", 7)));

            // Assert — still exactly 6 events, not 7
            Assert.Equal(6, booking.DomainEvents.Count);
        }

        [Fact]
        public void Confirm_ShouldRaiseBookingConfirmed_WithCorrectSeatCount()
        {
            // Arrange
            var booking = Booking.Create(1);
            booking.AddSeat(SeatNumber.Of("A", 1));
            booking.AddSeat(SeatNumber.Of("A", 2));

            // Act
            booking.Confirm();

            // Assert
            var confirmedEvent = booking.DomainEvents.OfType<BookingConfirmed>().Single();
            Assert.Equal(1, confirmedEvent.CustomerId);
            Assert.Equal(2, confirmedEvent.SeatCount);
        }

        [Fact]
        public void Confirm_WhenNoSeats_ShouldThrow_AndNotRaiseAnyEvent()
        {
            // Arrange
            var booking = Booking.Create(1);

            // Act & Assert
            Assert.Throws<DomainException>(() => booking.Confirm());
            Assert.Empty(booking.DomainEvents);
        }

        [Fact]
        public void Cancel_ShouldRaiseBookingCancelled()
        {
            // Arrange
            var booking = Booking.Create(1);
            booking.AddSeat(SeatNumber.Of("A", 1));

            // Act
            booking.Cancel();

            // Assert
            var cancelledEvent = booking.DomainEvents
                .OfType<BookingCancelled>()
                .Single();
        }

        [Fact]
        public void Cancel_WhenAlreadyCancelled_ShouldThrow_AndNotRaiseASecond()
        {
            // Arrange
            var booking = Booking.Create(1);
            booking.AddSeat(SeatNumber.Of("A", 1));
            booking.Cancel();

            // Act & Assert — second cancel attempt should throw, not add a second event
            Assert.Throws<DomainException>(() => booking.Cancel());
            Assert.Single(booking.DomainEvents.OfType<BookingCancelled>());
        }

        [Fact]
        public void ClearDomainEvents_ShouldEmptyTheCollection()
        {
            // Arrange
            var booking = Booking.Create(1);
            booking.AddSeat(SeatNumber.Of("A", 1));
            booking.Confirm();
            Assert.NotEmpty(booking.DomainEvents);

            // Act
            booking.ClearDomainEvents();

            // Assert
            Assert.Empty(booking.DomainEvents);
        }
    }
}