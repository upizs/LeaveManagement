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

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        //Dependendy injuction
        public LeaveRequestController(
            IUnitOfWork unitOfWork ,
            IMapper mapper,
            UserManager<Employee> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        #endregion

        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController
        public async Task<ActionResult> Index()
        {
            var leaveRequests = await _unitOfWork.LeaveReuqests.FindAll(); ;
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

        public async  Task<ActionResult> MyLeave()
        {
            //Find and get all resources
            var user = await _userManager.GetUserAsync(User);
            var leaveRequests = await _unitOfWork.LeaveReuqests.FindAll(request => request.RequestingEmployeeId == user.Id);
            var leaveAllocations = await _unitOfWork.LeaveAllocations.FindAll(allocation => allocation.EmployeeId == user.Id);

            //Map all that needs mapping
            var mappedLeaveRequests = _mapper.Map<List<LeaveRequestViewModel>>(leaveRequests);
            var mappedLeaveAllocations = _mapper.Map<List<LeaveAllocationViewModel>>(leaveAllocations);

            //Create a model
            var model = new MyLeaveViewModel
            {
                LeaveAllocations = mappedLeaveAllocations,
                LeaveRequests = mappedLeaveRequests
            };


            return View(model);
        }

        public async Task<ActionResult> CancelRequest(int id)
        {
            var leaveRequest = await _unitOfWork.LeaveReuqests.Find(request => request.Id == id);

            //if request was pending cancel without returning the days requested.
            leaveRequest.Canceled = true;
            await _unitOfWork.Save();


            //however if it was Approved, return requested days to leaveAllocation
            if (leaveRequest.Approved == true)
            {
                var period = DateTime.Now.Year;
                var leaveAllocation = await _unitOfWork.LeaveAllocations
                    .Find(allocation => allocation.EmployeeId == leaveRequest.RequestingEmployeeId
                    && allocation.Period == period
                    && allocation.LeaveTypeId == leaveRequest.LeaveTypeId) ;

                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                leaveAllocation.NumberOfDays += daysRequested;
                await _unitOfWork.Save();
            }

            return RedirectToAction(nameof(MyLeave));
        }

        // GET: LeaveRequestController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var leaveRequest = await _unitOfWork.LeaveReuqests.Find(request => request.Id == id);
            var model = _mapper.Map<LeaveRequestViewModel>(leaveRequest);
            return View(model);
        }

        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var leaveRequest = await _unitOfWork.LeaveReuqests.Find(request => request.Id == id);
                var employeeId = leaveRequest.RequestingEmployeeId;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var period = DateTime.Now.Year;
                var leaveAllocation = await _unitOfWork.LeaveAllocations
                    .Find(allocation => allocation.EmployeeId == employeeId
                                        && allocation.LeaveTypeId == leaveTypeId
                                        && allocation.Period == period);

                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                leaveAllocation.NumberOfDays -= daysRequested;

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById =user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                _unitOfWork.LeaveReuqests.Update(leaveRequest);
                _unitOfWork.LeaveAllocations.Update(leaveAllocation);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index), "Home");
               
            }
            
            catch (Exception)
            {
                
                return RedirectToAction(nameof(Index), "Home");
            }
        }

            
        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var leaveRequest = await _unitOfWork.LeaveReuqests.Find(request => request.Id == id);
                var user = await _userManager.GetUserAsync(User);

                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                _unitOfWork.LeaveReuqests.Update(leaveRequest);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));

            }

            catch (Exception)
            {

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: LeaveRequestController/Create
        public async Task<ActionResult> Create()
        {
            var leaveTypes = await _unitOfWork.LeaveTypes.FindAll();
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
        public async Task<ActionResult> Create(CreateLeaveRequestViewModel model)
        {
            

            try
            {
                //Converted toDateTime to use it for validation and mapping purposes
                var startDate = DateTime.ParseExact(model.StartDate, "MM/dd/yyyy", null);
                var endDate = DateTime.ParseExact(model.EndDate, "MM/dd/yyyy", null);

                //has to create the SelectListItem, 
                //becasue otherwise the model would 
                //return empty list in case of fail
                
                var leaveTypes = await _unitOfWork.LeaveTypes.FindAll();
                var leaveTypesItems = leaveTypes.Select(type => new SelectListItem
                {
                    Text = type.Name,
                    Value = type.Id.ToString()
                });

                model.LeaveTypes = leaveTypesItems;
                var period = DateTime.Now.Year;

                //Check if valid
                #region Validation

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (DateTime.Compare(startDate, endDate) > 1)
                {
                    ModelState.AddModelError("", "Start Date cannot be further in the future than the End Date");
                    return View(model);
                }

                var employee = await _userManager.GetUserAsync(User);
                var leaveAllocation = await _unitOfWork.LeaveAllocations
                    .Find(allocation => allocation.EmployeeId == employee.Id
                                        && allocation.LeaveTypeId == model.LeaveTypeId
                                        && allocation.Period == period);
                int daysRequested = (int)(endDate.Date - startDate.Date).TotalDays;

                if (daysRequested > leaveAllocation.NumberOfDays) 
                {
                    ModelState.AddModelError("", "You have requested more days for " + leaveAllocation.LeaveType.Name +  " than you have. "
                        + "You have " + leaveAllocation.NumberOfDays.ToString() + " days left");
                    return View(model);
                }

                #endregion

                //Create a model that is properly mappable
                var leaveRequestModel = new LeaveRequestViewModel
                {
                    LeaveTypeId = model.LeaveTypeId,
                    RequestingEmployeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now
                    
                    
                };
                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                
                await _unitOfWork.LeaveReuqests.Create(leaveRequest);
                await _unitOfWork.Save();

                //Redirects to home index, because leaverequest index is admin only
                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

    }
}
