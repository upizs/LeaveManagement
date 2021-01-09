using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Data
{
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        //How many days available for one person for one leaveType in year
        public int DefaultDays { get; set; }

        public DateTime DateCreated { get; set; }

    }
}
