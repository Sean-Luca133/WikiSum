using AngleSharp;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WikiSum.Services.Scrapping
{
    public class ScrapeService : IScrapeService
    {
        private readonly HttpClient _httpClient;
        public ScrapeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                   "MyScraper/1.0 (contact: jouw-email@example.com)"
            );
            Debug.WriteLine("[ScrapService]: Constructor wordt goed aan geroepen.");
        }
        public async Task<string?> GetRequestAsync(Uri requestUri)
        {
            Debug.WriteLine($"[ScrapService]: Getting URI {requestUri}.");

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                string html = responseBody;
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(html));

                // Verwijder elementen die geen nuttige tekst bevatten
                document.QuerySelectorAll("script, style, noscript, template,iframe,svg,canvas,header,footer,nav, aside")
                    .ToList()
                    .ForEach(element => element.Remove());

                // Haal alle tekst uit het document
                var text = document.Body?.TextContent ?? string.Empty;

                // Normaliseer witruimte
                text = Regex.Replace(text, @"\s+", " ").Trim();

                Console.WriteLine(text);
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                return text;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("\nException Caught!");
                Debug.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}
