using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Principal;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        [Display(Name = "Customer ID")]
        [Required]
        public int CustID { get; set; }
        
        [Display(Name = "Schedule ID")]
        [Required]
        public int ScheduleID { get; set; }

        [Display(Name = "Passenger Name")]
        [Required(ErrorMessage = "Please enter passenger's name.")]
        public string PassengerName { get; set; }

        [Display(Name = "Passport Number")]
        [Required(ErrorMessage = "Please enter passenger's passport number.")]
        public string PassportNo { get; set; }

        [Display(Name = "Nationality")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter a valid nationality.")]
        [Required(ErrorMessage = "Please enter passenger's nationality.")]
        public string Nationality { get; set; }

        [Display(Name = "Seat Class")]
        [Required]
        public string SeatClass { get; set; }

        [Display(Name = "Amount Payable")]
        public double AmtPayable { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(3000, ErrorMessage = "Please enter a description of less than 3000 characters.")]
        public string Remark{ get; set; }

        [Display(Name = "Date Booked")]
        public DateTime DateTimeCreated { get; set; }
    }

}
