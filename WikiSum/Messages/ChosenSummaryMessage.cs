using CommunityToolkit.Mvvm.Messaging.Messages;
using WikiSum.Models;

namespace WikiSum.Messages
{
    public class ChosenSummaryMessage(WebsiteParagraph paragraph) : ValueChangedMessage<WebsiteParagraph>(paragraph)
    {
     
    }
}
