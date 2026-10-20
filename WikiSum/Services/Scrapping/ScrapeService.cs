using System.Diagnostics;

namespace WikiSum.Services.Scrapping
{
    public class ScrapeService : IScrapeService
    {
        private readonly HttpClient _httpClient;
        public ScrapeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Debug.WriteLine("[ScrapService]: Constructor wordt goed aan geroepen.");
        }
        public async Task<HttpResponseMessage> GetRequestAsync(Uri requestUri)
        {
            return await _httpClient.GetAsync(requestUri);
        }
    }
}
