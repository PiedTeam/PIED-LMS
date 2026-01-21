using PIED_LMS.Domain.Abstractions;

namespace PIED_LMS.Persistence;

public class EFUnitOfWork(PiedLmsDbContext dbContext) : IUnitOfWork
{
    public void Dispose() => dbContext.Dispose();

    public async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
