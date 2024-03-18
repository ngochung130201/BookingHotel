namespace BusinessLogic.Dtos.Pages
{
    public class PagesResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string TitleVi { get; set; } = default!;
        public string? Alias { set; get; }
        public short Status { set; get; }
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
    }
}