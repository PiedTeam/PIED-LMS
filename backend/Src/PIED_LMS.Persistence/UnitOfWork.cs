using System.Collections;
using PIED_LMS.Domain.Abstractions;

namespace PIED_LMS.Persistence;

public class EFUnitOfWork(PiedLmsDbContext dbContext) : IUnitOfWork
{
    private readonly Hashtable _repositories = new();

    public void Dispose() => dbContext.Dispose();

    public async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T).Name;
        if (!_repositories.ContainsKey(type))
        {
            var repositoryInstance = new Repository<T>(dbContext);
            _repositories.Add(type, repositoryInstance);
        }
        return (IRepository<T>)_repositories[type];
    }
}
