namespace BusinessLogic.Dtos.SpecialDayRates
{
    public class SpecialDayRatesDto
    {
        public int Id { get; set; } = default!;

        public DateTime SinceDay { get; set; } 

        public DateTime ToDay { get; set; }

        public string? Title { get; set; } 

        public int? PercentDiscount { get; set; } 

        public string? Description { get; set; }
    }
}
