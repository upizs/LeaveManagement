using AutoMapper;
using LeaveManagementWebApp.Contracts;
using LeaveManagementWebApp.Data;
using LeaveManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        //Dependendy injuction
        public LeaveAllocationController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<Employee> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            var leaveTypes = await _unitOfWork.LeaveTypes.FindAll();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes.ToList());
            var model = new CreateLeaveAllocationViewModel
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);
        }

        public async Task<ActionResult> SetLeave(int leaveTypeId)
        {
            var leaveType = await _unitOfWork.LeaveTypes.Find(type => type.Id == leaveTypeId);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var period = DateTime.Now.Year;
            foreach (var emp in employees)
            {
                //To avoid duplicates do not create a new allocation if employee
                //already has allocation
                var exist = await _unitOfWork.LeaveAllocations.Exists(allocation => allocation.LeaveTypeId == leaveTypeId 
                                                                                && allocation.EmployeeId == emp.Id
                                                                                && allocation.Period == period);
                if (!exist)
                    continue;

                var allocation = new LeaveAllocationViewModel
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = leaveTypeId,
                    NumberOfDays = leaveType.DefaultDays,
                    Period = period
                };

                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                await _unitOfWork.LeaveAllocations.Create(leaveAllocation);
                await _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> ListEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeViewModel>>(employees);

            return View(model);
        }

        public async Task<ActionResult> Details(string employeeId)
        {
            var employee = await _userManager.FindByIdAsync(employeeId);
            var mappedEmployee = _mapper.Map<EmployeeViewModel>(employee);
            var allocations = await _unitOfWork.LeaveAllocations.FindAll(allocation => allocation.EmployeeId == employeeId, 
                                                                         includes: new List<string> { "LeaveType" });
            var mappedAllocations = _mapper.Map<List<LeaveAllocationViewModel>>(allocations);

            var model = new ViewAllocationsViewModel
            {
                Employee = mappedEmployee,
                LeaveAllocations = mappedAllocations,

            };

            return View(model);
        }

        public async Task<ActionResult> Edit(int allocationId)
        {
            var leaveAllocation = await _unitOfWork.LeaveAllocations.Find(allocation => allocation.Id == allocationId,
                                                                            includes: new List<string> { "Employee", "LeaveType" });
            var model = _mapper.Map<EditLeaveAllocationViewModel>(leaveAllocation);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //had to use this method, because mapping was causing an error
                var leaveAllocation = await _unitOfWork.LeaveAllocations.Find(allocation => allocation.Id == model.Id);
                leaveAllocation.NumberOfDays = model.NumberOfDays;

                _unitOfWork.LeaveAllocations.Update(leaveAllocation);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Details), new {id = model.EmployeeId });

            }
            catch
            {
                ModelState.AddModelError("", "Error While Saving");
                return View(model);
            }
        }

    }
}
