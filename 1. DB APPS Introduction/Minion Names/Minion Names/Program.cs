using System;
using System.Data.SqlClient;

namespace Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server = whatever;" + "Integrated Security = true;" + "Initial Catalog = MinionsDB;";

            SqlConnection dbCon = new SqlConnection(connectionString);

            int villainId = int.Parse(Console.ReadLine());

            dbCon.Open();

            using (dbCon)
            {
                SqlCommand cmd = new SqlCommand($"SELECT Name FROM Villains WHERE Id = {villainId}", dbCon);

                string villainName = (string)cmd.ExecuteScalar();
                if (villainName == null)
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                    return;
                }

                Console.WriteLine($"Villain: {villainName}");

                cmd = new SqlCommand($"SELECT ROW_NUMBER() OVER(ORDER BY m.Name) AS RowNum, m.Name, m.Age FROM Minions AS m " +
                    $"JOIN MinionsVillains As mv ON mv.MinionId = m.Id WHERE mv.VillainId = {villainId} ORDER BY m.Name", dbCon);

                SqlDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("(no minions)");
                        reader.Close();
                        dbCon.Close();
                        return;
                    }

                    while (reader.Read())
                    {
                        string minionName = (string)reader["Name"];
                        long rowNum = (long)reader["RowNum"];
                        int minionAge = (int)reader["Age"];

                        Console.WriteLine($"{rowNum}. {minionName} {minionAge}");
                    }
                }
            }
        }
    }
}
