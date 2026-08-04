using HrAssistant.Models;

namespace HrAssistant.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> GetEmployeeAsync(
            string employeeId);
    }


    public class EmployeeService : IEmployeeService
    {
        private readonly Dictionary<string, Employee> _employees =
            new()
            {
                ["1001"] = new Employee
                {
                    Id = "1001",
                    Name = "John Smith",
                    Department = "Engineering",
                    LeaveBalance = 18
                },

                ["1002"] = new Employee
                {
                    Id = "1002",
                    Name = "Mary Brown",
                    Department = "Finance",
                    LeaveBalance = 22
                }
            };


        public Task<Employee?> GetEmployeeAsync(
            string employeeId)
        {
            var employee =
                _employees.GetValueOrDefault(employeeId);

            return Task.FromResult(employee);
        }
    }
}