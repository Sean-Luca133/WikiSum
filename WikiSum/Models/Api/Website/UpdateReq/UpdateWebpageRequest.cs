namespace WikiSum.Models.Api.Website.UpdateReq
{
    public class UpdateWebpageRequest
    {
        public List<UpdateChapterRequest> Chapters { get; set; } = new();
    }

    public class UpdateChapterRequest
    {
        public string OriginalTitle { get; set; }

        public List<UpdateTranslationRequest> Translations { get; set; } = new();

    }
    public class UpdateTranslationRequest
    {
        public string Title { get; set; }
        public string Language { get; set; } = string.Empty;
        public List<UpdateSummaryRequest> Summaries { get; set; } = new();
    }
    public class UpdateSummaryRequest
    {
        public string SummaryText { get; set; } = string.Empty;
    }
}
