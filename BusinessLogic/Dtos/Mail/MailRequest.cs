namespace BusinessLogic.Dtos.Mail
{
    public class MailRequest
    {
        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string From { get; set; } = default!;
        public List<string>? ListMailTo { get; set; }
    }
}