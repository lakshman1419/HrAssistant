using HrAssistant.Models;
using HrAssistant.Services;

namespace HrAssistant.Tools
{
    public class EmployeeTools
    {
        private readonly IEmployeeService _employeeService;


        public EmployeeTools(
            IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        public async Task<Employee?> GetEmployeeAsync(
            string employeeId)
        {
            return await _employeeService
                .GetEmployeeAsync(employeeId);
        }
    }
}