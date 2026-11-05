using WikiSum.Models;
using WikiSum.Services.API.Health;
using WikiSum.Services.API.Post;

namespace WikiSum.Services.API
{
    public interface IAPIService
    {
        Task PostWebsiteAsyc(string url, List<WebsiteParagraph> websiteParagraphs, string language);
        Task<WebsiteRequest> GetWebsiteByUrlAsync(string url);
        Task<List<WebsiteRequest>> GetAllWebsitesAsyc();
        Task<HealthRequest> GetApiStatus();
        Task DelteWebsiteByUrlAsync(string url);
    }
}
