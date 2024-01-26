using System.Security.Claims;
using BlazorJWTAuth.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorJWTAuth.Auth;

public class AppAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly UserService _userService;

    public User CurrentUser { get; private set; } = new();

    public AppAuthenticationStateProvider(UserService userService)
    {
        _userService = userService;
        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
    }

    public async Task LoginAsync(string username, string password)
    {
        var principal = new ClaimsPrincipal();
        var user = await _userService.SendAuthenticateRequestAsync(username, password);

        if (user != null)
        {
            principal = user.ToClaimPrincipal();
            CurrentUser = user;
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public void Logout()
    {
        _userService.ClearBrowserUserData();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();
        var user = _userService.FetchUserFromBrowser();

        if (user == null)
            return new AuthenticationState(principal);

        var authenticatedUser = await _userService.SendAuthenticateRequestAsync(user.Username, user.Password);
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
        
        User.FromClaimsPrincipal(authenticationState.User);
    }

    public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;
}