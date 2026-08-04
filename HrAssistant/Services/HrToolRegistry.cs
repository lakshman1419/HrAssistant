using HrAssistant.Tools;

namespace HrAssistant.Services
{
    public class HrToolRegistry
    {
        public EmployeeTools Employee { get; }

        public LeaveTools Leave { get; }

        public PolicyTools Policy { get; }


        public HrToolRegistry(
            EmployeeTools employeeTools,
            LeaveTools leaveTools,
            PolicyTools policyTools)
        {
            Employee = employeeTools;
            Leave = leaveTools;
            Policy = policyTools;
        }
    }
}