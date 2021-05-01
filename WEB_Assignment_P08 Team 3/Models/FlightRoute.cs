using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class FlightRoute
    {
        [Display(Name = "Route Id")]
        public int RouteId { get; set; }

        [Display(Name = "Departure City")]
        [StringLength(50, ErrorMessage = "city name cannot exceed 50 chracters")]
        public string DepartureCity { get; set; }

        [Display(Name = "Departure Country")]
        [StringLength(50, ErrorMessage = "country name cannot exceed 50 chracters")]
        public string DepartureCountry { get; set; }

        [Display(Name = "Arrival City")]
        [StringLength(50, ErrorMessage = "city name cannot exceed 50 chracters")]
        public string ArrivalCity { get; set; }

        [Display(Name = "Arrival Country")]
        [StringLength(50, ErrorMessage = "country name cannot exceed 50 chracters")]
        public string ArrivalCountry { get; set; }

        [Display(Name = "Flight Duration")]
        public int? FlightDuration { get; set; }
    }
}
