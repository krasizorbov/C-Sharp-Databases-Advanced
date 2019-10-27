using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Increase_Minion_Age
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] selectedMinionsIds = Console.ReadLine().Split().Select(int.Parse).ToArray();

            string connectionString = @"Server = whatever;Integrated Security = true;Initial Catalog = MinionsDB;";

            SqlConnection dbCon = new SqlConnection(connectionString);

            dbCon.Open();

            List<int> minionsIds = new List<int>();
            List<string> minionsNames = new List<string>();
            List<int> minionsAges = new List<int>();

            using (dbCon)
            {
                SqlCommand cmd = new SqlCommand($"SELECT * FROM Minions WHERE Id IN ({String.Join(", ", selectedMinionsIds)})", dbCon);

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
                        minionsIds.Add((int)reader["Id"]);
                        minionsNames.Add((string)reader["Name"]);
                        minionsAges.Add((int)reader["Age"]);
                    }
                }

                for (int i = 0; i < minionsIds.Count; i++)
                {
                    int id = minionsIds[i];
                    string name = minionsNames[i].ToLower();
                    int age = minionsAges[i] + 1;

                    cmd = new SqlCommand($"UPDATE Minions SET Name = '{name}', Age = {age} WHERE Id = {id}", dbCon);

                    cmd.ExecuteNonQuery();
                }

                cmd = new SqlCommand($"SELECT * FROM Minions", dbCon);
                reader = cmd.ExecuteReader();

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
                        Console.WriteLine($"{(string)reader["Name"]} {(int)reader["Age"]}");
                    }
                }
            }
        }
    }
}
