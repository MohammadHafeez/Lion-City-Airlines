using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WEB_Assignment_P08_Team_3.DAL;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class ValidateEmailExists: ValidationAttribute
    {
        private StaffDAL staffContext = new StaffDAL();
        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            string email = Convert.ToString(value);
            Staff staff = (Staff)validationContext.ObjectInstance;
            int staffId = staff.StaffID;
            if (staffContext.IsEmailExist(email, staffId))
                return new ValidationResult
                ("Email address already exists!");
            else
                // validation passed
                return ValidationResult.Success;
        }
    }
}
