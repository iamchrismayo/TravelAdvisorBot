using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace TravelAdvisorBot.Dialogs
{
    [Serializable]
    [LuisModel("2ff82461-9ffe-4e55-8d51-f2fd808b0691", "f0392b343da4490f9bc91197d6a5eda2")]
    public class RootDialog : LuisDialog<object>
    {
        private const string TravelAdviceOption = "Travel Advice";
        private const string SearchFlightsOption = "Search Flights";
        private const string SearchHotelsOption = "Search Hotels";
            
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! Try asking me things like 'search hotels in Seattle', 'search hotels near LAX airport' or 'show me the reviews of The Bot Resort'");

            context.Wait(this.MessageReceived);
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