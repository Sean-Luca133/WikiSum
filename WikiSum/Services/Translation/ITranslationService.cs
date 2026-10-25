using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services.Translation
{
    public interface ITranslationService
    {
        public Task<string> TranslateText(string text, string fromLang, string toLang);
    }
}
