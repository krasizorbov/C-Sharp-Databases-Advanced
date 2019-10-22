using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Print_All_Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server = DESKTOP-NTEG5RA\SQLEXPRESS;Integrated Security = true;Initial Catalog = MinionsDB;";

            SqlConnection dbCon = new SqlConnection(connectionString);

            List<string> minionsInitial = new List<string>();
            List<string> minionsArranged = new List<string>();

            dbCon.Open();
            using (dbCon)
            {
                SqlCommand cmd = new SqlCommand($"SELECT Name FROM Minions", dbCon);

                SqlDataReader reader = cmd.ExecuteReader();

                using (reader)
                {
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        dbCon.Close();
                        return;
                    }

                    while (reader.Read())
                    {
                        minionsInitial.Add((string)reader["Name"]);
                    }
                }
            }

            while (minionsInitial.Count > 0)
            {
                minionsArranged.Add(minionsInitial[0]);
                minionsInitial.RemoveAt(0);

                if (minionsInitial.Count > 0)
                {
                    minionsArranged.Add(minionsInitial[^1]);
                    int index = minionsInitial.LastIndexOf(minionsInitial[^1]);
                    minionsInitial.RemoveAt(index);
                }
            }

            minionsArranged.ForEach(m => Console.WriteLine(m));
        }
    }
}
