namespace TE.DomainStorytellingApplied.Domain;

// Booked room (entity)
public class Room(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}
