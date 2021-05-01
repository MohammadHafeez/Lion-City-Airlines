using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WEB_Assignment_P08_Team_3.Models;
using WEB_Assignment_P08_Team_3.DAL;
using System.Security.Cryptography.X509Certificates;

namespace WEB_Assignment_P08_Team_3.Controllers
{
    public class AdminController : Controller
    {
        private FlightScheduleDAL flightScheduleContext = new FlightScheduleDAL();
        private FlightRouteDAL flightRouteContext = new FlightRouteDAL();

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                TempData["UserError"] = "Sign in as an admin to view this page.";
                return RedirectToAction("Login", "Home");
            }

            HttpContext.Session.SetString("Role", "Admin");
            return View();
        }
        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}
