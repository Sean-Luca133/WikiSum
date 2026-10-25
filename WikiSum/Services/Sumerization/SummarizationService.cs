using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services.Sumerization
{

    public static class ApiLanguagenKeys
    {
        public static string LanguagerKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
        public static string LanguageEndpoint = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
    }
    public class SummarizationService: ISummarizationService
    {
        private TextAnalyticsClient client = new(new Uri(ApiLanguagenKeys.LanguageEndpoint), new AzureKeyCredential(ApiLanguagenKeys.LanguagerKey));




        // Perform the text analysis operation.
        public async Task<ExtractiveSummarizeOperation> SummarizeText(List<string> document)
        {
            return client.ExtractiveSummarize(WaitUntil.Completed, document);
        }
       
    };
}

