using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using WEB_Assignment_P08_Team_3.Models;
using System.Security.Cryptography.X509Certificates;

namespace WEB_Assignment_P08_Team_3.DAL
{
    public class StaffDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public StaffDAL()
        {
            //Read ConnectionString from appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "LionCityAirlinesConnectionString");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        public List<Staff> GetAllStaff()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Staff";

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<Staff> staffList = new List<Staff>();
            while (reader.Read())
            {
                staffList.Add(new Staff
                {
                    StaffID = reader.GetInt32(0),
                    StaffName = reader.GetString(1),
                    Gender = !reader.IsDBNull(2) ? reader.GetString(2)[0] : (char)0,
                    DateEmployed = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null,
                    Vocation = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                    EmailAddr = reader.GetString(5),
                    Password = reader.GetString(6),
                    Status = reader.GetString(7)
                });
            }
            reader.Close();
            conn.Close();

            return staffList;
        }
        public int Add(Staff staff)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Staff (StaffName,Gender,DateEmployed,Vocation, EmailAddr,Password ,Status)
                              OUTPUT INSERTED.StaffID
                              VALUES(@name, @gender, @dateemployed, @vocation ,@email, @password, @status)";
            cmd.Parameters.AddWithValue("@name", staff.StaffName);
            cmd.Parameters.AddWithValue("@gender", staff.Gender);
            cmd.Parameters.AddWithValue("@dateemployed", staff.DateEmployed);
            cmd.Parameters.AddWithValue("@vocation", staff.Vocation);
            cmd.Parameters.AddWithValue("@email", staff.EmailAddr);
            cmd.Parameters.AddWithValue("@password", "p@55Staff");
            cmd.Parameters.AddWithValue("@status", "Active");
            System.Diagnostics.Debug.WriteLine(staff.Gender);
            conn.Open();
            staff.StaffID = (int)cmd.ExecuteScalar();
            conn.Close();
            return staff.StaffID;
        }
        public bool IsEmailExist(string email, int staffId)
        {
            bool emailFound = false;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT StaffID FROM Staff
                              WHERE EmailAddr=@selectedEmail";
            cmd.Parameters.AddWithValue("@selectedEmail", email);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { 
                while (reader.Read())
                {
                    if (reader.GetInt32(0) != staffId)
                        emailFound = true;
                }
            }
            else
            { 
                emailFound = false; 
            }
            reader.Close();
            conn.Close();
            return emailFound;
        }
        public Staff GetDetails(int StaffID)
        {
            Staff staff = new Staff();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Staff
            WHERE StaffID = @selectedStaffID";
            cmd.Parameters.AddWithValue("@selectedStaffID", StaffID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    staff.StaffID = StaffID;
                    staff.StaffName = !reader.IsDBNull(1) ? reader.GetString(1) : null;
                    staff.Gender = !reader.IsDBNull(2) ? reader.GetString(2)[0] : (char)0;
                    staff.DateEmployed = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null;
                    staff.Vocation = !reader.IsDBNull(4) ? reader.GetString(4) : null;
                    staff.EmailAddr = !reader.IsDBNull(5) ? reader.GetString(5) : null;
                    staff.Status = !reader.IsDBNull(7) ? reader.GetString(7) : null;
                }
            }
            reader.Close();
            conn.Close();
            return staff;
        }
        public int Update(Staff staff)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Staff SET Status=@status 
                              WHERE StaffID=@selectedStaffID";
            cmd.Parameters.AddWithValue("@status", staff.Status);
            cmd.Parameters.AddWithValue("@selectedStaffID", staff.StaffID);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }
        public List<Staff> GetAssign(int ScheduleID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Staff WHERE StaffID NOT IN (SELECT s.StaffID FROM Staff s INNER JOIN FlightCrew fc ON s.StaffID = fc.StaffID INNER JOIN FlightSchedule fs ON fc.ScheduleID = fs.ScheduleID 
                                WHERE Cast(fs.DepartureDateTime AS date) = (SELECT CAST(DepartureDateTime AS date) FROM FlightSchedule WHERE ScheduleID = @selectedScheduleID)) AND Vocation != @admin AND Status = @active";
            cmd.Parameters.AddWithValue("@selectedScheduleID", ScheduleID);
            cmd.Parameters.AddWithValue("@admin", "Administrator");
            cmd.Parameters.AddWithValue("@active", "Active");
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Staff> staffList = new List<Staff>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    staffList.Add(new Staff
                    {
                        StaffID = reader.GetInt32(0),
                        StaffName = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                        Gender = !reader.IsDBNull(2) ? reader.GetString(2)[0] : (char)0,
                        DateEmployed = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null,
                        Vocation = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                        EmailAddr = !reader.IsDBNull(5) ? reader.GetString(5) : null,
                        Status = !reader.IsDBNull(7) ? reader.GetString(7) : null
                    });
                }
            }
            reader.Close();
            conn.Close();
            return staffList;
        }
    }
}
