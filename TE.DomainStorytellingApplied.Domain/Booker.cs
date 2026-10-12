namespace TE.DomainStorytellingApplied.Domain;

public class Booker(int id, string name, string email)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Email { get; } = email;
}
