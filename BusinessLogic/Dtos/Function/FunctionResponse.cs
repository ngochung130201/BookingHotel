namespace BusinessLogic.Dtos.Function
{
    public class FunctionResponse
    {
        public int Id { get; set; }
        public string FunctionId { get; set; } = default!;
        public string Name { set; get; } = default!;
        public string? URL { set; get; }
        public string? ParentId { set; get; }
        public string? IconCss { get; set; }
        public int SortOrder { set; get; }
        public DateTime CreatedOn { get; set; }
    }
}
