namespace TestAuthBlz.Models;

public class LoginResultModel
{
    public string Message { get; set; } = "";
    
    public string Token { get; set; } = "";
    public bool IsSuccess { get; set; }
}