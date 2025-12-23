using System.Linq.Expressions;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
    Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> UpdateRangeAsync(IEnumerable<T> entities);
    Task<T> DeleteAsync(T entity);
    AppDbContext GetDbContext();
}