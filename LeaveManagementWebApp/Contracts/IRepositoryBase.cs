using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public interface IGenericRepository<T> where T : class
    {
        //Finds all and allows to filter straight away using lambda,
        //also allows to order by 'q=>q.OrderBy(q=>q.Id)'
        Task<IList<T>> FindAll(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderBy = null,
            List<string> includes = null
            );
        //Finds one record that matches any input
        Task<T> Find(
            Expression<Func<T, bool>> expression , 
            List<string> includes = null);
        //Checks if specific record that matched any input exists
        Task<bool> Exists(Expression<Func<T, bool>> expression);

        //returns bool to let me know whether the action was succesful or not
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
