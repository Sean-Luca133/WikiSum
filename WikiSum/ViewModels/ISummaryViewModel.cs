using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WikiSum.ViewModels
{
    public interface ISummaryViewModel
    {
        Task<List<string>?> GetWebsite();
        ICommand SearchCommand { get; set; }
        ICommand TranslateTextCommand { get; set; }
    }
}
