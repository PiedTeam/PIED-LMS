namespace PIED_LMS.Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);

    IRepository<T> Repository<T>() where T : class;

    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);

}
