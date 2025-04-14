using Microsoft.EntityFrameworkCore;
using emp.Data;
using emp.Models;

var builder = WebApplication.CreateBuilder(args);

// MySQL კავშირის სტრიქონი
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// `ApplicationDbContext` რეგისტრაცია MySQL-თან
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Employee API",
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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API v1");
        options.RoutePrefix = string.Empty;  
    });
}

app.UseHttpsRedirection();

app.MapGet("/api/employees", async (ApplicationDbContext dbContext) =>
{
    var allEmployees = await dbContext.Employees.ToListAsync();
    return allEmployees.Any() ? Results.Ok(allEmployees) : Results.NotFound("No employees found.");
});

app.MapPost("/api/employees", async (AddEmployeeDto addEmployeeDto, ApplicationDbContext dbContext) =>
{
    var employeeEntity = new Employee
    {
        Id = Guid.NewGuid(), // don't forget this if you're using GUIDs
        Name = addEmployeeDto.Name,
        Email = addEmployeeDto.Email,
        Phone = addEmployeeDto.Phone,
        Salary = addEmployeeDto.Salary
    };

    dbContext.Employees.Add(employeeEntity);
    await dbContext.SaveChangesAsync();

    return Results.Ok(employeeEntity);
});

app.MapGet("/api/employees/{id:guid}", async (Guid id, ApplicationDbContext dbContext) =>
{
    var employee = await dbContext.Employees.FindAsync(id);

    return employee is not null
        ? Results.Ok(employee)
        : Results.NotFound($"Employee with ID {id} not found.");
});

app.MapPut("/api/employees/{id:guid}", async (Guid id, UpdateEmployeeDto updateEmployeeDto, ApplicationDbContext dbContext) =>
{
    var employee = await dbContext.Employees.FindAsync(id);

    if (employee is null)
    {
        return Results.NotFound($"Employee with ID {id} not found.");
    }

    employee.Name = updateEmployeeDto.Name;
    employee.Email = updateEmployeeDto.Email;
    employee.Phone = updateEmployeeDto.Phone;
    employee.Salary = updateEmployeeDto.Salary;

    await dbContext.SaveChangesAsync();

    return Results.Ok(employee);
});

app.MapDelete("/api/employees/{id:guid}", async (Guid id, ApplicationDbContext dbContext) =>
{
    var employee = await dbContext.Employees.FindAsync(id);

    if (employee is null)
    {
        return Results.NotFound($"Employee with ID {id} not found.");
    }

    dbContext.Employees.Remove(employee);
    await dbContext.SaveChangesAsync();

    return Results.Ok($"Employee with ID {id} deleted successfully.");
});

app.Run();



