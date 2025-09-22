

namespace StaffManagementSystem.Application.Interfaces
{
    public interface IGenericRepository<T, TKey> where T : class
    {
        Task<T?> Get(TKey Id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T?> Patch(TKey Id, Action<T> patchAction);
        Task Delete(TKey Id);
    }
}
