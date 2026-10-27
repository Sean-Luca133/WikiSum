using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services.Sumerization
{
    public interface ISummarizationService
    {
        Task<AbstractiveSummarizeOperation> SummarizeText(List<string> document);
    }
}
