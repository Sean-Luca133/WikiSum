namespace WikiSum.Models.Api.Website
{
    public class TranslationsRequest
    {
        public string Title { get; set; }

        public string Language { get; set; }
        public List<SummaryRequest> Summaries { get; set; } = new();

    }
}
