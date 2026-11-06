using Azure.AI.TextAnalytics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WikiSum.Models.Api.Health;
using WikiSum.Models.Api.Website;
using WikiSum.Services.API;
using WikiSum.Services.LanguageDetection;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;


namespace WikiSum.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private ILanguageDetectionService _languageDetectionService;
        private IScrapeService _scrapeService;
        private ITranslationService _translationService;
        private ISummarizationService _summarizationService;
        private IAPIService _apiService;
        public ICommand TranslateTextCommand { get; set; }
        public string TestText { get; set; } = "How are you?";
        private string _status = "Fetching api status ...";
        private ObservableCollection<WebsiteRequest> _websites;
        public ICommand TranslateTestCommand { get; set; }
        public ICommand SummarizeServiceTestCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand LanguageDetectionServiceTestCommand { get; set; }
        public ICommand DeleteWebsiteCommand { get; set; }

        public ICommand CheckStatusCommand { get; set; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WebsiteRequest> Websites
        {
            get => _websites;
            set => SetProperty(ref _websites, value);
        }

        public MainViewModel(ILanguageDetectionService languageDetectionService, ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService, IAPIService apiService)
        {
            _languageDetectionService = languageDetectionService;
            _translationService = translationService;
            _summarizationService = summarizationService;
            _scrapeService = scrapeService;
            _apiService = apiService;
            Websites = new();
            BindCommands();

        }
        public void BindCommands()
        {
            LanguageDetectionServiceTestCommand = new AsyncRelayCommand(DetectLanguageTest);
            SearchCommand = new AsyncRelayCommand(GetWebsite);
            TranslateTestCommand = new AsyncRelayCommand(TranslateTest);
            SummarizeServiceTestCommand = new AsyncRelayCommand(SummarizeTest);
            DeleteWebsiteCommand = new AsyncRelayCommand<WebsiteRequest>(DeleteWebsite);
        }

        public async Task InitializeAsync()
        {
            try
            {
                Debug.WriteLine($"[MainViewModel] Fetching websites...");

                await GetAllPreviousWebsites();

                Debug.WriteLine($"[MainViewModel] Loaded {Websites.Count} websites");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainViewModel] Error: {ex.Message}");
            }
        }
        public async Task GetAllPreviousWebsites()
        {
            List<WebsiteRequest> websites = await _apiService.GetAllWebsitesAsyc();

            Websites = new ObservableCollection<WebsiteRequest>(websites);
        }

        public async Task CheckStatus()
        {
            HealthRequest status = await _apiService.GetApiStatus();
            Status = "Api status: " + status.Status;
        }

        public async Task<List<(string, string)>?> GetWebsite()
        {
            return await _scrapeService.GetRequestAsync(new Uri("https://en.wikipedia.org/wiki/Belgium"));
        }

        private async Task DeleteWebsite(WebsiteRequest website)
        {
            if (website == null)
                return;

            // Verwijder uit de API/database
            await _apiService.DelteWebsiteByUrlAsync(website.Url);

            // Verwijder uit de UI
            Websites.Remove(website);
        }
        private async Task<string> TranslateText(string text, string start, string to)
        {
            return await _translationService.TranslateText(text, start, to);
        }

        private async Task<AbstractiveSummarizeOperation> Summarizetext(List<string> document)
        {
            return await _summarizationService.SummarizeText(document);
        }

        private async Task<DetectedLanguage?> DetectLanguage(string document)
        {
            return await _languageDetectionService.DetectLanguageInput(document);
        }

        private async Task DetectLanguageTest()
        {
            DetectedLanguage? language = await DetectLanguage(TestText);
            if (language != null)
            {
                Debug.WriteLine($"[MainViewModel]: {language?.Name}");
            }
            else
            {
                Debug.WriteLine($"[MainViewModel]: No language found");
            }

        }

        private async Task SummarizeTest()
        {
            List<(string, string)>? documents = await GetWebsite();
            if (documents == null)
            {
                throw new ApplicationException("Wiki doesn't exist");
            }
            // negeer de references aka het laatste item
            documents = documents.GetRange(0, documents.Count - 1);
            foreach (var (title, sum) in documents)
            {
                // Prepare the input of the text analysis operation. You can add multiple documents to this list and
                // perform the same operation on all of them simultaneously.
                List<string> batchedDocuments = new() { sum };

                AbstractiveSummarizeOperation returnData = await Summarizetext(batchedDocuments);
                var summaries = returnData.GetValues()
                                            .SelectMany(collection => collection)
                                            .Where(doc => !doc.HasError)
                                            .SelectMany(doc => doc.Summaries);

                foreach (var summary in summaries)
                {
                    Debug.WriteLine($"[MainViewModel]: Text: {summary.Text.Replace("\n", " ")}");
                }

            }
            Debug.WriteLine("[MainViewModel]: Summarization Done");

        }

        // method for testing the translation Check debug for translation
        private async Task TranslateTest()
        {
            //TestText await GetWebsite();
            TestText = await TranslateText(TestText, "en", "nl");
            Debug.WriteLine(TestText);
        }
    }
}
