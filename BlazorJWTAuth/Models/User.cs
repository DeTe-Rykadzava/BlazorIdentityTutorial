using System.Security.Claims;

namespace BlazorJWTAuth.Models;

public class User
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int Age { get; set; }
    public List<string> Roles { get; set; } = new();

    public ClaimsPrincipal ToClaimPrincipal() => new(new ClaimsIdentity(
        new Claim[]
        {
            new Claim(ClaimTypes.Name, Username),
            new Claim(ClaimTypes.Hash, Password),
            new Claim(nameof(Age), Age.ToString())
        }.Concat(Roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray()),
        "BlazorJWTAuth"));

    public static User FromClaimsPrincipal(ClaimsPrincipal principal) => new()
    {
        Username = principal.FindFirst(ClaimTypes.Name)?.Value ?? "",
        Password = principal.FindFirst(ClaimTypes.Hash)?.Value ?? "",
        Age = Convert.ToInt32(principal.FindFirst(nameof(Age))?.Value),
        Roles = principal.FindAll(ClaimTypes.Role).Select(s => s.Value).ToList()
    };
}