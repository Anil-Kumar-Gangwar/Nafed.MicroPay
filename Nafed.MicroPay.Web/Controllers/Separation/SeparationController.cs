using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.FileHelper;

namespace MicroPay.Web.Controllers.Separation
{
    public class SeparationController : BaseController
    {
        private readonly ISeparationService sepService;
        private readonly IDropdownBindService ddlService;
        public SeparationController(ISeparationService sepService, IDropdownBindService ddlService)
        {
            this.sepService = sepService;
            this.ddlService = ddlService;
        }
        // GET: Sepration
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetSuperAnnuatingList()
        {
            log.Info("SeparationController/GetSuperAnnuatingList");
            try
            {
                var getSuperAnnuating = sepService.GetSuperAnnuating();
                return PartialView("_SuperAnnuatingList", getSuperAnnuating);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult PostSuperAnnuatingList(List<SuperAnnuating> objList, FormCollection frm, string ButtonType)
        {
            log.Info("SeparationController/GetSuperAnnuatingList");
            try
            {
                if (objList != null)
                {
                    var checkId = Convert.ToString(frm["Radio"]);
                    var empId = Convert.ToInt32(checkId);
                    var getSepId = objList.Where(x => x.EmployeeId == empId).FirstOrDefault().SeprationId;
                    if (ButtonType == "Start")
                    {
                        var getEmp = objList.Where(x => x.EmployeeId == empId && x.StatusId == 0).FirstOrDefault();
                        SuperAnnuating obj = new SuperAnnuating
                        {
                            EmployeeId = getEmp.EmployeeId,
                            StatusId = (int)SeprationStatus.Start,
                            SeprationType = 1,
                            DateOfAction = DateTime.Now,
                            CreatedBy = userDetail.UserID,
                            CreatedOn = DateTime.Now
                        };
                        sepService.StartProcess(obj);
                        TempData["Message"] = "Process Start successfully.";
                        return RedirectToAction("Index");
                    }
                    else if (ButtonType == "Clearace")
                    {
                        return RedirectToAction("Clearance", new { empId = empId, sepId = getSepId });
                    }
                    else if (ButtonType == "DivisionalApproval")
                    {
                        return RedirectToAction("DivisionalApproval", new { empId = empId, sepId = getSepId });
                    }
                    return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Upload And Download And Get Documents
        [HttpGet]
        public ActionResult GetUploadDocument(int sepId, int empId)
        {
            log.Info($"SeparationController/GetUploadDocument");
            try
            {
                SuperAnnuating objSupp = new SuperAnnuating
                {
                    SeprationId = sepId,
                    EmployeeId = empId

                };
                return PartialView("_UploadDocument", objSupp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            HttpFileCollectionBase files = Request.Files;
            string fileName = string.Empty; string filePath = string.Empty;
            string fname = "";
            List<string> filename = new List<string>();
            string fullPath = "";
            TempData["UploadedFiles"] = null;

            var separationId = Convert.ToInt32(Request.Form["separationId"]);
            var employeeId = Convert.ToInt32(Request.Form["employeeId"]);

            #region Check Mime Type
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (!IsValidFileName(file.FileName))
                {
                    stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                    stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                    stringBuilder.Append($"I. a to z characters.");
                    stringBuilder.Append($"II. numbers(0 to 9).");
                    stringBuilder.Append($"III. - and _ with space.");
                }
                var contentType = GetFileContentType(file.InputStream);
                var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                if (dicValue != contentType)
                {
                    stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                    stringBuilder.Append("<br>");
                }
            }
            if (stringBuilder.ToString() != "")
            {
                return Json(stringBuilder.ToString());
            }
            #endregion
            #region Save file on floder
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                fname = ExtensionMethods.SetUniqueFileName(file.FileName + "-", Path.GetExtension(file.FileName));
                fileName = fname;
                filename.Add(fileName);
                fullPath = Request.MapPath("~/Document/Separation/" + fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                fname = Path.Combine(Server.MapPath("~/Document/Separation/"), fname);
                file.SaveAs(fname);
            }
            #endregion
            SuperAnnuating obj = new SuperAnnuating
            {
                EmployeeId = employeeId,
                StatusId = (int)SeprationStatus.Upload,
                SeprationId = separationId,
                UpdatedBy = userDetail.UserID,
                UpdatedOn = DateTime.Now,
                DocumentName = fileName

            };
            sepService.UploadDocument(obj);

            return Json(files.Count + " File Uploaded!");
        }
        #endregion

        #region Send for Clearance
        public JsonResult GetEmployeeByDepartment(int departmentID)
        {
            log.Info($"SeparationController/GetEmployeeByDepartment");
            try
            {
                var employeedetails = ddlService.GetEmployeeByDepartmentDesignationID(null, departmentID, null);
                SelectListModel selectemployeeDetails = new SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
                employeedetails.Insert(0, selectemployeeDetails);
                TempData["TempEmployeeList"] = employeedetails;
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
        public ActionResult Clearance()
        {
            log.Info($"SeparationController/Clearance");
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

        public ActionResult _GetClearance(int empId, int sepId)
        {
            log.Info($"SeparationController/_GetClearance/{empId}");
            try
            {
                EmployeeProcessApprovalVM empVM = new EmployeeProcessApprovalVM();
                empVM.empProcess = new EmployeeProcessApproval();
                empVM.empProcess.EmployeeID = empId;
                empVM.empProcess.EmpProcessAppID = sepId;
                ViewBag.Department = ddlService.ddlDepartmentList();
                empVM.EmployeeList = new List<SelectListModel>();
                empVM.EmployeeList.Insert(0, new SelectListModel { id = 0, value = "Select" });
                TempData.Remove("TempEmployeeList");
                TempData.Remove("TempEmpProcessApp");
                return PartialView("_SetReportingForClearance", empVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult _PostClearance(EmployeeProcessApprovalVM empVM, string ButtonType)
        {
            log.Info($"SeparationController/_PostClearance");
            try
            {
                if (ButtonType == "Add")
                {
                    var tememployeeList = (List<SelectListModel>)TempData["TempEmployeeList"];
                    empVM.EmployeeList = tememployeeList;
                    TempData["TempEmployeeList"] = empVM.EmployeeList;
                    TempData.Keep("TempEmployeeList");
                    if (empVM.empProcess.ReportingTo == 0)
                    {
                        var empProcessAprList = (List<EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
                        empVM.empProcessApp = empProcessAprList;
                        TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                        ModelState.AddModelError("ReportingBlankModel", "Please select Reporting To");
                        return Json(new { status = false, htmlData = ConvertViewToString("_SetReportingForClearance", empVM) }, JsonRequestBehavior.AllowGet);
                    }
                    var empProcessApr = (List<EmployeeProcessApproval>)TempData["TempEmpProcessApp"];

                    if (empProcessApr == null || empProcessApr.Count == 0)
                    {
                        empProcessApr = new List<EmployeeProcessApproval>() {
                            new EmployeeProcessApproval() {
                                 sno = 1,
                                 ReportingTo =empVM.empProcess.ReportingTo,
                                 CreatedBy=userDetail.UserID,
                                 CreatedOn =DateTime.Now,
                                  ReportingToName=tememployeeList.Where(x=> x.id==empVM.empProcess.ReportingTo).FirstOrDefault().value,
                            } };
                    }
                    else
                    {
                        if (empProcessApr.Count < 5)
                        {
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
                                TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                                ViewBag.Department = ddlService.ddlDepartmentList();
                                return Json(new { part = 1, htmlData = ConvertViewToString("_TempReportingList", empVM) }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                empProcessApr.Add(new EmployeeProcessApproval()
                                {
                                    sno = empVM.empProcessApp.Count + 1,
                                    ReportingTo = empVM.empProcess.ReportingTo,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    ReportingToName = tememployeeList.Where(x => x.id == empVM.empProcess.ReportingTo).FirstOrDefault().value,
                                });
                            }
                        }
                    }
                    empVM.empProcessApp = empProcessApr;
                    TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                    TempData.Keep("TempEmpProcessApp");
                    ViewBag.Department = ddlService.ddlDepartmentList();
                    return Json(new { part = 1, htmlData = ConvertViewToString("_TempReportingList", empVM) }, JsonRequestBehavior.AllowGet);
                }
                if (ModelState.IsValid)
                {
                    var tememployeeList = (List<SelectListModel>)TempData["TempEmployeeList"];
                    empVM.EmployeeList = tememployeeList;
                    var empProcessAprList = (List<EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
                    empVM.empProcessApp = empProcessAprList;
                    empVM.ProcessId = (int)SeprationStatus.Clearance;
                    empVM.ApprovalType = 1;
                    var result = sepService.SendForClearance(empVM);
                    if (result)
                    {
                        TempData.Remove("TempEmployeeList");
                        TempData.Remove("TempEmpProcessApp");
                        return Json(new { part = 2, msgType = "success", msg = "Successfully send for clearance." }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    empVM.EmployeeList = ddlService.ddlDepartmentList();
                    empVM.EmployeeList.Insert(0, new SelectListModel { id = 0, value = "Select" });
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
            var empProcessApr = (List<EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
            if (empProcessApr != null && empProcessApr.Count > 0)
            {
                bool status = false; string msg = string.Empty; string msgType = string.Empty;
                EmployeeProcessApprovalVM empVM = new EmployeeProcessApprovalVM();
                var deletedRow = empProcessApr.Where(x => x.sno == sNo).FirstOrDefault();
                if (deletedRow != null)
                {
                    empProcessApr.Remove(deletedRow);
                    status = true;
                    msgType = "success";
                }
                empVM.empProcessApp = empProcessApr;
                TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                return Json(new { status = status, msgType = msgType, msg = msg, htmlData = ConvertViewToString("_TempReportingList", empVM) }, JsonRequestBehavior.AllowGet);

            }
            return Content("");

        }
        #endregion

        #region View Clearance Status
        [HttpGet]
        public ViewResult ClearanceApproval(int empId, int sepId)
        {
            log.Info($"SeparationController/View");
            try
            {
                //sepService.IsClearanceApprovalDone(sepId);
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public PartialViewResult GetClearanceStatus(int empId, int sepId, int aprType)
        {
            log.Info("SeparationController/GetClearanceStatus");
            try
            {
                var getApprovalStatus = sepService.GetClearanceApprovalStatus(sepId, aprType);
                return PartialView("_ApprovalStatus", getApprovalStatus);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region Upload Circular Documents
        [HttpGet]
        public ActionResult GetCirularDocument(int sepId, int empId)
        {
            log.Info($"SeparationController/GetCirularDocument");
            try
            {
                SuperAnnuating objSupp = new SuperAnnuating
                {
                    SeprationId = sepId,
                    EmployeeId = empId

                };
                return PartialView("_UploadCircular", objSupp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadCircular()
        {
            HttpFileCollectionBase files = Request.Files;
            string fileName = string.Empty; string filePath = string.Empty;
            string fname = "";
            List<string> filename = new List<string>();
            string fullPath = "";
            TempData["UploadedFiles"] = null;

            var separationId = Convert.ToInt32(Request.Form["separationId"]);
            var employeeId = Convert.ToInt32(Request.Form["employeeId"]);
            var fileNo = Convert.ToString(Request.Form["fileNo"]);
            var referenceNo = Convert.ToString(Request.Form["referenceNo"]);
            var apprDate = Convert.ToDateTime(Request.Form["apprDate"]);

            #region Check Mime Type
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (!IsValidFileName(file.FileName))
                {
                    stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                    stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                    stringBuilder.Append($"I. a to z characters.");
                    stringBuilder.Append($"II. numbers(0 to 9).");
                    stringBuilder.Append($"III. - and _ with space.");
                }
                var contentType = GetFileContentType(file.InputStream);
                var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                if (dicValue != contentType)
                {
                    stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                    stringBuilder.Append("<br>");
                }
            }
            if (stringBuilder.ToString() != "")
            {
                return Json(stringBuilder.ToString());
            }
            #endregion

            #region Save file on floder
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                fname = ExtensionMethods.SetUniqueFileName(file.FileName + "-", Path.GetExtension(file.FileName));
                fileName = fname;
                filename.Add(fileName);
                fullPath = Request.MapPath("~/Document/Separation/" + fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                fname = Path.Combine(Server.MapPath("~/Document/Separation/"), fname);
                file.SaveAs(fname);
            }
            #endregion
            SuperAnnuating obj = new SuperAnnuating
            {
                EmployeeId = employeeId,
                StatusId = (int)SeprationStatus.CircularUplaoded,
                SeprationId = separationId,
                UpdatedBy = userDetail.UserID,
                UpdatedOn = DateTime.Now,
                CircularDocument = fileName,
                FileNo = fileNo,
                ReferenceNo = referenceNo,
                ApprovedDate = apprDate

            };
            sepService.UploadCircular(obj);

            return Json(files.Count + " File Uploaded!");
        }
        #endregion

        [HttpGet]
        public ActionResult DivisionalApproval()
        {
            log.Info($"SeparationController/DivisionalApproval");
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

        public ActionResult _GetDivisionalApproval(int empId, int sepId)
        {
            log.Info($"SeparationController/_GetDivisionalApproval/{empId}");
            try
            {
                EmployeeProcessApprovalVM empVM = new EmployeeProcessApprovalVM();
                empVM.empProcess = new EmployeeProcessApproval();
                empVM.empProcess.EmployeeID = empId;
                empVM.empProcess.EmpProcessAppID = sepId;
                ViewBag.Department = ddlService.ddlDepartmentList();
                empVM.EmployeeList = new List<SelectListModel>();
                empVM.EmployeeList.Insert(0, new SelectListModel { id = 0, value = "Select" });
                TempData.Remove("TempEmployeeList");
                TempData.Remove("TempEmpProcessApp");
                return PartialView("_SetReportingForDivisional", empVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        public ActionResult _PostDivisional(EmployeeProcessApprovalVM empVM, string ButtonType)
        {
            log.Info($"SeparationController/_PostDivisional");
            try
            {
                if (ButtonType == "Add")
                {
                    var tememployeeList = (List<SelectListModel>)TempData["TempEmployeeList"];
                    empVM.EmployeeList = tememployeeList;
                    TempData["TempEmployeeList"] = empVM.EmployeeList;
                    TempData.Keep("TempEmployeeList");
                    if (empVM.empProcess.ReportingTo == 0)
                    {
                        var empProcessAprList = (List<EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
                        empVM.empProcessApp = empProcessAprList;
                        TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                        ModelState.AddModelError("ReportingBlankModel", "Please select Reporting To");
                        return Json(new { status = false, htmlData = ConvertViewToString("_SetReportingForClearance", empVM) }, JsonRequestBehavior.AllowGet);
                    }
                    var empProcessApr = (List<EmployeeProcessApproval>)TempData["TempEmpProcessApp"];

                    if (empProcessApr == null || empProcessApr.Count == 0)
                    {
                        empProcessApr = new List<EmployeeProcessApproval>() {
                            new EmployeeProcessApproval() {
                                 sno = 1,
                                 ReportingTo =empVM.empProcess.ReportingTo,
                                 CreatedBy=userDetail.UserID,
                                 CreatedOn =DateTime.Now,
                                  ReportingToName=tememployeeList.Where(x=> x.id==empVM.empProcess.ReportingTo).FirstOrDefault().value,
                            } };
                    }
                    else
                    {
                        if (empProcessApr.Count < 5)
                        {
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
                                TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                                ViewBag.Department = ddlService.ddlDepartmentList();
                                return Json(new { part = 1, htmlData = ConvertViewToString("_TempReportingList", empVM) }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                empProcessApr.Add(new EmployeeProcessApproval()
                                {
                                    sno = empVM.empProcessApp.Count + 1,
                                    ReportingTo = empVM.empProcess.ReportingTo,
                                    CreatedBy = userDetail.UserID,
                                    CreatedOn = DateTime.Now,
                                    ReportingToName = tememployeeList.Where(x => x.id == empVM.empProcess.ReportingTo).FirstOrDefault().value,
                                });
                            }
                        }
                    }
                    empVM.empProcessApp = empProcessApr;
                    TempData["TempEmpProcessApp"] = empVM.empProcessApp;
                    TempData.Keep("TempEmpProcessApp");
                    ViewBag.Department = ddlService.ddlDepartmentList();
                    return Json(new { part = 1, htmlData = ConvertViewToString("_TempReportingList", empVM) }, JsonRequestBehavior.AllowGet);
                }
                if (ModelState.IsValid)
                {
                    var tememployeeList = (List<SelectListModel>)TempData["TempEmployeeList"];
                    empVM.EmployeeList = tememployeeList;
                    var empProcessAprList = (List<EmployeeProcessApproval>)TempData["TempEmpProcessApp"];
                    empVM.empProcessApp = empProcessAprList;
                    empVM.ProcessId = (int)SeprationStatus.DivisionalApproval;
                    empVM.ApprovalType = 2;
                    var result = sepService.SendForClearance(empVM);
                    if (result)
                    {
                        TempData.Remove("TempEmployeeList");
                        TempData.Remove("TempEmpProcessApp");
                        return Json(new { part = 2, msgType = "success", msg = "Successfully send for approval." }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    empVM.EmployeeList = ddlService.ddlDepartmentList();
                    empVM.EmployeeList.Insert(0, new SelectListModel { id = 0, value = "Select" });
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

        public ActionResult Resignation()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ResignationApply()
        {
            BindDropDown();
            Resignation resignation = new Nafed.MicroPay.Model.Resignation();
            int? statuId = sepService.CheckForResignation((int)userDetail.EmployeeID);
            if (statuId != null)
            {
                resignation.StatusId = statuId;
                return View("ApplyResignation", resignation);
            }
            var getEmpDtls = sepService.GetEmployeeDetail((int)userDetail.EmployeeID);

            if (getEmpDtls != null)
            {
                resignation = getEmpDtls;
                var noticePeriod = CalculateNoticePeriod(resignation.Pr_Loc_DOJ);
                resignation.NoticePeriod = noticePeriod;
            }

            var approvalSettings = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.LeaveApproval);
            if (approvalSettings != null)
            {
                resignation._ProcessWorkFlow = new ProcessWorkFlow();
                resignation._ProcessWorkFlow.ReceiverID = approvalSettings.ReportingTo;
            }
            return View("ApplyResignation", resignation);
        }

        int CalculateNoticePeriod(DateTime Pr_loc_Date)
        {
            int noticeDays = 0;
            var calcTotlDays = Convert.ToInt32((DateTime.Now.Date - Pr_loc_Date.Date).TotalDays);
            if (calcTotlDays <= 90)
            {
                noticeDays = 1;

            }
            else if (calcTotlDays < 365 && calcTotlDays > 90)
            {
                noticeDays = 14;

            }
            if (calcTotlDays >=365)
            {
                noticeDays = 30;

            }
            return noticeDays;           
        }
        void BindDropDown()
        {
            List<SelectListModel> ddlReason = new List<SelectListModel>();
            ddlReason.Add(new SelectListModel { id = 1, value = "Better opportunity" });
            ddlReason.Add(new SelectListModel { id = 2, value = "Further studies" });
            ddlReason.Add(new SelectListModel { id = 3, value = "Personal reason" });
            ddlReason.Add(new SelectListModel { id = 4, value = "Other" });
            ViewBag.ddlReason = ddlReason;
        }
        [HttpPost]
        public ActionResult ResignationApply(Resignation resignation)
        {
            BindDropDown();
            if (ModelState.IsValid)
            {
                HttpFileCollectionBase files = Request.Files;
                string fileName = string.Empty; string filePath = string.Empty;
                string fname = "";
                List<string> filename = new List<string>();
                string fullPath = "";

                #region Check Mime Type
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    if (!IsValidFileName(file.FileName))
                    {
                        stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                        stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                        stringBuilder.Append($"I. a to z characters.");
                        stringBuilder.Append($"II. numbers(0 to 9).");
                        stringBuilder.Append($"III. - and _ with space.");
                    }
                    var contentType = GetFileContentType(file.InputStream);
                    var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                    if (dicValue != contentType)
                    {
                        stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                        stringBuilder.Append("<br>");
                    }
                }
                if (stringBuilder.ToString() != "")
                {
                    TempData["Error"] = stringBuilder.ToString();
                    return View("ApplyResignation", resignation);
                }
                #endregion

                #region Save file on floder
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    fname = ExtensionMethods.SetUniqueFileName(file.FileName + "-", Path.GetExtension(file.FileName));
                    fileName = fname;
                    filename.Add(fileName);
                    fullPath = Request.MapPath("~/Document/Separation/" + fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    fname = Path.Combine(Server.MapPath("~/Document/Separation/"), fname);
                    file.SaveAs(fname);
                }
                #endregion

                resignation.EmployeeId = (int)userDetail.EmployeeID;
                resignation.StatusId = 1;
                resignation.CreatedBy = userDetail.UserID;
                resignation.CreatedOn = DateTime.Now;
                resignation.DocName = fileName;
                ProcessWorkFlow workFlow = null;
                workFlow = new ProcessWorkFlow()
                {
                    SenderID = userDetail.EmployeeID,
                    ReceiverID = resignation._ProcessWorkFlow.ReceiverID,
                    SenderDepartmentID = userDetail.DepartmentID,
                    SenderDesignationID = userDetail.DesignationID,
                    CreatedBy = userDetail.UserID,
                    EmployeeID = (int)userDetail.EmployeeID,
                    Scomments = "Resignation submitted",
                    ProcessID = (int)WorkFlowProcess.Separation,
                    StatusID = (int)EmpLeaveStatus.Pending
                };
                resignation._ProcessWorkFlow = workFlow;
                var res = sepService.Resignation(resignation);
                if (res)
                {
                    TempData["Message"] = "Resignation submitted successfully.";
                    return RedirectToAction("Resignation");
                }
                else
                {
                    TempData["Error"] = "Error while applying for Resignation.";
                    return View("ApplyResignation", resignation);
                }
            }
            else
                return View("ApplyResignation", resignation);
        }
    }
}