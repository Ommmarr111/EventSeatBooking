using EventSeatBooking.Domain.Entities;
using EventSeatBooking.Domain.Exceptions;
using EventSeatBooking.Domain.ValueObjects;

namespace EventSeatBooking.Domain.Tests
{
    public class ScreeningTests
    {
        private static List<SeatNumber> ThreeSeats() => new()
        {
            SeatNumber.Of("A", 1),
            SeatNumber.Of("A", 2),
            SeatNumber.Of("A", 3)
        };

        [Fact]
        public void Create_WithFutureShowtime_ShouldSucceed()
        {
            var screening = Screening.Create("Interstellar", DateTime.UtcNow.AddDays(1), ThreeSeats());

            Assert.Equal(3, screening.Seats.Count);
        }

        [Fact]
        public void Create_WithPastShowtime_ShouldThrowDomainException()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Screening.Create("Interstellar", DateTime.UtcNow.AddDays(-1), ThreeSeats()));

            Assert.Contains("must be in the future", ex.Message);
        }

        [Fact]
        public void ReserveSeat_OnAvailableSeat_ShouldSetStatusToReserved()
        {
            var screening = Screening.Create("Interstellar", DateTime.UtcNow.AddDays(1), ThreeSeats());
            var seatNumber = SeatNumber.Of("A", 1);

            screening.ReserveSeat(seatNumber);

            var seat = screening.GetSeat(seatNumber);
            Assert.Equal(Enums.SeatStatus.Reserved, seat.Status);
        }

        [Fact]
        public void ReserveSeat_AlreadyReserved_ShouldThrowDomainException()
        {
            var screening = Screening.Create("Interstellar", DateTime.UtcNow.AddDays(1), ThreeSeats());
            var seatNumber = SeatNumber.Of("A", 1);
            screening.ReserveSeat(seatNumber);

            var ex = Assert.Throws<DomainException>(() => screening.ReserveSeat(seatNumber));
            Assert.Contains("not available", ex.Message);
        }

        [Fact]
        public void GetSeat_ForNonExistentSeat_ShouldThrowDomainException()
        {
            var screening = Screening.Create("Interstellar", DateTime.UtcNow.AddDays(1), ThreeSeats());

            var ex = Assert.Throws<DomainException>(() => screening.GetSeat(SeatNumber.Of("Z", 99)));
            Assert.Contains("does not exist", ex.Message);
        }

        [Fact]
        public void ReleaseSeat_OnReservedSeat_ShouldSetStatusBackToAvailable()
        {
            var screening = Screening.Create("Interstellar", DateTime.UtcNow.AddDays(1), ThreeSeats());
            var seatNumber = SeatNumber.Of("A", 1);
            screening.ReserveSeat(seatNumber);

            screening.ReleaseSeat(seatNumber);

            Assert.Equal(Enums.SeatStatus.Available, screening.GetSeat(seatNumber).Status);
        }
    }
}