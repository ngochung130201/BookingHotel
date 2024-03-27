namespace BusinessLogic.Dtos.SpecialDayRates
{
    public class SpecialDayRatesResponse
    {
        public int Id { get; set; } = default!;

        public DateTime SinceDay { get; set; }

        public DateTime ToDay { get; set; }

        public string? Title { get; set; }

        public int? PercentDiscount { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}
