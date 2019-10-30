using SoftUni.Data;
using System;
using System.Linq;
using System.Text;

namespace P09_Employee147
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = GetEmployee147(context);

                Console.WriteLine(result);
            }
        }
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var employee = context.Employees.Where(e => e.EmployeeId == 147).Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Projects = e.EmployeesProjects.Select(ep => ep.Project.Name).OrderBy(p => p)
                }).FirstOrDefault();

            result.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var project in employee.Projects)
            {
                result.AppendLine($"{project}");
            }
            return result.ToString().TrimEnd();  
        }
    }
}
