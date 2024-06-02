namespace Challengify.Services;

public interface ICacheService {
    public Task<T?> GetObjectAsync<T>(string key);
    public Task SetObjectAsync<T>(string key, T value, TimeSpan expirationTime);
    public Task RemoveAsync(string key);
}