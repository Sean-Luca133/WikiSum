using AngleSharp.Common;
using Azure.AI.TextAnalytics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;
using System.Windows.Input;
using WikiSum.Models;
using WikiSum.Services.LanguageDetection;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;
using Timer = System.Timers.Timer;


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

        // Summary lists (needed for timer)
        private string[] transSummaries { get; set; }
        private string[] transHeadingings{ get; set; }
        private int currentListIndex { get; set; } = 0;
        private Timer timer = new Timer(2000);



        // Url properties
        private string urlInput = string.Empty;
        private string feedbackMessage = string.Empty;
        private bool isError = false;
        private string actionMessage = string.Empty;
        private bool isProcessing = false; 
        
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
        public bool IsError
        {
            get => isError;
            set => SetProperty(ref isError, value);
            
        }
        public string ActionMessage
        {
            get => actionMessage;
            set => SetProperty(ref actionMessage, value);
        }
        public bool IsProcessing
        {
            get => isProcessing;
            set => SetProperty(ref isProcessing, value);
        }


        public SummaryViewModel(ILanguageDetectionService languageDetectionService,ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService)
        {
            _languageDetectionService = languageDetectionService;
            _translationService = translationService;
            _summarizationService = summarizationService;
            _scrapeService = scrapeService;
            LoadLanguageAbriviations();
            SetTimerEventHandeler();

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
        
        private void SetTimerEventHandeler()
        {
            timer.Elapsed += new ElapsedEventHandler(SummeryPerInterval);
        }

        public void BindCommands()
        {
            SearchCommand = new AsyncRelayCommand(GetSummary);
        }

        public async Task GetSummary()
        {
            // Reset propeties on new search 
            FeedbackMessage = "";
            IsError = false;

            // Start process
            IsProcessing = true;

            Content = new ObservableCollection<WebsiteParagraph>();
            ActionMessage = "Setting up service...";
            
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
            ActionMessage = "Getting page content...";
            List<(string, string)> websiteContent = await _scrapeService.GetRequestAsync(new Uri(UrlInput));
            if (websiteContent == null)
            {
                FeedbackMessage = "No content found, please check if given url exists";
                IsError = true;

                // End process
                IsProcessing = false;
                return (false, new List<(string, string)>());
            }

            if (websiteContent.Count < 1)
            {
                FeedbackMessage = "No headings found in article, Check if it is a wikipedia link";
                IsError = true;

                // End process
                IsProcessing = false;
                return (false, new List<(string, string)>());
            }

            return (true,websiteContent);
        }

        private bool UrlIsValid()
        {
            if (UrlInput == string.Empty)
            {
                FeedbackMessage = "No url given, please specify the wiki page";
                IsError = true;

                // End process
                IsProcessing = false;
                return false;
            }


            if (!Uri.IsWellFormedUriString(UrlInput, UriKind.Absolute))
            {
                FeedbackMessage = "Url is not valid, please check if given url exists";
                IsError = true;

                // End process
                IsProcessing = false;
                return false;
            }

            return true;
        }

        private async Task TranslateAndSummerizeResult(List<(string, string)> websiteContent, string outputAbbreviation)
        {
            ActionMessage = "Getting page language and summarizing paragraphs...";
            // detect language based on first paragraph
            string foundLanguage = "";
            string firstParagraph = websiteContent.Select(w => w.Item2).FirstOrDefault();
    
            // 5120 is the max length service allows
            if (firstParagraph.Length > 5120)
            {
                foundLanguage = await DetectLanguage(firstParagraph.Substring(0, 5119));
            }
            else
            {
                foundLanguage = await DetectLanguage(firstParagraph);
            }

            Debug.WriteLine($"[SummaryViewModel] language: {foundLanguage}");
            string inputAbbreviation = LanguageAbreviations[foundLanguage];



            List<string> currentParagraphs = websiteContent.Select(t => t.Item2).ToList();

            // 1. Translate all paragraphs concurrently if not supported by summary service
            var preTranslationTasks = currentParagraphs.Select(async item =>
            {
                string translated = await TranslateText(item, inputAbbreviation, "en");
                return (translated);
            });


            List<string> paragraphResults;
            // if not supported first translate to english
            if (!supportedLanguages.Contains(LanguageAbreviations[foundLanguage]))
            {
                ActionMessage = "Not Supported language found, translating to continue...";
                string[] translatedParagraphs = await Task.WhenAll(preTranslationTasks);
                paragraphResults = translatedParagraphs.ToList();
            }
            else
            {
                paragraphResults = currentParagraphs;
            }

            ActionMessage = "Summarizing paragraphs...";
            // Make batch for summary (max per batch 25)
            var summaries = new List<AbstractiveSummary>();

            foreach (var batch in paragraphResults.Chunk(25))
            {
                var result = await Summarizetext(batch.ToList());

                var summaryPart = result.GetValues()
                    .SelectMany(collection => collection)
                    .Where(doc => !doc.HasError)
                    .SelectMany(doc => doc.Summaries);

                summaries.AddRange(summaryPart);
            }


           



            Debug.WriteLine($"[SummaryViewModel]: {summaries.Count()}");
            Debug.WriteLine($"[SummaryViewModel]: {summaries.Distinct()}");


            List<string> totalSummariesList = new List<string>();
            List<string> headingList = websiteContent.Select(w => w.Item1).ToList();

            ActionMessage = "Translating results...";
            var afterTranslationTasksPar = totalSummariesList.Select(async item =>
            {
                string translated = await TranslateText(item, "en", outputAbbreviation);
                return (translated);
            });


            var afterTranslationTasksHead = headingList.Select(async item =>
            {
                string translated = await TranslateText(item, inputAbbreviation, outputAbbreviation);
                return (translated);
            });

            foreach (AbstractiveSummary summary in summaries)
            {
                string summaryTotal = $"{summary.Text.Replace("\n", " ")} ";
                
                Debug.WriteLine($"[SummaryViewModel]: {summaryTotal}");
                totalSummariesList.Add(summaryTotal);

            }

            // Translate
            transSummaries = await Task.WhenAll(afterTranslationTasksPar);
            transHeadingings = await Task.WhenAll(afterTranslationTasksHead);


            ActionMessage = "Showing results...";
            // Timer
            timer.Start();
            Debug.WriteLine("[SummaryViewModel]: Search is done");





            //foreach ((string heading, string paragraph) in websiteContent)
            //{

            //    Debug.WriteLine(paragraph);


            //    // Summary
            //    string summaryTotal = "";

                

            //    // Get summary data
            //    var summaries = summaryResult.GetValues()
            //                               .SelectMany(collection => collection)
            //                               .Where(doc => !doc.HasError)
            //                               .SelectMany(doc => doc.Summaries);


            //    // Put everything together
            //    foreach (var summary in summaries)
            //    {
            //        summaryTotal += $"{summary.Text.Replace("\n", " ")} ";
            //    }


            //    Debug.WriteLine($"[SummaryViewModel] Summary: {summaryTotal}");
  
            //    // Translate
            //    string transSummary = await TranslateText(summaryTotal,"en", outputAbbreviation);
            //    string transHeading = await TranslateText(heading, inputAbbreviation, outputAbbreviation);


            //    // Add directly to main thread to have a live update
            //    MainThread.BeginInvokeOnMainThread(() =>
            //    {
            //        Content.Add(new WebsiteParagraph(transHeading, transSummary));
            //});

                
            //}
        }
        private void SummeryPerInterval(object sender, ElapsedEventArgs e)
        {

            if(currentListIndex >= transSummaries.Length - 1)
            {
                ActionMessage = string.Empty;
                timer.Stop();
                currentListIndex = 0;

                // End process
                IsProcessing = false;
            }
            else
            {
                // Add directly to main thread to have a live update
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Content.Add(new WebsiteParagraph(transHeadingings.ElementAt(currentListIndex), transSummaries.ElementAt(currentListIndex)));
                });
                currentListIndex++;
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
