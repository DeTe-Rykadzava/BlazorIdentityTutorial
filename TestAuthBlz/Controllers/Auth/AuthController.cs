using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TestAuthBlz.Models;

namespace TestAuthBlz.Controllers.Auth;

[ApiController]
public class AuthController : ControllerBase
{
    private class AuthOptions
    {
        public const string ISSUER = "TestAuthBlz";
        public const string AUDIENCE = "TestAuthBlz";
        private const string KEY = "NxoHwDHUXsV/z4ShBh9qFNKWcpZzcFlXMithTjnL1rz+UcDktirUbQIFIjcmVMqDQLHwPfjlXyPEwO2Y/JuPSgroiShHjfrMC0Uq8Xi6zAH+ia0yqPBpPVGB9WDqQ5Y6JAy+L89yY2jLJ8pMibqdW3BbWJ/yGr+vU81vrGMEY3SAY46Sa8c7hvbvWpQZSNydtaKX110ILKOHfbIJRV+bzda01zmtBd+QFgPJQaPyMtfjbqU+CiZOuNYekCqeqtFLFUP+GVSrO0H/8YdDr9RuBdEiD+Ju2wS/SUXPhatFkCjqC3qpQCQfUw4v+51HB7EkqKbTDh8dZHS4nCxuBPEs8PiLKJ7jAoGXCaVhfTxXiV8=";

        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }

    [HttpPost]
    [Route("api/auth/sign_in")]
    public async Task<IActionResult> SignIn(SignInModel model)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, model.Email),
            new Claim(ClaimTypes.Name, model.Email),
            new Claim(ClaimTypes.Role, "Administrator")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var jwt = new JwtSecurityToken(
            issuer:AuthOptions.ISSUER,
            audience:AuthOptions.AUDIENCE,
            claims:claimsIdentity.Claims,
            signingCredentials:new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        
        return Ok(new SignInAuthModel{AuthToken = encodedJwt, Username = model.Email});
    }

    [HttpGet]
    [Route("api/auth/sign_in/context/{jwtToken}")]
    public async Task<IActionResult> SignIn(string jwtToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(jwtToken);

        if (token.Issuer != AuthOptions.ISSUER ||
            token.Audiences.FirstOrDefault(x => x == AuthOptions.AUDIENCE) == null)
        {
            return BadRequest("[KVAK] Token is not valid =(");
        }

        var identity = new ClaimsIdentity(token.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties());
        return Redirect("/");
    }

    [HttpGet]
    [Route("api/auth/sign_out")]
    public async Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}

