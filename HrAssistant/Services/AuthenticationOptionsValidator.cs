using HrAssistant.Models;
using Microsoft.Extensions.Options;

namespace HrAssistant.Services;

public sealed class AuthenticationOptionsValidator : IValidateOptions<AuthenticationOptions>
{
    public ValidateOptionsResult Validate(string? name, AuthenticationOptions options)
    {
        if (options.Users is null || options.Users.Count != 2)
        {
            return ValidateOptionsResult.Fail("Exactly one Admin and one Employee user must be configured.");
        }

        if (options.Users.Any(user =>
                string.IsNullOrWhiteSpace(user.Username)
                || string.IsNullOrWhiteSpace(user.Password)
                || string.IsNullOrWhiteSpace(user.DisplayName)))
        {
            return ValidateOptionsResult.Fail("Each account must have a username, password, and display name.");
        }

        if (options.Users.Any(user => user.Password.StartsWith("SET-LOCAL-", StringComparison.Ordinal)))
        {
            return ValidateOptionsResult.Fail("Replace the credential placeholders with local passwords before starting the API.");
        }

        if (options.Users.Count(user => user.Role == "Admin") != 1
            || options.Users.Count(user => user.Role == "Employee") != 1)
        {
            return ValidateOptionsResult.Fail("Exactly one Admin and one Employee user must be configured.");
        }

        if (options.Users
            .Select(user => user.Username.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() != 2)
        {
            return ValidateOptionsResult.Fail("Usernames must be unique.");
        }

        return ValidateOptionsResult.Success;
    }
}