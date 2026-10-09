namespace TE.DomainStorytellingApplied.Domain;

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
