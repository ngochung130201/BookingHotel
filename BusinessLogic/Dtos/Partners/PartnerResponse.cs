namespace BusinessLogic.Dtos.Partners
{
    public class PartnerResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;

        public string TitleVi { get; set; } = default!;

        public string? ImageUrl { get; set; }

        public short Status { set; get; }

        public string CreatedBy { get; set; } = default!;

        public DateTime CreatedOn { get; set; }
    }
}