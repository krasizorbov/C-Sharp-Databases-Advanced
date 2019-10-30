using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace P05_EmployeesFromResearchAndDevelopment
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = GetEmployeesFromResearchAndDevelopment(context);

                Console.WriteLine(result);
            }
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employeeInfo = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new 
                    { FirstName = x.FirstName, 
                    LastName = x.LastName, 
                    DepartmentName = x.Department.Name, 
                    Salary = x.Salary })
                .OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName).ToArray();

            StringBuilder result = new StringBuilder();

            foreach (var employee in employeeInfo)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:F2}");
            }
            return result.ToString().TrimEnd();
        }
    }
}
