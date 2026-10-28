using WikiSum.ViewModels;

namespace WikiSum.Views;

public partial class SummaryPage : ContentPage
{
	public SummaryPage(ISummaryViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}