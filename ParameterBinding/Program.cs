using System.Reflection.Metadata.Ecma335;

namespace ParameterBinding
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.MapGet("/employees", () => Employee.GetAllEmployees());

            app.MapGet("/employees/search", (string query) =>
            {
                return Employee.Search(query);
            });

            app.MapGet("/Employees/{id}", (int id) => Employee.GetOneEmployee(id));

            app.MapPost("/Employees", (Employee employee) =>
            {
                Employee.CreateOneEmployee(employee);
            });

            app.Run();
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public decimal Salary { get; set; }

        private static List<Employee> Employees = new List<Employee>()
        {
            new Employee() { Id=1, FullName="Ozan Yaprak", Salary=34000},
            new Employee() { Id=2, FullName="Erhan Yaprak", Salary=22000},
            new Employee() { Id=3, FullName="Erman Yaprak", Salary=5000},
        };

        public static List<Employee> GetAllEmployees()
        { return Employees; }

        public static Employee? GetOneEmployee(int id)
        { return id > 0 ? Employees.FirstOrDefault(x => x.Id == id) : null; }

        public static void CreateOneEmployee(Employee employee)
        {
            Employees.Add(employee);
        }

        public static List<Employee>? Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) { return Employees; }

            var lowerQuery = query.ToLowerInvariant();

            return Employees.Where(x => !string.IsNullOrWhiteSpace(x.FullName) && x.FullName.ToLowerInvariant().Contains(lowerQuery)).ToList();
        }
    }
}