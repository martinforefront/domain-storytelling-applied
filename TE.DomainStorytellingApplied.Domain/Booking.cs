namespace TE.DomainStorytellingApplied.Domain;

public class Booking
{
    internal Booking(int id, Room room, TimePeriod period, Booker booker)
    {
        Id = id;
        Room = room;
        Period = period;
        Booker = booker;
    }

    public int Id { get; }
    public Room Room { get; }
    public TimePeriod Period { get; }
    public Booker Booker { get; }
    public bool IsConfirmed { get; private set; }

    internal void Confirm() => IsConfirmed = true;
    internal void Cancel() => IsConfirmed = false;
}
