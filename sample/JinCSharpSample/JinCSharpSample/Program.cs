// See https://aka.ms/new-console-template for more information
using JinCSharpSample;

var hobbies = new EmployeeHobbies
{
    Indoor = new List<string> { "piano" },
    Outdoor = new List<string> { "surf" }
};
var employee = new Employee
{
    Age = 25,
    Hobbies = hobbies,
    Name = "John Doe",
    Id = 0
};

Console.WriteLine(employee.ToJson());