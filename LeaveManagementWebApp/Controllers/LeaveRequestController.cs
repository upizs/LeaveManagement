using AutoMapper;
using LeaveManagementWebApp.Contracts;
using LeaveManagementWebApp.Data;
using LeaveManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        #region dependencies

        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _allocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        //Dependendy injuction
        public LeaveRequestController(
            ILeaveRequestRepository leaveRequestRepo,
            ILeaveTypeRepository leaveTypeRepo,
            ILeaveAllocationRepository allocationRepo,
            IMapper mapper,
            UserManager<Employee> userManager)
        {
            _leaveRequestRepo = leaveRequestRepo;
            _leaveTypeRepo = leaveTypeRepo;
            _allocationRepo = allocationRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        #endregion

        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController
        public ActionResult Index()
        {
            var leaveRequests = _leaveRequestRepo.FindAll();
            var leaveRequestsModel = _mapper.Map<List<LeaveRequestViewModel>>(leaveRequests);
            var model = new AdminLeaveRequestViewModel
            {
                TotalRequests = leaveRequestsModel.Count,
                ApprovedRequests = leaveRequestsModel.Count(request => request.Approved == true),
                PendingRequests = leaveRequestsModel.Count(request => request.Approved == null),
                RejectedRequests = leaveRequestsModel.Count(request => request.Approved == false),
                LeaveRequests = leaveRequestsModel

            };
            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LeaveRequestController/Create
        public ActionResult Create()
        {
            var leaveTypes = _leaveTypeRepo.FindAll();
            var leaveTypesItems = leaveTypes.Select(type => new SelectListItem { 
                Text = type.Name,
                Value = type.Id.ToString()
            });

            var model = new CreateLeaveRequestViewModel
            {
                LeaveTypes = leaveTypesItems
            };

            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestViewModel model)
        {
            

            try
            {
                //has to create the SelectListItem, 
                //becasue otherwise the model would 
                //return empty list in case of fail

                var leaveTypes = _leaveTypeRepo.FindAll();
                var leaveTypesItems = leaveTypes.Select(type => new SelectListItem
                {
                    Text = type.Name,
                    Value = type.Id.ToString()
                });

                model.LeaveTypes = leaveTypesItems;

                //Check if valid
                #region Validation

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (DateTime.Compare(model.StartDate, model.EndDate) > 1)
                {
                    ModelState.AddModelError("", "Start Date cannot be further in the future than the End Date");
                    return View(model);
                }

                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _allocationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypeId);
                int daysRequested = (int)(model.EndDate.Date - model.StartDate.Date).TotalDays;

                if (daysRequested > allocation.NumberOfDays) 
                {
                    ModelState.AddModelError("", "You have requested more days for this allocation than you have. "
                        + "You have " + allocation.NumberOfDays.ToString() + " days left");
                    return View(model);
                }

                #endregion

                //Create a model that is properly mappable
                var leaveRequestModel = new LeaveRequestViewModel
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now
                    
                };
                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var success = _leaveRequestRepo.Create(leaveRequest);

                if (!success)
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return View(model);
                }
                //Redirects to home index, because leaverequest index is admin only
                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
