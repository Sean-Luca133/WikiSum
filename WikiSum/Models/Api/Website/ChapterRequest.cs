namespace WikiSum.Models.Api.Website
{
    public class ChapterRequest
    {
        public string Title { get; set; }
        public List<SummaryRequest> Summaries { get; set; } = new();
    }
}
