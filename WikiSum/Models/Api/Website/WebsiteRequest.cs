namespace WikiSum.Models.Api.Website
{
    public class WebsiteRequest
    {
        public string Url { get; set; }
        public List<ChapterRequest> Chapters { get; set; } = new();

        public string LanguagesText =>
    string.Join(", ", Languages);
        public List<string> Languages =>
        Chapters
            .SelectMany(c => c.Translations)
            .Select(t => t.Language)
            .Distinct()
            .ToList();
    }
}
