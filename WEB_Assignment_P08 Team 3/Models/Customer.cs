using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Principal;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json.Serialization;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class Customer
    {
        [Display(Name = "CustomerID")]
        public int CustomerID { get; set; }

        [Display(Name = "Name")]
        [Required]
        [StringLength(50, ErrorMessage = "Please enter a name within 50 characters.")]
        public string Name { get; set; }

        [Display(Name = "Nationality")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter a valid nationality.")]
        public string Nationality { get; set; }

        [Display(Name = "BirthDate")]
        [DataType(DataType.Date)]
        [DateOfBirth(MinAge = 5,MaxAge =150, ErrorMessage = "Lion City Airline members must be at least 5 years old.")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Telephone No")]
        [RegularExpression("(^[+]([0-9]+)([,0-9 ]*)([0-9 ])*$)|(^ *$)", ErrorMessage = "Please enter your phone number with a + and your country code at the start.")]
        public string TelNo { get; set; }

        [Display(Name = "Email Address")]
        [Required]
        [RegularExpression("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddr { get; set; }
        
        [Display(Name = "Password")]
        [Required]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,32}$", ErrorMessage = "Password should contain be at least 8 characters long and contain one uppercase character, one lowercase character and one number")]
        [StringLength(255, ErrorMessage = "Please enter a password within 255 characters.")]
        public string Password { get; set; }
    }

    public class DateOfBirthAttribute : ValidationAttribute
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            var val = (DateTime)value;

            if (val.AddYears(MinAge) > DateTime.Now)
                return false;

            return (val.AddYears(MaxAge) > DateTime.Now);
        }

    }
}
