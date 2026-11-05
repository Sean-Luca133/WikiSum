using Azure;
using Azure.AI.TextAnalytics;

namespace WikiSum.Services.EntityReconition
{
    public static class ApiLanguagenKeys
    {
        public static string LanguagerKey = Environment.GetEnvironmentVariable("LANGUAGE_KEY");
        public static string LanguageEndpoint = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
    }



    public class EntityRecognitionService : IEntityRecognitionService
    {
        private TextAnalyticsClient client = new(new Uri(ApiLanguagenKeys.LanguageEndpoint), new AzureKeyCredential(ApiLanguagenKeys.LanguagerKey));

        public CategorizedEntityCollection GetNamedEntities(string document)
        {
            Response<CategorizedEntityCollection> response = client.RecognizeEntities(document);
            CategorizedEntityCollection entitiesInDocument = response.Value;
            return entitiesInDocument;
        }

 
    }
}