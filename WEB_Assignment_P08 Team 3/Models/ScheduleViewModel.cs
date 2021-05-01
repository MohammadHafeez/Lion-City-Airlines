using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class ScheduleViewModel
    {
        [Display(Name = "Schedule Id")]
        public int ScheduleId { get; set; }

        [Display(Name = "Flight Number")]
        [StringLength(20, ErrorMessage = "Flight number cannot exceed 20 chracters")]
        public string FlightNumber { get; set; }

        [Display(Name = "Route Id")]

        public int RouteId { get; set; }

        [Display(Name = "Departure City")]
        [StringLength(50, ErrorMessage = "City name cannot exceed 50 chracters")]
        public string DepartureCity { get; set; }

        [Display(Name = "Arrival City")]
        [StringLength(50, ErrorMessage = "City name cannot exceed 50 chracters")]
        public string ArrivalCity { get; set; }

        [Display(Name = "Aircraft Id")]
        public int? AircraftId { get; set; }

        [Display(Name = "Depature Timing")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? DepartureDateTime { get; set; }

        [Display(Name = "Arrival Timing")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? ArrivalDateTime { get; set; }

        [Display(Name = "Economy Class Price")]
        public double EconomyClassPrice { get; set; }

        [Display(Name = "Business Class Price")]
        public double BusinessClassPrice { get; set; }

        [Display(Name = "Schedule Status")]
        public string Status { get; set; }

        public int BookingAmt { get; set; }
    }
}
