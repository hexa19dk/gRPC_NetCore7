namespace GrpcNet7.Services
{
    public interface ICacheService
    {
        T GetData <T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expTime);
        object RemoveData(string key);
    }
}
