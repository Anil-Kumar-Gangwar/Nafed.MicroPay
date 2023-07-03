using MicroPay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Model;
namespace MicroPay.Web.Controllers
{
    public class FileTrackingSystemController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IFileTrackingSytemService fileTrackingServ;
        // GET: FileTrackingSystem
        public FileTrackingSystemController(IDropdownBindService ddlService, IFileTrackingSytemService fileTrackingServ)
        {
            this.ddlService = ddlService;
            this.fileTrackingServ = fileTrackingServ;
        }
        public ActionResult Index()
        {
            FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();

            return View(fileTrackVM);
        }
        public ActionResult GetTrackingFileList()
        {
            log.Info($"FileTrackingSystemController/GetTrackingFileList");
            try
            {
                FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();
                fileTrackVM.fileWorkFlowList = fileTrackingServ.GetFileTrackingList();
                ViewBag.Employee = ddlService.GetAllEmployee();
                return PartialView("_FileTrackingList", fileTrackVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            log.Info($"FileTrackingSystemController/Create");
            try
            {
                FileTrackingViewModel vm = new Models.FileTrackingViewModel();
                vm.fileWorkFlow = new FileWorkflow();
                ViewBag.Department = ddlService.ddlDepartmentList();
                ViewBag.Designation = ddlService.ddlDesignationList();
                var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID,null,null);
                vm.lstEmployeeList = employeedetails;

                //ViewBag.InitiatedEmployee = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID, null, null);
                ViewBag.ParkedEmployee = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID, null, null);
                return View(vm);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public ActionResult Edit(int fileWorkFlowID)
        {
            log.Info($"FileTrackingSystemController/Edit");
            try
            {
                FileTrackingViewModel fileVM = new Models.FileTrackingViewModel();

                var getFileTrackingData = fileTrackingServ.GetFileTrackingList().Where(x => x.WorkFlowID == fileWorkFlowID).FirstOrDefault();
                if (getFileTrackingData != null)
                {
                    ViewBag.Department = ddlService.ddlDepartmentList();
                    ViewBag.Designation = ddlService.ddlDesignationList();
                    var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(null, getFileTrackingData.initiateDepartmentID, getFileTrackingData.initiateDesignationID);
                    fileVM.lstEmployeeList = employeedetails;
                    //ViewBag.InitiatedEmployee = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID, getFileTrackingData.initiateDepartmentID, getFileTrackingData.initiateDesignationID);
                    ViewBag.ParkedEmployee = ddlService.GetEmployeeByDepartmentDesignationID(null, getFileTrackingData.ParkByDepartmentID, getFileTrackingData.ParkByDesignationID);

                    fileVM.fileWorkFlow = getFileTrackingData;                  
                    fileVM.fileWorkFlow.intEmployeeID=new int[] { fileVM.fileWorkFlow.initiateEmployeeID };

                }
                return View("Create", fileVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        [HttpPost]
        public ActionResult Create(FileTrackingViewModel fileTrackingVM)
        {
            log.Info($"FileTrackingSystemController/Create");
            try
            {
                ModelState.Remove("fileWorkFlow.WorkFlowID");
                ModelState.Remove("fileWorkFlow.initiateDesignationID");
                if (fileTrackingVM.fileWorkFlow.intEmployeeID==null)
                    ModelState.AddModelError("InitiateEmployeeValidator", "Please select Employee");
                if (ModelState.IsValid)
                {
                    if (fileTrackingVM.fileWorkFlow.WorkFlowID == 0)
                    {
                        fileTrackingVM.fileWorkFlow.created_by = userDetail.UserID;
                        fileTrackingVM.fileWorkFlow.created_on = DateTime.Now;
                        fileTrackingServ.InsertFileTracking(fileTrackingVM.fileWorkFlow);
                        TempData["Message"] = "Successfully Assign";
                        return RedirectToAction("Index");
                    }
                    else if (fileTrackingVM.fileWorkFlow.WorkFlowID > 0)
                    {
                        fileTrackingVM.fileWorkFlow.created_by = userDetail.UserID;
                        fileTrackingVM.fileWorkFlow.created_on = DateTime.Now;
                        fileTrackingServ.UpdateFileTracking(fileTrackingVM.fileWorkFlow);
                        TempData["Message"] = "Successfully Assign";
                        return RedirectToAction("Index");
                    }
                    return View(fileTrackingVM);
                }
                else
                {
                    ViewBag.Department = ddlService.ddlDepartmentList();
                    ViewBag.Designation = ddlService.ddlDesignationList();
                    var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(null, fileTrackingVM.fileWorkFlow.initiateDepartmentID, fileTrackingVM.fileWorkFlow.initiateDesignationID);
                    fileTrackingVM.lstEmployeeList = employeedetails;
                //    ViewBag.InitiatedEmployee = ddlService.GetEmployeeByDepartmentDesignationID(userDetail.BranchID, fileTrackingVM.fileWorkFlow.initiateDepartmentID, fileTrackingVM.fileWorkFlow.initiateDesignationID);
                    ViewBag.ParkedEmployee = ddlService.GetEmployeeByDepartmentDesignationID(null, fileTrackingVM.fileWorkFlow.ParkByDepartmentID, fileTrackingVM.fileWorkFlow.ParkByDesignationID);
                    return View(fileTrackingVM);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public JsonResult GetEmployeeByDesignation(int departmentID, int designationID)
        {
            log.Info($"FileTrackingSystemController/GetEmployeeByDesignation");
            try
            {
                var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(null, departmentID, designationID);
                SelectListModel selectemployeeDetails = new SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
                employeedetails.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeedetails, "id", "value");
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetEmpCheckboxList(int departmentID, int designationID)
        {
            log.Info($"FileTrackingSystemController/GetEmpCheckboxList");
            try
            {
                FileTrackingViewModel fileTrack = new Models.FileTrackingViewModel();
                int? desid = null;
                if (designationID == 0)
                    desid = null;
                else
                    desid = designationID;

                var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(null, departmentID, desid);
                // SelectListModel selectemployeeDetails = new SelectListModel();
                //selectemployeeDetails.id = 0;
                //selectemployeeDetails.value = "Select";
                //employeedetails.Insert(0, selectemployeeDetails);
                fileTrack.lstEmployeeList = employeedetails;
                return Json(new { employees = ConvertViewToString("_CheckBoxList", fileTrack) }, JsonRequestBehavior.AllowGet);
                // return PartialView("_CheckBoxList", fileTrack);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetTrackingListForPopup(int departmentID)
        {
            log.Info($"FileTrackingSystemController/GetTrackingListForPopup");
            try
            {
                FileTrackingViewModel fileTrackVM = new Models.FileTrackingViewModel();
                fileTrackVM.fileWorkFlowList = fileTrackingServ.GetFileTrackingListForPopup(departmentID);
                return PartialView("_FileTrackingLogsPopup", fileTrackVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
    }
}