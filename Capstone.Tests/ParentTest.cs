using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Capstone.Tests
{
    [TestClass]
    public class ParentTest
    {
        private TransactionScope trans;

        protected string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=excelsior_venues;Integrated Security=True";
        protected int cityId = 0;
        protected int venueId = 0;
        protected int spaceId = 0;
        protected int reservationId = 0;

        [TestInitialize]
        public void Setup()
        {
            trans = new TransactionScope();

            string sql = File.ReadAllText("test-script.sql");
            string sql1 = "INSERT INTO venue (name, city_id, description) " +
                        "VALUES('Hogwarts', @id, 'A wizards haven.'); SELECT SCOPE_IDENTITY();";
            string sql2 = "INSERT INTO space(venue_id, name, is_accessible, open_from, open_to, daily_rate, max_occupancy) " +
                        "VALUES(@id, 'Chamber of Secrets', 1, 2, 12, 5000, 200); SELECT SCOPE_IDENTITY();";
            string sql3 = "INSERT INTO reservation(space_id, number_of_attendees, start_date, end_date, reserved_for) " +
                        "VALUES(@id, 100, '2020-02-20', '2020-02-24', 'Harry'); SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cityId = Convert.ToInt32(cmd.ExecuteScalar());
                    
                cmd = new SqlCommand(sql1, conn);
                cmd.Parameters.AddWithValue("@id", cityId);
                venueId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SqlCommand(sql2, conn);
                cmd.Parameters.AddWithValue("@id", venueId);
                spaceId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SqlCommand(sql3, conn);
                cmd.Parameters.AddWithValue("@id", spaceId);
                reservationId = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        [TestCleanup]
        public void Reset()
        {
            trans.Dispose();
        }

        protected int GetRowCount(string table)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM {table}", conn);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count;
            }
        }

    }
}
