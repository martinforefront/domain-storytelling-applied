using System.Net.Mail;

namespace TE.DomainStorytellingApplied.Domain;

public sealed record ContactDetails
{
    public string Name { get; }
    public string Email { get; }

    public ContactDetails(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Namn måste anges.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("E-postadress måste anges.", nameof(email));
        }

        var trimmedEmail = email.Trim();
        if (!MailAddress.TryCreate(trimmedEmail, out var address) ||
            address.Address != trimmedEmail)
        {
            throw new ArgumentException("E-postadressen är ogiltig.", nameof(email));
        }

        Name = name.Trim();
        Email = trimmedEmail;
    }
}
