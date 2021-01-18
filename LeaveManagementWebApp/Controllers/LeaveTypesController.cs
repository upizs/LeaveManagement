using AutoMapper;
using LeaveManagementWebApp.Contracts;
using LeaveManagementWebApp.Data;
using LeaveManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementWebApp.Controllers
{
    //Authorize all the controller to restrict access for all the actions
    [Authorize(Roles = "Administrator")]
    public class LeaveTypesController : Controller
    {
        private readonly ILeaveTypeRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        //Dependendy injuction
        public LeaveTypesController(ILeaveTypeRepository repo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        // GET: LeaveTypesController
        
        public async Task<ActionResult> Index()
        {
            //var leavetypes = await _repo.FindAll();
            var leaveTypes = await _unitOfWork.LeaveTypes.FindAll();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes.ToList());
           
            return View(model);
        }

        // GET: LeaveTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            //var exist = await _repo.Exists(id);
            var exist = await _unitOfWork.LeaveTypes.Exists(type => type.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            //var leaveType = await _repo.FindById(id);
            var leaveType = await _unitOfWork.LeaveTypes.Find(type => type.Id == id);
            var model = _mapper.Map<LeaveTypeViewModel>(leaveType);
            return View(model);
        }

        // GET: LeaveTypesController/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: LeaveTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LeaveTypeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) 
                {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveType>(model);
                leaveType.DateCreated = DateTime.Now;

                //var isSuccess = await _repo.Create(leaveType);
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Something went wrong..");
                //    return View(model);
                //}

                await _unitOfWork.LeaveTypes.Create(leaveType);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong..");
                return View();
            }
        }

        // GET: LeaveTypesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            //var exist = await _repo.Exists(id);
            var exist = await _unitOfWork.LeaveTypes.Exists(type => type.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            //var leaveType = _repo.FindById(id);
            var leaveType = await _unitOfWork.LeaveTypes.Find(type => type.Id == id);

            var model = _mapper.Map<LeaveTypeViewModel>(leaveType);

            return View(model);
        }

        // POST: LeaveTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LeaveTypeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveType>(model);
                //var isSuccess = await _repo.Update(leaveType);
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Something went wrong..");
                //    return View(model);
                //}

                _unitOfWork.LeaveTypes.Update(leaveType);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));

            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong..");
                return View(model);
            }
        }

        // GET: LeaveTypesController1/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            //var leaveType = await _repo.FindById(id);
            var leaveType = await _unitOfWork.LeaveTypes.Find(type => type.Id == id);

            if (leaveType == null)
            {
                return NotFound();
            }

            //var isSuccess = await _repo.Delete(leaveType);
            //if (!isSuccess)
            //{

            //    return BadRequest();
            //}
            _unitOfWork.LeaveTypes.Delete(leaveType);
            await _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }

        // POST: LeaveTypesController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, LeaveTypeViewModel model)
        {
            try
            {
                //var leaveType = await _repo.FindById(id);
                var leaveType = await _unitOfWork.LeaveTypes.Find(type => type.Id == id);
                if (leaveType == null)
                {
                    return NotFound();
                }
                
                _unitOfWork.LeaveTypes.Delete(leaveType);
                await _unitOfWork.Save();

                //var isSuccess = await _repo.Delete(leaveType);
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Something went wrong..");
                //    return View(model);
                //}
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }

        
    }
}
