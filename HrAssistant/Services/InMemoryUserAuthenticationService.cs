using HrAssistant.Models;
using Microsoft.Extensions.Options;

namespace HrAssistant.Services;

public sealed class InMemoryUserAuthenticationService : IUserAuthenticationService
{
    private readonly IReadOnlyList<AuthenticationUser> _users;

    public InMemoryUserAuthenticationService(IOptions<AuthenticationOptions> options)
    {
        _users = options.Value.Users;
    }

    public SafeUserProfile? Authenticate(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var user = _users.FirstOrDefault(candidate =>
            string.Equals(candidate.Username, username.Trim(), StringComparison.OrdinalIgnoreCase)
            && string.Equals(candidate.Password, password, StringComparison.Ordinal));

        return user is null
            ? null
            : new SafeUserProfile(user.Username, user.DisplayName, user.Role);
    }
}