namespace BusinessLogic.Dtos.Pages
{
    public class PagesDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string TitleVi { get; set; } = default!;
        public string? Content { get; set; }
        public string? ContentVi { get; set; }
        public string Alias { set; get; } = default!;
        public short Status { set; get; }
    }
}