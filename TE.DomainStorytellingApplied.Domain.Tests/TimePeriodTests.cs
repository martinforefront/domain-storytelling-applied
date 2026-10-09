using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimePeriodTests
{
    [Fact]
    public void GivenEndAfterStart_WhenCreated_ShouldKeepTheTimes()
    {
        var start = new DateTime(2026, 11, 1, 10, 0, 0);
        var end = start.AddHours(1);

        var period = new TimePeriod(start, end);

        period.Start.ShouldBe(start);
        period.End.ShouldBe(end);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void GivenEndAtOrBeforeStart_WhenCreated_ShouldThrow(int minutes)
    {
        var start = new DateTime(2026, 11, 1, 10, 0, 0);

        var exception = Should.Throw<ArgumentException>(
            () => new TimePeriod(start, start.AddMinutes(minutes)));

        exception.ParamName.ShouldBe("end");
    }
}
