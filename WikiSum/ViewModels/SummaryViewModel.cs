using AngleSharp.Common;
using Azure.AI.TextAnalytics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WikiSum.Models;
using WikiSum.Services.LanguageDetection;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;


namespace WikiSum.ViewModels
{
    public class SummaryViewModel : ObservableObject, ISummaryViewModel
    {
        // Services/objects
        private ILanguageDetectionService _languageDetectionService;
        private IScrapeService _scrapeService;
        private ITranslationService _translationService;
        private ISummarizationService _summarizationService;
        private ObservableCollection<WebsiteParagraph> content = new ObservableCollection<WebsiteParagraph>();
        private ObservableCollection<Website> visitedWebsites = new ObservableCollection<Website>();

        // Language properties 
        public Dictionary<string, string> LanguageAbreviations { get; set; } = new Dictionary<string, string>();
        public List<string> LanguageOptions { get; set; }
        private int selectedPageLanguage;
        private List<string> supportedLanguages = new List<string>
{
    "zh-hans", // Chinese (Simplified)
    "zh-hant", // Chinese (Traditional)
    "en",      // English
    "fr",      // French
    "de",      // German
    "he",      // Hebrew
    "it",      // Italian
    "ja",      // Japanese
    "ko",      // Korean
    "pl",      // Polish
    "pt",      // Portuguese
    "es",      // Spanish
    "nl",      // Dutch
    "sv",      // Swedish
    "da",      // Danish
    "fi",      // Finnish
    "ru",      // Russian
    "no",      // Norwegian
    "tr",      // Turkish
    "ar",      // Arabic
    "cs",      // Czech
    "hu",      // Hungarian
    "th"       // Thai
};

        // Url properties
        private string urlInput = string.Empty;
        private string feedbackMessage = string.Empty;
        
        // Commands
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
        

        public SummaryViewModel(ILanguageDetectionService languageDetectionService,ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService)
        {
            _languageDetectionService = languageDetectionService;
            _translationService = translationService;
            _summarizationService = summarizationService;
            _scrapeService = scrapeService;
            LoadLanguageAbriviations();

            LanguageOptions = LanguageAbreviations.Keys.ToList();
            BindCommands();

        }

        private void LoadLanguageAbriviations()
        {
            LanguageAbreviations.Add("Dutch", "nl");
            LanguageAbreviations.Add("English", "en");
            LanguageAbreviations.Add("French", "fr");
            LanguageAbreviations.Add("Spanish", "es");
            // Extra languages can be add here
        }

        public void BindCommands()
        {
            SearchCommand = new AsyncRelayCommand(GetSummary);
        }

        public async Task GetSummary()
        {
            // Reset propeties on new search 
            FeedbackMessage = "";
            Content = new ObservableCollection<WebsiteParagraph>();

            // Input checks

            if (!UrlIsValid())
            {
                return;
            }

            
            (bool ContentIsCorrect, List<(string, string)> websiteContent) = await GetWebsiteContent();
            if (!ContentIsCorrect)
            {
                return;
            }

            await TranslateAndSummerizeResult(websiteContent, LanguageAbreviations[LanguageOptions.GetItemByIndex(SelectedPageLanguage)]);

            visitedWebsites.Add(new(UrlInput, Content));
        }

        private async Task<(bool, List<(string, string)>)> GetWebsiteContent()
        {
            List<(string, string)> websiteContent = await _scrapeService.GetRequestAsync(new Uri(UrlInput));
            if (websiteContent == null)
            {
                FeedbackMessage = "No content found, please check if given url exists";
                return (false, new List<(string, string)>());
            }

            if (websiteContent.Count < 1)
            {
                FeedbackMessage = "No headings found in article, Check if it is a wikipedia link";
                return (false, new List<(string, string)>());
            }

            return (true,websiteContent);
        }

        private bool UrlIsValid()
        {
            if (UrlInput == string.Empty)
            {
                FeedbackMessage = "No url given, please specify the wiki page";
                return false;
            }


            if (!Uri.IsWellFormedUriString(UrlInput, UriKind.Absolute))
            {
                FeedbackMessage = "Url is not valid, please check if given url exists";
                return false;
            }

            return true;
        }

        private async Task TranslateAndSummerizeResult(List<(string, string)> websiteContent, string outputAbbreviation)
        {
            foreach ((string heading, string paragraph) in websiteContent)
            {

                Debug.WriteLine(paragraph);

                string foundLanguage = "";
                // Detect language of paragraph
                // 5120 is the max length service allows
                if (paragraph.Length > 5120)
                {
                    foundLanguage = await DetectLanguage( paragraph.Substring(0, 5119));
                }
                else
                {
                    foundLanguage = await DetectLanguage(paragraph);
                }


                Debug.WriteLine($"[SummaryViewModel] language: {foundLanguage}");
                string inputAbbreviation = LanguageAbreviations[foundLanguage];

                string inputParagraph = "";
                // if not supported first translate to english
                if (!supportedLanguages.Contains(LanguageAbreviations[foundLanguage]))
                {
                    // Translate to English if not supported
                    inputParagraph = await TranslateText(paragraph, inputAbbreviation, "en");

                }
                else
                {
                    inputParagraph = paragraph;
                }


                // Summary
                string summaryTotal = "";

                // Make batch for summary
                List<string> batchedDocuments = new() { inputParagraph };
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
  
                // Translate
                string transSummary = await TranslateText(summaryTotal,"en", outputAbbreviation);
                string transHeading = await TranslateText(heading, inputAbbreviation, outputAbbreviation);


                // Add directly to main thread to have a live update
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Content.Add(new WebsiteParagraph(transHeading, transSummary));
            });

                Debug.WriteLine("Search is done");
            }
        }

        private async Task<string> TranslateText(string text, string start, string to)
        {
            return await _translationService.TranslateText(text, start, to);
        }
        private async Task<string?> DetectLanguage(string paragraph)
        {
            DetectedLanguage? language = await _languageDetectionService.DetectLanguageInput(paragraph);
            return language?.Name;
        }

        private async Task<AbstractiveSummarizeOperation> Summarizetext(List<string> document)
        {
            return await _summarizationService.SummarizeText(document);
        }

       
    }
}
