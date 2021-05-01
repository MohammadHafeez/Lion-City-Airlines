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
    public class BookingDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public BookingDAL()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("LionCityAirlinesConnectionString");
            conn = new SqlConnection(strConn);
        }
        public int Add(Booking b)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Booking ( CustomerID, ScheduleID, PassengerName,
                                 PassportNumber, Nationality, SeatClass, AmtPayable, Remarks, DateTimeCreated)
                                OUTPUT INSERTED.BookingID
                                VALUES( @CustomerID, @ScheduleID, @PassengerName,
                                 @PassportNumber, @Nationality, @SeatClass, @AmtPayable, @Remarks, @DateTimeCreated)";

                cmd.Parameters.AddWithValue("@CustomerID", b.CustID);
                cmd.Parameters.AddWithValue("@ScheduleID", b.ScheduleID);
                cmd.Parameters.AddWithValue("@PassengerName", b.PassengerName);
                cmd.Parameters.AddWithValue("@PassportNumber", b.PassportNo);
                cmd.Parameters.AddWithValue("@Nationality", b.Nationality);
                cmd.Parameters.AddWithValue("@SeatClass", b.SeatClass);
                cmd.Parameters.AddWithValue("@AmtPayable", b.AmtPayable);
                if (b.Remark != null)
                {
                    cmd.Parameters.AddWithValue("@Remarks", b.Remark);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Remarks", DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@DateTimeCreated", b.DateTimeCreated);

            conn.Open();
            b.BookingID = (int)cmd.ExecuteScalar();
            conn.Close();
            return b.BookingID;
        }

        public List<Booking> GetBookingHistory(int custID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT*FROM Booking WHERE CustomerID = @CustID";

            cmd.Parameters.AddWithValue("@CustID", custID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Booking> bookingList = new List<Booking>();
            while (reader.Read())
            {
                bookingList.Add(new Booking
                {
                    BookingID = reader.GetInt32(0),
                    CustID = reader.GetInt32(1),
                    ScheduleID = reader.GetInt32(2),
                    PassengerName = reader.GetString(3),
                    PassportNo = reader.GetString(4),
                    Nationality = reader.GetString(5),
                    SeatClass = reader.GetString(6),
                    AmtPayable = Convert.ToDouble(reader.GetDecimal(7)),
                    Remark = !reader.IsDBNull(8) ? reader.GetString(8) : null,
                    DateTimeCreated = reader.GetDateTime(9)
                });
            }

            reader.Close();
            conn.Close();
            return bookingList;
        }

        public List<Booking> GetAllBooking()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT*FROM Booking";

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Booking> bookingList = new List<Booking>();
            while (reader.Read())
            {
                bookingList.Add(new Booking
                {
                    BookingID = reader.GetInt32(0),
                    CustID = reader.GetInt32(1),
                    ScheduleID = reader.GetInt32(2),
                    PassengerName = reader.GetString(3),
                    PassportNo = reader.GetString(4),
                    Nationality = reader.GetString(5),
                    SeatClass = reader.GetString(6),
                    AmtPayable = Convert.ToDouble(reader.GetDecimal(7)),
                    Remark = !reader.IsDBNull(8) ? reader.GetString(8) : null,
                    DateTimeCreated = reader.GetDateTime(9)
                });
            }

            reader.Close();
            conn.Close();
            return bookingList;
        }

        public Booking GetSpecificBooking(int bookingID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT*FROM Booking WHERE BookingID = @bookingID";

            cmd.Parameters.AddWithValue("@bookingID", bookingID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Booking booking = new Booking { };
            while (reader.Read())
            {
                booking = new Booking
                {
                    BookingID = reader.GetInt32(0),
                    CustID = reader.GetInt32(1),
                    ScheduleID = reader.GetInt32(2),
                    PassengerName = reader.GetString(3),
                    PassportNo = reader.GetString(4),
                    Nationality = reader.GetString(5),
                    SeatClass = reader.GetString(6),
                    AmtPayable = Convert.ToDouble(reader.GetDecimal(7)),
                    Remark = !reader.IsDBNull(8) ? reader.GetString(8) : null,
                    DateTimeCreated = reader.GetDateTime(9)
                };
            }
            reader.Close();
            conn.Close();

            return booking;
        }
    }
}

