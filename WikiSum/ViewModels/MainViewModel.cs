using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using WikiSum.Services.Scrapping;

namespace WikiSum.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        private IScrapeService _scrapeService;
        public MainViewModel(IScrapeService scrapeService)
        {
            _scrapeService = scrapeService;
            BindCommands();
        }
        public void BindCommands()
        {
            SearchCommand = new AsyncRelayCommand(GetWebsite);
        }
        public async Task GetWebsite()
        {
            await _scrapeService.GetRequestAsync(new Uri("https://en.wikipedia.org/wiki/Universally_unique_identifier"));
        }
        public ICommand SearchCommand { get; set; }
    }
}
