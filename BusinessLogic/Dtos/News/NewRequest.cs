using BusinessLogic.Request;

namespace BusinessLogic.Dtos.News
{
    public class NewRequest : RequestParameter
    {
        public bool? Status { get; set; }
    }
}
