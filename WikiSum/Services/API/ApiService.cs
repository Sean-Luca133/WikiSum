using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using WikiSum.Models;
using WikiSum.Services.API.Post;
namespace WikiSum.Services.API
{
    public class ApiService : IAPIService
    {
        private readonly HttpClient _httpClient;
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                   "MyScraper/1.0 (contact: jouw-email@example.com)"
            );
        }

        private string _apiEndpoint = Environment.GetEnvironmentVariable("BACKEND_API_ENDPOINT");



        // TODO ZORG DAT DE LANGUAGE OOK MEE GAAT
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

            // Serialize using Newtonsoft.Json
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
        public Task GetWebsiteByIdAsync(string id)
        {
            throw new NotImplementedException();
        }


        public Task DelteWebsiteByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task GetAllWebsitesAsyc()
        {
            throw new NotImplementedException();
        }

        public Task GetApiStatus()
        {
            throw new NotImplementedException();
        }


    }
}
