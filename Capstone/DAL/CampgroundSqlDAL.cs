﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class CampgroundSqlDAL
    {
        private string connectionString;

        public CampgroundSqlDAL(string DatabaseConnection)
        {
            connectionString = DatabaseConnection;
        }

        public List<Campground> ViewAllCampgrounds()
        {
            List<Campground> output2 = new List<Campground>();
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SELECT * from campground WHERE park_id = 1", conn);

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Campground campground = new Campground();
                            campground.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                            campground.CampName = Convert.ToString(reader["name"]);
                            campground.OpeningMonth = Convert.ToDateTime(reader["open_from_mm"]);
                            campground.ClosingMonth = Convert.ToDateTime(reader["open_to_mm"]);
                            campground.DailyFee = Convert.ToDecimal(reader["daily_fee"]);


                            output2.Add(campground);
                        }
                    }
                }

                catch (SqlException ex)
                {
                    Console.WriteLine("An error occured reading the database: " + ex.Message);
                }
                return output2;
            }
        }

    }
}