using SoftUni.Data;
using System;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace P13_FindEmployeesByFirstNameStartingWithSA
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = GetEmployeesByFirstNameStartingWithSa(context);

                Console.WriteLine(result);
            }
        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var employees = context.Employees.Where(e => EF.Functions.Like(e.FirstName, "sa%")).Select(e => new 
            {
                FirstName = e.FirstName,
                LastName = e.LastName,
                JobTitle = e.JobTitle,
                Salary = e.Salary
            }).OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList();

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }
            return result.ToString().TrimEnd();
        }
    }
}
