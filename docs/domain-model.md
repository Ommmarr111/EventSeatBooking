# Domain Model — Event Seat Booking

## Aggregates

### Booking (Aggregate Root)
Owns a collection of `BookedSeat`. Represents one customer's attempt to reserve seats for a specific screening.

**Invariants enforced inside `Booking`:**
- Max 6 seats per booking
- No duplicate seat within the same booking
- Seats can only be added while status is `Pending`
- Cannot confirm a booking with zero seats
- Cannot confirm a cancelled booking; a confirmed booking *can* be cancelled

**Why these live here and not in a service:** each rule only makes sense in the context of *this specific booking's* state. A service sitting outside the entity can be bypassed if anyone gets a reference to the internal seat list directly — keeping the rule inside the entity, behind a private setter and a controlled method (`AddSeat`), makes it impossible to violate regardless of who's calling the code.

### Screening (Aggregate Root)
Represents one showing of an event at a specific time. Owns the seat map and each seat's availability status (`Available` / `Reserved` / `Booked`) for that screening.

**Why `Screening` is a separate aggregate, not owned by `Booking`:** many `Booking`s reference the same `Screening`, and a `Screening`'s own lifecycle (created, rescheduled, cancelled) is independent of any single booking. They don't need to change together atomically, so they don't belong in the same consistency boundary.

## The cross-aggregate problem: seat availability

`Booking` only knows about seats *it* has added — it has no visibility into other bookings for the same screening. So "is seat A1 actually free right now" is a question neither aggregate can answer alone:
- `Booking` doesn't see other bookings.
- `Screening` owns availability data, but the actual decision to reserve involves creating/updating a `Booking` too.

**Resolution: `SeatAvailabilityService` (Domain Service).** This coordinates between `Screening` and `Booking` — it checks `Screening`'s seat map, and if available, tells `Screening` to mark the seat `Reserved` and returns success so the caller can proceed to add it to the `Booking`. This rule doesn't belong to either aggregate individually because it depends on both.

## Concurrency

Two requests could try to reserve the same seat at the same instant. This isn't re-solved from scratch here — the same reasoning already proven in [Gym Management System](https://github.com/Ommmarr111/Gym-Management-System) applies:
- A **row version / concurrency token** on `Screening` would catch a lost-update case (two updates to the same seat map based on stale reads) via EF Core's optimistic concurrency, similar in spirit to the atomic conditional update used for refresh token revocation.
- Alternatively, a reservation could be enforced at the database level with a unique constraint on `(ScreeningId, SeatNumber, Status=Booked)`, comparable to how gym capacity is protected with `Serializable` isolation.

This project doesn't re-implement and re-test concurrency handling, since Gym System already demonstrates the underlying skill with three different concrete solutions to three different race conditions.

## Repository boundary

Only `IBookingRepository` and `IScreeningRepository` exist — one per aggregate root. There is no repository for `BookedSeat`; it's never fetched or persisted independently of the `Booking` it belongs to.
