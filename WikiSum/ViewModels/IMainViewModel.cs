using System.Windows.Input;

namespace WikiSum.ViewModels
{
    public interface IMainViewModel
    {
        Task<List<(string, string)>?> GetWebsite();
        ICommand LanguageDetectionServiceTestCommand { get; set; }
        ICommand SearchCommand { get; set; }
        ICommand TranslateTextCommand { get; set; }
        ICommand TranslateTestCommand { get; set; }
        ICommand SummarizeServiceTestCommand { get; set; }
        ICommand GetEntitiesCommand { get; set; }
        string TestText { get; set; }

    }
}
