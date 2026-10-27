using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiSum.Services.Translation;

namespace WikiSum.Services
{

    public static class ApiTranslationKeys
    {
        public static string TranslatorKey = Environment.GetEnvironmentVariable("TRANSLATOR_KEY");
        public static string TranslatorRegion = Environment.GetEnvironmentVariable("TRANSLATOR_REGION");
    }

    public class TranslationService : ITranslationService
    {
        public async Task<string> TranslateText(string text, string fromLang, string toLang)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiTranslationKeys.TranslatorKey);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", ApiTranslationKeys.TranslatorRegion);

            var endpoint = $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from={fromLang}&to={toLang}";

            var body = new object[] { new { Text = text } };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var translation = JArray.Parse(jsonResponse)[0]["translations"][0]["text"]?.ToString();
            return translation ?? "(no translation)";
        }
    }
}