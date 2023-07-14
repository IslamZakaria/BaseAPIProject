using System.Data;

namespace Presistence.Contexts
{
    public interface IApplicationDbContext
    {
        public IDbConnection Connection { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
