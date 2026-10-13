using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ContactDetailsTests
{
    [Fact]
    public void GivenSameDetails_WhenCreatedSeparately_ShouldBeEqualByValue()
    {
        var first = new ContactDetails("Anna", "anna@example.com");
        var second = new ContactDetails("Anna", "anna@example.com");

        first.ShouldNotBeSameAs(second);
        first.ShouldBe(second);
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Theory]
    [InlineData("Bertil", "anna@example.com")]
    [InlineData("Anna", "another@example.com")]
    public void GivenDifferentDetails_WhenComparing_ShouldNotBeEqual(string name, string email)
    {
        new ContactDetails(name, email).ShouldNotBe(new ContactDetails("Anna", "anna@example.com"));
    }

    [Fact]
    public void GivenSurroundingWhitespace_WhenCreated_ShouldTrimDetails()
    {
        var details = new ContactDetails("  Anna Andersson  ", "  anna@example.com  ");

        details.Name.ShouldBe("Anna Andersson");
        details.Email.ShouldBe("anna@example.com");
        details.ShouldBe(new ContactDetails("Anna Andersson", "anna@example.com"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenMissingName_WhenCreated_ShouldReject(string? name)
    {
        var exception = Should.Throw<ArgumentException>(
            () => new ContactDetails(name!, "anna@example.com"));

        exception.ParamName.ShouldBe("name");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("anna@")]
    [InlineData("@example.com")]
    [InlineData("anna@example.com, bertil@example.com")]
    [InlineData("Anna <anna@example.com>")]
    public void GivenInvalidEmail_WhenCreated_ShouldReject(string? email)
    {
        var exception = Should.Throw<ArgumentException>(
            () => new ContactDetails("Anna", email!));

        exception.ParamName.ShouldBe("email");
    }
}
