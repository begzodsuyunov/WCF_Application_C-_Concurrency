using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Flight
    {
        //Flight class with properties flight code, destination, departure time, status, and gate,
        //and a constructor to initialize these properties with values
        public string FlightCode { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Status { get; set; }
        public string Gate { get; set; }

        public Flight(string flightCode, string destination, DateTime departureTime)
        {
            FlightCode = flightCode;
            Destination = destination;
            DepartureTime = departureTime;
            Status = "On Time";
            Gate = "-";
        }
    }
}
