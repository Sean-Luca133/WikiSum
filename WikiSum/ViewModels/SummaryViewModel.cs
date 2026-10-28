using Azure.AI.TextAnalytics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;
using WikiSum.Services.Scrapping;
using System.Text.RegularExpressions;
using WikiSum.Models;
using System.Collections.ObjectModel;


namespace WikiSum.ViewModels
{
    public class SummaryViewModel : ObservableObject, ISummaryViewModel
    {
        private IScrapeService _scrapeService;
        private ITranslationService _translationService;
        private ISummarizationService _summarizationService;
        public ICommand TranslateTextCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        private ObservableCollection<WebsiteParagraph> content = new ObservableCollection<WebsiteParagraph>();

        public ObservableCollection<WebsiteParagraph> Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        public SummaryViewModel(ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService)
        {
            _translationService = translationService;
            _summarizationService = summarizationService;
            _scrapeService = scrapeService;
            BindCommands();

        }
        public void BindCommands()
        {
            SearchCommand = new AsyncRelayCommand(GetSummary);
            // add translate command here
        }

        public async Task GetSummary()
        {
           List<string> WebsiteContent =  await _scrapeService.GetRequestAsync(new Uri("https://en.wikipedia.org/wiki/Belgium"));

           foreach(string summary in WebsiteContent)
            {
                Debug.WriteLine($"[SummaryViewModel]: {summary}");
                // header can be added as first parameter
                Content.Add(new("", summary));
            }
        }


        private async Task<string> TranslateText(string text, string start, string to)
        {
            return await _translationService.TranslateText(text, start, to);
        }

        private async Task<AbstractiveSummarizeOperation> Summarizetext(List<string> document)
        {
            return await _summarizationService.SummarizeText(document);
        }

       
    }
}
