using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services.LanguageDetection
{
    public interface ILanguageDetectionService
    {
        public Task<DetectedLanguage?> DetectLanguageInput(string document);
    }
}
