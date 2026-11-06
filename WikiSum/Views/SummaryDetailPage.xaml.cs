using WikiSum.ViewModels;

namespace WikiSum.Views;

public partial class SummaryDetailPage : ContentPage
{
	public SummaryDetailPage(ISummaryDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}