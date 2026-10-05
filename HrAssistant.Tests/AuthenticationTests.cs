using System.Text.Json;
using HrAssistant.Controllers;
using HrAssistant.Models;
using HrAssistant.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace HrAssistant.Tests;

public sealed class AuthenticationTests
{
    private const string AdminPassword = "admin-test-password";
    private const string EmployeePassword = "employee-test-password";

    [Theory]
    [InlineData("admin", AdminPassword, "Admin")]
    [InlineData("employee", EmployeePassword, "Employee")]
    public void Login_returns_only_safe_profile_fields(string username, string password, string expectedRole)
    {
        var controller = CreateController();

        var result = controller.Login(new LoginRequest(username, password));

        var response = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<LoginResponse>(response.Value);
        Assert.Equal(expectedRole, body.User.Role);
        Assert.Equal(username, body.User.Username);
        Assert.DoesNotContain(password, JsonSerializer.Serialize(body));
    }

    [Theory]
    [InlineData("unknown-user", "any-password")]
    [InlineData("admin", "wrong-password")]
    public void Login_returns_generic_unauthorized_response_for_invalid_credentials(string username, string password)
    {
        var controller = CreateController();

        var result = controller.Login(new LoginRequest(username, password));

        var response = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var body = JsonSerializer.Serialize(response.Value);
        Assert.Contains("Invalid username or password.", body);
        Assert.DoesNotContain(username, body);
        Assert.DoesNotContain(password, body);
    }

    [Theory]
    [InlineData(null, "some-password")]
    [InlineData("admin", null)]
    [InlineData(" ", "some-password")]
    public void Login_rejects_missing_or_blank_input(string? username, string? password)
    {
        var controller = CreateController();

        var result = controller.Login(new LoginRequest(username, password));

        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Username and password are required.", JsonSerializer.Serialize(response.Value));
    }

    [Fact]
    public void Options_validator_requires_exactly_one_user_per_supported_role()
    {
        var options = CreateOptions();
        options.Users[1] = new AuthenticationUser
        {
            Username = "another-admin",
            Password = EmployeePassword,
            DisplayName = "Another Admin",
            Role = "Admin"
        };

        var result = new AuthenticationOptionsValidator().Validate(null, options);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public void Options_validator_rejects_tracked_password_placeholders()
    {
        var options = CreateOptions();
        options.Users[0] = new AuthenticationUser
        {
            Username = "admin",
            Password = "SET-LOCAL-ADMIN-PASSWORD",
            DisplayName = "Admin",
            Role = "Admin"
        };

        var result = new AuthenticationOptionsValidator().Validate(null, options);

        Assert.False(result.Succeeded);
    }

    private static AuthController CreateController()
    {
        var options = Options.Create(CreateOptions());
        return new AuthController(new InMemoryUserAuthenticationService(options));
    }

    private static AuthenticationOptions CreateOptions() => new()
    {
        Users =
        [
            new AuthenticationUser
            {
                Username = "admin",
                Password = AdminPassword,
                DisplayName = "Admin",
                Role = "Admin"
            },
            new AuthenticationUser
            {
                Username = "employee",
                Password = EmployeePassword,
                DisplayName = "Employee",
                Role = "Employee"
            }
        ]
    };
}