using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WikiSum.Messages;
using WikiSum.Models;
using WikiSum.Models.Api.Website;
using WikiSum.Services.Navigation;

namespace WikiSum.ViewModels
{
    public class WebsiteSummaryViewModel : ObservableRecipient, IWebsiteSummaryViewModel, IRecipient<ChosenWebsiteMessage>
    {
        // Commands
        public ICommand SeeDetailCommand { get; set; }
        public ICommand NavigateBackCommand { get; set; }

        // Language properties 
        public Dictionary<string, string> LanguageAbreviations { get; set; } = new Dictionary<string, string>();

        public ObservableCollection<string> languageOptions = new();
        
        public int selectedLanguage = -1; // Set selected language to first in list

        // Objects/Services
        public List<TranslationsRequest> Translations { get; set; } = new();
        private ObservableCollection<WebsiteParagraph> paragraphs = new ObservableCollection<WebsiteParagraph>();
        private INavigationService _navigationService;

        
        public ObservableCollection<WebsiteParagraph> Paragraphs
        {
            get => paragraphs;
            set => SetProperty(ref paragraphs, value);

        }

        public int SelectedLanguage
        {
            get => selectedLanguage;
            set {
                SetProperty(ref selectedLanguage, value);
                SetSelectedLanguageParagraphs();
            }
        }

        public ObservableCollection<string> LanguageOptions
        {
            get => languageOptions;
            set
            {
                SetProperty(ref languageOptions, value);
            }
        }


        public WebsiteSummaryViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Bindcommands();
            Messenger.Register<WebsiteSummaryViewModel, ChosenWebsiteMessage>(this, (r, m) => r.Receive(m));
        }

        private void Bindcommands()
        {
            SeeDetailCommand = new AsyncRelayCommand<WebsiteParagraph>(SeeDetail);
            NavigateBackCommand = new AsyncRelayCommand(NavigateBack);
        }

        public void Receive(ChosenWebsiteMessage message)
        {
            Debug.WriteLine($"[WebsiteSummaryViewModel]: Receved message: {message.Value}");
            List<ChapterRequest> paragraphChapters = message.Value.Chapters;

            // Use first chapter to get all possible languages
            ChapterRequest? firstChapter = paragraphChapters.ElementAt(0);

            LanguageOptions = firstChapter != null ? new ObservableCollection<string>(firstChapter.Translations.Select(t => t.Language).ToList()) : new ObservableCollection<string>();
            Debug.WriteLine($"[WebsiteSummaryViewModel]: Available Languages: {LanguageOptions.Count}");
            SelectedLanguage = 0;
            Translations = firstChapter != null && LanguageOptions.Count > 0 ? paragraphChapters.SelectMany(c => c.Translations).ToList() : new List<TranslationsRequest>();
            SetSelectedLanguageParagraphs();
        }

        private void SetSelectedLanguageParagraphs()
        {
            List<string> CurrentLanguageSummaries = Translations.Where(c => c.Language == LanguageOptions.ElementAt(SelectedLanguage)).SelectMany(c => c.Summaries.Select(s => s.SummaryText)).ToList();
            Debug.WriteLine($"[WebsiteSummaryViewModel]: Used Summaries count: {CurrentLanguageSummaries.Count}");

            List<string> Headings =  LanguageOptions.Count > 0 ? Translations.Where(c => c.Language == LanguageOptions.ElementAt(SelectedLanguage)).Select(c => c.Title).ToList() : new List<string>();
            Debug.WriteLine($"[WebsiteSummaryViewModel]: heading count: {Headings.Count}");

            // reset paragraphs on every call.
            Paragraphs.Clear();
            for (int i = 0; i < CurrentLanguageSummaries.Count; i++)
            {
                Debug.WriteLine($"[WebsiteSummaryViewModel]: Header {i + 1}: {Headings.ElementAt(i)}");
                Debug.WriteLine($"[WebsiteSummaryViewModel]: Summary {i + 1}: {CurrentLanguageSummaries.ElementAt(i)}");
                Paragraphs.Add(new WebsiteParagraph(Headings.ElementAt(i), CurrentLanguageSummaries.ElementAt(i)));
            }
        }

        private async Task SeeDetail(WebsiteParagraph paragraph)
        {
            Debug.WriteLine($"[SummaryViewModel]: Send message: {paragraph}");
            await _navigationService.NavigateToDetailAsync();
            WeakReferenceMessenger.Default.Send(new ChosenSummaryMessage(paragraph));

        }

        private async Task NavigateBack()
        {
            Debug.WriteLine($"Navigate back");
            await _navigationService.NavigateBack();
        }
    }
    }
