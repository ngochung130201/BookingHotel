namespace BusinessLogic.Dtos.FeedBacks
{
    public class FeedBacksDto
    {
        public int Id { get; set; } = default!;

        public string? Name { get; set; } 

        public string? Email { get; set; } 

        public string? Title { get; set; } 

        public string? Content { get; set; } 

        public string? Reply { get; set; } 

        public string? ReplyBy { get; set; } 
    }
}
