using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using WikiSum.Models;
using WikiSum.Models.Api.Health;
using WikiSum.Models.Api.Website;
using WikiSum.Models.Api.Website.UpdateReq;
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
        //TODO ZET ALLES IN TRY CATCH BLOKKEN
        public async Task PostWebsiteAsyc(string url, List<WebsiteParagraph> websiteParagraphs, string language)
        {
            Debug.WriteLine($"[ApiService]: Posting new website to api");

            WebsiteRequest websiteRequest = new WebsiteRequest
            {
                Url = url,
                Chapters = websiteParagraphs
                .Select(paragraph => new ChapterRequest
                {
                    Translations = new List<TranslationsRequest>
                    {
                        new TranslationsRequest
                        {
                            Title = paragraph.Header,
                            Language = language,
                            Summaries = new List<SummaryRequest>
                            {
                                new SummaryRequest
                                {
                                    SummaryText = paragraph.ParagraphSummary
                                }
                            }
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
            try
            {
                string encodedUrl = Uri.EscapeDataString(url);

                HttpResponseMessage response = await _httpClient.GetAsync(
                    _apiEndpoint + "Webpage/" + encodedUrl
                );

                Debug.WriteLine($"[ApiService]: Response status: {response.StatusCode}");
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.WriteLine($"[ApiService]: Response body: {responseBody}");
                response.EnsureSuccessStatusCode();

                WebsiteRequest website = JsonConvert.DeserializeObject<WebsiteRequest>(responseBody);

                return website;
            }
            catch (Exception ex)
            {
                return null;
            }
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
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiEndpoint + "Health");
                Debug.WriteLine($"[ApiService]: Response status: {response.StatusCode}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"[ApiService]: Response body: {responseBody}");
                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<HealthRequest>(responseBody);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ApiService]: Error with message :{e.Message}");
                return new()
                {
                    Status = "Not Healthy",
                    Message = "404",
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }
        }

        public async Task<WebsiteRequest> AddNewLanguageToWebsiteByUrlAsync(string url, UpdateWebpageRequest request)
        {
            string encodedUrl = Uri.EscapeDataString(url);

            string json = JsonConvert.SerializeObject(
                request,
                Formatting.Indented
            );

            Debug.WriteLine($"[ApiService]: PUT request:\n{json}");

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.PutAsync(
                _apiEndpoint + "Webpage/" + encodedUrl,
                content
            );

            string responseBody = await response.Content.ReadAsStringAsync();

            Debug.WriteLine($"[ApiService]: PUT status: {response.StatusCode}");
            Debug.WriteLine($"[ApiService]: PUT response: {responseBody}");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<WebsiteRequest>(responseBody)
                   ?? new WebsiteRequest();
        }
    }
}
