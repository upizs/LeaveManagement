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

        public bool Create(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindAll()
        {
            return _db.LeaveRequests
                .Include(request => request.RequestingEmployee)
                .Include(request => request.ApprovedBy)
                .Include(request => request.LeaveType)
                .ToList();
        }

        public LeaveRequest FindById(int id)
        {
            return _db.LeaveRequests
                .Include(request => request.RequestingEmployee)
                .Include(request => request.ApprovedBy)
                .Include(request => request.LeaveType)
                .FirstOrDefault(request => request.Id ==id);
        }

        public bool Exists(int id)
        {
            var exists = _db.LeaveRequests.Any(history => history.Id == id);
            return exists;
        }

        public bool Save()
        {
            var changes = _db.SaveChanges();
            return changes > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindByEmployee(string employeeId)
        {
            return _db.LeaveRequests
                .Where(request => request.RequestingEmployeeId == employeeId)
                .ToList();
        }
    }
}
