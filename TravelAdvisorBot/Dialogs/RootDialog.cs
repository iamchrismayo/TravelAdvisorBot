namespace TravelAdvisorBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Threading.Tasks;
    using Models;

#pragma warning disable 1998

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private FlightsQuery flightsQuery;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            this.flightsQuery = new FlightsQuery();

            PromptDialog.Text(context, this.AfterPromptDepartureCity, "Where are you flying from?", "I'm sorry, I don't understand. Please try again.", 3);
        }

        private async Task AfterPromptDepartureCity(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.flightsQuery.DepartureCity = await result;

                PromptDialog.Text(context, this.AfterPromptReturnCity, "Where do you want to fly to?", "I'm sorry, I don't understand. Please try again.", 3);
            }
            catch (TooManyAttemptsException)
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task AfterPromptReturnCity(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.flightsQuery.ReturnCity = await result;

                await context.PostAsync("Done");
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Too Many Attempts");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}