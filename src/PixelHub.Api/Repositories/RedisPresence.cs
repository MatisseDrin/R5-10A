using StackExchange.Redis;

public class RedisPresence
{
    private const string Key = "joueurs:en-ligne";
    private const int TimeoutSeconds = 60;

    private readonly IDatabase _redis;

    public RedisPresence(IConnectionMultiplexer connection)
    {
        _redis = connection.GetDatabase();
    }

    public Task TouchAsync(string pseudo)
    {
        var expiration = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return _redis.SortedSetAddAsync(Key, pseudo, expiration);
    }

    public async Task<long> CountOnlineAsync()
    {
        var limite = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - TimeoutSeconds;

        await _redis.SortedSetRemoveRangeByScoreAsync(Key, double.NegativeInfinity, limite);

        return await _redis.SortedSetLengthAsync(Key);
    }
}