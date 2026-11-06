using WikiSum.ViewModels;

namespace WikiSum.Views;

public partial class EntityPopup : ContentView
{
    public EntityPopup(EntityPopupViewModel EntityPopupViewModel)
    {
        InitializeComponent();
        BindingContext = EntityPopupViewModel;
    }
}