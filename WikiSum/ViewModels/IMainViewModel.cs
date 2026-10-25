using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WikiSum.ViewModels
{
    public interface IMainViewModel
    {
        ICommand TranslateTextCommand { get; set; }
        ICommand TranslateTestCommand { get; set; }
        ICommand SummarizeTestcommand { get; set; }
        string TestText { get; set; }

    }
}
