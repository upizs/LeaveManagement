using LeaveManagementWebApp.Contracts;
using LeaveManagementWebApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Repository
{
    //Inherits from LeaveType interface instead of Base interface bacause LeaveType 
    //specific actions can be added in LeaveType interface.
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public LeaveTypeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(LeaveType entity)
        {
            await _db.LeaveTypes.AddAsync(entity);
            return await Save();

        }

        public async Task<bool> Delete(LeaveType entity)
        {
            _db.LeaveTypes.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveType>> FindAll()
        {
           return await _db.LeaveTypes.ToListAsync();
        }

        public async Task<LeaveType> FindById(int id)
        {
            return await _db.LeaveTypes.FindAsync(id);
            
        }


        public async Task<bool> Exists(int id)
        {
            var exists = await _db.LeaveTypes.AnyAsync(type => type.Id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            //if SaveChanges returns less than one change means something has gone
            //wrong and no changes where saved.
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveType entity)
        {
            _db.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
