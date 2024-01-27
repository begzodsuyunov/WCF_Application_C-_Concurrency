using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _00010358_Service
{
    /// Flight class in WCF service
    public class Flight
    {
        public string FlightCode { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
    }
}