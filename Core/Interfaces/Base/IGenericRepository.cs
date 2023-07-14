using Core.Entities.BaseEntities;
using System.Data;
using System.Linq.Expressions;


namespace Core.Interfaces.Base
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        #region Get

        /// <summary>
        /// Get the entity with given Id
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="Id">The Id of the entity to locate.</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="includes">A function to include navigation properties.</param>
        /// <returns>Entity matching the specified Id, or null if no matching entity was found.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<TResult?> GetByIdAsync<TResult>(Expression<Func<T, TResult>> selector,
                                             long Id,
                                             bool disableTracking = true,
                                             Expression<Func<T, bool>> filter = null,
                                             params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Check if the Filtered entity exists or not
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <returns>true if exists, false if not</returns>
        Task<bool> Exists(Expression<Func<T, bool>> filter = null);

        Task<TResult?> GetPropertyWithSelectorAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                             bool disableTracking = true,
                                                             Expression<Func<T, bool>> filter = null,
                                                             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                             params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Get The full list of items you would like to paginate
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="includes">A function to include navigation properties.</param>
        /// <returns>The full list of items, or null if no matching entity was found.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<IList<TResult>> GetPagedWithSelectorAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                                int pageNumber,
                                                                int pageSize,
                                                                bool disableTracking = true,
                                                                Expression<Func<T, bool>> filter = null,
                                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                                params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Get The full list of items
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="includes">A function to include navigation properties.</param> 
        /// <returns>The full list of items, or null if entity was found.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<IList<TResult>> GetAllWithSelectorAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                              bool disableTracking = true,
                                                              Expression<Func<T, bool>> filter = null,
                                                              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                              params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Get The full list of items
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="includes">A function to include navigation properties.</param> 
        /// <returns>The full list of items, or null if entity was found.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<IQueryable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                       bool disableTracking = true,
                                                       Expression<Func<T, bool>> filter = null,
                                                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                       params Expression<Func<T, object>>[] includes);

        Task<int> GetCountAsync(Expression<Func<T, bool>> filter = null);
 
        #endregion

        #region StoreProcedure
        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<T> QuerySingleAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Multiple Result From a Query
        /// </summary>
        /// <typeparam name="T1">The Desired Mapped Object For The First Called Query</typeparam>
        /// <typeparam name="T2">The Desired Mapped Object For The Second Called Query</typeparam>
        /// <param name="sql">Sql Query To be Called</param>
        /// <param name="param">Parameters To be Passed</param>
        /// <param name="commandType">Type of Command</param>
        /// <param name="transaction">Transaction To Use</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Tuple of Tuple<IList<T1>, IList<T2>></returns>
        Task<Tuple<IList<T1>, IList<T2>>> QueryMultipleAsync<T1, T2>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default);

        Task<int> ExecuteAsync(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default);
        #endregion

        #region Add
        Task<T> AddAsync(T entity);
        Task<List<T>> AddRangeAsync(List<T> entities);
        #endregion

        #region Update
        Task<int> UpdateAsync(T entity);
        Task<int> UpdateRangeAsync(List<T> entities);
        Task BulkUpdateAsync(IEnumerable<T> entities);
        #endregion

        #region Delete
        Task<int> DeleteAsync(T entity);
        Task<int> DeleteRangeAsync(List<T> entities);
        Task BulkDeleteAsync(IEnumerable<T> entities);
        #endregion
    }
}
