using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WikiSum.Models;

namespace WikiSum.ViewModels
{
    public interface ISummaryDetailViewModel
    {
        FormattedString AnalyzedParagraph { get; set; }
        ICommand NavigateBackCommand { get; set; }
        Task NavigateBack();
    }
}
