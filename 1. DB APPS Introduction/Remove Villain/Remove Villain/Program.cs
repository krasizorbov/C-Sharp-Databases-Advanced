using System;
using System.Data.SqlClient;

namespace Remove_Villain
{
    class Program
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            string connectionString = @"Server = DESKTOP-NTEG5RA\SQLEXPRESS;Integrated Security = true;Initial Catalog = MinionsDB;";

            SqlConnection dbCon = new SqlConnection(connectionString);

            dbCon.Open();
            using (dbCon)
            {
                SqlCommand cmd = new SqlCommand("SELECT Id FROM Villains WHERE Id = @Id", dbCon);

                cmd.Parameters.AddWithValue("@Id", villainId);

                int? result = (int?)cmd.ExecuteScalar();

                if (result == null)
                {
                    Console.WriteLine("No such villain was found.");
                    dbCon.Close();
                    return;
                }

                cmd = new SqlCommand("SELECT COUNT(*) FROM MinionsVillains WHERE VillainId = @Id", dbCon);
                cmd.Parameters.AddWithValue("@Id", villainId);
                int minionsCount = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("DELETE FROM MinionsVillains WHERE VillainId = @Id", dbCon);
                cmd.Parameters.AddWithValue("@Id", villainId);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("SELECT Name FROM Villains WHERE Id = @Id", dbCon);
                cmd.Parameters.AddWithValue("@Id", villainId);
                string villainName = (string)cmd.ExecuteScalar();

                cmd = new SqlCommand("DELETE FROM Villains WHERE Id = @Id", dbCon);
                cmd.Parameters.AddWithValue("@Id", villainId);
                cmd.ExecuteNonQuery();

                Console.WriteLine($"{villainName} was deleted.");
                Console.WriteLine($"{minionsCount} minions were released.");
            }
        }
    }
}
