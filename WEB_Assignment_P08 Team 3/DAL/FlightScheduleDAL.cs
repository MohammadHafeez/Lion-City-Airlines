using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using WEB_Assignment_P08_Team_3.Models;
using System.Data.SqlTypes;

namespace WEB_Assignment_P08_Team_3.DAL
{
    public class FlightScheduleDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public FlightScheduleDAL()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("LionCityAirlinesConnectionString");
            conn = new SqlConnection(strConn);
        }

        public List<FlightSchedule> GetAllFlightSchedule()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from FlightSchedule";
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<FlightSchedule> flightScheduleList = new List<FlightSchedule>();
            while (reader.Read())
            {
                flightScheduleList.Add(
                    new FlightSchedule
                    {
                        ScheduleId = reader.GetInt32(0),
                        FlightNumber = reader.GetString(1),
                        RouteId = reader.GetInt32(2),
                        AircraftId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                        DepartureDateTime = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(4),
                        ArrivalDateTime = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                        EconomyClassPrice = Convert.ToDouble(reader.GetDecimal(6)),
                        BusinessClassPrice = Convert.ToDouble(reader.GetDecimal(7)),
                        Status = reader.GetString(8)
                    }); ;
            }
            reader.Close();
            conn.Close();

            return flightScheduleList;
        }

        //Method for FlightBooking under Package 1: Customer
        public List<ScheduleViewModel> GetAvailableFlights()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM FlightSchedule fs INNER JOIN FlightRoute fr ON fs.RouteID = fr.RouteID WHERE status = 'Opened' AND DepartureDateTime - GETDATE() > 1";
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<ScheduleViewModel> scheduleVM = new List<ScheduleViewModel> { };
            while (reader.Read())
            {
                scheduleVM.Add( new ScheduleViewModel
                {
                    ScheduleId = reader.GetInt32(0),
                    FlightNumber = reader.GetString(1),
                    RouteId = reader.GetInt32(2),
                    AircraftId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    DepartureDateTime = reader.GetDateTime(4),
                    ArrivalDateTime = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                    EconomyClassPrice = Convert.ToDouble(reader.GetDecimal(6)),
                    BusinessClassPrice = Convert.ToDouble(reader.GetDecimal(7)),
                    Status = reader.GetString(8),
                    DepartureCity = reader.GetString(10),
                    ArrivalCity = reader.GetString(12)
                }); 
            }
            reader.Close();
            conn.Close();

            return scheduleVM;
        }

        public void Add(FlightSchedule flightSchedule)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"insert into FlightSchedule (FlightNumber, RouteID, AircraftID, DepartureDateTime, ArrivalDateTime, EconomyClassPrice, BusinessClassPrice) 
            values(@fN, @rId, @aId, @dDt, @aDt, @eCp, @bCp)";

            cmd.Parameters.AddWithValue("@fN", flightSchedule.FlightNumber);
            cmd.Parameters.AddWithValue("@rId", flightSchedule.RouteId);

            if (flightSchedule.AircraftId == null)
            {
                cmd.Parameters.AddWithValue("@aId", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@aId", flightSchedule.AircraftId);
            }

            if (flightSchedule.DepartureDateTime == null)
            {
                cmd.Parameters.AddWithValue("@dDt", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@dDt", flightSchedule.DepartureDateTime);
            }

            if (flightSchedule.ArrivalDateTime == null)
            {
                cmd.Parameters.AddWithValue("@aDt", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@aDt", flightSchedule.ArrivalDateTime);
            }

            cmd.Parameters.AddWithValue("@eCp", flightSchedule.EconomyClassPrice);
            cmd.Parameters.AddWithValue("@bCp", flightSchedule.BusinessClassPrice);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void Update(FlightSchedule flightSchedule)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"update flightschedule set status = @s where scheduleid = @sId";

            cmd.Parameters.AddWithValue("@sId", flightSchedule.ScheduleId);
            cmd.Parameters.AddWithValue("@s", flightSchedule.Status);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //Method for FlightBooking under Package 1: Customer
        public double GetSeatPrice(int scheduleID, string seatClass)
        {
            SqlCommand cmd = conn.CreateCommand();
            if (seatClass == "Economy")
            {
                cmd.CommandText = @"SELECT EconomyClassPrice FROM FlightSchedule WHERE ScheduleID = @sId";
            }
            else
            {
                cmd.CommandText = @"SELECT BusinessClassPrice FROM FlightSchedule WHERE ScheduleID = @sId";
            }
            conn.Open();
            cmd.Parameters.AddWithValue("@sId", scheduleID);
            SqlDataReader reader = cmd.ExecuteReader();
            double price = 0.00;
            while (reader.Read())
            {
                price = Convert.ToDouble(reader.GetDecimal(0));
            }

            reader.Close();
            conn.Close();
            return (price);
        }
        public ScheduleViewModel GetScheduleDetails(int scheduleID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM FlightSchedule fs INNER JOIN FlightRoute fr ON fs.RouteID = fr.RouteID WHERE scheduleID = @selectedScheduleID";
            cmd.Parameters.AddWithValue("@selectedScheduleID", scheduleID);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            ScheduleViewModel scheduleVM = new ScheduleViewModel();
            while (reader.Read())
            {
                scheduleVM = new ScheduleViewModel
                {
                    ScheduleId = reader.GetInt32(0),
                    FlightNumber = reader.GetString(1),
                    RouteId = reader.GetInt32(2),
                    AircraftId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    DepartureDateTime = reader.GetDateTime(4),
                    ArrivalDateTime = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                    EconomyClassPrice = Convert.ToDouble(reader.GetDecimal(6)),
                    BusinessClassPrice = Convert.ToDouble(reader.GetDecimal(7)),
                    Status = reader.GetString(8),
                    DepartureCity = reader.GetString(10),
                    ArrivalCity = reader.GetString(12)
                }; ;
            }
            reader.Close();
            conn.Close();

            return scheduleVM;
        }
    }
}
