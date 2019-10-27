using System;
using System.Data.SqlClient;

namespace Add_Minion
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] minionInfo = Console.ReadLine().Split();
            string minionName = minionInfo[1];
            int minionAge = int.Parse(minionInfo[2]);
            string minionTown = minionInfo[3];

            string[] villainInfo = Console.ReadLine().Split();
            string villainName = villainInfo[1];

            string connectionString = @"Server = whatever;" + "Integrated Security = true;" + "Initial Catalog = MinionsDB;";
            SqlConnection dbCon = new SqlConnection(connectionString);

            dbCon.Open();
            using (dbCon)
            {
                SqlCommand cmd = new SqlCommand($"SELECT Id FROM Towns WHERE Name = '{minionTown}'", dbCon);

                if (cmd.ExecuteScalar() == null)
                {
                    cmd = new SqlCommand($"INSERT INTO Towns(Name) VALUES ('{minionTown}')", dbCon);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Town {minionTown} was added to the database.");
                }

                cmd = new SqlCommand($"SELECT COUNT(*) FROM Villains WHERE Name = '{villainName}'", dbCon);

                if ((int)cmd.ExecuteScalar() == 0)
                {
                    cmd = new SqlCommand($"INSERT INTO Villains(Name, EvilnessFactorId) VALUES ('{villainName}', 4)", dbCon);

                    cmd.ExecuteNonQuery();

                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }

                cmd = new SqlCommand($"SELECT Id FROM Towns WHERE Name = '{minionTown}'", dbCon);
                int townId = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand($"INSERT INTO Minions(Name, Age, TownId) VALUES('{minionName}', {minionAge}, {townId})", dbCon);
                cmd.ExecuteNonQuery();

                int villainId = (int)new SqlCommand($"SELECT Id FROM Villains WHERE Name = '{villainName}'", dbCon).ExecuteScalar();

                int minionId = (int)new SqlCommand($"SELECT Id FROM Minions WHERE Name = '{minionName}' AND Age = {minionAge}", dbCon).ExecuteScalar();

                cmd = new SqlCommand($"INSERT INTO MinionsVillains VALUES('{minionId}','{villainId}')", dbCon);
                cmd.ExecuteNonQuery();

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
        }
    }
}
