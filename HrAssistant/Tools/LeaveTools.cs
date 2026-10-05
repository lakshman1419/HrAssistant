using HrAssistant.Services;

namespace HrAssistant.Tools
{
    public class LeaveTools
    {
        private readonly ILeaveService _leaveService;


        public LeaveTools(
            ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }


        public async Task<string> GetLeaveBalanceAsync(
            string employeeId,
            string? leaveType = null)
        {
            return await _leaveService
                .GetLeaveBalanceAsync(employeeId, leaveType);
        }
    }
}