namespace TravelAdvisorBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Threading.Tasks;
    using Properties;
    using Extensions;

#pragma warning disable 1998

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            // TODO: Refactor to new context message and help message/menu.
            var reply = context.MakeMessage();

            var options = new[]
            {
                Resources.RootDialog_SendWelcomeMessage_SearchFlights,
                Resources.RootDialog_SendWelcomeMessage_SearchHotels
            };

            reply.AddHeroCard(
                Resources.RootDialog_SendWelcomeMessage_Title,
                Resources.RootDialog_SendWelcomeMessage_Subtitle,
                options,
                new[] { "https://placeholdit.imgix.net/~text?txtsize=28&txt=Travel%20Advisor%20Bot&w=640&h=330" });

            await context.PostAsync(reply);

            context.Wait(this.WelcomeMessageOptionSelected);
        }

        private async Task WelcomeMessageOptionSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text == Resources.RootDialog_SendWelcomeMessage_SearchFlights)
            {
                var searchFlightsDialog = new SearchFlightsDialog();

                context.Call(searchFlightsDialog, this.AfterSearchFlightsDialog);
            } else if(message.Text == Resources.RootDialog_SendWelcomeMessage_SearchHotels)
            {
                await context.PostAsync(string.Format(Resources.RootDialog_SendWelcomeMessage_NotYetImplemented, message.Text));
                await this.SendWelcomeMessageAsync(context);
            }
            else
            {
                await context.PostAsync(string.Format(Resources.RootDialog_SendWelcomeMessage_NotUnderstood, message.Text));
                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task AfterSearchFlightsDialog(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var response = await result;

                await this.SendWelcomeMessageAsync(context);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry you're having issues with finding flights, we're working on it.");
                // TODO: Is there anything else I can help you with?
                await this.SendWelcomeMessageAsync(context);
            }
        }
    }
}