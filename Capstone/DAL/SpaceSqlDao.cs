using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class SpaceSqlDao
    {
        private string connectionString;
        private string getSpaceSQL = "SELECT * FROM space INNER JOIN venue ON " +
            "venue.id = space.venue_id WHERE venue_id = @venueId;";
        private string spaceAvailability = "SELECT * FROM space " +
                "WHERE id NOT IN(SELECT space_id FROM reservation AS r " +
                "WHERE (@openFrom BETWEEN r.start_date AND r.end_date) " +
                "OR(@openTo BETWEEN r.start_date AND r.end_date) OR " +
                "(@openFrom < r.start_date AND @openTo > r.end_date) " +
                "GROUP BY r.space_id) AND(open_from IS NULL OR " +
                "((MONTH(@openFrom) BETWEEN open_from AND open_to) AND " +
                "(MONTH(@openTo) BETWEEN open_from AND open_to)))";
        private string getVenueNameSQL = "SELECT venue.name FROM venue " +
            "INNER JOIN space ON space.venue_id = venue.id " +
            "WHERE space.id = @spaceId;";
       

        public SpaceSqlDao(string databaseConnection)
        {
            connectionString = databaseConnection;
        }

        public Dictionary<int, Space> GetSpaces(int id)
        {
            int venueId = id;
            Dictionary<int, Space> valuePairs = new Dictionary<int, Space>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(getSpaceSQL, conn);
                    cmd.Parameters.AddWithValue("@venueId", venueId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Space space = new Space();

                        space.Id = Convert.ToInt32(reader["id"]);
                        space.Venue_Id = Convert.ToInt32(reader["venue_id"]);
                        space.Name = Convert.ToString(reader["name"]);
                        space.Is_Accessible = Convert.ToBoolean(reader["is_accessible"]);
                        space.Open_From = (reader["open_from"] as int?) ?? 0;
                        space.Open_To = (reader["open_to"] as int?) ?? 0;
                        space.Rate = Convert.ToDecimal(reader["daily_rate"]);
                        space.Max_Occupancy = Convert.ToInt32(reader["max_occupancy"]);

                        valuePairs.Add(space.Id, space);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred getting spaces.");
                Console.WriteLine(ex.Message);
            }

            return valuePairs;
        }

        public Dictionary<int, Space> GetAvailableSpaces(DateTime openFrom, DateTime openTo, int venueId, int attendees)
        {
            Dictionary<int, Space> availableSpaces = new Dictionary<int, Space>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    bool capacityCheck = false;
                    bool limitToThisVenue = false;
                    int i = 0;

                    SqlCommand cmd = new SqlCommand(spaceAvailability, conn);
                    cmd.Parameters.AddWithValue("@openFrom", openFrom);
                    cmd.Parameters.AddWithValue("@openTo", openTo);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        limitToThisVenue = (venueId == Convert.ToInt32(reader["venue_id"]) || venueId == 0);
                        capacityCheck = attendees <= Convert.ToInt32(reader["max_occupancy"]);
                        if (limitToThisVenue && capacityCheck && i < 5)
                        {
                            Space space = new Space();
                            space.Id = Convert.ToInt32(reader["id"]);
                            space.Venue_Id = Convert.ToInt32(reader["venue_id"]);
                            space.Name = Convert.ToString(reader["name"]);
                            space.Is_Accessible = Convert.ToBoolean(reader["is_accessible"]);
                            space.Open_From = (reader["open_from"] as int?) ?? 0;
                            space.Open_To = (reader["open_to"] as int?) ?? 0;
                            space.Rate = Convert.ToDecimal(reader["daily_rate"]);
                            space.Max_Occupancy = Convert.ToInt32(reader["max_occupancy"]);

                            availableSpaces.Add(space.Id, space);

                            i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred getting available spaces.");
                Console.WriteLine(ex.Message);
            }

            return availableSpaces;

        }

        public string GetVenueName(int id)
        {
            string result = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(getVenueNameSQL, conn);
                    cmd.Parameters.AddWithValue("@spaceId", id);
                    result = Convert.ToString(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred getting venue name.");
                Console.WriteLine(ex.Message);
            }
            return result;
        }
       
    }
}
