using System.Collections.ObjectModel;

namespace BusinessLogic.Services.Common
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
        //TimeZoneInfo TimeZoneInfo { get; }
    }

    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        //TimeZoneInfo IDateTimeService.TimeZoneInfo => GetTimeZoneInfo();

        //public TimeZoneInfo GetTimeZoneInfo()
        //{
        //    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

        //    foreach (TimeZoneInfo timeZone in timeZones)
        //    {
        //        if (timeZone.Id == "Asia/Ho_Chi_Minh")
        //        {
        //            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
        //        }
        //    }
        //    return TimeZoneInfo.Local;
        //}
    }
}