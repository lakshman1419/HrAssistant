namespace HrAssistant.Models;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public List<AuthenticationUser> Users { get; init; } = [];
}

public sealed class AuthenticationUser
{
    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;
}