using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
   public class Venue
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int City_Id { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            string result = "";
            result = Name + "/n";
            return Id.ToString().PadRight(6) + Name.PadRight(30) + 
                City_Id.ToString().PadRight(20) + Description.PadRight(10);
        }

    }
}
