namespace BusinessLogic.Constants.Application
{
    public static class ApplicationConstants
    {
        public const string ERROR_MESSAGE = "ERROR_MESSAGE";

        public const string DISPLAY_ORDER_DES = "DisplayOrder Descending";
        public const string DISPLAY_ORDER_ASC = "DisplayOrder Ascending";
        public enum DISPLAY_ORDER
        {
            DESCENDING = 1,ASCENDING = 2
        }

        public class UserClaims
        {
            public const string Roles = "Roles";
        }
        public class LanguageType
        {
            public const string English = "en-US";
            public const string Vietnamese = "vi-VN";
        }
    }
}