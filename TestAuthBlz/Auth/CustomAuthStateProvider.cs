using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using TestAuthBlz.Models;

namespace TestAuthBlz.Auth;

public class CustomAuthStateProvider(
    ILogger<CustomAuthStateProvider> logger,
    HttpClient httpClient,
    IHttpContextAccessor contextAccessor)
    : AuthenticationStateProvider
{
    private readonly HttpClient? _client = httpClient;
    private readonly HttpContext? _context = contextAccessor.HttpContext;  
    
    public async Task<LoginResultModel> LoginAsync(SignInModel model)
    {
        var requestContent = new StringContent(JsonConvert.SerializeObject(model));
        
        requestContent.Headers.Clear();
        requestContent.Headers.Add("Content-Type", "application/json");
        
        try
        {
            var result = await _client.PostAsync(new Uri("api/auth/sign_in", UriKind.Relative),requestContent);
            if (!result.IsSuccessStatusCode)
            {
                return new LoginResultModel{Message = "Error login or password", IsSuccess = false};
            }

            var authModel = JsonConvert.DeserializeObject<SignInAuthModel>(await result.Content.ReadAsStringAsync());
            
            if(authModel == null)
                return new LoginResultModel{Message = "Error with authentication", IsSuccess = false};

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(SignInModel.ToClaimsPrincipal(model.Email))));
            return new LoginResultModel{Message = "Success", Token = authModel.AuthToken, IsSuccess = true};
        }
        catch (Exception e)
        {
            return new LoginResultModel{Message = "Error while send auth request", IsSuccess = false};
        }   
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (_context == null)
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
            if (_context.User.Identity?.IsAuthenticated == false)
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }

            return new AuthenticationState(_context.User);
        }
        catch (Exception e)
        {
            logger.LogError($"{e.Message}\n{e.InnerException}");
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }
}