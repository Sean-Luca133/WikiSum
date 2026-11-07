using System.Collections.ObjectModel;
using System.Windows.Input;
using WikiSum.Models.Api.Website;

namespace WikiSum.ViewModels
{
    public interface IMainViewModel
    {
        ICommand SeeSummaryCommand { get; set; }
        ICommand DeleteWebsiteCommand { get; set; }
        string Status { get; set; }
        ObservableCollection<WebsiteRequest> Websites { get; set; }
        Task CheckStatus();
        Task InitializeAsync();
    }
}
