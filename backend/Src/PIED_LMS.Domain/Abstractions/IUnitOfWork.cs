namespace PIED_LMS.Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
