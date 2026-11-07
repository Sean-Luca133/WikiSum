using CommunityToolkit.Mvvm.Messaging.Messages;
using WikiSum.Models;
using WikiSum.Models.Api.Website;

namespace WikiSum.Messages
{
    public class ChosenWebsiteMessage(WebsiteRequest website) : ValueChangedMessage<WebsiteRequest>(website)
    {
    }
}
