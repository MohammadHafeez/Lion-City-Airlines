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
    public class FlightBookingController : Controller
    {
        private FlightScheduleDAL flightScheduleContext = new FlightScheduleDAL { };
        private List<ScheduleViewModel> availableFlightList = new List<ScheduleViewModel> { };
        private List<int> availableId = new List<int> { };
        private List<SelectListItem> idList = new List<SelectListItem>();
        private List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
        private List<SelectListItem> numberList = new List<SelectListItem>();
        private List<string> seatclass = new List<string> { "Economy", "Business" };
        private List<SelectListItem> classList =  new List<SelectListItem>{};
        private BookingDAL bookingContext = new BookingDAL { };

        public FlightBookingController()
        {
            List<ScheduleViewModel> flightList = flightScheduleContext.GetAvailableFlights();
            foreach (ScheduleViewModel flight in flightList)
            {
                availableFlightList.Add(flight);
            }

            foreach (ScheduleViewModel flight in availableFlightList)
            {
                availableId.Add(flight.ScheduleId);
            }

            foreach (int i in availableId)
            {
                idList.Add(
                  new SelectListItem
                  {
                      Value = i.ToString(),
                      Text = i.ToString(),
                  });
            }
            foreach (int i in numbers)
            {
                numberList.Add(
                  new SelectListItem
                  {
                      Value = i.ToString(),
                      Text = i.ToString(),
                  });
            }
            foreach (string s in seatclass)
            {
                classList.Add(
                  new SelectListItem
                  {
                      Value = s.ToString(),
                      Text = s.ToString(),
                  });
            }
        }
        public ActionResult ChooseFlight()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToAction("Login", "Home");
            }
            ViewData["IdList"] = idList;
            ViewData["Numbers"] = numberList;
            return View(availableFlightList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseFlight(IFormCollection formData)
        {

            int scheduleID = Convert.ToInt32(formData["scheduleID"]);
            TempData["Schedule"] = scheduleID;
            return RedirectToAction("NoOfPassengers");
        }


        public ActionResult NoOfPassengers()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToAction("Login", "Home");
            }
            int scheduleID = (int)TempData["Schedule"];
            if (scheduleID == 0)
            {
                return RedirectToAction("ChooseFlight");
            }
            ViewData["Schedule"] = scheduleID;
            if (scheduleID == 0){
                return RedirectToAction("ChooseFlight");
            }
            ViewData["Numbers"] = numberList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NoOfPassengers(IFormCollection formData)
        {
            int scheduleID = Convert.ToInt32(formData["scheduleID"]);
            int number = Convert.ToInt32(formData["number"]);
            TempData["Number"] = number;
            TempData["Schedule"] = scheduleID;
            return RedirectToAction("MakeBooking");
        }
       
        public ActionResult MakeBooking()
        {

            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToAction("Login", "Home");
            }
            int number = (int)TempData["Number"];
            int scheduleID = (int)TempData["Schedule"];
            if (scheduleID == 0 || number == 0)
            {
                return RedirectToAction("ChooseFlight");
            }
            List<Booking> bList = new List<Booking> { };
            for (int i = 0; i < number; i++)
            {
                bList.Add(
                    new Booking
                    {
                        CustID = (int)HttpContext.Session.GetInt32("ID"),
                        ScheduleID = scheduleID,
                        PassengerName = null,
                        PassportNo = null,
                        Nationality = null,
                        SeatClass = "Economy",
                        AmtPayable = 0.00,
                        Remark = null,
                        DateTimeCreated = DateTime.Now,
                    });
            }

            BookingModel bookingList = new BookingModel();
            bookingList.Amount = number;
            bookingList.NewBookings = bList;
            ViewData["SeatClass"] = classList;
            return View(bookingList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeBooking(BookingModel bookingModel)
        {
            if (bookingModel == null || bookingModel.NewBookings.Count == 0)
            {
                return RedirectToAction("ChooseFlight");
            }
            foreach(Booking b in bookingModel.NewBookings)
            {
                if (ModelState.IsValid)
                {
                    b.AmtPayable = flightScheduleContext.GetSeatPrice(b.ScheduleID,b.SeatClass);
                }
                else
                {
                    ViewData["SeatClass"] = classList;
                    return View(bookingModel);
                }
            }

            //Update database for each individual booking
            foreach (Booking b in bookingModel.NewBookings)
            {
                bookingContext.Add(b);
            }

            TempData["Bookings"] = JsonConvert.SerializeObject(bookingModel);

            return RedirectToAction("ConfirmBooking");

        }

        public ActionResult ConfirmBooking()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToAction("Login", "Home");
            }
            BookingModel bookingModel = (JsonConvert.DeserializeObject<BookingModel>((string)TempData["Bookings"]));
            if (bookingModel == null)
            {
                return RedirectToAction("ChooseFlight");
            }
            ViewData["ScheduleID"] = bookingModel.NewBookings[0].ScheduleID;
            int count = 0; //No of Passengers
            double cost = 0; //Total cost of bookings
            int economy = 0;
            int business = 0;
            foreach(Booking b in bookingModel.NewBookings)
            {
                count += 1;
                cost += b.AmtPayable;
                if (b.SeatClass == "Economy")
                {
                    economy += 1;
                }
                else
                {
                    business += 1;
                }
            }
            ViewData["NoOfPassengers"] = count;
            ViewData["Price"] = cost.ToString("$#,####.00");
            ViewData["Economy"] = "Economy Class x" + economy.ToString();
            ViewData["Business"] = "Business Class x" + business.ToString();
            return View();

        }


    }
}
