using System.Collections.ObjectModel;
using System.Windows.Input;
using WikiSum.Models;

namespace WikiSum.ViewModels
{
    public interface ISummaryViewModel
    {
        Task GetSummary();
        ICommand SearchCommand { get; set; }
        ICommand SeeDetailCommand { get; set; }
        ObservableCollection<WebsiteParagraph> Content { get; set; }
        ObservableCollection<Website> VisitedWebsites { get; set; }
        List<string> LanguageOptions { get; set; }
        int SelectedPageLanguage { get; set; }
        string UrlInput { get; set; }
        string FeedbackMessage { get; set; }
        bool IsError { get; set; }
        string ActionMessage { get; set; }
        bool IsProcessing { get; set; }
    }
}
