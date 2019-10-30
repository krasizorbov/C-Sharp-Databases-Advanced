using SoftUni.Data;
using System;
using System.Linq;
using System.Text;

namespace P12_IncreaseSalaries
{
    class Program
    {
        public static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = IncreaseSalaries(context);

                Console.WriteLine(result);
            }
        }
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            context.Employees.Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }
            .Contains(e.Department.Name)).ToList().ForEach(e => e.Salary *= 1.12m);

            context.SaveChanges();

            var employees = context.Employees
                    .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }
                    .Contains(e.Department.Name))
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList();

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }
            return result.ToString().TrimEnd();
        }
    }
}
