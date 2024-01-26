namespace BlazorJWTAuth.Models;

// public record LoginResult(string Message, string Token, bool Success);

public class LoginResult
{
    public string Message { get; set; } = "";
    public string? Token { get; set; }
    public bool Success { get; set; }

    public LoginResult(string message, bool success, string? token = null)
    {
        Message = message;
        Token = token;
        Success = success;
    }
}
