using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB_Assignment_P08_Team_3.Models
{
    public class APIViewModel
    {
        public Global Global { get; set; }

        public List<Country> Top5Countries { get; set; }

        public List<Country> Bottom5Countries { get; set; }

        public APIViewModel()
        {
            Top5Countries = new List<Country>();
            Bottom5Countries = new List<Country>();
        }
    }
}
