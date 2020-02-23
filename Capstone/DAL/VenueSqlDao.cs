using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class VenueSqlDao
    {
        private string connectionString;
        private string getVenuesSQL = "SELECT * FROM venue ORDER BY name;";
        private string getCity = "SELECT name FROM city WHERE id = @cityID;";
        private string getState = "SELECT state.name FROM state " +
            "INNER JOIN city ON city.state_abbreviation = state.abbreviation WHERE city.id = @cityID;";
        private string getCategories = "SELECT * FROM category " +
            "INNER JOIN category_venue AS cv ON cv.category_id = category.id " +
            "INNER JOIN venue ON venue.id = cv.venue_id " +
            "WHERE venue.id = @venueID;";

        public VenueSqlDao(string databaseConnection)
        {
            connectionString = databaseConnection;
        }

        public Dictionary<int, Venue> GetAllVenues()
        {
            Dictionary<int, Venue> valuePairs = new Dictionary<int, Venue>();
            int venueKey = 1;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(getVenuesSQL, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Venue venue = new Venue();

                        venue.Id = Convert.ToInt32(reader["id"]);
                        venue.Name = Convert.ToString(reader["name"]);
                        venue.Description = Convert.ToString(reader["description"]);
                        venue.City_Id = Convert.ToInt32(reader["city_id"]);

                        valuePairs.Add(venueKey, venue);
                        venueKey++;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error ocurred reading the venue table.");
                Console.WriteLine(ex.Message);
            }
            return valuePairs;
        }

        public string PrintVenueDetails(Venue venue)
        {
            string cityName = "";
            string stateName = "";
            List<string> categories = new List<string>();
            string result = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(getCity, conn);

                    cmd.Parameters.AddWithValue("@cityID", venue.City_Id);
                    cityName = Convert.ToString(cmd.ExecuteScalar());

                    cmd = new SqlCommand(getState, conn);
                    cmd.Parameters.AddWithValue("@cityID", venue.City_Id);
                    stateName = Convert.ToString(cmd.ExecuteScalar());

                    cmd = new SqlCommand(getCategories, conn);
                    cmd.Parameters.AddWithValue("@venueID", venue.Id);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        categories.Add(Convert.ToString(reader["name"]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error ocurred printing venue details.");
                Console.WriteLine(ex.Message);
            }

            result = venue.Name + "\nLocation: " + cityName + ", " + stateName + "\nCategories: "  + String.Join(',',categories)
                + "\n\n" + venue.Description;

            return result;

        }
    }
}
