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
    public class FlightRouteDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public FlightRouteDAL()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("LionCityAirlinesConnectionString");
            conn = new SqlConnection(strConn);
        }

        public List<FlightRoute> GetAllFlightRoute()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from flightroute";
            conn.Open();

            List<FlightRoute> flightRouteList = new List<FlightRoute>();

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                flightRouteList.Add(new FlightRoute
                {
                    RouteId = reader.GetInt32(0),
                    DepartureCity = reader.GetString(1),
                    DepartureCountry = reader.GetString(2),
                    ArrivalCity = reader.GetString(3),
                    ArrivalCountry = reader.GetString(4),
                    FlightDuration = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                });
            }

            reader.Close();
            conn.Close();

            return flightRouteList;
        }

        public void Add(FlightRoute flightRoute)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"insert into flightRoute (DepartureCity, DepartureCountry, ArrivalCity, ArrivalCountry, FlightDuration)
                                values(@departureCity, @departureCountry, @arrivalCity, @arrivalCountry, @flightDuration)";

            cmd.Parameters.AddWithValue("@departureCity", flightRoute.DepartureCity);
            cmd.Parameters.AddWithValue("@departureCountry", flightRoute.DepartureCountry);
            cmd.Parameters.AddWithValue("@arrivalCity", flightRoute.ArrivalCity);
            cmd.Parameters.AddWithValue("@arrivalCountry", flightRoute.ArrivalCountry);
            if (flightRoute.FlightDuration == null)
            {
                cmd.Parameters.AddWithValue("@flightDuration", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@flightDuration", flightRoute.FlightDuration);
            }

            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }
    }
}
