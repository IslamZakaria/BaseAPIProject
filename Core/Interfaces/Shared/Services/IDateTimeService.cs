namespace Core.Interfaces.Shared.Services
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
        DateTime Now { get; }
        string? GetDateAsString(string? Date);
        DateTime? GetDateFromString(string? Date);
    }
}
