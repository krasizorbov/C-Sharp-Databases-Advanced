//using SoftUni.Data;
using SoftUni.Data;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = GetEmployeesFullInformation(context);

                Console.WriteLine(result);
            } 
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employeeInfo = context.Employees.OrderBy(x => x.EmployeeId);

            StringBuilder result = new StringBuilder();

            foreach (var employee in employeeInfo)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return result.ToString().TrimEnd();
        }
    }
}
