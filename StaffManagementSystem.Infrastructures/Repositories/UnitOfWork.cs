using StaffManagementSystem.Application.Interfaces;
using StaffManagementSystem.Domain.Entities;
using StaffManagementSystem.Infrastructures.DBContext;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Infrastructures.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _appDbContext;

        Hashtable repositories;
        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            repositories = new Hashtable();
        }
        public IGenericRepository<T, TKey> Repository<T, TKey>() where T : class
        {
            var key = typeof(T).Name;

            if (!repositories.ContainsKey(key))
            {
                repositories.Add(key, new GenericRepository<T, TKey>(_appDbContext));
            }

            return repositories[key] as IGenericRepository<T, TKey>;

        }


        public int Complete()
        {
            return _appDbContext.SaveChanges();
        }


        public void Dispose()
        {
            _appDbContext.Dispose();
        }

    }
}
