using WikiSum.ViewModels;

namespace WikiSum.Views;

public partial class MainPage : ContentPage
{
    private readonly IMainViewModel _viewModel;
    public MainPage(IMainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CheckStatus();
        await _viewModel.InitializeAsync();
    }
}