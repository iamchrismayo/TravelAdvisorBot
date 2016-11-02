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
        //[Prompt("Where are you flying from?")]
        public string DepartureCity { get; set; }

        //[Prompt("Where are you flying to?")]
        public string ReturnCity { get; set; }

        //[Prompt("What date would you like to leave?")]
        public string DepartureDate { get; set; }

        //[Prompt("What date would you like to return?")]
        public string ReturnDate { get; set; }
    }
}