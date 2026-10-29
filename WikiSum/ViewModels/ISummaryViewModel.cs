using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using WikiSum.Models;

namespace WikiSum.ViewModels
{
    public interface ISummaryViewModel
    {
        Task GetSummary();
        ICommand SearchCommand { get; set; }
        ICommand TranslateTextCommand { get; set; }
        ObservableCollection<WebsiteParagraph> Content { get; set; }
        ObservableCollection<Website> VisitedWebsites { get; set; }
        List<string> LanguageOptions { get; set; }
        int SelectedPageLanguage { get; set; }
        string UrlInput { get; set; }
        string FeedbackMessage { get; set; }
    }
}
