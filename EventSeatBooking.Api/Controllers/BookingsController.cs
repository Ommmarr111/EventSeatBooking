using EventSeatBooking.Application.UseCases;
using EventSeatBooking.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventSeatBooking.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class BookingsController : ControllerBase
    {
        private readonly ReserveSeatUseCase _reserveSeatUseCase;
        private readonly ConfirmBookingUseCase _confirmBookingUseCase;
        private readonly CancelBookingUseCase _cancelBookingUseCase;

        public BookingsController(
            ReserveSeatUseCase reserveSeatUseCase,
            ConfirmBookingUseCase confirmBookingUseCase,
            CancelBookingUseCase cancelBookingUseCase)
        {
            _reserveSeatUseCase = reserveSeatUseCase;
            _confirmBookingUseCase = confirmBookingUseCase;
            _cancelBookingUseCase = cancelBookingUseCase;
        }

        public record ReserveSeatRequest(int CustomerId, string SeatRow, int SeatColumnNumber);

        [HttpPost("screenings/{screeningId}/reserve-seat")]
        public async Task<IActionResult> ReserveSeat(int screeningId, [FromBody] ReserveSeatRequest request)
        {
            try
            {
                var bookingId = await _reserveSeatUseCase.ExecuteAsync(
                    screeningId, request.CustomerId, request.SeatRow, request.SeatColumnNumber);

                return Ok(new { bookingId });
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("bookings/{bookingId}/confirm")]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            try
            {
                await _confirmBookingUseCase.ExecuteAsync(bookingId);
                return Ok();
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("bookings/{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                await _cancelBookingUseCase.ExecuteAsync(bookingId);
                return Ok();
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}