using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_Assignment_P08_Team_3.Models;
using WEB_Assignment_P08_Team_3.DAL;
using Microsoft.AspNetCore.Routing;

namespace WEB_Assignment_P08_Team_3.Controllers
{
    public class FlightRouteController : Controller
    {
        FlightRouteDAL flightRouteContext = new FlightRouteDAL();
        FlightScheduleDAL flightScheduleContext = new FlightScheduleDAL();

        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                TempData["UserError"] = "Sign in as an admin to view this page";
                return RedirectToAction("Login", "Home");
            }

            return View(flightRouteContext.GetAllFlightRoute());
        }

        // GET: FlightRouteController/Create
        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                TempData["UserError"] = "Sign in as an admin to view this page";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: FlightRouteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FlightRoute flightRoute)
        {
            if (flightRoute.FlightDuration > 18)
            {
                TempData["TooLong"] = "Flight Duration cannot exceed 18 hours";
                return View();
            }

            if (flightRoute.DepartureCountry.All(char.IsDigit) || flightRoute.DepartureCity.All(char.IsDigit) || flightRoute.ArrivalCountry.All(char.IsDigit) || flightRoute.ArrivalCity.All(char.IsDigit))
            {
                TempData["Invalid"] = "Arrival/Departure Country/City is invalid";
                return View();
            }

            foreach (FlightRoute fr in flightRouteContext.GetAllFlightRoute())
            {
                if (fr.ArrivalCity == flightRoute.ArrivalCity && fr.ArrivalCountry == flightRoute.ArrivalCountry && fr.DepartureCity == flightRoute.DepartureCity && fr.DepartureCountry == flightRoute.DepartureCountry)
                {
                    TempData["Exists"] = "Flight route already exists";
                    return View();
                }
            }

            flightRoute.RouteId = flightRouteContext.GetAllFlightRoute().Count + 1;

            if (ModelState.IsValid)
            {
                flightRouteContext.Add(flightRoute);
                TempData["route_success"] = "Successfully created new route";
            }
            else
            {
                TempData["route_failure"] = "New route not created";
            }

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Details(int? id)
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                TempData["UserError"] = "Sign in as an admin to view this page";
                return RedirectToAction("Login", "Home");
            }

            DetailsViewModel detailsViewModel = new DetailsViewModel();
            foreach (FlightRoute fr in flightRouteContext.GetAllFlightRoute())
            {
                if (fr.RouteId == id)
                {
                    detailsViewModel.FlightRoute = fr;
                }
            }

            foreach (FlightSchedule fs in flightScheduleContext.GetAllFlightSchedule())
            {
                if (fs.RouteId == detailsViewModel.FlightRoute.RouteId)
                {
                    detailsViewModel.flightScheduleList.Add(fs);
                }
            }

            return View(detailsViewModel);
        }
    }
}
