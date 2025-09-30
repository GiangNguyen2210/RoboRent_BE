using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        internal readonly DbSet<T> DbSet;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = dbContext.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync(T entity)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public AppDbContext GetDbContext()
        {
            return _dbContext;
        }
    }
}