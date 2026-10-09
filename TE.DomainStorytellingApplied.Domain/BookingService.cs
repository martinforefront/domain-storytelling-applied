namespace TE.DomainStorytellingApplied.Domain;

// Checks room availability before confirming a booking.
public class BookingService
{
    public void Confirm(Booking booking, IEnumerable<Booking> existingBookings)
    {
        bool isOccupied = existingBookings.Any(existing =>
            existing.Id != booking.Id &&
            existing.Room.Id == booking.Room.Id &&
            existing.IsConfirmed &&
            booking.Period.Start < existing.Period.End &&
            booking.Period.End > existing.Period.Start);

        if (isOccupied)
        {
            throw new InvalidOperationException("The room is already booked for this time period.");
        }

        booking.Confirm();
    }
}
