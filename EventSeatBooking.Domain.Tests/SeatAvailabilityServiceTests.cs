using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.Services;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Tests
{
    public class SeatAvailabilityServiceTests
    {
        private readonly SeatAvailabilityService _service = new();

        private static Screening MakeScreening() => Screening.Create(
            "Interstellar",
            DateTime.UtcNow.AddDays(1),
            new List<SeatNumber> { SeatNumber.Of("A", 1) });

        [Fact]
        public void ReserveSeatForBooking_WhenSeatIsAvailable_ShouldReserveOnScreening_AndAddToBooking()
        {
            // Arrange
            var screening = MakeScreening();
            var booking = Booking.Create(customerId: 1, 1);
            var seatNumber = SeatNumber.Of("A", 1);

            // Act
            _service.ReserveSeatForBooking(screening, booking, seatNumber);

            // Assert — both aggregates reflect the change
            Assert.Equal(Enums.SeatStatus.Reserved, screening.GetSeat(seatNumber).Status);
            Assert.Single(booking.Seats);
        }

        [Fact]
        public void ReserveSeatForBooking_WhenSeatAlreadyReserved_ShouldThrow_AndLeaveBookingUntouched()
        {
            // Arrange — this is the key test: proves the ordering we discussed actually protects consistency
            var screening = MakeScreening();
            var firstBooking = Booking.Create(customerId: 1, 1);
            var secondBooking = Booking.Create(customerId: 2, 2);
            var seatNumber = SeatNumber.Of("A", 1);

            _service.ReserveSeatForBooking(screening, firstBooking, seatNumber); // first customer succeeds

            // Act & Assert — second customer tries the same seat
            var ex = Assert.Throws<DomainException>(() =>
                _service.ReserveSeatForBooking(screening, secondBooking, seatNumber));

            Assert.Contains("not available", ex.Message);

            // The critical assertion: secondBooking must NOT have the seat,
            // since Screening rejected it before Booking.AddSeat ever ran
            Assert.Empty(secondBooking.Seats);
        }
    }
}