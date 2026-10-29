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
        private IScrapeService _scrapeService;
        private ITranslationService _translationService;
        private ISummarizationService _summarizationService;
        public ICommand TranslateTextCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        private ObservableCollection<WebsiteParagraph> content = new ObservableCollection<WebsiteParagraph>();
        private string urlInput = string.Empty;
        private string feedbackMessage = string.Empty;

        public ObservableCollection<WebsiteParagraph> Content
        {
            get => content;
            set => SetProperty(ref content, value);
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
            BindCommands();

        }
        public void BindCommands()
        {
            SearchCommand = new AsyncRelayCommand(GetSummary);
            // add translate command here
        }

        public async Task GetSummary()
        {
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

            List<(string, string)>? WebsiteContent =  await _scrapeService.GetRequestAsync(new Uri(UrlInput));

            if(WebsiteContent.Count < 1)
            {
                FeedbackMessage = "Given url could not be summarized, Check if it is a wikipedia link";
                return;
            }

            // range is to only get first element to Make it faster to test
            foreach ((string heading, string paragraph) in WebsiteContent)
            {
                Debug.WriteLine("[SummaryViewModel]: {paragraph}");
                string summaryTotal = "";
                // make batch for summary
                List<string> batchedDocuments = new() { paragraph };
                AbstractiveSummarizeOperation summaryResult = await Summarizetext(batchedDocuments);

                // get summary data
                var summaries = summaryResult.GetValues()
                                           .SelectMany(collection => collection)
                                           .Where(doc => !doc.HasError)
                                           .SelectMany(doc => doc.Summaries);


                // put everything together
                foreach (var summary in summaries)
                {
                    summaryTotal += $"{summary.Text.Replace("\n", " ")} ";
                }


                Debug.WriteLine($"[SummaryViewModel]: {summaryTotal}");
                // header can be added as first parameter
                Content.Add(new(heading, summaryTotal));
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
