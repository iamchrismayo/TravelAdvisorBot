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
        [Prompt("Please enter your {&}")]
        public string DepartureCity { get; set; }

        [Prompt("Please enter your {&}")]
        public string ArrivalCity { get; set; }

        [Prompt("Please enter your {&}")]
        public DateTime DepartureDate { get; set; }

        [Prompt("Please enter your {&}")]
        public DateTime ReturnDate { get; set; }
    }
}