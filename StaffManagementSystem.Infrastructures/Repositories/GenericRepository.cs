using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Infrastructures.DBContext;

namespace StaffManagementSystem.Infrastructures.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<T?> Get(TKey Id)
        {
            return await _dbSet.FindAsync(Id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<T> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Update(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> Patch(TKey id, Action<T> patchAction) 
        {
            var entity = await Get(id);
            if (entity == null) return null;

            patchAction(entity); 
            _context.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task Delete(TKey Id)
        {
            var entity = await Get(Id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
