using System.Windows.Input;

namespace WikiSum.ViewModels
{
    public interface IMainViewModel
    {
        Task<List<(string, string)>?> GetWebsite();
        ICommand SearchCommand { get; set; }
        ICommand TranslateTextCommand { get; set; }
        ICommand TranslateTestCommand { get; set; }
        ICommand SummarizeTestCommand { get; set; }
        string TestText { get; set; }

    }
}
