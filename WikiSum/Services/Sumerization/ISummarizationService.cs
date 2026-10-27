using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services.Sumerization
{
    public interface ISummarizationService
    {
        public Task<ExtractiveSummarizeOperation> SummarizeText(List<string> document);
    }
}
