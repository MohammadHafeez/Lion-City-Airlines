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
    public class CustomerDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CustomerDAL()
        {
            
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "LionCityAirlinesConnectionString");
            conn = new SqlConnection(strConn);
        }

        public List<Customer> GetAllCustomers()
        {

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Customer";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Customer> customerList = new List<Customer>();
            while (reader.Read())
            {
                customerList.Add(
                    new Customer
                    {
                        CustomerID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Nationality = !reader.IsDBNull(2) ? reader.GetString(2):null,
                        BirthDate = !reader.IsDBNull(3)? reader.GetDateTime(3): (DateTime?)null ,
                        TelNo = !reader.IsDBNull(4) ? reader.GetString(4):null,
                        EmailAddr = reader.GetString(5),
                        Password = reader.GetString(6),
                    });
            }
            reader.Close();
            conn.Close();

            return customerList;
        }

        public Customer GetSpecificCustomer(int id)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Customer where CustomerID = @CustomerID";
            cmd.Parameters.AddWithValue("@CustomerID", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Customer customer = new Customer { };
            while (reader.Read())
            {
                customer =
                    new Customer
                    {
                        CustomerID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Nationality = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                        BirthDate = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null,
                        TelNo = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                        EmailAddr = reader.GetString(5),
                        Password = reader.GetString(6),
                    };
            }

            reader.Close();
            conn.Close();

            return customer;
        }

        public int Add(Customer customer)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Customer ( CustomerName, Nationality, BirthDate,
                                 TelNo, EmailAddr,Password)
                                OUTPUT INSERTED.CustomerID
                                VALUES( @CustomerName, @Nationality, @BirthDate,
                                 @TelNo, @EmailAddr, @Password)";
            cmd.Parameters.AddWithValue("@CustomerName",customer.Name);
            if (customer.Nationality != null )
            {
                cmd.Parameters.AddWithValue("@Nationality", customer.Nationality);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Nationality", DBNull.Value);
            }

            if (customer.BirthDate != null)
            {
                cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate);
            }
            else
            {
                cmd.Parameters.AddWithValue("@BirthDate", DBNull.Value);
            }

            if (customer.TelNo != null)
            {
                cmd.Parameters.AddWithValue("@TelNo", customer.TelNo);
            }
            else
            { 
                cmd.Parameters.AddWithValue("@TelNo", DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@EmailAddr", customer.EmailAddr);
            cmd.Parameters.AddWithValue("@Password", customer.Password);
            conn.Open();
            customer.CustomerID = (int)cmd.ExecuteScalar();
            conn.Close();
            return customer.CustomerID;
        }

        public int UpdatePassword(string password,int id)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Customer SET Password= @Password
                                WHERE  CustomerID = @CustomerID";
            cmd.Parameters.AddWithValue("@Password",password);
            cmd.Parameters.AddWithValue("@CustomerID", id);
            conn.Open();
            int rowAffected = 0;
            rowAffected += cmd.ExecuteNonQuery();
            conn.Close();
            return rowAffected;
        }

    }
}
