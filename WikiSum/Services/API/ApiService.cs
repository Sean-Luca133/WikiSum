using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using WikiSum.Models;
using WikiSum.Services.API.Health;
using WikiSum.Services.API.Post;
namespace WikiSum.Services.API
{
    public class ApiService : IAPIService
    {
        private readonly HttpClient _httpClient;
        private string _apiEndpoint = Environment.GetEnvironmentVariable("BACKEND_API_ENDPOINT");

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                   "MyScraper/1.0 (contact: jouw-email@example.com)"
            );
        }
        public async Task PostWebsiteAsyc(string url, List<WebsiteParagraph> websiteParagraphs, string language)
        {
            Debug.WriteLine($"[ApiService]: Posting new website to api");

            WebsiteRequest websiteRequest = new WebsiteRequest
            {
                Url = url,
                Chapters = websiteParagraphs
                .Select(paragraph => new ChapterRequest
                {
                    Title = paragraph.Header,
                    Summaries = new List<SummaryRequest>
                    {
                        new SummaryRequest
                        {
                            SummaryText = paragraph.ParagraphSummary,
                            Language = language
                        }
                    }
                })
                .ToList()
            };

            string json = JsonConvert.SerializeObject(
                websiteRequest,
                Formatting.Indented
            );

            Debug.WriteLine($"[ApiService]: JSON request:\n{json}");

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.PostAsync(_apiEndpoint + "Webpage", content);

            Debug.WriteLine($"[ApiService]: Response status: {response.StatusCode}");

            string responseBody = await response.Content.ReadAsStringAsync();

            Debug.WriteLine($"[ApiService]: Response body: {responseBody}");

            response.EnsureSuccessStatusCode();
        }
        public async Task<WebsiteRequest> GetWebsiteByUrlAsync(string url)
        {
            string encodedUrl = Uri.EscapeDataString(url);

            HttpResponseMessage response = await _httpClient.GetAsync(
                _apiEndpoint + "Webpage/" + encodedUrl
            );

            Debug.WriteLine($"[ApiService]: Response status: {response.StatusCode}");
            string responseBody = await response.Content.ReadAsStringAsync();

            Debug.WriteLine($"[ApiService]: Response body: {responseBody}");
            response.EnsureSuccessStatusCode();

            var website = JsonConvert.DeserializeObject<WebsiteRequest>(responseBody);

            return website ?? new WebsiteRequest();
        }


        public async Task DelteWebsiteByUrlAsync(string url)
        {
            string encodedUrl = Uri.EscapeDataString(url);

            HttpResponseMessage response = await _httpClient.DeleteAsync(
                _apiEndpoint + "Webpage/" + encodedUrl
            );

        }

        public async Task<List<WebsiteRequest>> GetAllWebsitesAsyc()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_apiEndpoint + "Webpage");

            Debug.WriteLine($"[ApiService]: Response status: {response.StatusCode}");
            string responseBody = await response.Content.ReadAsStringAsync();

            Debug.WriteLine($"[ApiService]: Response body: {responseBody}");
            response.EnsureSuccessStatusCode();

            var websites = JsonConvert.DeserializeObject<List<WebsiteRequest>>(responseBody);

            return websites ?? new List<WebsiteRequest>();
        }

        public async Task<HealthRequest> GetApiStatus()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_apiEndpoint + "Health");

            Debug.WriteLine($"[ApiService]: Response status: {response.StatusCode}");
            string responseBody = await response.Content.ReadAsStringAsync();

            Debug.WriteLine($"[ApiService]: Response body: {responseBody}");
            response.EnsureSuccessStatusCode();

            var healthRequest = JsonConvert.DeserializeObject<HealthRequest>(responseBody);

            return healthRequest ?? new HealthRequest();
        }


    }
}
