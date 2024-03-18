namespace BusinessLogic.Request
{
    public class FileRequestParameter
    {
        public string Base64 { get; set; } = default!;
        public bool IsConvertToWebp { get; set; } = true;
    }
}
