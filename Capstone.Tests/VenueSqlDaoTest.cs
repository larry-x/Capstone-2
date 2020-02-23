using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Capstone.Models;
using Capstone.DAL;

namespace Capstone.Tests
{
    [TestClass]
    public class VenueSqlDaoTest : ParentTest
    {
        [TestMethod]
        public void GetAllVenuesTest()
        {           
            VenueSqlDao dao = new VenueSqlDao(connectionString);
            Dictionary<int, Venue> VenueTestObj = dao.GetAllVenues();
            Assert.AreEqual(GetRowCount("venue"), VenueTestObj.Count);
        }

        [TestMethod]
        public void PrintVenueTest()
        {
            string expected = "Hogwarts\nLocation: Platform 9 and 3/4, Scotland?\nCategories: \n\nA wizards haven.";
            Venue venue = new Venue();
            venue.Id = venueId;
            venue.Name = "Hogwarts";
            venue.Description = "A wizards haven.";
            venue.City_Id = cityId;

            VenueSqlDao dao = new VenueSqlDao(connectionString);
            string actual = dao.PrintVenueDetails(venue);

            Assert.AreEqual(expected, actual);
        }


    }

}
