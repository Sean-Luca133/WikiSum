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


namespace WikiSum.ViewModels
{
    public class SummaryViewModel : ObservableObject, ISummaryViewModel
    {
        private IScrapeService _scrapeService;
        private ITranslationService _translationService;
        private ISummarizationService _summarizationService;
        public ICommand TranslateTextCommand { get; set; }
        public ICommand SearchCommand { get; set; }


        public SummaryViewModel(ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService)
        {
            _translationService = translationService;
            _summarizationService = summarizationService;
            _scrapeService = scrapeService;
            BindCommands();

        }
        public void BindCommands()
        {
            SearchCommand = new AsyncRelayCommand(GetWebsite);
            // add translate command here
        }

        public async Task<List<string>?> GetWebsite()
        {
            return await _scrapeService.GetRequestAsync(new Uri("https://en.wikipedia.org/wiki/Belgium"));


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
