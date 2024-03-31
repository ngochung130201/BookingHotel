using BusinessLogic.Dtos.News;

namespace BusinessLogic.Dtos.Home
{
    public class HomeResponse
    {
        public List<NewsResponse> News { get; set; } = new();
        public string LanguageType { get; set; } = default!;
    }
}
