# Requirements
```sh
dotnet --version 
```
Should be 10 or higher. If not install the dotnet 10 SDK.

# Getting the code
```sh
git clone https://github.com/martinforefront/domain-storytelling-applied.git
```

# Building
```sh
cd domain-storytelling-applied
dotnet build
```

# Running tests
```sh
cd domain-storytelling-applied
dotnet test
```

# Structure
Domain objects goes in `TE.DomainStorytellingApplied.Domain` \
Test cases goes in `TE.DomainStorytellingApplied.Domain.Tests`

# Room booking aggregate

The solution contains a class library and tests only; there is no application.

- `RoomSchedule` is the aggregate root for one room, identified by the room ID.
  Its private list owns the room's bookings. `Reserve` checks availability,
  creates a reserved booking, and stores it. `Confirm` confirms an existing
  reservation. `Cancel` releases its time. Cancelled bookings cannot be confirmed.
- `Booking` is an entity inside the schedule. Its constructor and state-changing
  methods are internal, so callers in other projects use the aggregate root.
  Internal access still permits other classes in the same assembly to call them.
- `Room` and `Booker` are independent entities referenced by bookings.
- `TimePeriod` is a value object that requires the end to be after the start.

Overlapping reserved or confirmed bookings are rejected with `RoomUnavailableException`.
Adjacent periods are allowed. A booking ID cannot be reused within a schedule,
including after cancellation. Cancellation of an unknown booking is rejected.
`Booking.Status` distinguishes `Reserved`, `Confirmed`, and `Cancelled`.
New bookings start as `Reserved`. Confirmation is a separate operation through
`RoomSchedule.Confirm(bookingId)`; repeating confirmation is allowed. `IsConfirmed` is derived from the status and is false after cancellation.
The existing rejected-attempt flow still returns an unstored booking in the initial
`Reserved` state; it does not reserve any time in the schedule.

# Booking coordination and notifications

`BookingSystem` replaces the old coordination flow and holds one `RoomSchedule`
per room ID. `BookingService` has been removed; overlap rules now live in the
aggregate root.

```csharp
using var output = new StringWriter();
var system = new BookingSystem(new NotificationService(output));
var booker = new Booker(1, "Anna", "anna@example.com");
var room = new Room(1, "Meeting room");
var period = new TimePeriod(
    new DateTime(2026, 11, 1, 10, 0, 0),
    new DateTime(2026, 11, 1, 12, 0, 0));
var booking = system.Book(booker, room, period);
system.Cancel(room.Id, booking.Id);
```

`Book` obtains the room's schedule, allocates an ID, and calls `Reserve` followed
by `Confirm`, preserving the immediate confirmation flow for now.
It writes confirmation or rejection to the booker's email address through
`NotificationService`. This writes to a supplied `TextWriter`; no real email is sent.
A rejected attempt is returned as an unconfirmed `Booking` and is not stored in
any schedule. Notification failure does not undo a successful reservation.
Cancellation delegates to the schedule and does not send a notification.

Use the same system instance for related booking attempts. The schedules and IDs
are in memory only. Separate instances do not share reservations. This example
supports sequential calls, not concurrent booking attempts. A persistent,
multi-user version needs atomic saving and concurrency control across instances.

`RoomScheduleTests` covers overlap, adjacent times, duplicate IDs, and cancellation.
`BookingSystemTests` covers notifications, room identity, cancellation, rejected
attempts, and retained reservations after notification failure.
Run the scenarios with `dotnet test`.
