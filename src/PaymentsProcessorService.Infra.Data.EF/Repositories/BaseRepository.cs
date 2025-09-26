using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGameWebAPI.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using PaymentsProcessorService.Domain.Models;
using System.Linq.Expressions;

namespace FiapCloudGameWebAPI.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetListByConditionAsync(Expression<Func<T, bool>> predicate) => 
            await _db.AsNoTracking().Where(predicate).ToListAsync();

        public async Task<T?> GetFirstOrDefaultByConditionAsync(Expression<Func<T, bool>> predicate) =>
            await _db.AsNoTracking().Where(predicate).FirstOrDefaultAsync();

        public async Task<T?> GetAsync(Guid id) =>
            await _db.FindAsync(id);

        public async Task<T> AddAsync(T entity)
        {
            _db.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var result = await _db.FindAsync(entity.Id);
            if (result is null)
                return null;

            _context.Entry(result)
                .CurrentValues
                .SetValues(entity);

            await _context.SaveChangesAsync();
            return entity;

        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _db.FindAsync(id);
            if (result is null)
                return false;

            _db.Remove(result);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
