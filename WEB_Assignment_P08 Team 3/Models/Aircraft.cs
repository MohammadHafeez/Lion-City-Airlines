using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class Aircraft
    {
        public int AircraftID { get; set; }

        [StringLength(50)]
        public string MakeModel { get; set; }

        public int? NumEconomySeat { get; set; }

        public int? NumBusinessSeat { get; set; }

        public DateTime? DateLastMaintenance { get; set; }

        public string Status { get; set; }
    }
}
