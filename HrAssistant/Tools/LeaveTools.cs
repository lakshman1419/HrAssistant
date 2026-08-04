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


        public async Task<int> GetLeaveBalanceAsync(
            string employeeId)
        {
            return await _leaveService
                .GetLeaveBalanceAsync(employeeId);
        }
    }
}