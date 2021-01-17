using LeaveManagementWebApp.Contracts;
using LeaveManagementWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;
        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(LeaveRequest entity)
        {
            await _db.LeaveRequests.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveRequest>> FindAll()
        {
            return await _db.LeaveRequests
                .Include(request => request.RequestingEmployee)
                .Include(request => request.ApprovedBy)
                .Include(request => request.LeaveType)
                .ToListAsync();
        }

        public async Task<LeaveRequest> FindById(int id)
        {
            return await _db.LeaveRequests
                .Include(request => request.RequestingEmployee)
                .Include(request => request.ApprovedBy)
                .Include(request => request.LeaveType)
                .FirstOrDefaultAsync(request => request.Id ==id);
        }

        public async Task<bool> Exists(int id)
        {
            var exists = await _db.LeaveRequests.AnyAsync(history => history.Id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveRequest>> FindByEmployee(string employeeId)
        {
            return await _db.LeaveRequests
                .Where(request => request.RequestingEmployeeId == employeeId)
                .ToListAsync();
        }
    }
}
