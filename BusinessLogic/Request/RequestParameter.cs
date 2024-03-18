using static BusinessLogic.Constants.Application.ApplicationConstants;

namespace BusinessLogic.Request
{
    public class RequestParameter
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsExport { get; set; }
        public string OrderBy { get; set; } = default!;

        protected RequestParameter()
        {
            PageNumber = 1;
            PageSize = 10;
            IsExport = false;
            OrderBy = "CreatedOn Descending";
        }

        public RequestParameter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
