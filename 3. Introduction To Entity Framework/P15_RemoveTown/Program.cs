using SoftUni.Data;
using System;
using System.Linq;

namespace P15_RemoveTown
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = RemoveTown(context);

                Console.WriteLine(result);
            }
        }
        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns.FirstOrDefault(t => t.Name == "Seattle");

            var townID = town.TownId;

            context.Towns.Remove(town);

            var count = context.Addresses.Where(a => a.TownId == townID).ToArray().Count();

            context.Employees.Where(e => e.Address.Town.Name == "Seattle")
                    .ToList()
                    .ForEach(e => e.AddressId = null);

            context.Addresses.Where(a => a.Town.Name == "Seattle")
                    .ToList()
                    .ForEach(a => context.Addresses.Remove(a));

            context.SaveChanges();

            return $"{count} addresses in Seattle were deleted";
        }
    }
}
