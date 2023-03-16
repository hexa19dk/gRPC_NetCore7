using StackExchange.Redis;
using System.Text.Json;

namespace GrpcNet7.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase _database;

        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            _database = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _database.StringGet(key);
            if (!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value);

            return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _database.KeyExists(key);
            if (_exist)
                return _database.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expTime)
        {
            var et = expTime.DateTime.Subtract(DateTime.Now);
            return _database.StringSet(key, JsonSerializer.Serialize(value), et);
        }
    }
}
