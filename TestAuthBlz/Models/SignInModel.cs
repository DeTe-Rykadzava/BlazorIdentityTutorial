using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace TestAuthBlz.Models;

public class SignInModel
{
    [Required(AllowEmptyStrings = false)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = "";
    
    [Required(AllowEmptyStrings = false)] public string Password { get; set; } = "";

    // типо превращаем в клаймы пользователя по имени
    public static ClaimsPrincipal ToClaimsPrincipal(string userName)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, userName),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, "Administrator")
        }));
    }
}