using BusinessLogic.Dtos.Partners;

namespace BusinessLogic.Dtos.Home
{
    public class HomeResponse
    {
        public List<PartnerResponse> Partners { get; set; } = new();
        public string LanguageType { get; set; } = default!;
    }
}
