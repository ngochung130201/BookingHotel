using BusinessLogic.Request;

namespace BusinessLogic.Dtos.CheckInOut
{
    public class CheckInOutRequest : RequestParameter
    {
        public short? StatusRoom { get; set; }
    }
}
