using System;
using System.Collections.Generic;
using System.Text;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone
{
    public class UserInterface
    {
        private string connectionString;
        private SpaceSqlDao spaceDao;
        private VenueSqlDao venueDao;
        private ReservationSqlDao reservationDao;

        private Dictionary<int, Venue> venueCollection = new Dictionary<int, Venue>();
        private Dictionary<int, Space> spaceCollection = new Dictionary<int, Space>();
        private Dictionary<int, Space> availableSpaces = new Dictionary<int, Space>();

        public Venue CurrentVenue { get; set; }
        public int MenuPath { get; set; }

        public UserInterface(string connectionString)
        {
            this.connectionString = connectionString;
            spaceDao = new SpaceSqlDao(this.connectionString);
            venueDao = new VenueSqlDao(this.connectionString);
            reservationDao = new ReservationSqlDao(this.connectionString);
        }

        public void Run()
        {
            string answer = "";
            
            while (answer != "q")
            {
                MenuPath = 0;

                Console.WriteLine("What would you like to do?");
                Console.WriteLine(" 1) List Venues");
                Console.WriteLine(" 2) Search for reservation");
                Console.WriteLine(" Q) Quit");
                answer = Console.ReadLine().ToLower();
                Console.WriteLine();
                switch (answer)
                {
                    case "1":
                        VenueMenu();
                        break;
                    case "2":
                        ReservationMenu(0);
                        break;
                    case "q":
                        break;
                    default:
                        Console.WriteLine("Please make a valid selection.");
                        break;
                }
            }
            Console.WriteLine("Thanks for using our system");
            Console.ReadLine();
        }

        public void VenueMenu()
        {
            int selection = 0;
            string answer = "";
            bool idIsValid = false;

            while (MenuPath == 0)
            {
                ListVenues();
                // PRINT ALL VENUES
                Console.WriteLine();
                answer = Console.ReadLine().ToLower();
                idIsValid = int.TryParse(answer, out selection);
                idIsValid = venueCollection.ContainsKey(selection);

                if (answer == "r")
                {
                    return;
                }
                else if (!idIsValid)
                {
                    Console.WriteLine("\nPlease make a valid selection.");
                    continue;
                }
                else
                {
                    CurrentVenue = venueCollection[selection]; // CurrentVenue is set NOWHERE ELSE
                    VenueMenuDeep();
                }
            } 
        }

        public void VenueMenuDeep()
        {
            string answer = "";
            // PRINT VENUE DETAILS
            Console.WriteLine(venueDao.PrintVenueDetails(CurrentVenue));

            while (MenuPath == 0)
            {
                Console.WriteLine();
                Console.WriteLine("What would you like to do next");
                Console.WriteLine(" 1) View spaces");
                Console.WriteLine(" R) Return to previous screen");
                answer = Console.ReadLine().ToLower();
                Console.WriteLine();

                switch (answer)
                {
                    case "1":
                        ListSpaces();
                        SpaceMenu();
                        break;
                    case "r":
                        return;
                    default:
                        Console.WriteLine("Please make a valid selection.");
                        break;
                }
            }
        }

        public void SpaceMenu()
        {
            string answer = "";

            while (MenuPath == 0)
            {
                Console.WriteLine("What would you like to do next?");
                Console.WriteLine("  1) Reserve a space");
                Console.WriteLine("  R) Return to previous screen");
                answer = Console.ReadLine().ToLower();

                switch (answer)
                {
                    case "1":
                        ReservationMenu(CurrentVenue.Id);
                        break;
                    case "r":
                        return;
                    default:
                        Console.WriteLine("\nPlease make a valid selection.\n");
                        Console.WriteLine();
                        break;
                }
            }
        }

        public void ReservationMenu(int id)
        {
            int tempId = id; // either CurrentVenue.Id or 0
            int daysNeeded = 0;
            int attendees = 0;
            int spaceNum = 0;
            DateTime dateStart = DateTime.Today;
            DateTime dateEnd = DateTime.Today;
            Reservation reservation = new Reservation();

            dateStart = DateCheck();
            daysNeeded = DaysCheck(dateStart);
            dateEnd = dateStart.AddDays(daysNeeded);
            attendees = NumberOfPeopleCheck();
            
            availableSpaces = spaceDao.GetAvailableSpaces(dateStart, dateEnd, tempId, attendees);

            if (availableSpaces.Count == 0)
            {
                NoSpaceMenu();
            }
            else
            {
                DisplayAvailableSpaces(daysNeeded, attendees);
                spaceNum = SpaceCheckMenu();

                if (spaceNum == 0)
                {
                    return;
                }
                else
                {
                    reservation = MakeReservationO(spaceNum, attendees, dateStart, dateEnd);
                    ReservationConfirmation(reservation, daysNeeded);
                    MenuPath = 1;
                }
            } 
        }

        public DateTime DateCheck()
        {
            DateTime start = new DateTime();
            DateTime y2k = Convert.ToDateTime("2000-01-01");
            Console.WriteLine("When do you need the space? (Post Y2K please)");

            while (!DateTime.TryParse(Console.ReadLine(), out start) || start < y2k)
            {
                Console.WriteLine("Invalid datetime format. Use yyyy-mm-dd.");
            }
            return start;
        }

        public int DaysCheck(DateTime start)
        {
            int daysNeeded = 0;

            do
            {
                Console.WriteLine("How many days you will need the space for? (No more than 1 year)");
            }
            while (!int.TryParse(Console.ReadLine(), out daysNeeded) || daysNeeded < 1 || daysNeeded > 365);

            return daysNeeded;
        }

        public int NumberOfPeopleCheck()
        {
            int people = 0;
            do
            {
                Console.WriteLine("How many people will be in attendance?");
            }
            while (!int.TryParse(Console.ReadLine(), out people) || people < 1);

            return people;
        }

        public void NoSpaceMenu()
        {
            Console.WriteLine("There are no available spaces. Would you like to try a different search?\nChoose (y)es or (n)o.");
            string answer = "";
            while (true)
            {
                answer = Console.ReadLine().ToLower();
                switch (answer)
                {
                    case "y":
                        return;
                    case "n":
                        MenuPath = 1;
                        return;
                    default:
                        Console.WriteLine("Please answer 'y' or 'n'.");
                        break;
                }
            }
        }

        public int SpaceCheckMenu()
        {
            int spaceNumber = 0;
            string answer = "";
            bool isValid = false;
            Console.WriteLine("Which space would you like to reserve?");

            do
            {
                Console.WriteLine("Please enter a valid space ID or 'r' cancel.");
                answer = Console.ReadLine().ToLower();
                isValid = int.TryParse(answer, out spaceNumber);
                if (answer == "r")
                {
                    MenuPath = 1;
                    break;
                }
            } while (!(isValid && availableSpaces.ContainsKey(spaceNumber)));

            return spaceNumber;
        }

        public Reservation MakeReservationO(int spaceNumber, int attendees, DateTime dateStart, DateTime dateEnd)
        {
            Reservation reservation = new Reservation();
            
            Console.WriteLine("Who is this reservation for?");
            string reservationName = Console.ReadLine();

            reservation.Space_Id = spaceNumber;
            reservation.Number_of_Attendees = attendees;
            reservation.Start_Date = dateStart;
            reservation.End_Date = dateEnd;
            reservation.Reserved_for = reservationName;

            return reservation;
        }

        public void ReservationConfirmation(Reservation re, int days)
        {
            int confirmationNum = 0;
            string venueName = "";
            int spaceID = re.Space_Id;
            decimal cost = 0;

            venueName = spaceDao.GetVenueName(spaceID);
            cost = days * availableSpaces[spaceID].Rate;
            confirmationNum = reservationDao.MakeReservation(re);
            re.Id = confirmationNum;

            Console.WriteLine("\nThanks for your reservation! The details for your event are listed below:\n");
            Console.WriteLine(re.ToString(venueName, availableSpaces[spaceID].Name, cost));
            Console.WriteLine("\nPress enter to return to the main menu.");
            Console.ReadLine();
        }

        public void ListVenues()
        {
            venueCollection = venueDao.GetAllVenues();
            Console.WriteLine("Which venue would you like to view?");

            foreach (KeyValuePair<int, Venue> kvp in venueCollection)
            {
                Console.WriteLine(kvp.Key + ") " + kvp.Value.Name);
            }
            Console.WriteLine("R) Return to previous screen.");
        }

        public void ListSpaces()
        {
            string result = "";

            spaceDao = new SpaceSqlDao(connectionString);
            spaceCollection = spaceDao.GetSpaces(CurrentVenue.Id);

            Console.WriteLine(CurrentVenue.Name + "\n");
            Console.WriteLine("Name".PadLeft(10) + "Open".PadLeft(27) + "Close".PadLeft(10) +
                "Daily Rate".PadLeft(25) + "Max. Occupancy".PadLeft(20));

            foreach (KeyValuePair<int, Space> kvp in spaceCollection)
            {
                result += kvp.Value.ToString() + "\n";
            }
            Console.WriteLine(result);
        }

        public void DisplayAvailableSpaces(int days, int attendees)
        {
            Console.WriteLine("\nThe following spaces are available based on your needs:\n");
            Console.WriteLine("Space #".PadRight(15) + "Name".PadRight(20) + "Daily Rate".PadRight(15) +
                "Max Occupancy".PadRight(17) + "Accessible?".PadRight(12) + "Total Cost".PadRight(10));
            Console.WriteLine();
            foreach (KeyValuePair<int, Space> kvp in availableSpaces)
            {
                    Console.WriteLine(kvp.Value.ToString(days));
            }
            Console.WriteLine();
        }
    }
}
