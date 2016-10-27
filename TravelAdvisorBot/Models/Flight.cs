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
        public string DepartureAirline { get; set; }
        public string DepartureFlightNumber { get; set; }

        public string ReturnAirport { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public string ReturnAirline { get; set; }
        public string ReturnFlightNumber { get; set; }

        public int Price { get; set; }

        public string Image { get; set; }
    }
}