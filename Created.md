გადატვირთე კომპიუტერი

გამორთე docker

activity-დან წაშალე docker

dotnet new webapp -n Empl1

cd Empl1

dotnet add package Microsoft.EntityFrameworkCore

dotnet add package Microsoft.EntityFrameworkCore.SqlServer

dotnet add package Swashbuckle.AspNetCore




(Create Models folder/Employee.cs class)
public Guid Id {get;set;}
public required string Name {get;set;}
public required string Email {get;set;}
public string? Phone {get;set;}
public decimal Salary {get;set;}



(Create Data folder/ApplicationDbContext.cs class)
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Empl1.Models;
using System.Threading.Tasks;

namespace Empl1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {

        }
        public DbSet<Employee> Employees {get;set;}
    }
    
}








(Program.cs)
using Microsoft.EntityFrameworkCore;
using Empl1.Data;
using Empl1.Models;

var builder = WebApplication.CreateBuilder(args);

// MySQL კავშირის სტრიქონი
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// `ApplicationDbContext` რეგისტრაცია MySQL-თან
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Weather Forecast API",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Forecast API v1");
        options.RoutePrefix = string.Empty;  // Swagger UI will be at the root (e.g., https://localhost:5001/)
    });
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" }[Random.Shared.Next(10)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}







dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet run

docker --version


(დააკოპირე ქვედა ტექსტი და Gpt-ს ჰკითხე: but i have already created once, now i want for another oroject, should i rename it or something?)


docker run -e "ACCEPT_EULA=1" -e "MSSQL_SA_PASSWORD=Your_password123" \
  -p 1433:1433 --name azuresqledge \
  -d mcr.microsoft.com/azure-sql-edge



შევქმნათ Azrue პროექტი


(Add connectionString in appsettings.json)
"ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1,1433;Database=EmplDb;User Id=sa;Password=Your_password123;Encrypt=False;"
    
  }




Program.cs-დან მოვაშოროთ კომენტარები

dotnet ef migrations add InitialCreate

dotnet ef database update


შევქმნათ Models-ში ორი კლასი AddEmployeeDto და UpdateEmployeeDto

AddEmployeeDto.cs:
public required string Name {get;set;}
        public required string Email {get;set;}
        public string? Phone {get;set;}
        public decimal Salary {get;set;}


UpdateEmployeeDto:
public required string Name {get;set;}
        public required string Email {get;set;}
        public string? Phone {get;set;}
        public decimal Salary {get;set;}


შევქმნათ კლასი Controllers
შევქმნათ api Controller
EmployeesController.cs:

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using emp.Data;
using emp.Models;

namespace emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public EmployeesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            var allEmployees = dbContext.Employees.ToList();
            return Ok(allEmployees);
        }




        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetAllEmployeeById(Guid id)
        {
            var employees = dbContext.Employees.Find(id);
            if(employees is null)
            {
                return NotFound();
            }
            return Ok(employees);
        }



        [HttpPost]
        public IActionResult AddEmployee(AddEmployeeDto addEmployeeDto)
        {
            var employeeEntity = new Employee()
            {
                Name = addEmployeeDto.Name,
                Email = addEmployeeDto.Email,
                Phone = addEmployeeDto.Phone,
                Salary = addEmployeeDto.Salary
            };
            dbContext.Employees.Add(employeeEntity);
            dbContext.SaveChanges();
            return Ok(employeeEntity);
        }


        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = dbContext.Employees.Find(id);
            if(employee is null)
            {
                return NotFound();
            }
            employee.Name = updateEmployeeDto.Name;
            employee.Email = updateEmployeeDto.Email;
            employee.Phone = updateEmployeeDto.Phone;
            employee.Salary = updateEmployeeDto.Salary;

            dbContext.SaveChanges();
            return Ok(employee);          
        }


        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteEmployee(Guid id)
        {
            var employee = dbContext.Employees.Find(id);
            if(employee is null)
            {
                return NotFound();
            }
            dbContext.Employees.Remove(employee);
            dbContext.SaveChanges();
            return Ok();      
        }
    }
}
