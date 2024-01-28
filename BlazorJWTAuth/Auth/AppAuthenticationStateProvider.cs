using System.Security.Claims;
using BlazorJWTAuth.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorJWTAuth.Auth;

public class AppAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthUserService _authUserService;
    private readonly ProtectedLocalStorage _localStorage;

    public User CurrentUser { get; private set; } = new();

    public AppAuthenticationStateProvider(AuthUserService authUserService, ProtectedLocalStorage localStorage)
    {
        _authUserService = authUserService;
        _localStorage = localStorage;
        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
    }

    public async Task LoginAsync(string username, string password)
    {
        var principal = new ClaimsPrincipal();
        var user = await _authUserService.SendAuthenticateRequestAsync(username, password);

        if (user != null)
        {
            principal = user.ToClaimPrincipal();
            CurrentUser = user;
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public void Logout()
    {
        _authUserService.ClearBrowserUserData();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();
        var user = await _authUserService.FetchUserFromBrowser();

        if (user == null)
            return new AuthenticationState(principal);

        var authenticatedUser = await _authUserService.SendAuthenticateRequestAsync(user.Username, user.Password);
        if(authenticatedUser == null)
            return new AuthenticationState(principal);

        CurrentUser = authenticatedUser;
        
        principal = authenticatedUser.ToClaimPrincipal();
        return new AuthenticationState(principal);
    }

    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
    {
        var authenticationState = await task;
        
        if(authenticationState == null)
            return;
        
        CurrentUser = User.FromClaimsPrincipal(authenticationState.User);
    }

    public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;
}