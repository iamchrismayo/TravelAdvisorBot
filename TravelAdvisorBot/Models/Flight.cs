using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAdvisorBot.Models
{
    public class Flight
    {
        public string DepartureAirport { get; set; }
        public DateTime DepartureDateTime { get; set; }

        public string ReturnAirport { get; set; }
        public DateTime ReturnDateTime { get; set; }

        public int Price { get; set; }

        public string Image { get; set; }
    }
}