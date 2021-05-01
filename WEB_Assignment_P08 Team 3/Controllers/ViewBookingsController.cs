using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WEB_Assignment_P08_Team_3.DAL;
using WEB_Assignment_P08_Team_3.Models;
using System.Data.SqlTypes;
using System.Security.Policy;

namespace WEB_Assignment_P08_Team_3.Controllers
{
    public class ViewBookingsController : Controller
    {
        private BookingDAL bookingContext = new BookingDAL { };

        //Method to view past booking
        public ActionResult BookingHistory()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToAction("Login", "Home");
            }
            int customerID = (int)HttpContext.Session.GetInt32("ID");
            List<Booking> bookingList = bookingContext.GetBookingHistory(customerID);
            bookingList.Reverse();
            return View(bookingList);
        }

        public ActionResult SpecificBooking(string id)
        {
            int bookingID = Convert.ToInt32(id);
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToAction("Login", "Home");
            }

            Booking booking = bookingContext.GetSpecificBooking(bookingID);
            return View(booking);
        }
    }
}
