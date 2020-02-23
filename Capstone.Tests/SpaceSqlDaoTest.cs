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
    public class SpaceSqlDaoTest : ParentTest
    {
        [TestMethod]
        public void GetSpacesOfSpecificVenueTest()
        {
            SpaceSqlDao dao = new SpaceSqlDao(connectionString);
            Dictionary<int, Space> spacesByVenueTestObj = dao.GetSpaces(venueId);

            Assert.AreEqual(1, spacesByVenueTestObj.Count);
        }

        [DataTestMethod]
        [DataRow("2020/02/18", "2020/02/22", 2, 0)]
        [DataRow("2020/03/01", "2020/03/31", 20, 1)]
        [DataRow("2020/01/01", "2020/01/30", 200, 0)]
        [DataRow("2020/02/26", "2020/03/03", 2000, 0)]
        public void GetSpaceAvailabilityTest(string start, string end, int attendees, int numberSpacesExpected)
        {
            DateTime openFrom = DateTime.Parse(start);
            DateTime openTo = DateTime.Parse(end);

            SpaceSqlDao dao = new SpaceSqlDao(connectionString);
            Dictionary<int, Space> spacesAvailableTestObj = dao.GetAvailableSpaces(openFrom, openTo, venueId, attendees);

            Assert.AreEqual(numberSpacesExpected, spacesAvailableTestObj.Count);
        }

        [TestMethod]
        public void GetVenueNameFromSpaceIDTEST()
        {
            SpaceSqlDao dao = new SpaceSqlDao(connectionString);
            string name = dao.GetVenueName(spaceId);

            Assert.AreEqual("Hogwarts", name);
        }
    }       
}
