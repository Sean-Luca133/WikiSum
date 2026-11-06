using WikiSum.Models;
using WikiSum.Models.Api.Health;
using WikiSum.Models.Api.Website;

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
