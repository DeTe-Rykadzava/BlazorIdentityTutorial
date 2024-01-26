using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlazorJWTAuth.Models;

namespace BlazorJWTAuth.Auth;

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationDataMemoryStorage _authenticationDataMemoryStorage;

    private readonly IHttpContextAccessor _httpContextAccessor;
    
    
    public UserService(HttpClient httpClient, AuthenticationDataMemoryStorage authenticationDataMemoryStorage, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _authenticationDataMemoryStorage = authenticationDataMemoryStorage;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User?> SendAuthenticateRequestAsync(string username, string password)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var baseUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";
        
        var response = await _httpClient.GetAsync($"{baseUri}/example-data/{username}.json");

        if (!response.IsSuccessStatusCode)
            return null;

        var token = await response.Content.ReadAsStringAsync();
        var claimPrincipal = CreateClaimsPrincipalFromToken(token);
        var user = User.FromClaimsPrincipal(claimPrincipal);
        
        return new User()
        {
            Age = 20,
            Password = "asd",
            Username = "test@gmail.com",
            Roles = new List<string>(){"Admin"}
        };
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

    public User? FetchUserFromBrowser()
    {
        var claimsPrincipal = CreateClaimsPrincipalFromToken(_authenticationDataMemoryStorage.Token);
        var user = User.FromClaimsPrincipal(claimsPrincipal);

        return user;
    }

    public void ClearBrowserUserData() => _authenticationDataMemoryStorage.Token = string.Empty;
    
    private void PersistUserToBrowser(string token) => _authenticationDataMemoryStorage.Token = token;
}