using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace TravelAdvisorBot.Dialogs
{
    [Serializable]
    public sealed class PromptDate : Prompt<DateTime, DateTime>
    {
        public PromptDate(string prompt, string retry, int attempts, string tooManyAttempts) 
            : base(new PromptOptions<DateTime>(prompt, retry, attempts: attempts, tooManyAttempts: tooManyAttempts))
        {
        }

        protected override bool TryParse(IMessageActivity message, out DateTime result)
        {
            // TODO: Add parsing of other date strings, "tomorrow", "yesterday", "this Saturday", "next week".
            return DateTime.TryParse(message.Text, out result);
        }

        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            DateTime parseResult;

            if (this.TryParse(message, out parseResult))
            {
                context.Done(parseResult);
            }
            else
            {
                this.promptOptions.Attempts -= 1;
                if (this.promptOptions.Attempts > 0)
                {
                    await context.PostAsync(string.Format(this.promptOptions.Retry, message.Text));
                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    if (this.promptOptions.TooManyAttempts != null)
                    {
                        await context.PostAsync(this.promptOptions.TooManyAttempts);
                    }

                    context.Fail(new TooManyAttemptsException(this.promptOptions.TooManyAttempts));
                }
            }
        }
    }
}