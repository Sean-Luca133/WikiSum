using Azure;
using Azure.AI.TextAnalytics;

namespace WikiSum.Services.EntityReconition
{

    public class EntityRecognitionService : IEntityRecognitionService
    {
        private TextAnalyticsClient client = new(new Uri(ApiLanguageKeys.LanguageEndpoint), new AzureKeyCredential(ApiLanguageKeys.LanguageKey));

        public CategorizedEntityCollection GetNamedEntities(string document)
        {
            Response<CategorizedEntityCollection> response = client.RecognizeEntities(document);
            CategorizedEntityCollection entitiesInDocument = response.Value;
            return entitiesInDocument;
        }

 
    }
}