using LeaveManagementWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Validations
{
    public class StartDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var todaysDate = DateTime.Now.Date;
            var startDateRequested = DateTime.ParseExact(value.ToString(), "MM/dd/yyyy", null);
            if (startDateRequested <= todaysDate)
            {
                return new ValidationResult("Start Date cannot be in past or today");
            }
            return ValidationResult.Success;
        }
    }
    public class EndDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var leaveRequest = (CreateLeaveRequestViewModel)validationContext.ObjectInstance;
            var startDateRequested = DateTime.ParseExact(leaveRequest.StartDate, "MM/dd/yyyy", null);
            var endDateRequested = DateTime.ParseExact(value.ToString(), "MM/dd/yyyy", null);
            if (startDateRequested >= endDateRequested)
            {
                return new ValidationResult("End Date cannot be sooned or same as Start Date");
            }
            return ValidationResult.Success;
        }
    }
}
