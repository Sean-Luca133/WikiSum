using Azure;
using Azure.AI.TextAnalytics;

namespace WikiSum.Services.Sumerization
{

<<<<<<< HEAD
    
    public class SummarizationService: ISummarizationService
=======
    public static class ApiLanguagenKeys
    {
        public static string LanguagerKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
        public static string LanguageEndpoint = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
    }
    public class SummarizationService : ISummarizationService
>>>>>>> 97b4b20197d30984ff874775ad561a85a03517eb
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

