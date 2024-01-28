using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorJWTAuth.Auth;

public class LocalStorageService(ProtectedLocalStorage localStorage, ILogger<LocalStorageService> logger)
{
    private readonly ILogger<LocalStorageService> _logger = logger;
    private readonly ProtectedLocalStorage _localStorage = localStorage;

    public async Task SetInStorage(string key, object token)
    {
        try
        {
            await _localStorage.SetAsync(key, token);
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot set item into local storage: \t" + e.Message + "\n" + e.InnerException);
        }
    }
    
    public async Task<T?> GetFromStorage<T>(string key)
    {
        try
        {
            var result = await _localStorage.GetAsync<T>(key);
            return result.Value;
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot get item from local storage: \t" + e.Message + "\n" + e.InnerException);
            return default(T);
        }
    }

    public async Task DeleteFromStorage(string key)
    {
        try
        {
            await _localStorage.DeleteAsync(key);
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot delete item from local storage: \t" + e.Message + "\n" + e.InnerException);
        }
    }

}