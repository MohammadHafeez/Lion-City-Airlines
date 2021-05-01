using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_Assignment_P08_Team_3.DAL;
using WEB_Assignment_P08_Team_3.Models;

namespace WEB_Assignment_P08_Team_3.Controllers
{
    public class FlightScheduleController : Controller
    {
        FlightScheduleDAL flightScheduleContext = new FlightScheduleDAL();
        FlightRouteDAL flightRouteContext = new FlightRouteDAL();
        BookingDAL bookingContext = new BookingDAL();
        AircraftDAL aircraftContext = new AircraftDAL();
        private List<string> status = new List<string> { "Opened", "Full", "Delayed", "Cancelled" };

        // GET: FlightScheduleController/Create
        public ActionResult Create(int? id)
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                TempData["UserError"] = "Sign in as an admin to view this page";
                return RedirectToAction("Login", "Home");
            }

            List<SelectListItem> routeIdList = new List<SelectListItem>();
            List<SelectListItem> aircraftIdList = new List<SelectListItem>();

            aircraftIdList.Add(new SelectListItem { Value = 0.ToString(), Text = "0 (to remain unassigned)" });
            foreach (FlightRoute fr in flightRouteContext.GetAllFlightRoute())
            {
                routeIdList.Add(new SelectListItem { Value = fr.RouteId.ToString(), Text = fr.RouteId.ToString() });
            }
            foreach (Aircraft a in aircraftContext.GetAllAircraft())
            {
                aircraftIdList.Add(new SelectListItem { Value = a.AircraftID.ToString(), Text = a.AircraftID.ToString() });
            }

            FlightSchedule fs = new FlightSchedule { RouteId = Convert.ToInt32(id), AircraftId = 0 };
            ViewData["routeIds"] = routeIdList;
            ViewData["aircraftIds"] = aircraftIdList;
            return View(fs);
        }

        // POST: FlightScheduleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FlightSchedule flightSchedule)
        {
            List<FlightRoute> flightRouteList = flightRouteContext.GetAllFlightRoute();
            List<int> routeIdList = new List<int>();

            if (Convert.ToDateTime(flightSchedule.DepartureDateTime).Day <= DateTime.Now.Day || Convert.ToDateTime(flightSchedule.DepartureDateTime).Month < DateTime.Now.Month)
            {
                TempData["Later"] = "Departure datetime must be at least the next day";
                return View();
            }

            if (flightSchedule.AircraftId == 0)
            {
                flightSchedule.AircraftId = null;
            }

            foreach (FlightRoute fr in flightRouteList)
            {
                routeIdList.Add(fr.RouteId);
                if (flightSchedule.RouteId == fr.RouteId)
                {
                    flightSchedule.ArrivalDateTime = Convert.ToDateTime(flightSchedule.DepartureDateTime).AddHours(Convert.ToDouble(fr.FlightDuration));
                }
            }

            flightSchedule.ScheduleId = flightRouteList.Count + 1;
            if (ModelState.IsValid)
            {
                if (flightSchedule.DepartureDateTime == null)
                {
                    flightSchedule.ArrivalDateTime = null;
                }
                flightScheduleContext.Add(flightSchedule);
                TempData["schedule_success"] = "Successfully created new schedule";
            }
            else
            {
                TempData["schedule_failure"] = "New schedule not created";
            }

            return RedirectToAction("Index", "Admin");
        }

        // GET: FlightScheduleController/Edit/5
        public ActionResult Edit()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                TempData["UserError"] = "Sign in as an admin to view this page";
                return RedirectToAction("Login", "Home");
            }

            ViewData["Status"] = status;

            List<SelectListItem> scheduleIdList = new List<SelectListItem>();
            foreach (FlightSchedule fs in flightScheduleContext.GetAllFlightSchedule())
            {
                scheduleIdList.Add(new SelectListItem { Value = fs.ScheduleId.ToString(), Text = fs.ScheduleId.ToString() });

            }

            List<FlightSchedule> flightScheduleList = flightScheduleContext.GetAllFlightSchedule();
            foreach (Booking b in bookingContext.GetAllBooking())
            {
                foreach (FlightSchedule fs in flightScheduleList)
                {
                    if (fs.ScheduleId == b.ScheduleID)
                    {
                        fs.BookingAmt += 1;
                    }
                }
            }

            ViewData["scheduleIds"] = scheduleIdList;
            return View(flightScheduleList);
        }

        // POST: FlightScheduleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IFormCollection formData)
        {
            int scheduleId = Convert.ToInt32(formData["ScheduleId"]);
            string status = formData["Status"];

            foreach (FlightSchedule flightSchedule in flightScheduleContext.GetAllFlightSchedule())
            {
                if (flightSchedule.ScheduleId == scheduleId)
                {
                    flightSchedule.Status = status;
                    flightScheduleContext.Update(flightSchedule);
                    TempData["StatusEditSuccuess"] = "Status Was Succuessfully Updated";
                    break;
                }
                else
                {
                    TempData["StatusEditFail"] = "Please choose a valid schedule ID";
                }
            }

            return Edit();
        }
    }
}
