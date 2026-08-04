namespace HrAssistant.Services
{
    public interface ILeaveService
    {
        Task<int> GetLeaveBalanceAsync(
            string employeeId);
    }


    public class LeaveService : ILeaveService
    {
        private readonly Dictionary<string, int> _leave =
            new()
            {
                ["1001"] = 18,
                ["1002"] = 22
            };


        public Task<int> GetLeaveBalanceAsync(
            string employeeId)
        {
            var balance =
                _leave.GetValueOrDefault(
                    employeeId,
                    0);

            return Task.FromResult(balance);
        }
    }
}