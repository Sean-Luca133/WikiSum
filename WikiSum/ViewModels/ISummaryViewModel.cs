using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WikiSum.ViewModels
{
    public interface ISummaryViewModel
    {
        Task<string> GetWebsite();
        ICommand SearchCommand { get; set; }
        ICommand TranslateTextCommand { get; set; }
        ICommand TranslateTestCommand { get; set; }
        ICommand SummarizeTestCommand { get; set; }
        string TestText { get; set; }

    }
}
