using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            Console.WriteLine(RemoveTown(context));

        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employee = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.Salary,
                    e.JobTitle,
                    e.EmployeeId
                })
                .OrderBy(e => e.EmployeeId)
                .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }
            return sb.ToString();
        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employee = context.Employees.Select(e => new { e.FirstName, e.Salary })
                .Where(employee => employee.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();
            StringBuilder sb = new StringBuilder();
            employee.ForEach(e => { sb.AppendLine($"{e.FirstName} - {e.Salary:f2}"); });
            return sb.ToString();
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)

        {
            var employees = context.Employees.Select(e => new { e.FirstName, e.LastName, e.Department.Name, e.Salary })
                .Where(e => e.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();
            StringBuilder stringBuilder = new StringBuilder();
            employees.ForEach(e => { stringBuilder.AppendLine($"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}"); });
            return stringBuilder.ToString();
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            var employeeWithLastName = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");
            employeeWithLastName.Address = address;
            context.SaveChanges();
            StringBuilder sb = new StringBuilder();
            var addresses = context.Employees.Select(e => new
            {
                e.AddressId,
                AddressText = e.Address.AddressText
            })
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .ToList();
            addresses.ForEach(res => { sb.AppendLine(res.AddressText); });


            return sb.ToString();
        }
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                 .Select(e => new
                 {
                     e.FirstName,
                     e.LastName,
                     ManagerFirstName = e.Manager.FirstName,
                     ManagerLastName = e.Manager.LastName,
                     Projects = e.EmployeesProjects.Select(p => new { p.Project })
                                                    .Where(p => p.Project.StartDate >= new DateTime(2001, 1, 1)
                                                    && p.Project.StartDate <= new DateTime(2003, 12, 31))
                                                    .ToList()
                 }).Take(10)
                 .ToList();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var e in employees)
            {
                stringBuilder.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Projects)
                {

                    stringBuilder.Append($"--{p.Project.Name} - {p.Project.StartDate} - ");
                    stringBuilder.AppendLine(p.Project.EndDate == null ? "not finished" : p.Project.EndDate.ToString());
                }
            }



            return stringBuilder.ToString();
        }
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                    .Select(a => new
                    {
                        TownName = a.Town.Name,
                        a.AddressText,
                        EmployeesCount = a.Employees.Count
                    }).OrderByDescending(a => a.EmployeesCount)
                    .ThenBy(t => t.TownName)
                    .ThenBy(a => a.AddressText)
                    .Take(10)
                    .ToList();
            StringBuilder sb = new StringBuilder();
            addresses.ForEach(a => { sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeesCount} employees"); });
            return sb.ToString();

        }
        public static string GetEmployee147(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects.Select(ep => new { ep.Project })
                    .OrderBy(p => p.Project.Name)
                    .ToList()
                }).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                e.Projects.ForEach(p => sb.AppendLine(p.Project.Name));
            }
            return sb.ToString();
        }
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Select(d => new
                {
                    d.Name,
                    ManagerName = d.Manager.FirstName + ' ' + d.Manager.LastName,
                    Employees = d.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList()
                }).Where(d => d.Employees.Count > 5)
                .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.ManagerName} ");
                d.Employees.ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}"));
            }
            return sb.ToString();
        }
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate,
                })
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate.ToString());
            }
            return sb.ToString();
        }
        public static string IncreaseSalaries(SoftUniContext context)
        {
            string[] departmentsToChange = { "Engineering", "Tool Design", "Marketing", "Information Services" };
            var employees = context.Employees
                .Where(e=>departmentsToChange.Contains(e.Department.Name))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    Salary =  e.Salary + (e.Salary * 0.12m)
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e=> e.LastName)
                .ToList();
            context.SaveChanges();
            StringBuilder sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }
                

            return sb.ToString();


        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e=>e.FirstName.Substring(0,2)=="Sa")
                .Select(e=> new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e=> e.LastName)
                .ToList();
            StringBuilder sb = new StringBuilder();
            employees.ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));

            return sb.ToString();
        }
        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectToRemove = context.Projects.FirstOrDefault(p => p.ProjectId == 2);

            foreach (var e in context.EmployeesProjects)
            {
                if (e.ProjectId == 2)
                {
                    e.Project = null;
                }
            }
            
            context.Projects.Remove(projectToRemove);
            context.SaveChanges();
            StringBuilder sb = new StringBuilder();
            context.Projects
                .Take(10)
                .ToList()
                .ForEach(e=> sb.AppendLine(e.Name));
            return sb.ToString();
        }
        public static string RemoveTown(SoftUniContext context)
        {
            var townToRemove = context.Towns.FirstOrDefault(t=>t.Name=="Seattle");

            int count = townToRemove==null?0: townToRemove.Addresses.Count();
            foreach (var e in context.Employees)
            {
                if (e.Address==null)
                {
                    continue;
                }
                if (e.Address.Town==townToRemove)
                {
                    e.Address = null;
                }
            }
            foreach (var a in context.Addresses)
            {
                if (a.TownId==townToRemove.TownId)
                {
                    a.TownId = null;
                }   
            }
            context.Towns.Remove(townToRemove); 
            context.SaveChanges();
            return $"{count} addresses in Seattle were deleted";
        }

    }
}



