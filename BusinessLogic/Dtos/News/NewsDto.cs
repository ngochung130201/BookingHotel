namespace BusinessLogic.Dtos.News
{
    public class NewsDto
    {
        public int Id { get; set; } = default!;

        public string? Title { get; set; } 

        public string? Thumbnail { get; set; } 

        public string? Content { get; set; } 

        public bool? Status { get; set; } 

        public bool? Hot { get; set; }
    }
}
