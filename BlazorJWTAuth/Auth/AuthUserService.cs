using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlazorJWTAuth.Models;

namespace BlazorJWTAuth.Auth;

public class AuthUserService(
    HttpClient httpClient,
    LocalStorageService localStorageService)
{

    public async Task<User?> SendAuthenticateRequestAsync(string username, string password)
    {
        var user = User.Users.FirstOrDefault(x => x.Username == username && x.Password == password);
        if (user == null)
            return null;
        var claimPrincipal = user.ToClaimPrincipal();
        var jwtToken = new JwtSecurityToken(
            issuer: "BlazorJWTAuth",
            audience: "BlazorJWTAuth",
            claims: claimPrincipal.Claims);
        PersistUserToBrowser(new JwtSecurityTokenHandler().WriteToken(jwtToken));
        return user;
    }

    private ClaimsPrincipal CreateClaimsPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var identity = new ClaimsIdentity();

        if (!tokenHandler.CanReadToken(token))
            return new ClaimsPrincipal(identity);
        
        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
        identity = new ClaimsIdentity(jwtSecurityToken.Claims, "BlazorJWTAuth");

        return new ClaimsPrincipal(identity);
    }

    public async Task<User?> FetchUserFromBrowser()
    {
        var token = await localStorageService.GetFromStorage<string>("AuthToken");
        if (string.IsNullOrWhiteSpace(token))
            return null;
        
        var claimsPrincipal = CreateClaimsPrincipalFromToken(token);
        var user = User.FromClaimsPrincipal(claimsPrincipal);

        return user;
    }

    public async void ClearBrowserUserData() => await localStorageService.DeleteFromStorage("AuthToken");
    
    private async void PersistUserToBrowser(string token) => await localStorageService.SetInStorage("AuthToken",token);
}