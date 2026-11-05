using System.Collections.ObjectModel;
using System.Windows.Input;
using WikiSum.Models.Api.Website;

namespace WikiSum.ViewModels
{
    public interface IMainViewModel
    {
        Task<List<(string, string)>?> GetWebsite();
        ICommand LanguageDetectionServiceTestCommand { get; set; }
        ICommand SearchCommand { get; set; }
        ICommand TranslateTextCommand { get; set; }
        ICommand TranslateTestCommand { get; set; }
        ICommand SummarizeServiceTestCommand { get; set; }
        ICommand DeleteWebsiteCommand { get; set; }
        string TestText { get; set; }
        string Status { get; set; }
        ObservableCollection<WebsiteRequest> Websites { get; set; }
        Task CheckStatus();
    }
}
