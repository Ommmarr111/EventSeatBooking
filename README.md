# Event Seat Booking

A domain model for a seat reservation system, built to demonstrate Domain-Driven Design done properly: business rules live inside the entities that own them, not scattered across a service layer pretending to be "business logic."

This is a domain layer, not a finished application — deliberately. No API, no database, no infrastructure. The discipline here is getting the model right in isolation first, so nothing downstream has to compensate for a weak domain.

---

## Design

### Aggregates

**`Booking`** — the aggregate root for a single customer's reservation attempt. Owns a collection of `BookedSeat` and enforces every rule that governs its own consistency:

- Maximum 6 seats per booking
- No duplicate seat within the same booking
- Seats can only be added while the booking is `Pending`
- A booking with zero seats cannot be confirmed
- A cancelled booking cannot be confirmed; a confirmed booking can still be cancelled

None of these checks live outside the entity. There is no `BookingService.AddSeat()` making the decision on `Booking`'s behalf — the only way to add a seat is through `Booking.AddSeat()`, which means the rule cannot be bypassed by code that forgets to call a validator first.

**`Screening`** — the aggregate root for a single showing: title, showtime, and the seat map for that showing (`Available` / `Reserved` / `Booked` per seat). It's a separate aggregate from `Booking` on purpose — many bookings reference the same screening, and a screening's lifecycle (created, rescheduled) doesn't need to change atomically with any individual booking. Collapsing them into one aggregate would create a far larger consistency boundary than the actual business rules require.

### The cross-aggregate problem

Neither aggregate can answer "is this seat actually available" on its own. `Booking` has no visibility into other bookings; `Screening` owns availability but has no concept of who's booking. This is resolved with `SeatAvailabilityService` — a Domain Service, used specifically because the rule spans two aggregates and belongs to neither individually.

The service does no validation itself; it coordinates. It asks `Screening` to reserve the seat first, and only adds the seat to `Booking` if that succeeds. If the seat is already taken, `Screening` rejects the reservation before `Booking` is ever touched — so the two aggregates never drift out of sync. Full reasoning, including the alternatives considered and rejected, is documented in [`docs/domain-model.md`](docs/domain-model.md).

### Domain Events

`SeatAddedEvent`, `BookingConfirmedEvent`, and `BookingCancelledEvent` are raised from inside `Booking`, and only on success — a rejected operation (seat limit exceeded, confirming an empty booking) raises nothing. Modeled as immutable C# records, collected on the aggregate, and cleared via `ClearDomainEvents()` once consumed. Publishing them is an application-layer concern, deliberately out of scope here.

### Concurrency

Two customers reserving the same seat simultaneously is a real scenario, addressed at the design level rather than implemented against an in-memory model with no actual concurrent access to protect. The design doc specifies the concrete mechanism — optimistic concurrency via a row version on `Screening`, or a database-level uniqueness constraint on seat reservation — to be applied once persistence exists.

---

## Testing

29 unit tests, all exercising the aggregates directly with no infrastructure and no mocking. Coverage includes:

- Boundary conditions (exactly 6 seats succeeds, a 7th is rejected)
- Negative cases proving a failed operation raises no domain event
- The cross-aggregate case: a second reservation attempt on an already-reserved seat is rejected by `Screening` before it ever reaches `Booking`

---

## What's next

- Application layer — use cases orchestrating the domain (`ReserveSeatCommandHandler`, event publishing)
- Infrastructure — EF Core repositories, one per aggregate root, matching the interfaces already defined
- API layer
- Concurrency implementation against real persistence, per the design doc
