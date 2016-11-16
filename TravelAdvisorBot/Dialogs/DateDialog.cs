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
    public class DateDialog : RetryDialog<DateTime>
    {
        public DateDialog(string prompt, string retry, int attempts = 3, string tooManyAttempts = null)
            : base(prompt, retry, attempts, tooManyAttempts)
        {
        }

        protected override bool TryParse(IMessageActivity message, out DateTime date)
        {
            return DateTime.TryParse(message.Text, out date);
        }
    }
}