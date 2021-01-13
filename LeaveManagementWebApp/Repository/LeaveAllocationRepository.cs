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
        public bool Create(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            return _db.LeaveAllocations
                .Include(allocation => allocation.LeaveType)
                .Include(allocation => allocation.Employee)
                .ToList();
        }

        public LeaveAllocation FindById(int id)
        {
            return _db.LeaveAllocations
                .Include(allocation => allocation.LeaveType)
                .Include(allocation => allocation.Employee)
                .FirstOrDefault(allocation => allocation.Id == id);
        }

        public bool Exists(int id)
        {
            var exists = _db.LeaveAllocations.Any(allocation => allocation.Id == id);
            return exists;
        }

        public bool Save()
        {
            var changes = _db.SaveChanges();
            return changes > 0;
        }

        public bool Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return Save();
        }
        //using FindAll() method instead of direct _db becase FindAll has .Include function
        public bool CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;

            return FindAll()
                .Where(allocation => allocation.EmployeeId == employeeId && allocation.LeaveTypeId == leaveTypeId && allocation.Period == period)
                .Any();
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .Where(allocation => allocation.EmployeeId == id && allocation.Period == period)
                .ToList();

        }

        public LeaveAllocation GetLeaveAllocationsByEmployeeAndType(string id, int leaveTypeId)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .FirstOrDefault(allocation => allocation.EmployeeId == id
                        && allocation.Period == period
                        && allocation.LeaveTypeId == leaveTypeId);
        }
    }
}
