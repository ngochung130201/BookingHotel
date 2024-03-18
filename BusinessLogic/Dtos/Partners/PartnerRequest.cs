using BusinessLogic.Request;

namespace BusinessLogic.Dtos.Partners
{
    public class PartnerRequest : RequestParameter
    {
        public short? Status { get; set; }
    }
}