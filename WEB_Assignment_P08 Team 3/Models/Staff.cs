using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class Staff
    {
        [Display(Name = "Staff ID")]
        public int StaffID { get; set; }

        [Display(Name = "Staff Name")]
        [StringLength(50, ErrorMessage = "Name should be within 50 characters")]
        [Required]
        public string StaffName { get; set; }
        [Required]
        public char? Gender { get; set; }

        [Display(Name = "Date Employed")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? DateEmployed { get; set; }

        [StringLength(50, ErrorMessage = "Vocation should be within 50 characters")]
        public string Vocation { get; set; }

        [Display(Name = "Email Address")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "Please enter a valid email address.")]
        [Required]
        [ValidateEmailExists]
        public string EmailAddr { get; set; }

        [Display(Name = "Password")]
        [StringLength(255, ErrorMessage = "Password should be within 255 characters.")]
        public string Password { get; set; }

        [StringLength(50, ErrorMessage = "Status should be within 50 characters")]
        public string Status { get; set; }
    }
}
