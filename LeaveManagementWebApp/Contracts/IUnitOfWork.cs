using LeaveManagementWebApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<LeaveType> LeaveTypes { get;  }
        IGenericRepository<LeaveRequest> LeaveReuqests { get;  }
        IGenericRepository<LeaveAllocation> LeaveAllocations { get; }

        Task Save();
    }
}
