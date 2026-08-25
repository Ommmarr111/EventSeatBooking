# Event Seat Booking — DDD Concept Project

A small, focused project built specifically to practice Domain-Driven Design — moving business rules *into* entities and aggregates, rather than scattering them across a service layer (see [Gym Management System](https://github.com/Ommmarr111/Gym-Management-System) for the contrast: that project uses an anemic domain model on purpose, this one doesn't).

**This is not a full application.** There's no API, no database, no infrastructure layer yet — just the domain model and unit tests proving its invariants. That's intentional: the goal here is to get the domain layer right in isolation before adding anything else around it.

## What's implemented

`Booking` is the Aggregate Root. It owns a collection of `BookedSeat` entities and enforces invariants that can only be checked at the aggregate level, not on any single seat:

- Max 6 seats per booking
- No duplicate seat within the same booking
- Seats can only be added while a booking is `Pending`
- A booking with no seats can't be confirmed
- A cancelled booking can't be confirmed; a confirmed booking *can* be cancelled

`SeatNumber` is a Value Object (row + number, compared by value, immutable via a private constructor + static factory).

All state changes go through behavior methods (`AddSeat`, `Confirm`, `Cancel`) — there are no public setters, so it's not possible to construct a `Booking` in an invalid state from outside the aggregate.

## Tests

15 unit tests in `EventSeatBooking.Domain.Tests`, covering the invariants above including boundary cases (exactly 6 seats succeeds, a 7th fails).

## What's next

- Domain Events (`BookingConfirmed`, `BookingCancelled`, `SeatsBooked`)
- `IBookingRepository` interface
- Application + Infrastructure layers (EF Core, minimal API) — deliberately deferred until the domain layer is solid on its own
