using System.ComponentModel.DataAnnotations;

namespace HrAssistant.Models;

public sealed record LoginRequest(
    [param: Required] string? Username,
    [param: Required] string? Password);