using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        //finds all records. Collection is any kind of Array
        Task<ICollection<T>> FindAll();
        Task<T> FindById(int id);

        Task<bool> Exists(int id);

        //returns bool to let me know whether the action was succesful or not
        Task<bool> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save();
    }
}
