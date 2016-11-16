using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace TravelAdvisorBot.Dialogs
{
    [Serializable]
    public class DateDialog : IDialog<string>
    {
        private readonly string prompt;

        private DateTime date;

        public DateDialog(string prompt)
        {
            this.prompt = prompt;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(this.prompt);

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (DateTime.TryParse(message.Text, out date))
            {
                context.Done(this.date.ToShortDateString());
            }
            else
            {
                await context.PostAsync($"Sorry, I don't understand '{message.Text}'. Try entering a date like '{DateTime.Now.ToShortDateString()}'.");

                await this.StartAsync(context);
            }
        }
    }
}