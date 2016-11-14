using System;
using System.Globalization;
using TravelAdvisorBot.Models;

namespace TravelAdvisorBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Threading.Tasks;
    using Models;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

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
            var searchFlightsDialog = new SearchFlightsDialog();

            context.Call(searchFlightsDialog, this.AfterSearchFlightsDialog);
        }

        private async Task AfterSearchFlightsDialog(IDialogContext context, IAwaitable<string> result)
        {
            var response = await result;

            await this.StartAsync(context);
        }
    }
}