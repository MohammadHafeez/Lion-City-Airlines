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
    public class CrewDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        public CrewDAL()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("LionCityAirlinesConnectionString");
            conn = new SqlConnection(strConn);
        }
        public List<FlightCrew> GetFlightCrew(int StaffID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM FlightCrew WHERE StaffID = @selectedStaff";
            cmd.Parameters.AddWithValue("@selectedStaff", StaffID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<FlightCrew> flightCrewList = new List<FlightCrew>();
            while (reader.Read())
            {
                flightCrewList.Add(
                    new FlightCrew
                    {
                        ScheduleID = reader.GetInt32(0),
                        StaffID = reader.GetInt32(1),
                        Role = reader.GetString(2),
                    }
                    );
            }
            reader.Close();
            conn.Close();
            return flightCrewList;
        }
        public List<Int32> GetFlightCrewID(int StaffID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM FlightCrew WHERE StaffID = @selectedStaff";
            cmd.Parameters.AddWithValue("@selectedStaff", StaffID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Int32> flightCrewIDList = new List<Int32>();
            while (reader.Read())
            {
                flightCrewIDList.Add(reader.GetInt32(0)
                    );
            }
            reader.Close();
            conn.Close();
            return flightCrewIDList;
        }
        public bool CheckCrew(int scheduleID, string role)
        {
            bool a = false;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM FlightCrew WHERE scheduleID = @selectedScheduleID AND Role = @selectedRole";
            cmd.Parameters.AddWithValue("@selectedScheduleID", scheduleID);
            cmd.Parameters.AddWithValue("@selectedRole", role);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                a = true;
            }
            else
            {
                a = false;
            }
            reader.Close();
            conn.Close();
            return a;
        }
        public int GetOldID(int scheduleID, string role)
        {
            int oldid = 1;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT StaffID FROM FlightCrew WHERE scheduleID = @selectedScheduleID AND Role = @selectedRole";
            cmd.Parameters.AddWithValue("@selectedScheduleID", scheduleID);
            cmd.Parameters.AddWithValue("@selectedRole", role);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                oldid = reader.GetInt32(0);
            }
            reader.Close();
            conn.Close();
            return oldid;
        }
        public int UpdateCrew(string newRole, int newStaffID, int newScheduleID, int oldStaffID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE FlightCrew SET StaffID=@selectedStaffID 
                              WHERE Role=@selectedRole AND StaffID=@selectedOldStaffID AND ScheduleID = @selectedScheduleID";
            cmd.Parameters.AddWithValue("@selectedStaffID", newStaffID);
            cmd.Parameters.AddWithValue("@selectedOldStaffID", oldStaffID);
            cmd.Parameters.AddWithValue("@selectedScheduleID", newScheduleID);
            cmd.Parameters.AddWithValue("@selectedRole", newRole);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }
        public int AddCrew(int newScheduleID, int newStaffID, string newRole)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO FlightCrew (ScheduleID,StaffID,Role)
                              OUTPUT INSERTED.ScheduleID
                              VALUES(@selectedScheduleID,@selectedStaffID,@selectedRole)";
            cmd.Parameters.AddWithValue("@selectedScheduleID", newScheduleID);
            cmd.Parameters.AddWithValue("@selectedStaffID", newStaffID);
            cmd.Parameters.AddWithValue("@selectedRole", newRole);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }
        public List<FlightCrew> GetAttendants(int newScheduleID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM FlightCrew WHERE ScheduleID = @selectedScheduleID AND Role = 'Flight Attendant'";
            cmd.Parameters.AddWithValue("@selectedScheduleID", newScheduleID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<FlightCrew> flightCrewList = new List<FlightCrew>();
            while (reader.Read())
            {
                flightCrewList.Add(
                    new FlightCrew
                    {
                        ScheduleID = reader.GetInt32(0),
                        StaffID = reader.GetInt32(1),
                        Role = reader.GetString(2),
                    }
                    );
            }
            reader.Close();
            conn.Close();
            return flightCrewList;
        }
    }
}
