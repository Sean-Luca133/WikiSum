using WikiSum.Models;

namespace WikiSum.Services.API
{
    public interface IAPIService
    {
        Task PostWebsiteAsyc(string url, List<WebsiteParagraph> websiteParagraphs, string language);
        Task GetWebsiteByIdAsync(string id);
        Task GetAllWebsitesAsyc();
        Task GetApiStatus();
        Task DelteWebsiteByIdAsync(string id);
    }
}
