namespace TE.DomainStorytellingApplied.Domain;

// Booked room (entity)
public class Room(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}

// Choosen time (Value object).
public record TimePeriod(DateTime Start, DateTime End);

// Make a booking (aggregate root).
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
