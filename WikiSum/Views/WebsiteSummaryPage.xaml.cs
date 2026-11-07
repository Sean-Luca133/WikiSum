using WikiSum.ViewModels;

namespace WikiSum.Views;

public partial class WebsiteSummaryPage : ContentPage
{
	public WebsiteSummaryPage(IWebsiteSummaryViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}