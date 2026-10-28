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
        public string TestText { get; set; } = "How are you?";
        public ICommand TranslateTestCommand { get; set; }
        public ICommand SummarizeTestCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        

        public SummaryViewModel(ITranslationService translationService,ISummarizationService summarizationService, IScrapeService scrapeService)
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

          public async Task<string> GetWebsite()
          {
            string? result = await _scrapeService.GetRequestAsync(new Uri("https://en.wikipedia.org/wiki/Universally_unique_identifier"));
            if (!String.IsNullOrEmpty(result))
            {
                Debug.WriteLine($"[MainViewModel]: {result}");
                return result;
            }
            return string.Empty;
              
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
            string document = await GetWebsite();
            Debug.WriteLine($"[MainViewModel]:{document.Length}");

            // Prepare the input of the text analysis operation. You can add multiple documents to this list and
            // perform the same operation on all of them simultaneously.
            List<string> batchedDocuments = new()
            {
                document
            };

            AbstractiveSummarizeOperation returnData = await Summarizetext(batchedDocuments);
            foreach (AbstractiveSummarizeResultCollection documents in returnData.GetValues())
            {
                foreach (AbstractiveSummarizeResult documentResult in documents)
                {
                    // Error check
                    if (documentResult.HasError)
                    {
                        Debug.WriteLine($"[MainViewModel]  Error!");
                        Debug.WriteLine($"[MainViewModel]  Document error code: {documentResult.Error.ErrorCode}");
                        Debug.WriteLine($"[MainViewModel]  Message: {documentResult.Error.Message}");
                        continue;
                    }


                    foreach (AbstractiveSummary summary in documentResult.Summaries)
                    {
                        Debug.WriteLine($"[MainViewModel]  Text: {summary.Text.Replace("\n", " ")}");
                        Debug.WriteLine($"[MainViewModel]  Contexts:");

                        foreach (AbstractiveSummaryContext context in summary.Contexts)
                        {
                            Debug.WriteLine($"[MainViewModel]    Offset: {context.Offset}");
                            Debug.WriteLine($"[MainViewModel]    Length: {context.Length}");
                        }
                    }
                }
;
                Debug.WriteLine("Summarization Done");

            }
        }

        // method for testing the translation Check debug for translation
        private async Task TranslateTest()
        {
            TestText = await GetWebsite();
            TestText = await TranslateText(TestText, "en", "nl");
            Debug.WriteLine(TestText);
        }
    }
}
