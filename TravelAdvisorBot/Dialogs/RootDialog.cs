#pragma warning disable 1998

namespace TravelAdvisorBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Properties;
    using Extensions;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;

            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();

            var options = new[]
            {
                Resources.RootDialog_SendWelcomeMessage_SearchFlights,
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

            }
            else
            {

            }
        }
    }
}