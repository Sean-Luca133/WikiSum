using System.Windows.Input;

namespace WikiSum.ViewModels
{
    public interface IMainViewModel
    {
        ICommand SearchCommand { get; set; }

        Task GetWebsite();
    }
}
