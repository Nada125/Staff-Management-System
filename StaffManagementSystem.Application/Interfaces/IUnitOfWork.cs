using StaffManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T, TKey> Repository<T, TKey>() where T : class;
        int Complete();

    }
}
