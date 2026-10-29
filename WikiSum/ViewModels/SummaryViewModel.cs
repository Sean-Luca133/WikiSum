using AngleSharp.Common;
using Azure.AI.TextAnalytics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WikiSum.Models;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;


namespace WikiSum.ViewModels
{
    public class SummaryViewModel : ObservableObject, ISummaryViewModel
    {
        // Services/objects
        private IScrapeService _scrapeService;
        private ITranslationService _translationService;
        private ISummarizationService _summarizationService;
        private ObservableCollection<WebsiteParagraph> content = new ObservableCollection<WebsiteParagraph>();
        private ObservableCollection<Website> visitedWebsites = new ObservableCollection<Website>();

        // Language properties 
        public Dictionary<string, string> LanguageAbreviations { get; set; } = new Dictionary<string, string>();
        public List<string> LanguageOptions { get; set; }
        private int selectedPageLanguage;

        // Url properties
        private string urlInput = string.Empty;
        private string feedbackMessage = string.Empty;
        
        // Commands
        public ICommand TranslateTextCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public ObservableCollection<WebsiteParagraph> Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }
        public ObservableCollection<Website> VisitedWebsites
        {
            get => visitedWebsites;
            set => SetProperty(ref visitedWebsites, value);
        }

        public int SelectedPageLanguage
        {
            get => selectedPageLanguage;
            set => SetProperty(ref selectedPageLanguage, value);
        }
        public string UrlInput
        {
            get => urlInput;
            set => SetProperty(ref urlInput, value);
        }

        public string FeedbackMessage
        {
            get => feedbackMessage;
            set => SetProperty(ref feedbackMessage, value);
        }
        

        public SummaryViewModel(ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService)
        {
            _translationService = translationService;
            _summarizationService = summarizationService;
            _scrapeService = scrapeService;
            LanguageAbreviations.Add("Dutch", "nl");
            LanguageAbreviations.Add("English", "en");
            LanguageAbreviations.Add("French", "fr");
            // Extra languages can be add here
            
           
            LanguageOptions = LanguageAbreviations.Keys.ToList();
            BindCommands();

        }
        public void BindCommands()
        {
            SearchCommand = new AsyncRelayCommand(GetSummary);
            // add translate command here
        }

        public async Task GetSummary()
        {
            // Reset propeties on new search 
            FeedbackMessage = "";
            Content = new ObservableCollection<WebsiteParagraph>();
            // Input checks
            if (UrlInput == string.Empty)
            {
                FeedbackMessage = "No url given, please specify the wiki page";
                return;
            }


            if (!Uri.IsWellFormedUriString(UrlInput, UriKind.Absolute))
            {
                FeedbackMessage = "Url is not valid, please check if given url exists";
                return;
            }

            List<(string, string)>? WebsiteContent = await _scrapeService.GetRequestAsync(new Uri(UrlInput));

            if(WebsiteContent == null)
            {
                FeedbackMessage = "No content found, please check if given url exists";
                return;
            }

            if (WebsiteContent.Count < 1)
            {
                FeedbackMessage = "No headings found in article, Check if it is a wikipedia link";
                return;
            }

            await TranslateAndSummerizeResult(WebsiteContent, LanguageAbreviations[LanguageOptions.GetItemByIndex(SelectedPageLanguage)]);
            Debug.WriteLine($"[SummaryViewModel] content count: {Content.Count}");

            visitedWebsites.Add(new(UrlInput, Content));
        }

        private async Task TranslateAndSummerizeResult(List<(string, string)> websiteContent, string inputAbbreviration)
        {
            foreach ((string heading, string paragraph) in websiteContent.GetRange(0, 4))
            {

                // Translate
                string tranParagraph = await TranslateText(paragraph, inputAbbreviration, "en");



                // Summary
                Debug.WriteLine($"[SummaryViewModel]: {tranParagraph}");
                string summaryTotal = "";

                // Make batch for summary
                List<string> batchedDocuments = new() { tranParagraph };
                AbstractiveSummarizeOperation summaryResult = await Summarizetext(batchedDocuments);

                // Get summary data
                var summaries = summaryResult.GetValues()
                                           .SelectMany(collection => collection)
                                           .Where(doc => !doc.HasError)
                                           .SelectMany(doc => doc.Summaries);


                // Put everything together
                foreach (var summary in summaries)
                {
                    summaryTotal += $"{summary.Text.Replace("\n", " ")} ";
                }


                Debug.WriteLine($"[SummaryViewModel] Summary: {summaryTotal}");


                // Add directly to main thread to have a live update
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Content.Add(new WebsiteParagraph(heading, summaryTotal));
            });

                Debug.WriteLine("Search is done");
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
