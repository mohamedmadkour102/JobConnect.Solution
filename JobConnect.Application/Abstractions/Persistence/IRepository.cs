namespace JobConnect.Application.Abstractions.Persistence;

/// <summary>Generic read/write repository abstraction (EF implementation in Infrastructure).</summary>
public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
