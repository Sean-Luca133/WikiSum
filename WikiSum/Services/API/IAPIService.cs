using WikiSum.Models;
using WikiSum.Models.Api.Health;
using WikiSum.Models.Api.Website;
using WikiSum.Models.Api.Website.UpdateReq;

namespace WikiSum.Services.API
{
    public interface IAPIService
    {
        Task PostWebsiteAsyc(string url, List<WebsiteParagraph> websiteParagraphs, string language);
        Task<WebsiteRequest> GetWebsiteByUrlAsync(string url);
        Task<List<WebsiteRequest>> GetAllWebsitesAsyc();
        Task<HealthRequest> GetApiStatus();
        Task<WebsiteRequest> AddNewLanguageToWebsiteByUrlAsync(string url, UpdateWebpageRequest request);
        Task DelteWebsiteByUrlAsync(string url);
    }
}
