using SoftUni.Data;
using System;
using System.Linq;
using System.Text;

namespace P04_EmployeesWithSalaryOver50000
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            var result = GetEmployeesWithSalaryOver50000(context);

            Console.WriteLine(result);

        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employeeInfo = context.Employees.Where(x => x.Salary > 50000).OrderBy(x => x.FirstName);

            StringBuilder result = new StringBuilder();

            foreach (var employee in employeeInfo)
            {
                result.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }
            return result.ToString().TrimEnd();
        }
    }
}
