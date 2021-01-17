using LeaveManagementWebApp.Contracts;
using LeaveManagementWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;
        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(LeaveAllocation entity)
        {
            await _db.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            return await _db.LeaveAllocations
                .Include(allocation => allocation.LeaveType)
                .Include(allocation => allocation.Employee)
                .ToListAsync();
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            return await _db.LeaveAllocations
                .Include(allocation => allocation.LeaveType)
                .Include(allocation => allocation.Employee)
                .FirstOrDefaultAsync(allocation => allocation.Id == id);
        }

        public async Task<bool> Exists(int id)
        {
            var exists = await _db.LeaveAllocations.AnyAsync(allocation => allocation.Id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
        //using FindAll() method instead of direct _db becase FindAll has .Include function
        public async Task<bool> CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();

            return allocations
                .Where(allocation => allocation.EmployeeId == employeeId && allocation.LeaveTypeId == leaveTypeId 
                && allocation.Period == period)
                .Any();
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string employeeId)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
            return allocations
                .Where(allocation => allocation.EmployeeId == employeeId && allocation.Period == period)
                .ToList();

        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string employeeId, int leaveTypeId)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
            return allocations
                .FirstOrDefault(allocation => allocation.EmployeeId == employeeId
                && allocation.Period == period
                && allocation.LeaveTypeId == leaveTypeId);
        }
    }
}
