using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_Assignment_P08_Team_3.DAL;
using WEB_Assignment_P08_Team_3.Models;

namespace WEB_Assignment_P08_Team_3.Controllers
{
    public class CustomerController : Controller
    {
        CustomerDAL customerContext = new CustomerDAL();

        // GET: CustomerController
        public ActionResult CustMain()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                TempData["UserError"] = "Sign in as a Customer to view this page.";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public ActionResult SignUp()
        {
            Customer customer = new Customer
            {

                Name = "",
                Nationality = null,
                BirthDate = null,
                TelNo = null,
                EmailAddr = null,
                Password = "p@55Cust"

            };
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(Customer customer)
        {
            List<Customer> existingCustomers = customerContext.GetAllCustomers();
            
             if (ModelState.IsValid)
             {
                foreach (Customer c in existingCustomers)
                {
                    if (customer.EmailAddr == c.EmailAddr)
                    {
                        TempData["Message"] = "This account already exists.";
                        return RedirectToAction("SignUp");
                    }

                }
                customerContext.Add(customer);
                return RedirectToAction("Login", "Home");
             }
             else
             {
                 return View(customer);
             }
            
        }

        public ActionResult ChangePassword()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string OldPassword,string NewPassword,string RepeatPassword)
        {
            int id = (int)HttpContext.Session.GetInt32("ID");
            Customer customer = customerContext.GetSpecificCustomer(id);
            if (customer.Password != OldPassword)
            {
                TempData["Message"] = "You have entered the wrong password.";
                return View();
            }
            else if (NewPassword != RepeatPassword)
            {
                TempData["Message"] = "Your new password and re-entered password do not match!";
                return View();
            }
            else if (NewPassword == null || NewPassword == "" || OldPassword == NewPassword)
            {
                TempData["Message"] = "Please enter a new password.";
                return View();
            }
            else
            {
                customer.Password = NewPassword;
                if (TryValidateModel(customer,customer.Password) == true)
                {
                    customerContext.UpdatePassword(NewPassword, id);
                    TempData["Password_Success"] = "You have successfully changed your password.";
                    return View();
                }
                else
                {
                    TempData["Message"] = "Password should contain be at least 8 characters long and contain one uppercase character, one lowercase character and one number.";
                    return View();
                }
            }

        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}
