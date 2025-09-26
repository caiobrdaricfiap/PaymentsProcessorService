using FiapCloudGames.Domain.Entities;
using PaymentsProcessorService.Domain.Models;
using System.Linq.Expressions;

namespace FiapCloudGameWebAPI.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetListByConditionAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetFirstOrDefaultByConditionAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetAsync(Guid id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
