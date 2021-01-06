using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        //finds all records. Collection is any kind of Array
        ICollection<T> FindAll();
        T FindById(int id);

        bool Exists(int id);

        //returns bool to let me know whether the action was succesful or not
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool Save();
    }
}
