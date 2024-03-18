namespace BusinessLogic.Dtos.Partners
{
    public class PartnerDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? TitleVi { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? DescriptionVi { get; set; }
        public short Status { set; get; }
        public DateTime CreatedOn { get; set; }
    }
}