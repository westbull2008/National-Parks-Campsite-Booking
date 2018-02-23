﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using System.Globalization;


namespace Capstone
{
    public class ProjectCLI
    {
        const string Command_Acadia = "1";
        const string Command_Arches = "2";
        const string Command_CuyahogaNationalValleyPark = "3";
        const string Command_Quit = "q";
        string DatabaseConnection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        CampgroundSqlDAL campgroundSqlDAL = null;


        public void MainParkList()
        {
            campgroundSqlDAL = new CampgroundSqlDAL(DatabaseConnection);
            PrintHeader();
            ParkMenu();

            Park park = new Park();

            while (true)
            {
                string command = Console.ReadLine();

                Console.Clear();

                switch (command.ToLower())
                {
                    case Command_Acadia:
                        park = GetParkInfo("Acadia");
                        break;

                    case Command_Arches:
                        park = GetParkInfo("Arches");
                        break;

                    case Command_CuyahogaNationalValleyPark:
                        park = GetParkInfo("Cuyahoga Valley");
                        break;

                    case Command_Quit:
                        Console.WriteLine("Thank you for using National Park Campsite Reservation!");
                        return;

                    default:
                        Console.WriteLine("The command provided was not a valid command, please try again.");
                        break;
                }

                CampgroundList(park);
            }
        }
        //Campground List
        const string Command_ViewCampgrounds = "1";
        const string Command_SearchForReservations = "2";
        const string Command_ReturnToPreviousScreen = "3";


        private void CampgroundList(Park park)
        {
            PrintHeader();
            CampgroundMenu();


            while (true)
            {
                string command = Console.ReadLine();
                Console.Clear();

                switch (command.ToLower())
                {
                    case Command_ViewCampgrounds:
                        ViewAllCampgrounds(park);
                        break;

                    case Command_SearchForReservations:
                        ReservationSearch(park);
                        break;

                    case Command_ReturnToPreviousScreen:
                        PreviousScreen();
                        break;


                    default:
                        Console.WriteLine("The command provided was not a valid command, please try again.");
                        break;
                }

                CampgroundMenu();
            }
        }

        private void ReservationSearch(Park park)
        {
            ViewAllCampgrounds(park);
            int campgroundId = CLIHelper.GetInteger("Which campground (enter 0 to cancel)? ");
            if (campgroundId == 0)
            {
                return;
            }
            else
            {
                Campground campground = campgroundSqlDAL.GetCampgroundById(campgroundId);

                string arrivalDate = CLIHelper.GetString("What is the arrival date? MM/DD/YYYY");
                string departureDate = CLIHelper.GetString("What is the departure date? MM/DD/YYYY");
                int arrivalMonth = Int32.Parse(arrivalDate.Substring(0, 2));
                int departureMonth = Int32.Parse(departureDate.Substring(0, 2));

                if(((arrivalMonth >= campground.OpeningMonth) && (arrivalMonth <= campground.ClosingMonth)) 
                    && ((departureMonth >= campground.OpeningMonth) && (departureMonth <= campground.ClosingMonth)))
                {
                    CampSiteSqlDAL dal = new CampSiteSqlDAL(DatabaseConnection);
                    List<CampSite> campSites = dal.Search(campgroundId, arrivalDate, departureDate);

                    Console.WriteLine($"Site No.    Max Occup.    Accessible?    Max RV Length   Utility   Cost");
                    if (campSites.Count > 0)
                    {
                        foreach (CampSite site in campSites)
                        {
                            Console.WriteLine($"{site.CampsiteNumber}    {site.MaxOccupancy}     {site.Accessible}      {site.MaxRvLength}     {site.Utilities}   {site.DailyFee.ToString("C")}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("**** NO RESULTS ****");
                    }
                }
                else
                {
                    Console.WriteLine("Sorry, park is closed, LOSER!");
                }
               

            }
        }
        //CAMPGROUND METHODS
        private void PreviousScreen()
        {
            PrintHeader();
            ParkMenu();
        }


        private string TranslateMonth(int month)
        {
            string result = "";

            if (month == 1)
            {
                result = "January";
            }
            else if (month == 2)
            {
                result = "February";
            }
            else if (month == 3)
            {
                result = "March";
            }
            else if (month == 4)
            {
                result = "April";
            }
            else if (month == 5)
            {
                result = "May";
            }
            else if (month == 6)
            {
                result = "June";
            }
            else if (month == 7)
            {
                result = "July";
            }
            else if (month == 8)
            {
                result = "August";
            }
            else if (month == 9)
            {
                result = "September";
            }
            else if (month == 10)
            {
                result = "October";
            }
            else if (month == 11)
            {
                result = "November";
            }
            else if (month == 12)
            {
                result = "December";
            }
            return result;
        }
        private void ViewAllCampgrounds(Park park)
        {
//            CampgroundSqlDAL dal = new CampgroundSqlDAL(DatabaseConnection);
            List<Campground> campgrounds = campgroundSqlDAL.ViewAllCampgrounds(park);

            if (campgrounds.Count > 0)
            {
                Console.WriteLine($"Name             Open             Close           Daily Fee");

                foreach (Campground campground in campgrounds)
                {
                    Console.WriteLine($"#{campground.CampgroundId}      {campground.CampName}        {TranslateMonth(campground.OpeningMonth)}     {TranslateMonth(campground.ClosingMonth)}   {campground.DailyFee.ToString("C")}");
                    Console.WriteLine();


                }
            }
            else
            {
                Console.WriteLine("**** NO RESULTS ****");
                Console.WriteLine();
            }
            return;
        }


        

        private Park GetParkInfo(string parkName)
        {
            ParkSqlDAL dal = new ParkSqlDAL(DatabaseConnection);
            Park park = dal.GetParkInfo(parkName);

            if (park != null)
            {
                    Console.WriteLine(park.ParkName + " National Park");
                    Console.WriteLine("Location:" + "\t" + park.Location);
                    Console.WriteLine("Established:" + "\t" + park.EstDate.ToString("MM/dd/yyyy"));
                    Console.WriteLine($"Area: {park.Area:###,###,##0} sq km");
                    Console.WriteLine($"Annual Visitors: {park.Visitors:###,###,##0}");
                    Console.WriteLine();
                    Console.WriteLine(park.Description);
                    Console.WriteLine();
 
            }
            else
            {
                Console.WriteLine("**** NO RESULTS ****");
                Console.WriteLine();
            }
            return park;
        }

        

        private void PrintHeader()
        {
            Console.WriteLine("__    _  _______  _______  ___   _______  __    _  _______  ___        _______  _______  ______    ___   _  _______");
            Console.WriteLine("|  |  | ||   _   ||       ||   | |       ||  |  | ||   _   ||   |      |       ||   _   ||    _ |  |   | | ||       |");
            Console.WriteLine("|   |_| ||  |_|  ||_     _||   | |   _   ||   |_| ||  |_|  ||   |      |    _  ||  |_|  ||   | ||  |   |_| ||  _____|");
            Console.WriteLine("|       ||       |  |   |  |   | |  | |  ||       ||       ||   |      |   |_| ||       ||   |_||_ |      _|| |_____");
            Console.WriteLine("|  _    ||       |  |   |  |   | |  |_|  ||  _    ||       ||   |___   |    ___||       ||    __  ||     |_ |_____  |");
            Console.WriteLine("| | |   ||   _   |  |   |  |   | |       || | |   ||   _   ||       |  |   |    |   _   ||   |  | ||    _  | _____| |");
            Console.WriteLine("|_|  |__||__| |__|  |___|  |___| |_______||_|  |__||__| |__||_______|  |___|    |__| |__||___|  |_||___| |_||_______|");
            Console.WriteLine();
        }
        private void ParkMenu()
        {
            Console.WriteLine("Please select park you would like to view: ");
            Console.WriteLine(" 1) - Acadia");
            Console.WriteLine(" 2) - Arches");
            Console.WriteLine(" 3) - Cuyahoga National Valley Park");
            Console.WriteLine(" Q) - Quit");
            Console.WriteLine();

        }

        private void CampgroundMenu()
        {
            Console.WriteLine("Select a Command");
            Console.WriteLine(" 1) View Campgrounds");
            Console.WriteLine(" 2) Search for Reservation");
            Console.WriteLine(" 3) Return to Previous Screen");
            Console.WriteLine();
        }

        private void SearchMenu()
        {
            Console.WriteLine("Search for Campground Reservation");
            Console.WriteLine();
            Console.WriteLine("Which campground (enter 0 to cancel)? ");
            Console.WriteLine("What is the arrival date? __/__/___");
            Console.WriteLine("What is the departure date? __/__/___");
            Console.WriteLine();
        }



    }
}
