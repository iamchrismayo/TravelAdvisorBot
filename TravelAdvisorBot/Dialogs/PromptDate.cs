using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace TravelAdvisorBot.Dialogs
{
    public sealed class PromptDate : Prompt<DateTime, DateTime>
    {
        public PromptDate(string prompt, string retry, int attempts, string tooManyAttempts) 
            : base(new PromptOptions<DateTime>(prompt, retry, attempts: attempts, tooManyAttempts: tooManyAttempts))
        {
        }

        protected override bool TryParse(IMessageActivity message, out DateTime result)
        {
            return DateTime.TryParse(message.Text, out result);
        }
    }
}