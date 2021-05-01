using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class DetailsViewModel
    {
        public FlightRoute FlightRoute { get; set; }

        public List<FlightSchedule> flightScheduleList { get; set; }

        public DetailsViewModel()
        {
            FlightRoute = new FlightRoute();

            flightScheduleList = new List<FlightSchedule>();
        }
    }
}
