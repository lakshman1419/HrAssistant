using HrAssistant.Models;

namespace HrAssistant.Services;

public interface IUserAuthenticationService
{
    SafeUserProfile? Authenticate(string? username, string? password);
}