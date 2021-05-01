using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Principal;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json.Serialization;
using System.Reflection.Metadata.Ecma335;

namespace WEB_Assignment_P08_Team_3.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Global
    {
        public int NewConfirmed { get; set; }
        public int TotalConfirmed { get; set; }
        public int NewDeaths { get; set; }
        public int TotalDeaths { get; set; }
        public int NewRecovered { get; set; }
        public int TotalRecovered { get; set; }
    }

    public class Premium
    {

    }

    public class Country
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string Slug { get; set; }
        public int NewConfirmed { get; set; }
        public int TotalConfirmed { get; set; }
        public int NewDeaths { get; set; }
        public int TotalDeaths { get; set; }
        public int NewRecovered { get; set; }
        public int TotalRecovered { get; set; }
        public DateTime Date { get; set; }
        public Premium Premium { get; set; }
    }

    public class CovidAdvisory
    {
        public Global Global { get; set; }
        public List<Country> Countries { get; set; }
        public DateTime Date { get; set; }
    }
}
