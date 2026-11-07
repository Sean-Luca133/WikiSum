using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using WikiSum.Models;

namespace WikiSum.ViewModels
{
    public interface IWebsiteSummaryViewModel
    {
        ICommand SeeDetailCommand { get; set; }
        ICommand NavigateBackCommand { get; set; }
        ObservableCollection<WebsiteParagraph> Paragraphs { get; set; }
        ObservableCollection<string>  LanguageOptions { get; set; }
        int SelectedLanguage { get; set; }

    }
}
