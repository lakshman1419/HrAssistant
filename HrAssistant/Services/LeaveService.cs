namespace HrAssistant.Services
{
    public interface ILeaveService
    {
        Task<string> GetLeaveBalanceAsync(
            string employeeId,
            string? leaveType = null);
    }


    public class LeaveService : ILeaveService
    {
        private readonly Dictionary<string, int> _leave =
            new()
            {
                ["1001"] = 18,
                ["1002"] = 22
            };


        public Task<string> GetLeaveBalanceAsync(
            string employeeId,
            string? leaveType = null)
        {
            if (!_leave.TryGetValue(employeeId, out var balance))
            {
                return Task.FromResult(
                    $"No leave balance was found for employee ID {employeeId}.");
            }

            if (!string.IsNullOrWhiteSpace(leaveType)
                && !string.Equals(leaveType, "TOTAL", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(
                    $"A {leaveType.ToUpperInvariant()} leave balance is not available for employee ID {employeeId}. " +
                    $"The current data source only records an overall balance of {balance} days, without a leave-type breakdown.");
            }

            return Task.FromResult(
                $"Employee {employeeId} has {balance} overall leave days recorded. The current data source does not specify a leave type.");
        }
    }
}