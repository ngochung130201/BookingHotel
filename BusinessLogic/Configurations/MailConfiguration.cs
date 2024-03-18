using BusinessLogic.Contexts;

namespace BusinessLogic.Configurations
{
    public class MailConfiguration
    {
        public string From { get; set; } = default!;
        public string Host { get; set; } = default!;
        public int Port { get; set; }
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool? SmtpUseSsl { get; set; }
        public string? TestEmail { get; set; } = default!;
        //public void SetValuesFromDatabase(ApplicationDbContext dbContext)
        //{
        //    // Assuming you have a single configuration record in the database
        //    var configurationFromDb = dbContext.EmailSettings.FirstOrDefault();

        //    if (configurationFromDb != null)
        //    {
        //        From = configurationFromDb.SmtpUserName ?? "";
        //        Host = configurationFromDb.SmtpServer ?? "";
        //        Port = configurationFromDb.SmtpPort;
        //        UserName = configurationFromDb.SmtpUserName ?? "";
        //        Password = configurationFromDb.SmtpPassword ?? "";
        //        DisplayName = configurationFromDb.SmtpUserName ?? "";
        //        SmtpUseSsl = configurationFromDb.SmtpUseSsl;
        //        TestEmail = configurationFromDb.TestEmail ?? "";
        //    }
        //}
    }
}