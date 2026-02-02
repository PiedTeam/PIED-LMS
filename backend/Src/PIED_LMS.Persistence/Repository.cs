using System;
using System.Collections.Generic;
using System.Text;
using PIED_LMS.Domain.Abstractions;

namespace PIED_LMS.Persistence;


public class Repository<T>(PiedLmsDbContext dbContext) : IRepository<T> where T : class
{
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }
}
