using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace TravelAdvisorBot.Models
{
    [Serializable]
    public class HotelsQuery
    {
        [Prompt("Please enter your {&}")]
        [Optional]
        public string Destination { get; set; }

        [Prompt("Near which Airport")]
        [Optional]
        public string AirportCode { get; set; }
    }
}