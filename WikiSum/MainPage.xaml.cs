using WikiSum.ViewModels;

namespace WikiSum;

public partial class MainPage : ContentPage
{
    public MainPage(IMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}