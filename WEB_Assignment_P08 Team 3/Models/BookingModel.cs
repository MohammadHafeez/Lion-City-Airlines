using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Principal;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json.Serialization;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class BookingModel
    {
        public int Amount { get; set; }
        public List<Booking> NewBookings { get; set; }

        public BookingModel()
        {
            
            NewBookings = new List<Booking> ();
        }
    }
}
