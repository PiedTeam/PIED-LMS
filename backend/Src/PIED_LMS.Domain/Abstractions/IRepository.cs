using System;
using System.Collections.Generic;
using System.Text;

namespace PIED_LMS.Domain.Abstractions;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
