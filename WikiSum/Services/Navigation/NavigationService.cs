using System.Diagnostics;
using WikiSum.Views;

namespace WikiSum.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private INavigation _navigation;
        private IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _navigation = Shell.Current?.Navigation;
        }

        public async Task NavigateBack()
        {
            Debug.WriteLine($"[NavigationService]: Navigate Back");
            if (_navigation.NavigationStack.Count > 1)
            {
                // go back to previous page in stack
                await _navigation.PopAsync();
            }
            else
            {
                throw new InvalidOperationException("Can't go back, no page available");
            }

        }

        public async Task NavigateToDetailAsync()
        {
            await _navigation.PushAsync(_serviceProvider.GetRequiredService<SummaryDetailPage>());
        }

        public async Task NavigateToSummaryAsync()
        {
            await _navigation.PushAsync(_serviceProvider.GetRequiredService<WebsiteSummaryPage>());
        }
    }
}