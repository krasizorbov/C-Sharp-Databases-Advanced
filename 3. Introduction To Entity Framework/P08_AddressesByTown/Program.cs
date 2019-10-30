using SoftUni.Data;
using System;
using System.Linq;
using System.Text;

namespace P08_AddressesByTown
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = GetAddressesByTown(context);

                Console.WriteLine(result);
            }
        }
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addressesByTown = context.Addresses.GroupBy(a => new 
            {   a.AddressId, 
                a.AddressText, 
                a.Town.Name 
            }, (key, group) => new 
            {   AddressText = key.AddressText,
                Town = key.Name, 
                Count = group.Sum(a => a.Employees.Count)
            })
                .OrderByDescending(o => o.Count).ThenBy(o => o.Town).ThenBy(o => o.AddressText).Take(10);

            StringBuilder addressesByTownResult = new StringBuilder();

            foreach (var address in addressesByTown)
            {
                addressesByTownResult.AppendLine($"{address.AddressText}, {address.Town} - {address.Count} employees");
            }
            return addressesByTownResult.ToString().TrimEnd();
        }
    }
}
