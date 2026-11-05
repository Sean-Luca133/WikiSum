using Azure.AI.TextAnalytics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;
using WikiSum.Services.EntityReconition;
using WikiSum.Services.LanguageDetection;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;


namespace WikiSum.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private IEntityRecognitionService _entityRecognitionService;
        private ILanguageDetectionService _languageDetectionService;
        private IScrapeService _scrapeService;
        private ITranslationService _translationService;
        private ISummarizationService _summarizationService;
        public ICommand TranslateTextCommand { get; set; }
        public string TestText { get; set; } = "How are you?";
        string document =
        "We love this trail and make the trip every year. The views are breathtaking and well worth the hike!"
        + " Yesterday was foggy though, so we missed the spectacular views. We tried again today and it was"
        + " amazing. Everyone in my family liked the trail although it was too challenging for the less"
        + " athletic among us. Not necessarily recommended for small children. A hotel close to the trail"
        + " offers services for childcare in case you want that.";

        public ICommand TranslateTestCommand { get; set; }
        public ICommand SummarizeServiceTestCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand LanguageDetectionServiceTestCommand { get; set; }
        public ICommand GetEntitiesCommand { get; set; }

        public MainViewModel(IEntityRecognitionService entityRecognitionService, ILanguageDetectionService languageDetectionService, ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService)
        {
            _languageDetectionService = languageDetectionService;
            _translationService = translationService;
            _summarizationService = summarizationService;
            _scrapeService = scrapeService;
            _entityRecognitionService = entityRecognitionService;
            BindCommands();

        }
        public void BindCommands()
        {
            LanguageDetectionServiceTestCommand = new AsyncRelayCommand(DetectLanguageTest);
            SearchCommand = new AsyncRelayCommand(GetWebsite);
            TranslateTestCommand = new AsyncRelayCommand(TranslateTest);
            SummarizeServiceTestCommand = new AsyncRelayCommand(SummarizeTest);
            GetEntitiesCommand = new AsyncRelayCommand(GetEntities);

        }

        public async Task<List<(string, string)>?> GetWebsite()
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

        private async Task GetEntities()
        {
            CategorizedEntityCollection entitiesInDocument = _entityRecognitionService.GetNamedEntities(document);
            Debug.WriteLine($"[MainViewModel]: Recognized {entitiesInDocument.Count} entities:");
            foreach (CategorizedEntity entity in entitiesInDocument)
            {
                Debug.WriteLine($"[MainViewModel]:  Text: {entity.Text}");
                Debug.WriteLine($"[MainViewModel]:  Offset: {entity.Offset}");
                Debug.WriteLine($"[MainViewModel]:  Length: {entity.Length}");
                Debug.WriteLine($"[MainViewModel]:  Category: {entity.Category}");
                if (!string.IsNullOrEmpty(entity.SubCategory))
                    Debug.WriteLine($"[MainViewModel]:  SubCategory: {entity.SubCategory}");
                Debug.WriteLine($"[MainViewModel]:  Confidence score: {entity.ConfidenceScore}");

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
