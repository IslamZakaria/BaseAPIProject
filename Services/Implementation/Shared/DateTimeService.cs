using Core.Interfaces.Shared.Services;
using System.Globalization;

namespace Services.Implementation.Shared
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow.AddHours(3);
        public DateTime Now => DateTime.Now;

        public string? GetDateAsString(string? Date)
        {
            if (string.IsNullOrWhiteSpace(Date))
            {
                return null;
            }
            DateTime ConvertedStringDatetime = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            return ConvertedStringDatetime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public DateTime? GetDateFromString(string? Date)
        {
            if (string.IsNullOrWhiteSpace(Date))
            {
                return null;
            }
            DateTime ConvertedStringDatetime = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            return ConvertedStringDatetime;
        }
    }
}
