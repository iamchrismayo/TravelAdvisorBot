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
    public abstract class RetryDialog<T> : IDialog<T>
    {
        protected readonly string prompt;
        protected readonly string retry;
        protected int attempts;
        protected readonly string tooManyAttempts;

        public RetryDialog(string prompt, string retry, int attempts = 3, string tooManyAttempts = null)
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

        protected virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            T parseResult;
            
            if (this.TryParse(message, out parseResult))
            {
                context.Done(parseResult);
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
                    if (tooManyAttempts != null)
                    {
                        await context.PostAsync(tooManyAttempts);
                    }

                    context.Fail(new TooManyAttemptsException(tooManyAttempts));
                }
            }
        }

        protected abstract bool TryParse(IMessageActivity message, out T result);
    }
}