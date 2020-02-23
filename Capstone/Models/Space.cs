using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Venue_Id { get; set; }
        public bool Is_Accessible { get; set; }
        public int Open_From { get; set; }
        public int Open_To { get; set; }
        public decimal Rate { get; set; }
        public int Max_Occupancy { get; set; }
        public Dictionary<int, string> Months { get; }

        public Space()
        {
            Months = new Dictionary<int, string>();
            Months[1] = "Jan";
            Months[2] = "Feb";
            Months[3] = "Mar";
            Months[4] = "Apr";
            Months[5] = "May";
            Months[6] = "Jun";
            Months[7] = "Jul";
            Months[8] = "Aug";
            Months[9] = "Sept";
            Months[10] = "Oct";
            Months[11] = "Nov";
            Months[12] = "Dec";
        }

        public override string ToString()
        {
            string result = "#";
            result = Id.ToString().PadRight(3) + Name.PadRight(30) + 
                MonthFromNumber(Open_From).PadRight(10) + MonthFromNumber(Open_To).PadRight(20) +
                "$" + Rate.ToString().PadRight(20) + Max_Occupancy.ToString().PadRight(5);
            return result;
        }

        public string ToString(int days)
        {
            string result = "#";
            decimal totalCost = days * Rate;
            string accessible = Is_Accessible ? "Yes" : "No";

            result = Id.ToString().PadRight(10) + Name.PadRight(30) +
                "$" + Rate.ToString().PadRight(15) + Max_Occupancy.ToString().PadRight(14) + 
                accessible.PadRight(11) + "$" + totalCost.ToString().PadRight(5);
            return result;
        }

        public string MonthFromNumber(int num)
        {
            string month = "All year";

            if (num != 0)
            {
                month = Months[num];
            }
            return month;
        }

        
    }
}
