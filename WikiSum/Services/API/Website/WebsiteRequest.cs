namespace WikiSum.Services.API.Post
{
    public class WebsiteRequest
    {
        public string Url { get; set; }
        public List<ChapterRequest> Chapters { get; set; } = new();
    }
}
