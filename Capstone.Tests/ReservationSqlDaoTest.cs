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
    public class ReservationSqlDaoTest : ParentTest
    {
        [TestMethod]
        public void MakeNewReservationTest()
        {
            int startingRowCount = 0;
            int endingRowCount = 0;

            Reservation reservation = new Reservation();
            reservation.Space_Id = spaceId;
            reservation.Start_Date = Convert.ToDateTime("1999-12-31");
            reservation.End_Date = Convert.ToDateTime("2000-01-01");
            reservation.Number_of_Attendees = 10;
            reservation.Reserved_for = "Y2K";

            startingRowCount = GetRowCount("reservation");
            ReservationSqlDao dao = new ReservationSqlDao(connectionString);
            dao.MakeReservation(reservation);
            endingRowCount = GetRowCount("reservation");

            Assert.AreEqual(startingRowCount + 1, endingRowCount);
        }

        [TestMethod]
        public void ReservationNumberShouldReflectRecentReservation()
        {
            Reservation reservation = new Reservation();
            reservation.Space_Id = spaceId;
            reservation.Start_Date = Convert.ToDateTime("1999-12-31");
            reservation.End_Date = Convert.ToDateTime("2000-01-01");
            reservation.Number_of_Attendees = 10;
            reservation.Reserved_for = "Y2K";

            ReservationSqlDao dao = new ReservationSqlDao(connectionString);
            int resNum = dao.MakeReservation(reservation);

            Assert.AreEqual(reservationId + 1, resNum);
        }
    }
}
