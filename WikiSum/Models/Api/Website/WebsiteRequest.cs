namespace WikiSum.Models.Api.Website
{
    public class WebsiteRequest
    {
        public string Url { get; set; }
        public List<ChapterRequest> Chapters { get; set; } = new();

        public string Language =>
        Chapters.FirstOrDefault()?
            .Summaries.FirstOrDefault()?
            .Language ?? "Unknown";
    }
}
