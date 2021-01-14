using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Models
{
    public class MyLeaveViewModel
    {
        public List<LeaveAllocationViewModel> LeaveAllocations { get; set; }
        public List<LeaveRequestViewModel> LeaveRequests { get; set; }
    }
}
