using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Reservation
    {
        public int Id { get; set; }       
        public int Space_Id { get; set; }
        public int Number_of_Attendees { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public string Reserved_for { get; set; }

        public string ToString(string venueName, string spaceName, decimal totalCost)
        {
            string result = "#";

            result = "Confirmation #: " + Id + "\nVenue: " + venueName + 
                "\nSpace: " + spaceName + "\nReserved For: " + Reserved_for +
                "\nAttendees: " + Number_of_Attendees + "\nArrival Date: " + Start_Date + "\nDepart Date: "
                + End_Date + "\nTotal Cost: " + "$" + totalCost;

            return result;
        }
    }

   

}
