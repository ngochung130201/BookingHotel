using System.Globalization;

namespace BusinessLogic.Helpers
{
    public class StringHelper
    {
        private static Random random = new Random();
        public static string MaskPhoneNumber(string phoneNumber)
        {
            // Check if the length of the phone number is not enough to hide
            if (phoneNumber.Length < 7)
            {
                return phoneNumber;
            }

            // Get the last 5 characters of the phone number
            string lastFiveDigits = phoneNumber.Substring(phoneNumber.Length - 7);

            // Create a string "x" with the length of the characters to hide
            string mask = new string('x', lastFiveDigits.Length);

            // Combine the mask string with the first digits of the phone number
            string maskedPhoneNumber = phoneNumber.Substring(0, phoneNumber.Length - 7) + mask;

            return maskedPhoneNumber;
        }
        
        public static DateTime ConvertDateFormat(string inputDate)
        {
            DateTime.TryParseExact(inputDate, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None,
                out var parsedDate);
            return parsedDate;
        }

        public static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static DateTime? ConvertDateTime(string dateStr, string timeStr)
        {
            // Định dạng cho ngày tháng (MM/dd/yyyy) và giờ (hh:mm tt)
            string[] dateTimeFormats = { "MM/dd/yyyy h:mm tt", "MM/dd/yyyy hh:mm tt" };
            // Kết hợp ngày và giờ thành một chuỗi hoàn chỉnh
            string dateTimeStr = dateStr + " " + timeStr;
            DateTime result;
            if (DateTime.TryParseExact(dateTimeStr, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result.ToUniversalTime();
            }

            return null;
        }

        public static double CalculatePercentageChange(int currentValue, int previousValue)
        {
            if (previousValue == 0)
            {
                return 100;
            }

            return ((currentValue - previousValue) / (double)previousValue) * 100;
        }

        public static string FormatPercentageChange(double change)
        {
            return string.Format("{0:0.00}%", change);
        }

        public static decimal CalculatePercentageChange(decimal currentValue, decimal previousValue)
        {
            if (previousValue == 0m)
            {
                return 100m;
            }

            return ((currentValue - previousValue) / previousValue) * 100m;
        }

        public static string FormatPercentageChange(decimal change)
        {
            return string.Format("{0:0.00}%", change);
        }
    }
}
