using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WEB_Assignment_P08_Team_3.Models;
using WEB_Assignment_P08_Team_3.DAL;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WEB_Assignment_P08_Team_3.Controllers
{
    public class StaffController : Controller
    {
        private StaffDAL staffContext = new StaffDAL();
        private CrewDAL crewContext = new CrewDAL();
        private FlightScheduleDAL scheduleContext = new FlightScheduleDAL();
        // GET: Staff
        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<Staff> staffList = staffContext.GetAllStaff();
            return View(staffList);
        }
        public ActionResult GetCrewSchedule(int StaffID)
        {
            CrewViewModel crewVM = new CrewViewModel();
            crewVM.flightCrewList = crewContext.GetFlightCrew(StaffID);
            return View(crewVM);
        }

        // GET: Staff/Details/5
        public ActionResult ViewStaffSchedule(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<FlightSchedule> ScheduleList = scheduleContext.GetAllFlightSchedule();
            List<int> CrewList = crewContext.GetFlightCrewID(id.Value);
            List <FlightSchedule> EditedScheduleList = new List<FlightSchedule>();
            Staff s = staffContext.GetDetails(id.Value);
            TempData["StaffName"] = s.StaffName;
            foreach (int i in CrewList)
            {
                foreach (FlightSchedule j in ScheduleList)
                {
                    if (i == j.ScheduleId)
                    {
                        EditedScheduleList.Add(j);
                    }
                }
            }
            return View(EditedScheduleList);
        }

        // GET: Staff/Create
        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                staff.StaffID = staffContext.Add(staff);
                return RedirectToAction("Index");
            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(staff);
            }
        }

        // GET: Staff/Edit/5
        public ActionResult Edit(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            Staff staff = staffContext.GetDetails(id.Value);
            if (staff == null)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            List<FlightSchedule> ScheduleList = scheduleContext.GetAllFlightSchedule();
            List<int> CrewList = crewContext.GetFlightCrewID(id.Value);
            List<DateTime?> ScheduleTimeList = new List<DateTime?>();
            foreach(int i in CrewList)
            {
                foreach (FlightSchedule j in ScheduleList)
                {
                    if (i == j.ScheduleId)
                    {
                        ScheduleTimeList.Add(j.DepartureDateTime);
                    }
                }
            }
            foreach(DateTime j in ScheduleTimeList)
            {
                if(j > DateTime.Now)
                {
                    TempData["ScheduleFound"] = "Status cannot be changed if staff has upcoming scheduled flight.";
                    return RedirectToAction("Index");
                }
            }
            return View(staff);
        }

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Staff staff)
        {
            if (ModelState.IsValid)
            {
                //Update staff record to database
                staffContext.Update(staff);
                TempData["SuccessMessage"] = "Status for " +staff.StaffName+ " successfully updated.";
                return RedirectToAction("Index");
            }
            else
            {
                //Input validation fails, return to the view
                //to display error message
                TempData["ErrorMessage"] = "Error, please try again.";
                return View(staff);
            }
        }
        public ActionResult DisplaySchedule()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            List<FlightSchedule> flightScheduleList = scheduleContext.GetAllFlightSchedule();
            return View(flightScheduleList);
        }
        public ActionResult Assign(int? id)
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            FlightPersonnelViewModel personnelVM = new FlightPersonnelViewModel();
            if (id != null)
            {
                personnelVM.scheduleVM = scheduleContext.GetScheduleDetails(id.Value);
                personnelVM.staffList = staffContext.GetAssign(id.Value);
                ViewData["availStaffList"] = StaffDDList(personnelVM.staffList);
                ViewData["roleList"] = RoleDDList(new List<SelectListItem>());
            } 
            return View(personnelVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(FlightPersonnelViewModel personnelVM)
        {
            ViewData["availStaffList"] = StaffDDList(personnelVM.staffList);
            ViewData["roleList"] = RoleDDList(new List<SelectListItem>());
            if(ModelState.IsValid)
            {
                string newRole = personnelVM.crew.Role;
                int newStaffID = personnelVM.crew.StaffID;
                int newScheduleID = personnelVM.scheduleVM.ScheduleId;
                if(newStaffID != 0)
                {
                    if(CheckVocation(newRole,newStaffID))
                    {
                        if(newRole != "Flight Attendant")
                        {
                            if(crewContext.CheckCrew(newScheduleID,newRole))
                            {
                                int oldStaffID = crewContext.GetOldID(newScheduleID,newRole);
                                crewContext.UpdateCrew(newRole, newStaffID, newScheduleID, oldStaffID);
                                TempData["SuccessMessage"] = "Crew successfully assigned!";
                            }
                            else
                            {
                                crewContext.AddCrew(newScheduleID, newStaffID, newRole);
                                TempData["SuccessMessage"] = "Crew successfully assigned!";
                            }
                        }
                        else
                        {
                            if(crewContext.GetAttendants(newScheduleID).Count<3)
                            {
                                crewContext.AddCrew(newScheduleID, newStaffID, newRole);
                                TempData["SuccessMessage"] = "Crew successfully updated!";
                            }
                            else
                            {
                                for(int i = 0; i < crewContext.GetAttendants(newScheduleID).Count; i++)
                                {
                                    //Stores in session state assigned FAs for selected FS
                                    HttpContext.Session.SetInt32("FA"+i,crewContext.GetAttendants(newScheduleID)[i].StaffID);
                                }
                                HttpContext.Session.SetInt32("FAScheduleID", newScheduleID);
                                HttpContext.Session.SetInt32("FANewStaffID", newStaffID);
                                return RedirectToAction("UpdateFA");
                            }
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Selected staff does not match selected role. Please Try Again.";
                        return RedirectToAction("Assign");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please select a staff member.";
                    return RedirectToAction("Assign");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Error please try again.";
                return RedirectToAction("DisplaySchedule");
            }
            return RedirectToAction("DisplaySchedule");
        }
        public ActionResult UpdateFA()
        {
            if (HttpContext.Session.GetString("Role") == null || HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            List<Staff> faList = new List<Staff>();
            if(HttpContext.Session.GetInt32("FAScheduleID")!=null)
            {
                for(int i = 0;i<3;i++)
                {
                    System.Diagnostics.Debug.WriteLine("hi"+HttpContext.Session.GetInt32("FA1"));
                    faList.Add(staffContext.GetDetails(HttpContext.Session.GetInt32("FA" + i).Value));
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return View(faList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateFA(int? id)
        {
            if(id != null)
            {
                int newStaffID = HttpContext.Session.GetInt32("FANewStaffID").Value;
                int newScheduleID = HttpContext.Session.GetInt32("FAScheduleID").Value;
                string newRole = "Flight Attendant";
                int oldStaffID = id.Value;
                crewContext.UpdateCrew(newRole, newStaffID, newScheduleID, oldStaffID);
                TempData["SuccessMessage"] = "Crew successfully updated!";
                return RedirectToAction("DisplaySchedule");
            }
            else
            {
                TempData["ErrorMessage"] = "Error Occured";
                return RedirectToAction("DisplaySchedule");
            }
        }

        public bool CheckVocation(string newRole, int staffID)
        {
            bool a = false;
            Staff s = staffContext.GetDetails(staffID);
            if(newRole == "Flight Captain" || newRole == "Second Pilot")
            {
                if (s.Vocation == "Pilot")
                {
                    a = true;
                }
                else
                {
                    a = false;
                }
            }
            if (newRole == "Cabin Crew Leader" || newRole == "Flight Attendant")
            {
                if (s.Vocation == "Flight Attendant")
                {
                    a = true;
                }
                else
                {
                    a = false;
                }
            }
            return a;
        }
        public List<SelectListItem> StaffDDList(List<Staff> sList)
        {
            List<SelectListItem> sDDList = new List<SelectListItem>();
            sDDList.Add(
                new SelectListItem
                {
                    Value = "0",
                    Text = "-=Select=-"
                });
            foreach (Staff s in sList)
            {
                sDDList.Add(
                   new SelectListItem
                   {
                       Value = s.StaffID.ToString(),
                       Text = s.StaffName +" - "+ s.Vocation
                   });
            }
            return sDDList;
        }
        public List<SelectListItem> RoleDDList(List<SelectListItem> rList)
        {
            rList.Add(
                new SelectListItem
                {
                    Value = "Flight Captain",
                    Text = "Flight Captain"
                });
            rList.Add(
                new SelectListItem
                {
                    Value = "Second Pilot",
                    Text = "Second Pilot"
                });
            rList.Add(
                new SelectListItem
                {
                    Value = "Cabin Crew Leader",
                    Text = "Cabin Crew Leader"
                });
            rList.Add(
                new SelectListItem
                {
                    Value = "Flight Attendant",
                    Text = "Flight Attendant"
                });
            return rList;
        }
    }
}