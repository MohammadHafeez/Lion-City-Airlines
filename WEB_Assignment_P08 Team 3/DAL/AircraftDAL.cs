using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using WEB_Assignment_P08_Team_3.Models;
namespace WEB_Assignment_P08_Team_3.DAL
{
    public class AircraftDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public AircraftDAL()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("LionCityAirlinesConnectionString");
            conn = new SqlConnection(strConn);
        }

        public List<Aircraft> GetAllAircraft()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from aircraft";
            conn.Open();

            List<Aircraft> flightRouteList = new List<Aircraft>();

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                flightRouteList.Add(new Aircraft
                {
                    AircraftID = reader.GetInt32(0),
                    MakeModel = reader.GetString(1),
                    NumEconomySeat = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                    NumBusinessSeat = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    DateLastMaintenance = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                    Status = reader.GetString(5)
                });
            }

            reader.Close();
            conn.Close();

            return flightRouteList;
        }
    }
}
