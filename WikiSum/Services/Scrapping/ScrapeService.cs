using System.Diagnostics;

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
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Debug.WriteLine(responseBody);
                return responseBody;
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
