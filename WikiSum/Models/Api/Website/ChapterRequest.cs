namespace WikiSum.Models.Api.Website
{
    public class ChapterRequest
    {
        public int WebpageId { get; set; }
        public List<TranslationsRequest> Translations { get; set; } = new();
    }
}
