using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using static Dapper.SqlMapper;
using Presistence.Contexts;
using Core.Interfaces.Base;
using Core.Entities.BaseEntities;

namespace Presistence.Repositories.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
    {
        private readonly ApplicationDbContext _context;
        public DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = context.Set<T>();
        }

        #region StoreProcedure
        public async Task<int> ExecuteAsync(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {

            return await _context.Connection.ExecuteAsync(sql, param, transaction, commandType: commandType);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return await _context.Connection.ExecuteScalarAsync<T>(sql, param, transaction, commandType: commandType);
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return (await _context.Connection.QueryAsync<T>(sql, param, transaction, commandType: commandType)).AsList();
        }
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return await _context.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);
        }
        public async Task<T> QuerySingleAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return await _context.Connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);
        }
        public async Task<Tuple<IList<T1>, IList<T2>>> QueryMultipleAsync<T1, T2>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            var data = await _context.Connection.QueryMultipleAsync(sql, param, transaction, commandType: commandType);
            return new Tuple<IList<T1>, IList<T2>>(data.Read<T1>().AsList(), data.Read<T2>().AsList());
        }

        #endregion

        #region Get
        public async Task<TResult?> GetByIdAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                          long Id,
                                                          bool disableTracking = true,
                                                          Expression<Func<T, bool>> filter = null,
                                                          params Expression<Func<T, object>>[] includes)
        {
            var entity = dbSet.Where(e => !e.IsDeleted &&
                                           e.Id == Id);

            if (disableTracking)
            {
                entity = entity.AsNoTracking();
            }

            if (filter is not null)
            {
                entity = entity.Where(filter);
            }

            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                {
                    entity = entity.Include(include);
                }
            }

            return await entity.Select(selector)
                               .FirstOrDefaultAsync();
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> filter = null)
        {
            var entity = dbSet.Where(e => !e.IsDeleted);

            if (filter is not null)
            {
                return await entity.AnyAsync(filter);
            }

            return await entity.AnyAsync();
        }

        public async Task<TResult?> GetPropertyWithSelectorAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                                          bool disableTracking = true,
                                                                          Expression<Func<T, bool>> filter = null,
                                                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                                          params Expression<Func<T, object>>[] includes)
        {
            var entity = dbSet.Where(r => !r.IsDeleted);

            if (disableTracking)
            {
                entity = entity.AsNoTracking();
            }

            if (filter is not null)
            {
                entity = entity.Where(filter);
            }

            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                {
                    entity = entity.Include(include);
                }
            }

            if (orderBy is not null)
            {
                entity = orderBy(entity);
            }

            return await entity.Select(selector)
                               .FirstOrDefaultAsync();
        }

        public async Task<IList<TResult>> GetPagedWithSelectorAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                                             int pageIndex,
                                                                             int pageCount,
                                                                             bool disableTracking = true,
                                                                             Expression<Func<T, bool>> filter = null,
                                                                             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                                             params Expression<Func<T, object>>[] includes)
        {
            var entity = dbSet.Where(e => !e.IsDeleted);

            if (disableTracking)
            {
                entity = entity.AsNoTracking();
            }

            if (filter is not null)
            {
                entity = entity.Where(filter);
            }

            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                {
                    entity = entity.Include(include);
                }
            }

            if (orderBy is not null)
            {
                return await orderBy(entity).Skip((pageIndex - 1) * pageCount)
                                            .Take(pageCount)
                                            .Select(selector)
                                            .ToListAsync();
            }
            return await entity.Skip((pageIndex - 1) * pageCount)
                               .Take(pageCount)
                               .Select(selector)
                               .ToListAsync();
        }

        public async Task<IList<TResult>> GetAllWithSelectorAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                                           bool disableTracking = true,
                                                                           Expression<Func<T, bool>> filter = null,
                                                                           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                                           params Expression<Func<T, object>>[] includes)
        {
            var entity = dbSet.Where(r => !r.IsDeleted);

            if (disableTracking)
            {
                entity = entity.AsNoTracking();
            }

            if (filter is not null)
            {
                entity = entity.Where(filter);
            }

            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                {
                    entity = entity.Include(include);
                }
            }

            if (orderBy is not null)
            {
                entity = orderBy(entity);
            }

            return await entity.Select(selector)
                               .ToListAsync();
        }

        public async Task<IQueryable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                                    bool disableTracking = true,
                                                                    Expression<Func<T, bool>> filter = null,
                                                                    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                                    params Expression<Func<T, object>>[] includes)
        {
            var entity = dbSet.Where(r => !r.IsDeleted);

            if (disableTracking)
            {
                entity = entity.AsNoTracking();
            }

            if (filter is not null)
            {
                entity = entity.Where(filter);
            }

            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                {
                    entity = entity.Include(include);
                }
            }

            if (orderBy is not null)
            {
                return orderBy(entity).Select(selector);
            }

            return entity.Select(selector);
        }

        public Task<int> GetCountAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter is not null)
            {
                return dbSet.CountAsync(filter);
            }
            return dbSet.CountAsync(f => !f.IsDeleted);
        }
        #endregion

        #region Insert
        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entity;
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            try
            {
                await _context.Set<T>().AddRangeAsync(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entities;
        }

        #endregion

        #region Update
        public async Task<int> UpdateRangeAsync(List<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            return await _context.SaveChangesAsync();

        }
        public async Task<int> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }
        public async Task BulkUpdateAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Delete
        public async Task<int> DeleteRangeAsync(List<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }
        public async Task BulkDeleteAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}

