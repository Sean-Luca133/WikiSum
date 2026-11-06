using Azure;
using Azure.AI.TextAnalytics;

namespace WikiSum.Services.Sumerization
{


    public class SummarizationService: ISummarizationService
    {
        private TextAnalyticsClient client = new(new Uri(ApiLanguageKeys.LanguageEndpoint), new AzureKeyCredential(ApiLanguageKeys.LanguagerKey));

        // Perform the text analysis operation.
        public async Task<AbstractiveSummarizeOperation> SummarizeText(List<string> document)
        {
            AbstractiveSummarizeOptions options = new();
            options.SentenceCount = 20;
            return client.AbstractiveSummarize(WaitUntil.Completed, document, options: options);
        }

    };
}

