using Azure.AI.TextAnalytics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;


namespace WikiSum.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private IScrapeService _scrapeService;
        private ITranslationService _translationService;
        private ISummarizationService _summarizationService;
        public ICommand TranslateTextCommand { get; set; }
        public string TestText { get; set; } = "How are you?";
        public ICommand TranslateTestCommand { get; set; }
        public ICommand SummarizeTestCommand { get; set; }
        public ICommand SearchCommand { get; set; }


        public MainViewModel(ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService)
        {
            _translationService = translationService;
            _summarizationService = summarizationService;
            _scrapeService = scrapeService;
            BindCommands();

        }
        public void BindCommands()
        {
            SearchCommand = new AsyncRelayCommand(GetWebsite);
            TranslateTestCommand = new AsyncRelayCommand(TranslateTest);
            SummarizeTestCommand = new AsyncRelayCommand(SummarizeTest);
        }

        public async Task<List<string>?> GetWebsite()
        {
            return await _scrapeService.GetRequestAsync(new Uri("https://en.wikipedia.org/wiki/Belgium"));


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

        private async Task SummarizeTest()
        {
            List<(string, string)>? documents = await GetWebsite();
            if (documents == null)
            {
                throw new ApplicationException("Wiki doesn't exist");
            }

            foreach (var (nummer, zin) in documents)
            {
                Debug.WriteLine(nummer);
                Debug.WriteLine(zin);
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
