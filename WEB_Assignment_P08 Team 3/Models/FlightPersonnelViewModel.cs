using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class FlightPersonnelViewModel
    {
        public ScheduleViewModel scheduleVM { get; set; }
        public List<Staff> staffList { get; set; }
        public FlightCrew crew { get; set; }
        public FlightPersonnelViewModel()
        {
            staffList = new List<Staff>();
            crew = new FlightCrew();
        }
    }
}
