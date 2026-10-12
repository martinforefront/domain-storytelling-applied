namespace TE.DomainStorytellingApplied.Domain;

// Bookat rum (entity)
public class Room(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}
