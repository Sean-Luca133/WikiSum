using WikiSum.ViewModels;

namespace WikiSum.Views;

public partial class MainPage : ContentPage
{
    public MainPage(IMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}