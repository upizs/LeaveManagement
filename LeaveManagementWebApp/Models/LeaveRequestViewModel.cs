using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Models
{
    public class LeaveRequestViewModel
    {
        
        public int Id { get; set; }
        public EmployeeViewModel RequestingEmployee { get; set; }
        public string RequestingEmployeeId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public LeaveTypeViewModel LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime DateActioned { get; set; }
        public bool? Approved { get; set; }
        public EmployeeViewModel ApprovedBy { get; set; }
        public string ApprovedById { get; set; }
    }

    public class AdminLeaveRequestViewModel
    {
        [Display(Name ="Total Requests")]

        public int TotalRequests { get; set; }
        [Display(Name = "Approved Requests")]
        public int ApprovedRequests { get; set; }
        [Display(Name = "Pendign Requests")]
        public int PendingRequests { get; set; }
        [Display(Name = "Rejected Requests")]
        public int RejectedRequests { get; set; }

        public List<LeaveRequestViewModel> LeaveRequests { get; set; }

    }

    public class CreateLeaveRequestViewModel
    {
        //Changed Dates to Strings because there was an issue with DatePicker
        //Will Convert it in Controller
        [Display(Name ="Start Date")]
        [Required]
        public string StartDate { get; set; }
        [Display(Name = "End Date")]
        [Required]
        public string EndDate { get; set; }
        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
        public int LeaveTypeId { get; set; }
    }
}
