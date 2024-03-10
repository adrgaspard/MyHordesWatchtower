using Microsoft.Extensions.Configuration;
using MyHordesWatchtower.Application;
using StackExchange.Redis;
using System.Text.Json;

namespace MyHordesWatchtower.Infrastructure.RedisClient
{
    public class RedisClient : IPubSub
    {
        private readonly ISubscriber _subscriber;

        public RedisClient(IConfiguration configuration)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configuration.GetValue<string>("RedisClient:Host") ?? "");
            _subscriber = redis.GetSubscriber();
        }

        public async Task Publish<TContent>(string channel, TContent content)
        {
            string value = JsonSerializer.Serialize(content);
            _ = await _subscriber.PublishAsync(RedisChannel.Literal(channel), value);
        }

        public async Task Subscribe<TContent>(string channel, Action<TContent> handler)
        {
            await _subscriber.SubscribeAsync(RedisChannel.Literal(channel), async (channel, value) =>
            {
                if (value.HasValue && value.ToString() is string stringValue)
                {
                    TContent? content = JsonSerializer.Deserialize<TContent>(stringValue);
                    if (content is not null)
                    {
                        await Task.Run(() => handler(content));
                    }
                }
            });
        }

        public async Task Unsubscribe<TContent>(string channel)
        {
            await _subscriber.UnsubscribeAsync(RedisChannel.Literal(channel));
        }
    }
}
