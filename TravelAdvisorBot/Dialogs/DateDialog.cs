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
        private readonly string retry;
        private int attempts;
        private readonly string tooManyAttempts;

        public DateDialog(string prompt, string retry, int attempts = 3, string tooManyAttempts = null)
        {
            this.prompt = prompt;
            this.retry = retry;
            this.attempts = attempts;
            this.tooManyAttempts = tooManyAttempts;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(this.prompt);

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            DateTime date;
            var message = await result;

            if (this.TryParse(message, out date))
            {
                context.Done(date.ToShortDateString());
            }
            else
            {
                --attempts;
                if (attempts > 0)
                {
                    await context.PostAsync(string.Format(this.retry, message.Text));
                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    if(tooManyAttempts != null)
                    {
                        await context.PostAsync(tooManyAttempts);
                    }

                    context.Fail(new TooManyAttemptsException(tooManyAttempts));
                }
            }
        }

        private bool TryParse(IMessageActivity message, out DateTime date)
        {
            return DateTime.TryParse(message.Text, out date);
        }
    }
}