using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace P06_AddingNewAddressAndUpdatingEmployee
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = AddNewAddressToEmployee(context);

                Console.WriteLine(result);
            }
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address() 
            { 
            AddressText = "Vitoshka 15",
            TownId = 4
            };

            var employeeName = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov")
                .Address = address;

            context.SaveChanges();

            StringBuilder result = new StringBuilder();

            var employeesAddressText = context.Employees.OrderByDescending(a => a.AddressId)
                .Take(10).Select(e => e.Address.AddressText);

            foreach (var addressText in employeesAddressText)
            {
                result.AppendLine($"{addressText}");
            }
            return result.ToString().TrimEnd();
        }
    }
}
