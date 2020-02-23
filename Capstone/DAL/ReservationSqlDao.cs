using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Capstone.DAL
{
    public class ReservationSqlDao
    {
        private string connectionString;
        private string addRes = "INSERT INTO reservation(space_id, number_of_attendees, start_date, end_date, reserved_for) " +
            "VALUES (@spaceId, @attendees, @dateFrom, @dateTo, @resName); " +
            "SELECT SCOPE_IDENTITY();";

        public ReservationSqlDao(string databaseConnection)
        {
            connectionString = databaseConnection;
        }

        public int MakeReservation(Reservation res)
        {
            int reservationID = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(addRes, conn);
                    cmd.Parameters.AddWithValue("@spaceId", res.Space_Id);
                    cmd.Parameters.AddWithValue("@attendees", res.Number_of_Attendees);
                    cmd.Parameters.AddWithValue("@dateFrom", res.Start_Date);
                    cmd.Parameters.AddWithValue("@dateTo", res.End_Date);
                    cmd.Parameters.AddWithValue("@resName", res.Reserved_for);

                    reservationID = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred making reservation.");
                Console.WriteLine(ex.Message);
            }

            return reservationID;
        }
    }
}
