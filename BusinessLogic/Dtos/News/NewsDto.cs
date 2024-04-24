namespace BusinessLogic.Dtos.News
{
    public class NewsDto
    {
        public int Id { get; set; } = default!;

        public string? Title { get; set; } 

        public string? Thumbnail { get; set; } 

        public string? Content { get; set; }

        public string? Summary { get; set; }

        public string? NewsType { get; set; }

        public string? Author { get; set; }

        public bool? Status { get; set; } 

        public bool? Hot { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
