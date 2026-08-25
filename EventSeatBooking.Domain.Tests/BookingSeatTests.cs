using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.Enums;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Tests
{
    public class BookingSeatTests
    {
        [Fact]
        public void AddSeat_ToPendingBooking_ShouldAddSeat()
        {
            // Arrange
            var booking = Booking.Create(1);
            var seat = SeatNumber.Of("A", 1);

            // Act
            booking.AddSeat(seat);

            // Assert
            Assert.Single(booking.Seats);
        }

        [Fact]
        public void AddSeat_DuplicateSeat_ShouldThrowDomainException()
        {
            // Arrange
            var booking = Booking.Create(1);
            var seat = SeatNumber.Of("A", 1);
            booking.AddSeat(seat);

            // Act & Assert
            var ex = Assert.Throws<DomainException>(() => booking.AddSeat(seat));
            Assert.Contains("already added", ex.Message);
        }

        [Fact]
        public void AddSeat_SeventhSeat_ShouldThrowDomainException()
        {
            // Arrange
            var booking = Booking.Create(1);
            for (int i = 1; i <= 6; i++)
                booking.AddSeat(SeatNumber.Of("A", i));

            // Act & Assert — the 7th seat should be rejected
            var ex = Assert.Throws<DomainException>(() => booking.AddSeat(SeatNumber.Of("A", 7)));
            Assert.Contains("Cannot book more than 6 seats", ex.Message);
        }

        [Fact]
        public void AddSeat_ExactlySixSeats_ShouldSucceed()
        {
            // Arrange
            var booking = Booking.Create(1);

            // Act
            for (int i = 1; i <= 6; i++)
                booking.AddSeat(SeatNumber.Of("A", i));

            // Assert — boundary case: 6 is allowed, only 7+ should fail
            Assert.Equal(6, booking.Seats.Count);
        }

        [Fact]
        public void AddSeat_ToConfirmedBooking_ShouldThrowDomainException()
        {
            // Arrange
            var booking = Booking.Create(1);
            booking.AddSeat(SeatNumber.Of("A", 1));
            booking.Confirm();

            // Act & Assert
            var ex = Assert.Throws<DomainException>(() => booking.AddSeat(SeatNumber.Of("A", 2)));
            Assert.Contains("pending booking", ex.Message);
        }

        [Fact]
        public void Confirm_WithNoSeats_ShouldThrowDomainException()
        {
            // Arrange
            var booking = Booking.Create(1);

            // Act & Assert
            var ex = Assert.Throws<DomainException>(() => booking.Confirm());
            Assert.Contains("no seats", ex.Message);
        }

        [Fact]
        public void Confirm_WithAtLeastOneSeat_ShouldSucceed()
        {
            // Arrange
            var booking = Booking.Create(1);
            booking.AddSeat(SeatNumber.Of("A", 1));

            // Act
            booking.Confirm();

            // Assert
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }
    }
}