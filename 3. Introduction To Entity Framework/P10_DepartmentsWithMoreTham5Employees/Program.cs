using SoftUni.Data;
using System;
using System.Linq;
using System.Text;

namespace P10_DepartmentsWithMoreTham5Employees
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = GetDepartmentsWithMoreThan5Employees(context);

                Console.WriteLine(result);
            }
        }
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(n => n.Name)
                .Select(s => new
                {
                    DepartmentName = s.Name,
                    ManagerFirstName = s.Manager.FirstName,
                    ManagerLastName = s.Manager.LastName,
                    Employees = s.Employees.Select(e => new
                    {
                        EmployeeFirsName = e.FirstName,
                        EmployeeLastName = e.LastName,
                        EmployeeJobTitle = e.JobTitle
                    }).OrderBy(f => f.EmployeeFirsName).ThenBy(l => l.EmployeeLastName)
                });

            foreach (var d in departments)
            {
                result.AppendLine($"{d.DepartmentName} - {d.ManagerFirstName} {d.ManagerLastName}");
                foreach (var e in d.Employees)
                {
                    result.AppendLine($"{e.EmployeeFirsName} {e.EmployeeLastName} - {e.EmployeeJobTitle}");
                }
            }
            return result.ToString().TrimEnd();
        }
    }
}
