namespace MyHordesWatchtower.Application
{
    public interface IPubSub
    {
        Task Publish<TContent>(string channel, TContent content);

        Task Subscribe<TContent>(string channel, Action<TContent> handler);

        Task Unsubscribe<TContent>(string channel);
    }
}
