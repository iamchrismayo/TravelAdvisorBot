using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Bot.Builder.FormFlow;

namespace TravelAdvisorBot.Models
{
    [Serializable]
    public class FlightsQuery
    {
        [Prompt("Where are you flying from?")]
        public string DepartureCity { get; set; }

        [Prompt("Where do you want to fly to?")]
        public string ReturnCity { get; set; }

        [Prompt("When do you want to leave?")]
        public string DepartureDate { get; set; }

        [Prompt("When would you like to return?")]
        public string ReturnDate { get; set; }
    }
}