using MiniORM.App.Data;
using MiniORM.App.Data.Entities;
using System;
using System.Linq;

namespace MiniORM.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server = DESKTOP-H5R51FC\SQLEXPRESS01; Integrated Security = true; Initial Catalog = MiniORM";

            var context = new SoftUniDbContext(connectionString);

            context.Employees.Add(new Employee
            {
                FirstName = "Gosho",
                LastName = "Inserted",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true
            });
            var employee = context.Employees.Last();
            employee.FirstName = "KRASIMIR";
            context.SaveChanges();

        }
    }
}
