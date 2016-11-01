using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Bot.Builder.FormFlow;

namespace TravelAdvisorBot.Models
{
    [Serializable]
    // TemplateUsage.
        //DateTime
        //Not understood.
    public class FlightsQuery
    {
        [Prompt("Where are you flying from?")]
        public string DepartureCity { get; set; }

        [Prompt("Where are you flying to?")]
        public string ReturnCity { get; set; }

        // Can supply multiple to chose one at randome.
        [Prompt("What date would you like to leave?")]
        // Used to replace field in Pattern Language.
        //[Describe("departure date")]
        //[Pattern("regex goes here...")]
        //Numeric
        //Optional
        //Pattern
        //Prompt
        //Template
        //Terms
        public string DepartureDate { get; set; }

        [Prompt("What date would you like to return?")]
        public string ReturnDate { get; set; }
    }
}