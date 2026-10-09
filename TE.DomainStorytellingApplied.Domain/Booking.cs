namespace TE.DomainStorytellingApplied.Domain;

// ENTITY - has its own identity.
public class Room(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}

// VALUE OBJECT - defined by its values.
public record TimePeriod(DateTime Start, DateTime End);

// ENTITY + AGGREGATE ROOT - controls changes to the booking.
public class Booking(int id, Room room, TimePeriod period)
{
    public int Id { get; } = id;
    public Room Room { get; } = room;
    public TimePeriod Period { get; } = period;
    public bool IsConfirmed { get; private set; }

    public void Confirm()
    {
        IsConfirmed = true;
    }
}
