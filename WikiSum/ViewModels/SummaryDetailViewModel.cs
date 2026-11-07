using Azure.AI.TextAnalytics;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Windows.Input;
using WikiSum.Messages;
using WikiSum.Services.EntityReconition;
using WikiSum.Services.Navigation;


namespace WikiSum.ViewModels
{
    public class SummaryDetailViewModel : ObservableRecipient, ISummaryDetailViewModel, IRecipient<ChosenSummaryMessage>
    {
        // services/objects
        private FormattedString analyzedParagraph;
        private IEntityRecognitionService _entityRecognitionService;
        private INavigationService _navigationService;
        private readonly IPopupService _popupService;

        // commands
        public ICommand NavigateBackCommand { get; set; }

        public FormattedString AnalyzedParagraph
        {
            get => analyzedParagraph;
            set => SetProperty(ref analyzedParagraph, value);
        }

        public SummaryDetailViewModel(IPopupService popupService, IEntityRecognitionService entityRecognitionService,INavigationService navigation)
        {
            _entityRecognitionService = entityRecognitionService;
            _navigationService = navigation;
            _popupService = popupService;
            Messenger.Register<SummaryDetailViewModel, ChosenSummaryMessage>(this, (r, m) => r.Receive(m));
            BindMethods();

        }

        private void BindMethods()
        {
            NavigateBackCommand = new AsyncRelayCommand(NavigateBack);
        }

        public async Task NavigateBack()
        {
           await _navigationService.NavigateBack();
        }

        public async Task DisplayPopup(string textValue,string category)
        {
            Debug.WriteLine($"[SummaryDetailViewModel]: Popup Text:{textValue}");
            Debug.WriteLine($"[SummaryDetailViewModel]: Popup Category:{category}");
            var queryAttributes = new Dictionary<string, object>
            {
                [nameof(EntityPopupViewModel.TextValue)] = textValue,
                [nameof(EntityPopupViewModel.Category)] = category
            };

            await _popupService.ShowPopupAsync<EntityPopupViewModel>(
                Shell.Current,
                options: PopupOptions.Empty,
                shellParameters: queryAttributes);
        }

        public void Receive(ChosenSummaryMessage message)
        {
            Debug.WriteLine($"[SummaryDetailViewModel]: Receveid message: {message.Value}");
            string paragraph = message.Value.ParagraphSummary;
            CategorizedEntityCollection entities = _entityRecognitionService.GetNamedEntities(paragraph);

            Debug.WriteLine($"[SummaryDetailViewModel]: Received entities: {entities.Count}");
            FormattedString formattedString = SetEntityMarks(paragraph, entities);
            AnalyzedParagraph = formattedString;
        }

        private FormattedString SetEntityMarks(string paragraph, CategorizedEntityCollection entities)
        {
            FormattedString formattedString = new FormattedString();
            int currentIndex = 0;
            foreach (CategorizedEntity entity in entities.OrderBy(e => e.Offset))
            {
                // Debug
                Debug.WriteLine($"[SummaryDetailViewModel]: entity offset: {entity.Offset}");
                Debug.WriteLine($"[SummaryDetailViewModel]: entity length: {entity.Length}");
                Debug.WriteLine($"[SummaryDetailViewModel]: entity text: {entity.Text}");
                Debug.WriteLine($"[SummaryDetailViewModel]: entity category: {entity.Category}");
                Debug.WriteLine($"[SummaryDetailViewModel]: Current index: {currentIndex}");

                // Get unmarked text
                if (entity.Offset > currentIndex)
                {
                    formattedString.Spans.Add(new Span
                    {
                        Text = paragraph.Substring(currentIndex, entity.Offset - currentIndex),
                    });
                }

                // per entity define span with click method for popup

                var recognizer = new TapGestureRecognizer();

                recognizer.Tapped += async (_, _) =>
                {
                    await DisplayPopup(entity.Text, (string)entity.Category);
                };
                var entitySpan = new Span
                {
                    Text = entity.Text,
                    TextColor = GetColor((string)entity.Category),

                };

                entitySpan.GestureRecognizers.Add(recognizer);

                formattedString.Spans.Add(entitySpan);


                currentIndex = entity.Offset + entity.Length;
            }

            // Set remaining text after last entity
            if (currentIndex < paragraph.Length)
            {
                formattedString.Spans.Add(new Span
                {
                    Text = paragraph.Substring(currentIndex)
                });
            }

            Debug.WriteLine($"[SummaryDetailViewModel]: EndString: {formattedString.GetValue}");
            return formattedString;
        }

        // Assign different colors to different entity group types
        private Color GetColor(string category)
        {
            return category switch
            {
                // People
                "PersonType" or "Person" => Colors.Green,

                // Concepts or things
                "Skill" or "Product" => Colors.Gold,

                // Locations
                "Addres" or "Location" or "AirPort" or "Area" or "City" or "Continent" or "CountryRegion" or "Geological" or "GPE" or "IpAddress" or "Location" => Colors.Blue,

                // Organizations
                "Organization" or "OrganizationMedical" or "OrganizationSports" or "OrganizationStockExchange" => Colors.Purple,

                // Events
                "Event" or "CulturalEvent" or "NaturalEvent" or "SportsEvent" => Colors.HotPink,

                // Numbers (except time)
                "Age" or "Currency" or "Quantity" or "Dimension" or "Height" or "Length" or "Number" or "NumberRange" or "Ordinal" or "Percentage" or "Speed" or "Temperature" or "Volume" or "Weight" => Colors.Red,

                // Time
                "Date" or "DateRange" or "DateTime" or "DateTimeRange" or "Duration" or "SetTemporal" or "Temporal" or "Time" or "TimeRange" => Colors.DarkOrange,
                _ => Colors.DarkGray
            };
        }
    }
}
