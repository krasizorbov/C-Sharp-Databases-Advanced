using System;
using System.Data.SqlClient;

namespace Villain_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server = whatever;" + "Integrated Security = true;" + "Initial Catalog = MinionsDB;"; 

            SqlConnection dbCon = new SqlConnection(connectionString);

            dbCon.Open();

            using (dbCon)
            {
                SqlCommand cmd = new SqlCommand("SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount FROM Villains AS v " +
                    "JOIN MinionsVillains AS  mv ON mv.VillainId = v.Id GROUP BY v.Name HAVING COUNT(mv.VillainId) > 3 " +
                    "ORDER BY MinionsCount DESC", dbCon);

                SqlDataReader reader = cmd.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        string villainName = (string)reader["Name"];
                        int count = (int)reader["MinionsCount"];
                        Console.WriteLine($"{villainName} - {count}");
                    }
                }
            }
        }
    }
}
