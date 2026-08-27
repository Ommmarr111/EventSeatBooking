using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.Enums;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Tests
{
    public class BookingTests
    {
        [Fact]
        public void Create_WithValidCustomerId_ShouldCreatePendingBooking()
        {
            // Act
            var booking = Booking.Create(customerId: 1, 1);

            // Assert
            Assert.Equal(1, booking.CustomerId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        public void Create_WithInvalidCustomerId_ShouldThrowDomainException(int invalidId, int screeningId)
        {
            // Act & Assert
            var ex = Assert.Throws<DomainException>(() => Booking.Create(invalidId, screeningId));
            Assert.Equal("Invalid customer.", ex.Message);
        }

        [Fact]
        public void Confirm_OnPendingBooking_ShouldSetStatusToConfirmed()
        {
            // Arrange
            var booking = Booking.Create(1, 1);
            booking.AddSeat(SeatNumber.Of("A", 1));

            // Act
            booking.Confirm();

            // Assert
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact]
        public void Confirm_OnCancelledBooking_ShouldThrowDomainException()
        {
            // Arrange
            var booking = Booking.Create(1, 1);
            booking.AddSeat(SeatNumber.Of("A", 1)); // ← add this line

            booking.Cancel();

            // Act & Assert
            var ex = Assert.Throws<DomainException>(() => booking.Confirm());
            Assert.Equal("Cancelled booking cannot be confirmed.", ex.Message);
        }

        [Fact]
        public void Cancel_OnPendingBooking_ShouldSetStatusToCancelled()
        {
            // Arrange
            var booking = Booking.Create(1, 1);

            // Act
            booking.Cancel();

            // Assert
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }

        [Fact]
        public void Cancel_OnConfirmedBooking_ShouldSetStatusToCancelled()
        {
            // Arrange 
            var booking = Booking.Create(1, 1);
            booking.AddSeat(SeatNumber.Of("A", 1)); // ← add this line

            booking.Confirm();

            // Act
            booking.Cancel();

            // Assert
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }

        [Fact]
        public void Cancel_OnAlreadyCancelledBooking_ShouldThrowDomainException()
        {
            // Arrange
            var booking = Booking.Create(1, 1);
            booking.Cancel();

            // Act & Assert
            var ex = Assert.Throws<DomainException>(() => booking.Cancel());
            Assert.Equal("Booking is already cancelled.", ex.Message);
        }
    }
}