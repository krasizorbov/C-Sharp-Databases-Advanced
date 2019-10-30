using SoftUni.Data;
using System;
using System.Linq;
using System.Text;

namespace P14_DeleteProjectByID
{
    public class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            using (context)
            {
                var result = DeleteProjectById(context);

                Console.WriteLine(result);
            }
        }
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var projectToRemove = context.Projects.FirstOrDefault(p => p.ProjectId == 2);

            var employeeProjects = context.EmployeesProjects.Where(ep => ep.ProjectId == 2).ToList();

            context.EmployeesProjects.RemoveRange(employeeProjects);

            context.Projects.Remove(projectToRemove);

            context.SaveChanges();

            var projects = context.Projects.Select(p => p.Name).Take(10).ToList();

            foreach (var p in projects)
            {
                result.AppendLine($"{p}");
            }
            return result.ToString().TrimEnd();
        }
    }
}
