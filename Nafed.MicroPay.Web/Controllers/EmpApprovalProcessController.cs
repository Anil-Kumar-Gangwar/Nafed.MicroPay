using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using MicroPay.Web.Models;
using System.Linq;
using System.Collections.Generic;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers
{
    public class EmpApprovalProcessController : BaseController
    {
        private readonly IEmpApprovalProcessService employeeService;
        private readonly IDropdownBindService ddlService;
        private readonly IUserService userService;
        public EmpApprovalProcessController(IEmpApprovalProcessService employeeService, IDropdownBindService ddlService, IUserService userService)
        {
            this.employeeService = employeeService;
            this.ddlService = ddlService;
            this.userService = userService;
        }
        public ActionResult Index()
        {
            //if (Request.Browser.IsMobileDevice)
            //{
            //    string strmanu = Request.Browser.MobileDeviceManufacturer.ToLower().ToString();
            //}
            //if (Request.Browser.ScreenPixelsWidth < 1000)
            //{

            //}
            log.Info($"EmpApprovalProcessController/Index");
            EmployeeViewModel empVM = new EmployeeViewModel();
            empVM.userRights = userAccessRight;
            empVM.branchList = ddlService.ddlBranchList();
            empVM.EmployeeList = ddlService.GetAllEmployee();
            return View(empVM);
        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            log.Info($"EmpApprovalProcessController/GetBranchEmployee");
            try
            {
                var employeedetails = ddlService.employeeByBranchID(branchID);
                Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
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

        [HttpGet]
        public ActionResult Edit(int employeeID, int? userTypeID)
        {
            log.Info($"EmpApprovalProcessController/Edit/{employeeID}");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetEmployeeGridView(EmployeeViewModel empVM)
        {
            EmployeeViewModel employeeVM = new EmployeeViewModel();
            var emp = employeeService.GetEmployeeList(empVM.BranchID, empVM.EmployeeID);
            employeeVM.Employee = emp;
            employeeVM.userRights = userAccessRight;
            return PartialView("_EmployeeGridView", employeeVM);
        }

        #region Appointment Confirmation Process Approval
        [HttpGet]
        public ActionResult _GetAppointmentProcessApproval(int employeeID, int? userTypeID)
        {
            log.Info($"EmployeeController/_GetEmpProcessApproval/{employeeID}");
            try
            {
                Model.EmployeeProcessApprovalVM empVM = new Model.EmployeeProcessApprovalVM();

                var processList = new[] {
                    (int) WorkFlowProcess.AppointmentConfirmation
                                    };

                if (!userTypeID.HasValue)
                {
                    var uTypeID = userService.GetUserTypeByEmployeeID((int?)employeeID);
                    userTypeID = uTypeID;
                }
                var empProcessApprovaList = employeeService.GetConfirmationApprovalProcesses(employeeID, processList).OrderBy(x => x.ProcessID).ToList();
                if (empProcessApprovaList != null && empProcessApprovaList.Count == 1)
                {
                    if (empProcessApprovaList.FirstOrDefault().ReportingTo == 0)
                    {
                        empVM.empProcess = new Model.EmployeeProcessApproval()
                        {
                            EmployeeID = empProcessApprovaList.FirstOrDefault().EmployeeID,
                            ProcessID = empProcessApprovaList.FirstOrDefault().ProcessID,
                            RoleID = (int)userTypeID
                        };
                        TempData["TempEmpProcessApp"] = null;
                        empProcessApprovaList = null;
                    }
                    else if (empProcessApprovaList.FirstOrDefault().ReportingTo > 0)
                    {
                        var sno = 1;
                        empProcessApprovaList.ForEach(x =>
                        {
                            x.RoleID = (int)userTypeID;
                            x.sno = sno++;
                        });

                        empVM.empProcess = new Model.EmployeeProcessApproval()
                        {
                            EmployeeID = empProcessApprovaList.FirstOrDefault().EmployeeID,
                            ProcessID = empProcessApprovaList.FirstOrDefault().ProcessID,
                            RoleID = (int)userTypeID
                        };
                        empVM.empProcessApp = empProcessApprovaList;
                        TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                        TempData.Keep("TempEmpProcessApp");
                    }
                }
                else
                {
                    empVM.empProcess = new Model.EmployeeProcessApproval()
                    {
                        EmployeeID = empProcessApprovaList.FirstOrDefault().EmployeeID,
                        ProcessID = empProcessApprovaList.FirstOrDefault().ProcessID,
                        RoleID = (int)userTypeID
                    };
                    if (empProcessApprovaList != null && empProcessApprovaList.Count > 0)
                    {
                        var sno = 1;
                        empProcessApprovaList.ForEach(x =>
                        {
                            x.RoleID = (int)userTypeID;
                            x.sno = sno++;
                        });
                    }
                    empVM.empProcessApp = empProcessApprovaList;
                    TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                    TempData.Keep("TempEmpProcessApp");
                }
                var ddlEmployee = ddlService.GetAllEmployee();
                ddlEmployee.RemoveAll(x => x.id == employeeID);
                empVM.EmployeeList = ddlEmployee;
                empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });

                empVM.empProcessApp = empProcessApprovaList;
                TempData["TempEmployeeList"] = ddlEmployee;
                return PartialView("_AppointmentConfContainer", empVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _PostAppointmentProcessApproval(Model.EmployeeProcessApprovalVM empVM, string ButtonType)
        {
            log.Info($"EmployeeController/_PostEmpProcessApprova");
            try
            {
                if (ButtonType == "Add")
                {
                    if (empVM.empProcess.ReportingTo == 0)
                    {
                        var tememployeeList = (List<Model.SelectListModel>)TempData["TempEmployeeList"];
                        empVM.EmployeeList = tememployeeList;
                        var empProcessAprList = (List<Model.EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
                        empVM.empProcessApp = empProcessAprList;
                        TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                        TempData["TempEmployeeList"] = empVM.EmployeeList;
                        ModelState.AddModelError("ReportingBlankModel", "Please select Reporting 1");
                        return Json(new { status = false, htmlData = ConvertViewToString("_AppointmentConfContainer", empVM) }, JsonRequestBehavior.AllowGet);
                    }
                    var empProcessApr = (List<Model.EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
                    var employeeList = (List<Model.SelectListModel>)TempData["TempEmployeeList"];
                    if (empProcessApr == null || empProcessApr.Count == 0)
                    {
                        empProcessApr = new List<Model.EmployeeProcessApproval>() {
                            new Model.EmployeeProcessApproval() {
                                sno = 1,
                                MultiReporting=true,
                                 RoleID = empVM.empProcess.RoleID,
                        ReportingTo =empVM.empProcess.ReportingTo,
                        AcceptanceAuthority=empVM.empProcess.AcceptanceAuthority,
                                 ReviewingTo=empVM.empProcess.ReviewingTo,
                                 EmployeeID=empVM.empProcess.EmployeeID,
                                 ProcessID=empVM.empProcess.ProcessID,
                                 ReportingToName=employeeList.Where(x=> x.id==empVM.empProcess.ReportingTo).FirstOrDefault().value,
                                 ReviewerName=empVM.empProcess.ReviewingTo.HasValue &&empVM.empProcess.ReviewingTo>0? employeeList.Where(x=> x.id==empVM.empProcess.ReviewingTo).FirstOrDefault().value:string.Empty,
                                 AcceptanceAuthorityName=empVM.empProcess.AcceptanceAuthority.HasValue && empVM.empProcess.AcceptanceAuthority>0  ?employeeList.Where(x=> x.id==empVM.empProcess.AcceptanceAuthority).FirstOrDefault().value:string.Empty,
                                 CreatedBy=userDetail.UserID,
                                 CreatedOn =DateTime.Now
                            } };
                    }
                    else
                    {
                        if (empProcessApr.Count < 5)

                            empProcessApr.ForEach(em =>
                            {
                                if ((em.ReportingTo == empVM.empProcess.ReportingTo) &&  em.IsDeleted == false)
                                {
                                    empVM._reportingError = true;
                                }
                            });
                        if (empVM._reportingError)
                        {
                            empVM.empProcessApp = empProcessApr;
                            TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                            //TempData.Keep("TempEmpProcessApp");                         
                            TempData.Keep("TempEmployeeList");
                            // return PartialView("_ConfirmationApprovalList", empVM);
                            return Json(new { part = 1, htmlData = ConvertViewToString("_ConfirmationApprovalList", empVM) }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            empProcessApr.Add(new Model.EmployeeProcessApproval()
                            {
                                sno = empProcessApr.Count + 1,
                                MultiReporting = true,
                                RoleID = empVM.empProcess.RoleID,
                                ReportingTo = empVM.empProcess.ReportingTo,
                                ReviewingTo = empVM.empProcess.ReviewingTo,
                                AcceptanceAuthority = empVM.empProcess.AcceptanceAuthority,
                                EmployeeID = empVM.empProcess.EmployeeID,
                                ProcessID = empVM.empProcess.ProcessID,
                                ReportingToName = employeeList.Where(x => x.id == empVM.empProcess.ReportingTo).FirstOrDefault().value,
                                ReviewerName = empVM.empProcess.ReviewingTo.HasValue && empVM.empProcess.ReviewingTo > 0 ? employeeList.Where(x => x.id == empVM.empProcess.ReviewingTo).FirstOrDefault().value : string.Empty,
                                AcceptanceAuthorityName = empVM.empProcess.AcceptanceAuthority.HasValue && empVM.empProcess.AcceptanceAuthority > 0 ? employeeList.Where(x => x.id == empVM.empProcess.AcceptanceAuthority).FirstOrDefault().value : string.Empty,
                                CreatedBy = userDetail.UserID,
                                CreatedOn = DateTime.Now
                            });
                        }
                    }

                    empVM.empProcessApp = empProcessApr;
                    TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                    TempData.Keep("TempEmpProcessApp");
                    TempData.Keep("TempEmployeeList");
                    //return PartialView("_ConfirmationApprovalList", empVM);
                    return Json(new { part = 1, htmlData = ConvertViewToString("_ConfirmationApprovalList", empVM) }, JsonRequestBehavior.AllowGet);
                }
                if (ModelState.IsValid)
                {
                    var tememployeeList = (List<Model.SelectListModel>)TempData["TempEmployeeList"];
                    empVM.EmployeeList = tememployeeList;
                    var empProcessAprList = (List<Model.EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
                    empVM.empProcessApp = empProcessAprList;

                    if (!empVM.empProcessApp.Any(x => x.EmpProcessAppID > 0))
                    {
                        empVM.empProcessApp.ForEach(em =>
                        {
                            em.CreatedBy = userDetail.UserID;
                            em.CreatedOn = DateTime.Now;
                            em.Fromdate = DateTime.Now;
                            em.ReviewingTo = em.ReviewingTo == 0 ? null : em.ReviewingTo;
                            em.AcceptanceAuthority = em.AcceptanceAuthority == 0 ? null : em.AcceptanceAuthority;
                        });


                        empVM.empProcessApp = empVM.empProcessApp.Where(x => x.ReportingTo != 0).ToList();
                        var result = employeeService.InsertMultiProcessApproval(empVM.empProcessApp);
                        if (result)
                        {
                            return Json(new { part = 2, msgType = "success", msg = "Employee process approval successfully configured." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        Model.EmployeeProcessApprovalVM newempVM = new Model.EmployeeProcessApprovalVM();
                        var result = false;
                        foreach (var em in empVM.empProcessApp)
                        {                        
                            em.ReviewingTo = em.ReviewingTo == 0 ? null : em.ReviewingTo;
                            em.AcceptanceAuthority = em.AcceptanceAuthority == 0 ? null : em.AcceptanceAuthority;
                            em.OldReviewingTo = em.OldReviewingTo == 0 ? null : em.OldReviewingTo;
                            em.OldAcceptanceAuthority = em.OldAcceptanceAuthority == 0 ? null : em.OldAcceptanceAuthority;
                            // this line of code is for remove duplicate and non isdeleted row from being insert in table
                            if ( !em.IsDeleted && em.OldReportingTo == em.ReportingTo && em.OldReviewingTo == em.ReviewingTo && em.OldAcceptanceAuthority == em.AcceptanceAuthority)
                            {
                            }
                            else
                            {
                                newempVM.empProcessApp.Add(new Model.EmployeeProcessApproval()
                                {
                                    MultiReporting = true,
                                    AcceptanceAuthority = em.AcceptanceAuthority,
                                    ReportingTo = em.ReportingTo,
                                    ReviewingTo = em.ReviewingTo,
                                    EmpProcessAppID = em.EmpProcessAppID,
                                    ProcessID = em.ProcessID,
                                    ToDate = DateTime.Now,
                                    EmployeeID = em.EmployeeID,
                                    UpdatedBy = userDetail.UserID,
                                    UpdatedOn = DateTime.Now,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    Fromdate = DateTime.Now,
                                    RoleID = em.RoleID,
                                    IsDeleted=em.IsDeleted
                                });
                            }

                        }

                        newempVM.empProcessApp = newempVM.empProcessApp.Where(x => x.ReportingTo != 0).ToList();


                        //Model.EmployeeProcessApprovalVM newempVM = new Model.EmployeeProcessApprovalVM();
                        //var result = false;     

                        if (newempVM.empProcessApp.Count > 0)
                        {
                            result = employeeService.UpdateMultiProcessApproval(newempVM.empProcessApp);
                        }
                        else
                        {
                            result = true;
                        }
                        if (result)
                        {
                           // TempData["Message"] = "Employee process approval successfully configured.";
                         //   return RedirectToAction("Index");
                             return Json(new { part = 2, msgType = "success", msg = "Employee process approval successfully configured." }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            empVM.EmployeeList = ddlService.GetAllEmployee();
                            empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });
                        }
                    }
                }
                else
                {
                    empVM.EmployeeList = ddlService.GetAllEmployee();
                    empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });
                }
                return PartialView("_ApprovalProcess", empVM);


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _RemoveAppointmentRow(int sNo)
        {
            var empProcessApr = (List<Model.EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
            if (empProcessApr != null && empProcessApr.Count > 0)
            {
                bool status = false;string msg = string.Empty;string msgType = string.Empty;
                Model.EmployeeProcessApprovalVM empVM = new Model.EmployeeProcessApprovalVM();
                var deletedRow = empProcessApr.Where(x => x.sno == sNo).FirstOrDefault();
                if (deletedRow != null)
                {
                    bool res = employeeService.IsProcessStarted(deletedRow.EmpProcessAppID, deletedRow.EmployeeID);
                    if (!res)
                    {
                        deletedRow.IsDeleted = true;
                        status=true;
                        msgType = "success";                       
                    }
                    else
                    {
                        status = false;
                        msgType = "error";
                        msg= "Can't delete this record,because it is in used in another process.";
                    }
                }

                // empProcessApr.Remove(deletedRow);
                empVM.empProcessApp = empProcessApr;
                TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                return Json(new { status= status, msgType = msgType, msg = msg,htmlData=ConvertViewToString("_ConfirmationApprovalList", empVM) }, JsonRequestBehavior.AllowGet);

               // return PartialView("_ConfirmationApprovalList", empVM);
            }
            return Content("");

        }
        #endregion

        #region Promotion Confirmation Process Approval
        [HttpGet]
        public ActionResult _GetPromotionProcessApproval(int employeeID, int? userTypeID)
        {
            log.Info($"EmployeeController/_GetPromotionProcessApproval/{employeeID}");
            try
            {
                Model.EmployeeProcessApprovalVM empVM = new Model.EmployeeProcessApprovalVM();

                var processList = new[] {
                    (int) WorkFlowProcess.PromotionConfirmation
                                    };

                if (!userTypeID.HasValue)
                {
                    var uTypeID = userService.GetUserTypeByEmployeeID((int?)employeeID);
                    userTypeID = uTypeID;
                }
                var empProcessApprovaList = employeeService.GetConfirmationApprovalProcesses(employeeID, processList).OrderBy(x => x.ProcessID).ToList();
                if (empProcessApprovaList != null && empProcessApprovaList.Count == 1)
                {
                    if (empProcessApprovaList.FirstOrDefault().ReportingTo == 0)
                    {
                        empVM.empProcess = new Model.EmployeeProcessApproval()
                        {
                            EmployeeID = empProcessApprovaList.FirstOrDefault().EmployeeID,
                            ProcessID = empProcessApprovaList.FirstOrDefault().ProcessID,
                            RoleID = (int)userTypeID
                        };
                        TempData["TempEmpProcessAppProm"] = null;
                        empProcessApprovaList = null;
                    }
                    else if (empProcessApprovaList.FirstOrDefault().ReportingTo > 0)
                    {
                        var sno = 1;
                        empProcessApprovaList.ForEach(x =>
                        {
                            x.RoleID = (int)userTypeID;
                            x.sno = sno++;
                        });

                        empVM.empProcess = new Model.EmployeeProcessApproval()
                        {
                            EmployeeID = empProcessApprovaList.FirstOrDefault().EmployeeID,
                            ProcessID = empProcessApprovaList.FirstOrDefault().ProcessID,
                            RoleID = (int)userTypeID
                        };
                        empVM.empProcessApp = empProcessApprovaList;
                        TempData["TempEmpProcessAppProm"] = empVM.empProcessApp;
                        TempData.Keep("TempEmpProcessAppProm");
                    }
                }
                else
                {
                    empVM.empProcess = new Model.EmployeeProcessApproval()
                    {
                        EmployeeID = empProcessApprovaList.FirstOrDefault().EmployeeID,
                        ProcessID = empProcessApprovaList.FirstOrDefault().ProcessID,
                        RoleID = (int)userTypeID
                    };
                    if (empProcessApprovaList != null && empProcessApprovaList.Count > 0)
                    {
                        var sno = 1;
                        empProcessApprovaList.ForEach(x =>
                        {
                            x.RoleID = (int)userTypeID;
                            x.sno = sno++;
                        });
                    }
                    empVM.empProcessApp = empProcessApprovaList;
                    TempData["TempEmpProcessAppProm"] = empVM.empProcessApp;
                    TempData.Keep("TempEmpProcessAppProm");
                }
                var ddlEmployee = ddlService.GetAllEmployee();
                ddlEmployee.RemoveAll(x => x.id == employeeID);
                empVM.EmployeeList = ddlEmployee;
                empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });

                empVM.empProcessApp = empProcessApprovaList;
                TempData["TempEmployeeListProm"] = ddlEmployee;
                return PartialView("_PromotionConfContainer", empVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _PostPromotionProcessApproval(Model.EmployeeProcessApprovalVM empVM, string ButtonType)
        {
            log.Info($"EmployeeController/_PostEmpProcessApprova");
            try
            {
                if (ButtonType == "Add")
                {
                    if (empVM.empProcess.ReportingTo == 0)
                    {
                        var tememployeeList = (List<Model.SelectListModel>)TempData["TempEmployeeListProm"];
                        empVM.EmployeeList = tememployeeList;
                        var empProcessAprList = (List<Model.EmployeeProcessApproval>)TempData["TempEmpProcessAppProm"];
                        empVM.empProcessApp = empProcessAprList;
                        TempData["TempEmpProcessAppProm"] = empVM.empProcessApp;
                        TempData["TempEmployeeListProm"] = empVM.EmployeeList;
                        ModelState.AddModelError("ReportingBlankModel", "Please select Reporting 1");
                        return Json(new { status = false, htmlData = ConvertViewToString("_PromotionConfContainer", empVM) }, JsonRequestBehavior.AllowGet);
                    }
                    var empProcessApr = (List<Model.EmployeeProcessApproval>)TempData["TempEmpProcessAppProm"];
                    var employeeList = (List<Model.SelectListModel>)TempData["TempEmployeeListProm"];
                    if (empProcessApr == null || empProcessApr.Count == 0)
                    {
                        empProcessApr = new List<Model.EmployeeProcessApproval>() {
                            new Model.EmployeeProcessApproval() {
                                sno = 1,
                                MultiReporting=true,
                                 RoleID = empVM.empProcess.RoleID,
                        ReportingTo =empVM.empProcess.ReportingTo,
                        AcceptanceAuthority=empVM.empProcess.AcceptanceAuthority,
                                 ReviewingTo=empVM.empProcess.ReviewingTo,
                                 EmployeeID=empVM.empProcess.EmployeeID,
                                 ProcessID=empVM.empProcess.ProcessID,
                                 ReportingToName=employeeList.Where(x=> x.id==empVM.empProcess.ReportingTo).FirstOrDefault().value,
                                 ReviewerName=empVM.empProcess.ReviewingTo.HasValue &&empVM.empProcess.ReviewingTo>0? employeeList.Where(x=> x.id==empVM.empProcess.ReviewingTo).FirstOrDefault().value:string.Empty,
                                 AcceptanceAuthorityName=empVM.empProcess.AcceptanceAuthority.HasValue && empVM.empProcess.AcceptanceAuthority>0  ?employeeList.Where(x=> x.id==empVM.empProcess.AcceptanceAuthority).FirstOrDefault().value:string.Empty,
                                 CreatedBy=userDetail.UserID,
                                 CreatedOn =DateTime.Now
                            } };
                    }
                    else
                    {
                        if (empProcessApr.Count < 5)

                            empProcessApr.ForEach(em =>
                            {
                                if ((em.ReportingTo == empVM.empProcess.ReportingTo) && em.IsDeleted == false)
                                {
                                    empVM._reportingError = true;
                                }
                            });
                        if (empVM._reportingError)
                        {
                            empVM.empProcessApp = empProcessApr;
                            TempData["TempEmpProcessAppProm"] = empVM.empProcessApp;
                            //TempData.Keep("TempEmpProcessApp");                         
                            TempData.Keep("TempEmployeeListProm");
                            // return PartialView("_ConfirmationApprovalList", empVM);
                            return Json(new { part = 1, htmlData = ConvertViewToString("_PromotionApprovalList", empVM) }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            empProcessApr.Add(new Model.EmployeeProcessApproval()
                            {
                                sno = empProcessApr.Count + 1,
                                MultiReporting = true,
                                RoleID = empVM.empProcess.RoleID,
                                ReportingTo = empVM.empProcess.ReportingTo,
                                ReviewingTo = empVM.empProcess.ReviewingTo,
                                AcceptanceAuthority = empVM.empProcess.AcceptanceAuthority,
                                EmployeeID = empVM.empProcess.EmployeeID,
                                ProcessID = empVM.empProcess.ProcessID,
                                ReportingToName = employeeList.Where(x => x.id == empVM.empProcess.ReportingTo).FirstOrDefault().value,
                                ReviewerName = empVM.empProcess.ReviewingTo.HasValue && empVM.empProcess.ReviewingTo > 0 ? employeeList.Where(x => x.id == empVM.empProcess.ReviewingTo).FirstOrDefault().value : string.Empty,
                                AcceptanceAuthorityName = empVM.empProcess.AcceptanceAuthority.HasValue && empVM.empProcess.AcceptanceAuthority > 0 ? employeeList.Where(x => x.id == empVM.empProcess.AcceptanceAuthority).FirstOrDefault().value : string.Empty,
                                CreatedBy = userDetail.UserID,
                                CreatedOn = DateTime.Now
                            });
                        }
                    }

                    empVM.empProcessApp = empProcessApr;
                    TempData["TempEmpProcessAppProm"] = empVM.empProcessApp;
                    TempData.Keep("TempEmpProcessAppProm");
                    TempData.Keep("TempEmployeeListProm");
                    //return PartialView("_ConfirmationApprovalList", empVM);
                    return Json(new { part = 1, htmlData = ConvertViewToString("_PromotionApprovalList", empVM) }, JsonRequestBehavior.AllowGet);
                }
                if (ModelState.IsValid)
                {
                    var tememployeeList = (List<Model.SelectListModel>)TempData["TempEmployeeListProm"];
                    empVM.EmployeeList = tememployeeList;
                    var empProcessAprList = (List<Model.EmployeeProcessApproval>)TempData["TempEmpProcessAppProm"];
                    empVM.empProcessApp = empProcessAprList;

                    if (!empVM.empProcessApp.Any(x => x.EmpProcessAppID > 0))
                    {
                        empVM.empProcessApp.ForEach(em =>
                        {
                            em.CreatedBy = userDetail.UserID;
                            em.CreatedOn = DateTime.Now;
                            em.Fromdate = DateTime.Now;
                            em.ReviewingTo = em.ReviewingTo == 0 ? null : em.ReviewingTo;
                            em.AcceptanceAuthority = em.AcceptanceAuthority == 0 ? null : em.AcceptanceAuthority;
                        });


                        empVM.empProcessApp = empVM.empProcessApp.Where(x => x.ReportingTo != 0 && !x.IsDeleted).ToList();
                        var result = employeeService.InsertMultiProcessApproval(empVM.empProcessApp);
                        if (result)
                        {
                            return Json(new { part = 2, msgType = "success", msg = "Employee process approval successfully configured." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {

                        Model.EmployeeProcessApprovalVM newempVM = new Model.EmployeeProcessApprovalVM();
                        var result = false;
                        foreach (var em in empVM.empProcessApp)
                        {
                            em.ReviewingTo = em.ReviewingTo == 0 ? null : em.ReviewingTo;
                            em.AcceptanceAuthority = em.AcceptanceAuthority == 0 ? null : em.AcceptanceAuthority;
                            em.OldReviewingTo = em.OldReviewingTo == 0 ? null : em.OldReviewingTo;
                            em.OldAcceptanceAuthority = em.OldAcceptanceAuthority == 0 ? null : em.OldAcceptanceAuthority;
                            // this line of code is for remove duplicate and non isdeleted row from being insert in table
                            if (!em.IsDeleted && em.OldReportingTo == em.ReportingTo && em.OldReviewingTo == em.ReviewingTo && em.OldAcceptanceAuthority == em.AcceptanceAuthority)
                            {
                            }
                            else
                            {
                                newempVM.empProcessApp.Add(new Model.EmployeeProcessApproval()
                                {
                                    MultiReporting = true,
                                    AcceptanceAuthority = em.AcceptanceAuthority,
                                    ReportingTo = em.ReportingTo,
                                    ReviewingTo = em.ReviewingTo,
                                    EmpProcessAppID = em.EmpProcessAppID,
                                    ProcessID = em.ProcessID,
                                    ToDate = DateTime.Now,
                                    EmployeeID = em.EmployeeID,
                                    UpdatedBy = userDetail.UserID,
                                    UpdatedOn = DateTime.Now,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    Fromdate = DateTime.Now,
                                    RoleID = em.RoleID,
                                    IsDeleted = em.IsDeleted
                                });
                            }

                        }

                        newempVM.empProcessApp = newempVM.empProcessApp.Where(x => x.ReportingTo != 0).ToList();


                        //Model.EmployeeProcessApprovalVM newempVM = new Model.EmployeeProcessApprovalVM();
                        //var result = false;     

                        if (newempVM.empProcessApp.Count > 0)
                        {
                            result = employeeService.UpdateMultiProcessApproval(newempVM.empProcessApp);
                        }
                        else
                        {
                            result = true;
                        }
                        if (result)
                        {
                            // TempData["Message"] = "Employee process approval successfully configured.";
                            //   return RedirectToAction("Index");
                            return Json(new { part = 2, msgType = "success", msg = "Employee process approval successfully configured." }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            empVM.EmployeeList = ddlService.GetAllEmployee();
                            empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });
                        }
                    }
                }
                else
                {
                    empVM.EmployeeList = ddlService.GetAllEmployee();
                    empVM.EmployeeList.Insert(0, new Model.SelectListModel { id = 0, value = "Select" });
                }
                return PartialView("_ApprovalProcess", empVM);


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _RemovePromotionRow(int sNo)
        {
            var empProcessApr = (List<Model.EmployeeProcessApproval>)TempData["TempEmpProcessAppProm"];
            if (empProcessApr != null && empProcessApr.Count > 0)
            {
                bool status = false; string msg = string.Empty; string msgType = string.Empty;
                Model.EmployeeProcessApprovalVM empVM = new Model.EmployeeProcessApprovalVM();
                var deletedRow = empProcessApr.Where(x => x.sno == sNo).FirstOrDefault();
                if (deletedRow != null)
                {
                    bool res = employeeService.IsProcessStarted(deletedRow.EmpProcessAppID, deletedRow.EmployeeID);
                    if (!res)
                    {
                        deletedRow.IsDeleted = true;
                        status = true;
                        msgType = "success";
                    }
                    else
                    {
                        status = false;
                        msgType = "error";
                        msg = "Can't delete this record,because it is in used in another process.";
                    }
                }

                // empProcessApr.Remove(deletedRow);
                empVM.empProcessApp = empProcessApr;
                TempData["TempEmpProcessAppProm"] = empVM.empProcessApp;
                return Json(new { status = status, msgType = msgType, msg = msg, htmlData = ConvertViewToString("_PromotionApprovalList", empVM) }, JsonRequestBehavior.AllowGet);

                // return PartialView("_ConfirmationApprovalList", empVM);
            }
            return Content("");

        }
        #endregion



    }
}