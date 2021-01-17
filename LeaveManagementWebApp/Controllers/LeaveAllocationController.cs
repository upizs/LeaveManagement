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
        public readonly ILeaveTypeRepository _typeRepo;
        private readonly ILeaveAllocationRepository _allocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        //Dependendy injuction
        public LeaveAllocationController(
            ILeaveTypeRepository typeRepo,
            ILeaveAllocationRepository allocationRepo, 
            IMapper mapper,
            UserManager<Employee> userManager)
        {
            _typeRepo = typeRepo;
            _allocationRepo = allocationRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            var leaveTypes = await _typeRepo.FindAll();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes.ToList());
            var model = new CreateLeaveAllocationViewModel
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);
        }

        public async Task<ActionResult> SetLeave(int id)
        {
            var leaveType = await _typeRepo.FindById(id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                //To avoid duplicates do not create a new allocation if employee
                //already has allocation
                var exist = await _allocationRepo.CheckAllocation(id, emp.Id);
                if (exist)
                    continue;
                
                var allocation = new LeaveAllocationViewModel
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leaveType.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                await _allocationRepo.Create(leaveAllocation);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> ListEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeViewModel>>(employees);

            return View(model);
        }

        public async Task<ActionResult> Details(string id)
        {
            var employee = await _userManager.FindByIdAsync(id);
            var mappedEmployee = _mapper.Map<EmployeeViewModel>(employee);
            var allocations = await _allocationRepo.GetLeaveAllocationsByEmployee(id);
            var mappedAllocations = _mapper.Map<List<LeaveAllocationViewModel>>(allocations);

            var model = new ViewAllocationsViewModel
            {
                Employee = mappedEmployee,
                LeaveAllocations = mappedAllocations,

            };

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var leaveAllocation = await _allocationRepo.FindById(id);
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
                var leaveAllocation = await _allocationRepo.FindById(model.Id);
                leaveAllocation.NumberOfDays = model.NumberOfDays;

                var isSuccess = await _allocationRepo.Update(leaveAllocation);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Error While Saving");
                    return View(model);
                }

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
