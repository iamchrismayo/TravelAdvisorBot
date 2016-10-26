using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace TravelAdvisorBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string TravelAdviceOption = "Travel Advice";
        private const string SearchFlightsOption = "Search Flights";
        private const string SearchHotelsOption = "Search Hotels";
            
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = (Activity)await result;

            // Handle conversation state changes, like members being added and removed
            // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
            // Not available in all channels
            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null && activity.MembersAdded.Any())
                {

                }

                if (activity.MembersRemoved != null && activity.MembersRemoved.Any())
                {

                }
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened

            }

            if (activity.Type == ActivityTypes.Message)
            {
                var message = await result;

                var attachment = CreateRootDialogHeroCard();

                var reply = context.MakeMessage();

                reply.Attachments.Add(attachment);

                await context.PostAsync(reply);

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private static Attachment CreateRootDialogHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Welcome to Travel Advisor",
                Text = "Hi, I'm the Travel Advisor Bot and I'm here to find the best hotels and flights for your upcoming travel. Here are some things I can help with:",
                Images = new List<CardImage> { new CardImage("https://placeholdit.imgix.net/~text?txtsize=20&txt=Travel%20Advisor%20Bot&w=200&h=100") },
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.ImBack, TravelAdviceOption, value: TravelAdviceOption),
                    new CardAction(ActionTypes.ImBack, SearchHotelsOption, value: SearchHotelsOption),
                    new CardAction(ActionTypes.ImBack, SearchFlightsOption, value: SearchFlightsOption)}
            };

            return heroCard.ToAttachment();
        }
    }
}