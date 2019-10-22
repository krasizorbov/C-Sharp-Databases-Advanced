using System;
using System.Data.SqlClient;

namespace Increase_Age_Stored_Procedure
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server = DESKTOP-NTEG5RA\SQLEXPRESS;Integrated Security = true;Initial Catalog = MinionsDB;";

            SqlConnection dbCon = new SqlConnection(connectionString);

            dbCon.Open();

            int id = int.Parse(Console.ReadLine());

            using (dbCon)
            {
                var cmd = new SqlCommand("EXEC usp_GetOlder @Id", dbCon);
                cmd.Parameters.AddWithValue("@Id", id);
         
                cmd.ExecuteNonQuery();
       
                cmd = new SqlCommand("SELECT * FROM Minions WHERE Id = @Id", dbCon);
                cmd.Parameters.AddWithValue("@Id", id);

                var reader = cmd.ExecuteReader();

                using (reader)
                {
                    reader.Read();

                    Console.WriteLine($"{(string)reader["Name"]} - {(int)reader["Age"]} years old");
                }
            }
        }
    }
}
