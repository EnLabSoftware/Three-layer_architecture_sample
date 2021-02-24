using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ThreeLayerSample.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext DbContext { get; }

        /// <summary>
        /// Get repository instance of an entity inside UnitOfWork scope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRepository<T> Repository<T>() where T : class;
        /// <summary>
        /// Saves changes to database, previously opening a transaction
        /// only when none exists. The transaction is opened with isolation
        /// level set in Unit of Work before calling this method.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransaction();

        /// <summary>
        /// Commits the current transaction (does nothing when none exists).
        /// </summary>
        Task CommitTransaction();

        /// <summary>
        /// Rolls back the current transaction (does nothing when none exists).
        /// </summary>
        Task RollbackTransaction();

    }
}
