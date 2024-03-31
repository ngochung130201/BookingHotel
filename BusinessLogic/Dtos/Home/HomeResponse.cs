using BusinessLogic.Dtos.News;
using BusinessLogic.Dtos.RoomTypes;

namespace BusinessLogic.Dtos.Home
{
    public class HomeResponse
    {
        public List<NewsResponse> News { get; set; } = new();
        public List<RoomTypesResponse> RoomTypes { get; set; } = new();
        public string LanguageType { get; set; } = default!;
    }
}
