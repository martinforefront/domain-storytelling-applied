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

# Simple room booking example

Choose a room → Choose a time period → Create a booking → Confirm the booking.

- `Room` is an entity with an ID.
- `TimePeriod` is a value object, defined by its start and end values.
- `Booking` is an entity and aggregate root. Its `Confirm()` method controls confirmation.

The three types are in `TE.DomainStorytellingApplied.Domain/Booking.cs`.
The four steps are exercised by `TE.DomainStorytellingApplied.Domain.Tests/BookingTests.cs`.
The solution contains only domain objects and tests; there is no application.

```sh
dotnet test
```

The test checks that a new booking keeps the selected room and time period,
starts unconfirmed, and becomes confirmed after calling `Confirm()`.

This minimal example shows the structure only. Validation, double-booking checks,
and persistence can be added later.
