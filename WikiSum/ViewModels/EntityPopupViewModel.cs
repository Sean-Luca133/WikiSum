using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.ViewModels
{
    public partial class EntityPopupViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        public partial FormattedString TextValue { get; set; } = "";

        [ObservableProperty]
        public partial FormattedString Category { get; set; } = "";

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            FormattedString textValueFormatted = new FormattedString();
            textValueFormatted.Spans.Add(new Span()
            {
                Text = "Value: ",
                FontAttributes = FontAttributes.Bold
            });


            textValueFormatted.Spans.Add(new Span()
            {
                Text = (string)query[nameof(EntityPopupViewModel.TextValue)]
            });

            TextValue = textValueFormatted;

            FormattedString categoryFormatted = new FormattedString();
            categoryFormatted.Spans.Add(new Span()
            {
                Text = "Category: ",
                FontAttributes = FontAttributes.Bold
            });


            categoryFormatted.Spans.Add(new Span()
            {
                Text = (string)query[nameof(EntityPopupViewModel.Category)]
            });

            Category = categoryFormatted;
        }

        readonly IPopupService _popupService;

        public EntityPopupViewModel(IPopupService popupService)
        {
            _popupService = popupService;

        }


        [RelayCommand]
        async Task OnCancel()
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        
    }
}
