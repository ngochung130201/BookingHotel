namespace BusinessLogic.Dtos.CostOverrun
{
    public class CostOverrunResponse
    {
        public int Id { get; set; } = default!;

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Image { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}
