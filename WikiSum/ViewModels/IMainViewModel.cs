using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WikiSum.ViewModels
{
    public interface IMainViewModel
    {
        Task<string> GetWebsite();
        ICommand SearchCommand { get; set; }
        ICommand TranslateTextCommand { get; set; }
        ICommand TranslateTestCommand { get; set; }
        ICommand SummarizeTestcommand { get; set; }
        string TestText { get; set; }

    }
}
