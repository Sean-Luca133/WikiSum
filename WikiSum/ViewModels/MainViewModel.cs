
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using WikiSum.Services.Translation;

namespace WikiSum.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel 
    {
        public ICommand TranslateTextCommand { get; set; }
        private ITranslationService _translationService;
        public string TestText { get; set; } = "How are you?";
        public ICommand TranslateTestCommand { get; set; }

        public MainViewModel(ITranslationService translationService)
        {
            _translationService = translationService;
            TranslateTestCommand = new AsyncRelayCommand(TranslateTest);
        }


        private async Task<string> TranslateText(string text, string start, string to)
        {
            return await _translationService.TranslateText(text, start, to);
        }

        private async Task TranslateTest()
        {
            TestText = await TranslateText(TestText, "en", "nl");
            Console.WriteLine(TestText);
            Debug.WriteLine(TestText);
        }
    }
}
