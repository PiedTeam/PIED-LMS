
using PIED_LMS.Domain.Abstractions;

namespace PIED_LMS.Persistence;

public class EFUnitOfWork(PiedLmsDbContext dbContext) : IUnitOfWork
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Type, object> _repositories = new();

    public void Dispose() => dbContext.Dispose();

    public async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);

    public IRepository<T> Repository<T>() where T : class
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(dbContext));
    }
}
