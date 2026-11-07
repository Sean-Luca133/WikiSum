using Azure.AI.TextAnalytics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WikiSum.Messages;
using WikiSum.Models;
using WikiSum.Models.Api.Health;
using WikiSum.Models.Api.Website;
using WikiSum.Services.API;
using WikiSum.Services.EntityReconition;
using WikiSum.Services.LanguageDetection;
using WikiSum.Services.Navigation;
using WikiSum.Services.Scrapping;
using WikiSum.Services.Sumerization;
using WikiSum.Services.Translation;


namespace WikiSum.ViewModels
{
    public class MainViewModel : ObservableRecipient, IMainViewModel
    {
        private INavigationService _navigationService;
        private IAPIService _apiService;

        private string _status = "Fetching api status ...";
        private ObservableCollection<WebsiteRequest> _websites;
        public ICommand DeleteWebsiteCommand { get; set; }
        public ICommand SeeSummaryCommand { get; set; }
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

        public MainViewModel(INavigationService navigation,IEntityRecognitionService entityRecognitionService,ILanguageDetectionService languageDetectionService, ITranslationService translationService, ISummarizationService summarizationService, IScrapeService scrapeService, IAPIService apiService)
        {
            _apiService = apiService;
            _navigationService = navigation;
            Websites = new();
            BindCommands();

        }

        public void BindCommands()
        {
            DeleteWebsiteCommand = new AsyncRelayCommand<WebsiteRequest>(DeleteWebsite);
            SeeSummaryCommand = new AsyncRelayCommand<WebsiteRequest>(SeeSummary);
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
            try { 
            HealthRequest status = await _apiService.GetApiStatus();
            Status = "Api status: " + status.Status;

                await GetAllPreviousWebsites();

                Debug.WriteLine($"[MainViewModel] Loaded {Websites.Count} websites");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainViewModel] Error: {ex.Message}");
            }
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

        private async Task SeeSummary(WebsiteRequest website)
        {
            Debug.WriteLine($"[SummaryViewModel]: Send message: {website}");
            await _navigationService.NavigateToSummaryAsync();
            WeakReferenceMessenger.Default.Send(new ChosenWebsiteMessage(website));
        }

    }
}

