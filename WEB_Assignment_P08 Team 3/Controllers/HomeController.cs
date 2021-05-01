using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WEB_Assignment_P08_Team_3.Models;
using WEB_Assignment_P08_Team_3.DAL;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.Http;
using Newtonsoft.Json;

namespace WEB_Assignment_P08_Team_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("Role", "Guest");
            return View();
        }
        
        public IActionResult Login()
        {
            HttpContext.Session.SetString("Role", "Guest");
            return View();
        }

        private CustomerDAL customerDetails = new CustomerDAL();
        private StaffDAL staffDetails = new StaffDAL();

        [HttpPost]
        public ActionResult Login(IFormCollection formData)
        {
            List<Customer> customerList = customerDetails.GetAllCustomers();
            List<Staff> staffList = staffDetails.GetAllStaff();

            string loginId = formData["txtLoginID"].ToString().ToLower();
            string password = formData["txtPassword"].ToString();

            foreach (Customer c in customerList)
            {
                if (c.EmailAddr.ToLower() == loginId && c.Password == password)
                {
                    HttpContext.Session.SetString("Role", "Customer");
                    HttpContext.Session.SetString("Name", c.Name);
                    HttpContext.Session.SetInt32("ID", c.CustomerID);
                    return RedirectToAction("CustMain", "Customer");
                }
            }

            foreach (Staff s in staffList)
            {
                if (s.EmailAddr == loginId && s.Password == password && s.Vocation == "Administrator")
                {
                    HttpContext.Session.SetString("Role", "Admin");
                    TempData["adminName"] = s.StaffName;
                    return RedirectToAction("Index", "Admin");
                }
                else if(s.EmailAddr == loginId && s.Password == password && s.Vocation == "Pilot")
                {
                    HttpContext.Session.SetString("Role", "Pilot");
                    TempData["adminName"] = s.StaffName;
                    return RedirectToAction("Index", "Admin");
                }
                else if (s.EmailAddr == loginId && s.Password == password && s.Vocation == "Flight Attendant")
                {
                    HttpContext.Session.SetString("Role", "Flight Attendant");
                    TempData["adminName"] = s.StaffName;
                    return RedirectToAction("Index", "Admin");
                }
            }
            
            TempData["Message"] = "Invalid Login credentials";
            return RedirectToAction("Login");
        }
        
        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Announcements()
        {
            return View();
        }
        

        public async Task<ActionResult> API()
        {
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.covid19api.com");
            HttpResponseMessage response = await client.GetAsync("/summary");

            APIViewModel apiModel = new APIViewModel();
            CovidAdvisory covidAdvisory = new CovidAdvisory();
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                covidAdvisory = JsonConvert.DeserializeObject<CovidAdvisory>(data);
                //List<Country> list = covidAdvisory.Countries; /*put a stopper on line 101 and hover over the list*/
            }
            else
            {
                TempData["fail"] = "Api unsuccessfully loaded";
                return View(new CovidAdvisory());
            }
            List<Country> countryList = covidAdvisory.Countries.OrderBy(o => o.TotalConfirmed).ToList();
            List<Country> TopFive = new List<Country> { };
            List<Country> BottomFive = new List<Country> { };
            for(int i = 0;i <5;i++)
            {
                BottomFive.Add(countryList[i]);
            }
            countryList.Reverse();
            for (int i = 0; i < 5; i++)
            {
                TopFive.Add(countryList[i]);
            }
            apiModel.Global = covidAdvisory.Global;
            apiModel.Top5Countries = TopFive;
            apiModel.Bottom5Countries = BottomFive;
            return View(apiModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}