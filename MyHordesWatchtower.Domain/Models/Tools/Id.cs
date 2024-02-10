namespace MyHordesWatchtower.Domain.Models.Tools
{
    public record Id<TId, TEntity>(TId Value) : IEquatable<Id<TId, TEntity>> where TId : IEquatable<TId> where TEntity : class;

    public record Id<TEntity> : Id<Guid, TEntity> where TEntity : class
    {
        public Id(Guid value) : base(value)
        {
        }
    }
}
