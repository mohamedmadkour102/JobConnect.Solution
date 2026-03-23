using JobConnect.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobConnect.Infrastructure.Persistence;

public abstract class RepositoryBase
{
    protected readonly AppDbContext Context;

    protected RepositoryBase(AppDbContext context)
    {
        Context = context;
    }
}

public abstract class RepositoryBase<TEntity> : RepositoryBase, IRepository<TEntity>
    where TEntity : class
{
    protected RepositoryBase(AppDbContext context) : base(context) { }

    protected DbSet<TEntity> Set => Context.Set<TEntity>();

    public virtual IQueryable<TEntity> Query() => Set.AsQueryable();

    public virtual void Add(TEntity entity) => Set.Add(entity);

    public virtual void Update(TEntity entity) => Set.Update(entity);

    public virtual void Remove(TEntity entity) => Set.Remove(entity);
}
