using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace WEB_Assignment_P08_Team_3.Models
{
    public class CrewViewModel
    {
        public List<Staff> staffList { get; set; }
        public List<FlightCrew> flightCrewList { get; set; }
        public CrewViewModel()
        {
            staffList = new List<Staff>();
            flightCrewList = new List<FlightCrew>();
        }
    }
}
