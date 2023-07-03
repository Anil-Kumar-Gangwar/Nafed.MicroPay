using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.IO;
using Common = Nafed.MicroPay.Common;
using static Nafed.MicroPay.Common.DocumentUploadFilePath;
using static Nafed.MicroPay.Common.FileHelper;
using System.Data;
using Nafed.MicroPay.Common;
using System.Text;

namespace MicroPay.Web.Controllers
{
    public class EmployeeLeaveController : BaseController
    {
        private readonly IEmployeeLeaveService leaveService;
        private readonly IDropdownBindService ddlService;
        private readonly IDesignationService designationService;

        private readonly IEmployeeService employeeService;
        private readonly ILeaveService lvService;

        public EmployeeLeaveController(IEmployeeLeaveService leaveService, IDropdownBindService ddlService, IDesignationService designationService,
            IEmployeeService employeeService, ILeaveService lvService)
        {
            this.leaveService = leaveService;
            this.ddlService = ddlService;
            this.designationService = designationService;
            this.employeeService = employeeService;
            this.lvService = lvService;
        }
        public ActionResult Index()
        {
            log.Info($"EmployeeLeaveController/Index");
            return View(userAccessRight);
        }

        public ActionResult LeaveApplicationGridView(Model.EmployeeLeave empLeave)
        {
            log.Info($"EmployeeLeaveController/EmployeeLeaveGridView");
            try
            {
                EmployeeLeaveViewModel LAVM = new EmployeeLeaveViewModel();
                empLeave.EmployeeId = (int)userDetail.EmployeeID;
                LAVM.GetEmployeeLeaveList = leaveService.GetEmployeeLeaveList(empLeave);
                LAVM.userRights = userAccessRight;
                return PartialView("_LeaveApplicationGridView", LAVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create(int? EmployeeId)
        {
            log.Info($"EmployeeLeaveController/_Create");
            try
            {

                Model.EmployeeLeave objEmployeeLeave = new Model.EmployeeLeave();
                var empDetails = (dynamic)null;
                if (EmployeeId.HasValue)
                {
                    empDetails = employeeService.GetEmployeeByID(EmployeeId.Value);
                    objEmployeeLeave.RequestType = Convert.ToInt32(Request.QueryString["RequestType"]);

                }
                else
                {
                    empDetails = employeeService.GetEmployeeByID((int)userDetail.EmployeeID);
                    //empDetails = genericRepo.GetByID<DataModel.Models.tblMstEmployee>(userDetail.EmployeeID);
                }

                objEmployeeLeave.Branch = empDetails.BranchName;
                objEmployeeLeave.EmployeeName = empDetails.Name;
                objEmployeeLeave.DesignationName = empDetails.DesignationName;
                objEmployeeLeave.EmployeeCode = empDetails.EmployeeCode;
                objEmployeeLeave.EmployeeId = empDetails.EmployeeID;
                objEmployeeLeave.DesignationID = empDetails.DesignationID;
                objEmployeeLeave.EmployeeTypeID = empDetails.EmployeeTypeID;
                var desigLevel = designationService.GetDesignationByID((int)empDetails.DesignationID).Level;
                // if (!string.IsNullOrEmpty(desigLevel))
                BindDropdowns(desigLevel, (int)objEmployeeLeave.EmployeeTypeID);

                var empReportingLevels = GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.LeaveApproval);
                if (empReportingLevels != null)
                {
                    var reportingLevel = 1;
                    if (empReportingLevels.ReviewingTo.HasValue)
                        reportingLevel = reportingLevel + 1;
                    if (empReportingLevels.AcceptanceAuthority.HasValue)
                        reportingLevel = reportingLevel + 1;
                    objEmployeeLeave.ApprovalRequiredUpto = reportingLevel;
                }

                //if (empReportings != null)
                //{
                //    int R_OfficerID = empReportings.ReportingTo > 0 ? empReportings.ReportingTo : 0;

                //    var employeeReporting = genericRepo.GetByID<DataModel.Models.tblMstEmployee>(R_OfficerID);
                //    if (empReportings.ReviewingTo > 0)
                //    {
                //        var empReviewer = genericRepo.GetByID<DataModel.Models.tblMstEmployee>(empReportings.ReviewingTo);
                //        objEmployeeLeave.ReviewerName = empReviewer.Name;
                //    }
                //    if (employeeReporting != null)
                //    {
                //        objEmployeeLeave.ReportingOfficerName = employeeReporting.Name;
                //        objEmployeeLeave.ReportingOfficeContactNumber = employeeReporting.MobileNo;
                //        objEmployeeLeave.ReportingOfficeAddress = employeeReporting.PAdd;
                //    }
                //}

                objEmployeeLeave.FromdayType = Model.DayType.FullDay;
                objEmployeeLeave.TodayType = Model.DayType.FullDay;
                var currentYear = DateTime.UtcNow.Year;
                //Get data from SP
                var empleavelist = leaveService.GetEmployeeLeaveDetails(empDetails.EmployeeCode, Convert.ToString(System.DateTime.Now.Year));
                DataTable DT = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(empleavelist);
                Session["dtdata"] = DT;

                //

                objEmployeeLeave.empLeaveBalance = new Model.EmpLeaveBalance();
                if (DT.Rows.Count > 0)
                {
                    objEmployeeLeave.empLeaveBalance.ELOpeningBal = Convert.ToDouble(DT.Rows[1]["OB"].ToString());
                    objEmployeeLeave.empLeaveBalance.MLOpeningBal = Convert.ToDouble(DT.Rows[2]["OB"].ToString());
                    objEmployeeLeave.empLeaveBalance.CLOpeningBal = Convert.ToDouble(DT.Rows[0]["OB"].ToString());
                    objEmployeeLeave.empLeaveBalance.MEOpeningBal = Convert.ToDouble(DT.Rows[3]["OB"].ToString());

                    objEmployeeLeave.empLeaveBalance.CLBal = Convert.ToDouble(DT.Rows[0]["Bal"].ToString());
                    objEmployeeLeave.empLeaveBalance.ELBal = Convert.ToDouble(DT.Rows[1]["Bal"].ToString());
                    objEmployeeLeave.empLeaveBalance.MLBal = Convert.ToDouble(DT.Rows[2]["Bal"].ToString());
                    objEmployeeLeave.empLeaveBalance.MEBal = Convert.ToDouble(DT.Rows[3]["Bal"].ToString());
                }
                else
                {
                    objEmployeeLeave.empLeaveBalance.ELOpeningBal = 0;
                    objEmployeeLeave.empLeaveBalance.MLOpeningBal = 0;
                    objEmployeeLeave.empLeaveBalance.CLOpeningBal = 0;
                    objEmployeeLeave.empLeaveBalance.MEOpeningBal = 0;
                }

                if (DT.Rows.Count > 0)
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.Rows[i]["LeaveCategoryID"].ToString() == "4")
                        {
                            objEmployeeLeave.empLeaveBalance.ELAccrued = Convert.ToDouble(DT.Rows[i]["CR"].ToString());
                            objEmployeeLeave.empLeaveBalance.ELAvailed = Convert.ToDouble(DT.Rows[i]["DR"].ToString());
                            var ElBal = objEmployeeLeave.empLeaveBalance.ELAccrued;
                        }
                        else if (DT.Rows[i]["LeaveCategoryID"].ToString() == "8")
                        {
                            objEmployeeLeave.empLeaveBalance.MLAccrued = Convert.ToDouble(DT.Rows[i]["CR"].ToString());
                            objEmployeeLeave.empLeaveBalance.MLAvailed = Convert.ToDouble(DT.Rows[i]["DR"].ToString());
                        }
                        else if (DT.Rows[i]["LeaveCategoryID"].ToString() == "2")
                        {
                            objEmployeeLeave.empLeaveBalance.CLAccrued = Convert.ToDouble(DT.Rows[i]["CR"].ToString());
                            objEmployeeLeave.empLeaveBalance.CLAvailed = Convert.ToDouble(DT.Rows[i]["DR"].ToString());
                        }
                        else if (DT.Rows[i]["LeaveCategoryID"].ToString() == "21")
                        {
                            objEmployeeLeave.empLeaveBalance.MEAccrued = Convert.ToDouble(DT.Rows[i]["CR"].ToString());
                            objEmployeeLeave.empLeaveBalance.MEAvailed = Convert.ToDouble(DT.Rows[i]["DR"].ToString());
                        }
                    }
                }
                else
                {
                    objEmployeeLeave.empLeaveBalance.ELAccrued = 0;
                    objEmployeeLeave.empLeaveBalance.ELAvailed = 0;

                    objEmployeeLeave.empLeaveBalance.MLAccrued = 0;
                    objEmployeeLeave.empLeaveBalance.MLAvailed = 0;

                    objEmployeeLeave.empLeaveBalance.CLAccrued = 0;
                    objEmployeeLeave.empLeaveBalance.CLAvailed = 0;

                    objEmployeeLeave.empLeaveBalance.MEAccrued = 0;
                    objEmployeeLeave.empLeaveBalance.MEAvailed = 0;
                }
                return View(objEmployeeLeave);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.EmployeeLeave createEmpLeave, HttpPostedFileBase file, FormCollection frm)
        {
            var empReportings = GetEmpProcessApprovalSetting((int)createEmpLeave.EmployeeId, WorkFlowProcess.LeaveApproval);


            Model.EmployeeLeave objEmployeeLeave = new Model.EmployeeLeave();
            objEmployeeLeave.empLeaveBalance = new Model.EmpLeaveBalance();
            log.Info($"EmployeeLeaveController/Create");

            var elBalance = Convert.ToDouble(Request.Form["EL_OBBalance"]);
            var mlBalance = Convert.ToDouble(Request.Form["ML_OBBalance"]);
            var clBalance = Convert.ToDouble(Request.Form["CL_Balance"]);
            var meBalance = Convert.ToDouble(Request.Form["ME_Balance"]);

            var dt_LeaveBalDtls = (DataTable)Session["dtdata"];
            createEmpLeave.empLeaveBalance = new Model.EmpLeaveBalance();

            createEmpLeave.empLeaveBalance.CLBal = clBalance;
            createEmpLeave.empLeaveBalance.MLBal = mlBalance;
            createEmpLeave.empLeaveBalance.ELBal = elBalance;
            createEmpLeave.empLeaveBalance.MEBal = meBalance;
            if (createEmpLeave.DateTo_DayType == 1)
                createEmpLeave.TodayType = Nafed.MicroPay.Model.DayType.HalfDay;
            else
                createEmpLeave.TodayType = Nafed.MicroPay.Model.DayType.FullDay;

            if (createEmpLeave.DateFrom_DayType == 1)
                createEmpLeave.FromdayType = Nafed.MicroPay.Model.DayType.HalfDay;
            else
                createEmpLeave.FromdayType = Nafed.MicroPay.Model.DayType.FullDay;

            if (!createEmpLeave.DateFrom.HasValue || !createEmpLeave.DateTo.HasValue)
            {
                createEmpLeave.TodayType = Nafed.MicroPay.Model.DayType.FullDay;
                createEmpLeave.FromdayType = Nafed.MicroPay.Model.DayType.FullDay;
                createEmpLeave.DateTo_DayType = 2;
                createEmpLeave.DateFrom_DayType = 2;
                if (createEmpLeave.LeaveCategoryID != 18)
                {
                    createEmpLeave.Unit = 0;
                }

            }

            if (dt_LeaveBalDtls.Rows.Count > 0)
            {
                createEmpLeave.empLeaveBalance.ELOpeningBal = (double?)Convert.ToDouble(dt_LeaveBalDtls.Rows[1]["OB"].ToString());
                createEmpLeave.empLeaveBalance.MLOpeningBal = Convert.ToDouble(dt_LeaveBalDtls.Rows[2]["OB"].ToString());
                createEmpLeave.empLeaveBalance.CLOpeningBal = Convert.ToDouble(dt_LeaveBalDtls.Rows[0]["OB"].ToString());
                createEmpLeave.empLeaveBalance.MEOpeningBal = Convert.ToDouble(dt_LeaveBalDtls.Rows[0]["OB"].ToString());
            }
            else
            {
                createEmpLeave.empLeaveBalance.ELOpeningBal = 0;
                createEmpLeave.empLeaveBalance.MLOpeningBal = 0;
                createEmpLeave.empLeaveBalance.CLOpeningBal = 0;
                createEmpLeave.empLeaveBalance.MEOpeningBal = 0;
            }
            if (dt_LeaveBalDtls.Rows.Count > 0)
            {
                for (int i = 0; i < dt_LeaveBalDtls.Rows.Count; i++)
                {
                    if (dt_LeaveBalDtls.Rows[i]["LeaveCategoryID"].ToString() == "4")
                    {
                        createEmpLeave.empLeaveBalance.ELAccrued = Convert.ToDouble(dt_LeaveBalDtls.Rows[i]["CR"].ToString());
                        createEmpLeave.empLeaveBalance.ELAvailed = Convert.ToDouble(dt_LeaveBalDtls.Rows[i]["DR"].ToString());
                        var ElBal = createEmpLeave.empLeaveBalance.ELAccrued;
                    }
                    else if (dt_LeaveBalDtls.Rows[i]["LeaveCategoryID"].ToString() == "8")
                    {
                        createEmpLeave.empLeaveBalance.MLAccrued = Convert.ToDouble(dt_LeaveBalDtls.Rows[i]["CR"].ToString());
                        createEmpLeave.empLeaveBalance.MLAvailed = Convert.ToDouble(dt_LeaveBalDtls.Rows[i]["DR"].ToString());
                    }
                    else if (dt_LeaveBalDtls.Rows[i]["LeaveCategoryID"].ToString() == "2")
                    {
                        createEmpLeave.empLeaveBalance.CLAccrued = Convert.ToDouble(dt_LeaveBalDtls.Rows[i]["CR"].ToString());
                        createEmpLeave.empLeaveBalance.CLAvailed = Convert.ToDouble(dt_LeaveBalDtls.Rows[i]["DR"].ToString());
                    }
                    else if (dt_LeaveBalDtls.Rows[i]["LeaveCategoryID"].ToString() == "21")
                    {
                        createEmpLeave.empLeaveBalance.MEAccrued = Convert.ToDouble(dt_LeaveBalDtls.Rows[i]["CR"].ToString());
                        createEmpLeave.empLeaveBalance.MEAvailed = Convert.ToDouble(dt_LeaveBalDtls.Rows[i]["DR"].ToString());
                    }
                }
            }
            else
            {
                createEmpLeave.empLeaveBalance.ELAccrued = 0;
                createEmpLeave.empLeaveBalance.ELAvailed = 0;
                createEmpLeave.empLeaveBalance.MLAccrued = 0;
                createEmpLeave.empLeaveBalance.MLAvailed = 0;
                createEmpLeave.empLeaveBalance.CLAccrued = 0;
                createEmpLeave.empLeaveBalance.CLAvailed = 0;
                createEmpLeave.empLeaveBalance.MEAccrued = 0;
                createEmpLeave.empLeaveBalance.MEAvailed = 0;
            }
            try
            {
                if (createEmpLeave.LeaveCategoryID == 18)
                {
                    ModelState.Remove("DateFrom");
                    ModelState.Remove("DateTo");

                }
                var desigLevel = designationService.GetDesignationByID((int)createEmpLeave.DesignationID).Level;
                if (!string.IsNullOrEmpty(desigLevel))
                    BindDropdowns(desigLevel, (int)createEmpLeave.EmployeeTypeID);
                //BindDropdowns(Convert.ToInt32(desigLevel));
                if (empReportings == null)
                {
                    TempData["Error"] = "You can not apply leave right now because either your Reporting Manager or Reviewing Manager is not set.";
                    return View(createEmpLeave);
                }
                if (createEmpLeave.DateTo < createEmpLeave.DateFrom)
                {
                    ModelState.AddModelError("DateToValidation", "To Date can not be less than From Date");
                    return View(createEmpLeave);
                }
                if (createEmpLeave.LeaveCategoryID == 0)
                {
                    ModelState.AddModelError("LeaveCategoryRequired", "Please Select Leave Type");
                    return View(createEmpLeave);
                }
                if (createEmpLeave.Reason == "")
                {
                    ModelState.AddModelError("ReasonRequired", "Please Enter Reason");
                    return View(createEmpLeave);
                }
                //if (userDetail.ReportingTo == null && userDetail.ReviwerTo == null)
                //if (empReportings.ReportingTo == 0)
                //{
                //    TempData["Error"] = "You can not apply leave right now because either your Reporting Manager is not set.";
                //    return View(createEmpLeave);
                //}

                if (ModelState.IsValid)
                {
                    var chkMapLeave = new List<Model.EmployeeLeave>();
                    DataTable DT = new DataTable();
                    if (createEmpLeave.LeaveCategoryID != 18)
                    {
                        chkMapLeave = leaveService.checkMapLeave(createEmpLeave.LeaveCategoryID, (DateTime)createEmpLeave.DateFrom, (int)userDetail.EmployeeID);
                        DT = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(chkMapLeave);


                        // var leaveCategoryList = genericRepo.GetByID<DataModel.Models.LeaveCategory>(createEmpLeave.LeaveCategoryID);

                        if (DT.Rows[0]["mapleave"].ToString() == "False")
                        {
                            TempData["Error"] = "Dear Employee, You can not take continuous leave with this leave category. You already applied leave which end date is " + Convert.ToDateTime(createEmpLeave.DateTo).ToShortDateString() + " to verify please check in your leave history.";
                            return View(createEmpLeave);
                        }
                    }
                    var leaveCategoryList = lvService.GetLeaveCategoryByID(createEmpLeave.LeaveCategoryID);
                    if (createEmpLeave.LeaveCategoryID == 8 || createEmpLeave.LeaveCategoryID == 13)
                    {
                        if (file == null)
                        {
                            ModelState.AddModelError("DocumentRequired", "Please upload document");
                            return View(createEmpLeave);
                        }
                        else
                        {
                            var contentType = GetFileContentType(file.InputStream);
                            var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));

                            if (!IsValidFileName(file.FileName))
                            {
                                TempData["Error"] = $"File name must not contain special characters.In File Name,Only following characters are allowed.<br />1. a to z characters.<br/>2. numbers(0 to 9). <br />3. - and _ with space.";
                                return View(createEmpLeave);
                            }

                            if (dicValue != contentType)
                            {
                                ModelState.AddModelError("", "File format is incorrect.");
                                TempData["Error"] = "File format is incorrect.";
                                return View(createEmpLeave);
                            }
                            int iFileSize = file.ContentLength;
                            if (iFileSize > 10734124)  // 2MB
                            {
                                // File exceeds the file maximum size
                                ModelState.AddModelError("DocumentRequired", "File size exceeds maximum limit.");
                                return View(createEmpLeave);
                            }
                        }
                    }
                    //else
                    //{
                    createEmpLeave.empLeaveBalance.MLBal = mlBalance;
                    objEmployeeLeave.empLeaveBalance.CLBal = clBalance;
                    objEmployeeLeave.empLeaveBalance.ELBal = elBalance;
                    objEmployeeLeave.empLeaveBalance.MLBal = meBalance;
                    if (leaveCategoryList != null)
                    {

                        if (createEmpLeave.LeaveCategoryID == 2)
                        {
                            var currentYear = DateTime.UtcNow.Year;
                            var appliedDateYear = createEmpLeave.DateTo.Value.Year;
                            if (appliedDateYear > currentYear)
                            {
                                if (leaveCategoryList.MaxLeave > 0)
                                {
                                    if (createEmpLeave.Unit > leaveCategoryList.MaxLeave)
                                    {
                                        TempData["Error"] = "You can not apply more than '" + leaveCategoryList.MaxLeave + "' days.";
                                        return View(createEmpLeave);
                                    }
                                }
                                else
                                {
                                    if (createEmpLeave.Unit > 14)
                                    {
                                        TempData["Error"] = "You can not apply more than 14 days.";
                                        return View(createEmpLeave);
                                    }
                                }
                            }
                            else if (appliedDateYear == currentYear)
                            {
                                if (createEmpLeave.Unit > Convert.ToDecimal(clBalance))
                                {
                                    TempData["Error"] = "You can not apply more than " + clBalance + " days, due to insufficient balance.";
                                    return View(createEmpLeave);
                                }
                            }
                        }
                        else
                        {

                            if (leaveCategoryList.MaxLeave > 0)
                            {
                                if (createEmpLeave.Unit > leaveCategoryList.MaxLeave)
                                {
                                    TempData["Error"] = "You can not apply more than " + leaveCategoryList.MaxLeave + " days, In this leave category.";
                                    return View(createEmpLeave);
                                }
                                else
                                {
                                    var currentYear = DateTime.UtcNow.Year;
                                    //    var empleaveDetails = genericRepo.Get<DataModel.Models.tblLeaveBal>(x => x.EmpCode == createEmpLeave.EmployeeCode &&
                                    //x.LeaveYear == currentYear.ToString()).FirstOrDefault();

                                    // var empleaveAccDetails = genericRepo.Get<DataModel.Models.tblLeaveTran>(x => x.EmpCode == createEmpLeave.EmployeeCode).Where(x => !x.CurrDate.HasValue ? 1 > 0 : (x.CurrDate.Value.Year == currentYear)).Select(x => new { EmpCode = x.EmpCode, LeaveType = x.LeaveType, totalAccrued = x.AccruedLeave, totalAvailed = x.Unit }).GroupBy(x => new { x.EmpCode, x.LeaveType }, (k, v) => new { k.LeaveType, totalOpening = v.Sum(q => q.totalAccrued), totalAvailed = v.Sum(q => q.totalAvailed) }).ToList();
                                }
                            }
                            else if (leaveCategoryList.MinLeave > 0 && createEmpLeave.Unit < leaveCategoryList.MinLeave)
                            {
                                TempData["Error"] = "Minimum " + leaveCategoryList.MinLeave + " days leave you have to apply, In this leave category.";
                                return View(createEmpLeave);
                            }
                        }
                    }
                    //}

                    bool chkdata = false;
                    if (createEmpLeave.LeaveCategoryID != 18)
                    {
                        chkdata = leaveService.LeaveDataExists(Convert.ToInt32(createEmpLeave.EmployeeId), Convert.ToDateTime(createEmpLeave.DateFrom), Convert.ToDateTime(createEmpLeave.DateTo), createEmpLeave.LeaveCategoryID);
                    }
                    if (chkdata)
                    {
                        TempData["Error"] = "Dear Employee, You already applied leave with these dates or within these days.";
                        return RedirectToAction("Create");

                    }
                    if (createEmpLeave.LeaveCategoryID == 4)
                    {
                        if (createEmpLeave.Unit > Convert.ToDecimal(elBalance))
                        {
                            TempData["Error"] = "You can not apply more than " + elBalance + " days, due to insufficient balance.";
                            return View(createEmpLeave);
                        }
                    }
                    if (createEmpLeave.LeaveCategoryID == 8)
                    {
                        if (createEmpLeave.Unit > Convert.ToDecimal(mlBalance))
                        {
                            TempData["Error"] = "You can not apply more than " + mlBalance + " days, due to insufficient balance.";
                            return View(createEmpLeave);
                        }
                    }
                    if (createEmpLeave.LeaveCategoryID == 18)
                    {
                        var chkdateforLE = leaveService.chkdateforLE(createEmpLeave.EmployeeId);
                        if (chkdateforLE.Count > 0)
                        {
                            DataTable DT1 = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(chkdateforLE);
                            if (DT1.Rows[0]["DateFrom"].ToString() != "")
                            {
                                if (Convert.ToDateTime(DT1.Rows[0]["DateFrom"].ToString()) > Convert.ToDateTime(DateTime.Now.Date.ToString()))
                                {
                                    TempData["Error"] = "You can apply Leave Encashment in six month gap only. ";
                                    return View(createEmpLeave);

                                }
                            }
                        }
                        if (createEmpLeave.LeaveCategoryID == 18)
                        {
                            if ((Convert.ToDecimal(elBalance) - (createEmpLeave.Unit)) < 30)
                            {
                                TempData["Error"] = "You can not apply Leave Encashment with this units, Your EL balance should be 30 after no of units apply. ";
                                return View(createEmpLeave);
                            }
                        }
                    }
                    var empDetails = employeeService.GetEmployeeByID(createEmpLeave.EmployeeId);
                    createEmpLeave.EmployeeCode = empDetails.EmployeeCode;
                    //createEmpLeave.EmployeeId = Convert.ToInt32(userDetail.EmployeeID.HasValue ? userDetail.EmployeeID : null);
                    createEmpLeave.EmployeeId = createEmpLeave.EmployeeId;
                    createEmpLeave.DateFrom = createEmpLeave.DateFrom;
                    createEmpLeave.DateTo = createEmpLeave.DateTo;
                    createEmpLeave.Unit = createEmpLeave.Unit;
                    createEmpLeave.LeaveCategoryID = createEmpLeave.LeaveCategoryID;
                    if (createEmpLeave.EmployeeId == userDetail.EmployeeID)
                    {
                        createEmpLeave.StatusID = 1;
                    }
                    else
                    {
                        createEmpLeave.StatusID = 8;
                    }
                    createEmpLeave.Reason = createEmpLeave.Reason;
                    createEmpLeave.ReportingOfficer = userDetail.ReportingTo;
                    createEmpLeave.LeaveBalance = createEmpLeave.LeaveBalance;
                    //createEmpLeave.LeavePurposeID = createEmpLeave.LeavePurposeID;
                    createEmpLeave.LeavePurposeID = 1;
                    createEmpLeave.ReviewerTo = userDetail.ReviwerTo;
                    createEmpLeave.CreatedBy = userDetail.UserID;
                    createEmpLeave.CreatedOn = DateTime.Now;

                    int emptypeID = empDetails.EmployeeTypeID;

                    if (emptypeID == 1)
                    {
                        if (createEmpLeave.LeaveCategoryID != 2)
                        {
                            TempData["Error"] = "You are not eligible for this Leave type.";
                            return View(createEmpLeave);
                        }
                    }

                    if (file != null)
                    {
                        createEmpLeave.DocumentName =
                            Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(file.FileName),
                            Path.GetExtension(file.FileName));
                        createEmpLeave.DocumentFilePath = Server.MapPath("~" + DocumentFilePath + "/" + createEmpLeave.DocumentName);
                    }
                    if (createEmpLeave.EmployeeId == userDetail.EmployeeID)
                    {
                        Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                        {
                            SenderID = empReportings.EmployeeID,
                            ReceiverID = empReportings.ReportingTo,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = (int)userDetail.EmployeeID,
                            Scomments = createEmpLeave.Reason,
                            ProcessID = (int)WorkFlowProcess.LeaveApproval,
                            StatusID = (int)EmpLeaveStatus.Pending

                        };
                        createEmpLeave._ProcessWorkFlow = workFlow;
                    }
                    else
                    {
                        Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                        {
                            SenderID = userDetail.EmployeeID,
                            ReceiverID = empReportings.ReportingTo,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = (int)createEmpLeave.EmployeeId,
                            Scomments = createEmpLeave.Reason,
                            ProcessID = (int)WorkFlowProcess.LeaveApproval,
                            StatusID = (int)EmpLeaveStatus.Approved
                        };
                        createEmpLeave._ProcessWorkFlow = workFlow;
                    }

                    int leaveID = leaveService.InsertEmployeeLeave(createEmpLeave);
                    //int leaveID = 1;
                    // bool isSaveLeaveBal = leaveService.InsertEmployeeLeaveBal(createEmpLeave);

                    if (leaveID > 0)
                    {
                        bool isSaveLeaveTrans = leaveService.InsertLeaveTrans(createEmpLeave, leaveID);
                        var leaveType = frm["hdnleaveType"].ToString();
                        if (!createEmpLeave.RequestType.HasValue)
                        {
                            leaveService.SenderSendMail(empReportings.EmployeeID, leaveType, createEmpLeave, "LeaveApply");
                            leaveService.RecieverSendMail(empReportings.ReportingTo, leaveType, createEmpLeave, "LeaveApply");
                        }
                        if (file != null)
                            SaveFile(file, createEmpLeave.DocumentName);


                        if (createEmpLeave.RequestType.HasValue)
                        {
                            TempData["Message"] = "Leave Adjust Successfully.";
                            return RedirectToAction("Index", "LeaveAdjustment");
                        }
                        else
                        {
                            TempData["Message"] = "Leave Successfully Applied.";
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(createEmpLeave);
        }

        //public int? GetDependentonleave(int leavecategoryID)
        //{
        //    log.Info($"EmployeeService/GetDependentonleave/LeaveCategoryID={leavecategoryID}");
        //   // return genericRepo.GetByID<Nafed.MicroPay.Data.Models.LeaveCategory>(leavecategoryID).DependentOnLeaveBal;
        //}

        private void BindDropdowns(string employeeLevel, int emptypeID)
        {
            var LeaveCategoryList = ddlService.ddlLeaveCategoryList((int?)userDetail.GenderID, employeeLevel, emptypeID);
            LeaveCategoryList = LeaveCategoryList.OrderBy(x => x.value).ToList();

            Model.SelectListModel selectLeaveCat = new Model.SelectListModel();
            selectLeaveCat.id = 0;
            selectLeaveCat.value = "Select";
            LeaveCategoryList.Insert(0, selectLeaveCat);
            ViewBag.LeaveCategory = new SelectList(LeaveCategoryList, "id", "value");

            List<Model.SelectListModel> LeavePurposeList = new List<Model.SelectListModel>();
            LeavePurposeList.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            ViewBag.LeavePurpose = new SelectList(LeavePurposeList, "id", "value");

        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            try
            {
                var employeedetails = ddlService.employeeByBranchID(branchID);
                Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "Select";
                employeedetails.Insert(0, selectemployeeDetails);
                var emp = new SelectList(employeedetails, "id", "value");
                //ViewBag.EmployeeDetails = new SelectList(employeedetails, "id", "value");
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        //public JsonResult GetReporting(int EmpID)
        //{
        //    try
        //    {
        //        var emp = genericRepo.Get<Nafed.MicroPay.Data.Models.tblMstEmployee>(x => x.EmployeeId == EmpID);

        //        int? R_OfficerID = emp.FirstOrDefault().ReportingTo.HasValue ? emp.FirstOrDefault().ReportingTo : null;

        //        var employeeReporting = ddlService.employeeReportingByID(R_OfficerID);
        //        Model.SelectListModel selectReportingDetails = new Model.SelectListModel();
        //        selectReportingDetails.id = 0;
        //        selectReportingDetails.value = "Select";
        //        employeeReporting.Insert(0, selectReportingDetails);
        //        var empReporting = new SelectList(employeeReporting, "id", "value");
        //        return Json(new { Reporting = empReporting }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        TempData["Error"] = "Error-Some error occurs please contact administrator";
        //        throw ex;
        //    }
        //}

        public JsonResult GetPupose(int LeaveCatID)
        {
            try
            {
                var leaveCategoryList = lvService.GetLeaveCategoryByID(LeaveCatID);
                var LeavePupose = ddlService.employeeLeavePurposeByID(LeaveCatID);
                var leaveCategoryGuidlines = leaveService.GetLeaveCategoryGuidlines(LeaveCatID).FirstOrDefault().leaveGuidelines;
                Model.SelectListModel selectLeavePuposeDetails = new Model.SelectListModel();
                selectLeavePuposeDetails.id = 0;
                selectLeavePuposeDetails.value = "Select";
                LeavePupose.Insert(0, selectLeavePuposeDetails);
                var empLeavePupose = new SelectList(LeavePupose, "id", "value");
                return Json(new { LeavePurposeDetail = empLeavePupose, MaxCFUnit = leaveCategoryList.MaxCFUnit, LeaveCategoryGuidlines = leaveCategoryGuidlines }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        private bool SaveFile(HttpPostedFileBase file, string DocumentName)
        {
            log.Info($"EmployeeLeaveController/SaveFile");

            string fileName = string.Empty; string filePath = string.Empty;
            try
            {
                if (file != null)
                {
                    #region Check Mime Type
                    if (!IsValidFileName(file.FileName))
                    {
                        return false;
                    }

                    //var contentType = GetFileContentType(file.InputStream);
                    //var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                    //if (dicValue != contentType)
                    //{
                    //    return false;
                    //}
                    #endregion

                    fileName = Path.GetFileName(file.FileName);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        fileName = DocumentName;
                        filePath = Common.DocumentUploadFilePath.DocumentFilePath;
                        string uploadedFilePath = Server.MapPath(filePath);
                        string sPhysicalPath = Path.Combine(uploadedFilePath, fileName);
                        file.SaveAs(sPhysicalPath);

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
            return true;
        }

        public ActionResult Withdraw(int LeaveID)
        {
            log.Info($"EmployeeLeaveController/Create");
            try
            {
                var leavetrans = leaveService.GetLeaveTransDetails(LeaveID);
                Model.ProcessWorkFlow workFlow = new Model.ProcessWorkFlow()
                {
                    SenderID = userDetail.EmployeeID,
                    ReceiverID = userDetail.EmployeeID,
                    SenderDepartmentID = userDetail.DepartmentID,
                    SenderDesignationID = userDetail.DesignationID,
                    CreatedBy = userDetail.UserID,
                    EmployeeID = (int)userDetail.EmployeeID,
                    Scomments = "Withdrawl",
                    ProcessID = (int)WorkFlowProcess.LeaveApproval,
                    StatusID = (int)EmpLeaveStatus.Withdrawl

                };

                bool isWithdraw = leaveService.WithdrawlLeave(LeaveID, Convert.ToInt32(leavetrans.LeaveID), workFlow);

                if (isWithdraw)
                {
                    leaveService.SenderSendMail(leavetrans.EmployeeId, leavetrans.LeaveCategoryName, leavetrans, "Withdrawal");
                    TempData["Message"] = "Leave Withdrawal Succesfully.";
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
        public ActionResult DownloadDocument(string FileID)
        {
            try
            {
                string fileextension = FileID.Substring(FileID.LastIndexOf("."), FileID.Length - FileID.LastIndexOf("."));
                if (fileextension == ".doc" || fileextension == ".docx")
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                }
                else if (fileextension == ".PDF" || fileextension == ".pdf")
                {
                    Response.ContentType = "application/pdf";
                }
                else if (fileextension == ".jpeg" || fileextension == ".jpg" || fileextension == ".bmp" || fileextension == ".png")
                {
                    Response.ContentType = "image/png";
                }
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + FileID);
                string path = Path.Combine(Server.MapPath("~/Document/"), FileID);
                if (System.IO.File.Exists(path))
                {
                    Response.WriteFile(path);
                    HttpContext.ApplicationInstance.CompleteRequest();
                    return new FilePathResult(path, Response.ContentType);
                }
                else
                {
                    TempData["error"] = " File is not found.";
                }
                return new FilePathResult(path, Response.ContentType);
            }
            //string fullPath = Server.MapPath("~/Document/");
            //    byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath + FileID);
            //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileID);        
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
        }

        #region SearchLeaveHistory

        public ActionResult SearchleavehistoryDetails(FormCollection formCollection)
        {
            log.Info($"EmployeeLeaveController/SearchleavehistoryDetails");
            try
            {
                BindDropdownsSearch();
                EmployeeLeaveViewModel employeeVM = new EmployeeLeaveViewModel();
                var userTypeID = userDetail.UserTypeID;
                List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
                return PartialView("_SearchleavehistoryDetails", employeeVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdownsSearch()
        {
            var ddlStatus = ddlService.ddlstatus();
            Model.SelectListModel selectStatus = new Model.SelectListModel();
            selectStatus.id = 0;
            selectStatus.value = "All";
            ddlStatus.Insert(0, selectStatus);
            ViewBag.LeaveStatus = new SelectList(ddlStatus, "id", "value");

            var ddlBranchList = ddlService.ddlBranchList();
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlLeaveType = ddlService.ddlLeaveType();
            Model.SelectListModel selectLT = new Model.SelectListModel();
            selectLT.id = 0;
            selectLT.value = "All";
            ddlLeaveType.Insert(0, selectLT);
            ViewBag.LeaveType = new SelectList(ddlLeaveType, "id", "value");

        }

        #endregion

        [HttpGet]
        public ActionResult _ExportTemplate(Model.EmployeeLeave empLeave)
        {
            BindDropdownsSearch();
            EmployeeLeaveViewModel LAVM = new EmployeeLeaveViewModel();
            empLeave.EmployeeId = (int)userDetail.EmployeeID;
            LAVM.GetEmployeeLeaveList = leaveService.GetEmployeeLeaveList(empLeave);
            LAVM.userRights = userAccessRight;
            return PartialView("_LeaveApplicationGridView", LAVM);
        }

        [HttpPost]
        public ActionResult _ExportTemplate(FormCollection frm)
        {
            log.Info("EmployeeLeaveController/_ExportTemplate");
            var statusID = Request.Form[0];
            var leavecategoryID = Request.Form[1];
            var fromdate = Request.Form[2];
            var todate = Request.Form[3];
            if (!string.IsNullOrEmpty(Convert.ToString(userDetail.EmployeeID)))
            {
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                fileName = ExtensionMethods.SetUniqueFileName("EmployeeLeaveSheet-", FileExtension.xlsx);
                var res = leaveService.GetLeaveForm((int)userDetail.EmployeeID, Convert.ToInt32(statusID), Convert.ToInt32(leavecategoryID), fromdate, todate, fileName, fullPath, userDetail.FullName);
                return Json(new { fileName = fileName, fullPath = fullPath + fileName, message = "success" });
            }

            else
            {

                ModelState.AddModelError("BranchRequired", "Select Branch");
                return PartialView("_ExportTemplate");
            }
        }

        public JsonResult GetUnit(DateTime DateFrom, DateTime DateTo, int BranchId, int LeaveCategoryID)
        {
            log.Info($"EmployeeLeaveController/getUnit/{DateFrom}/{DateTo},{userDetail.BranchID},{LeaveCategoryID}");
            //getUnitDetails(DateFrom, DateTo, BranchId, LeaveCategoryID);
            string unit_remark;
            var unit = leaveService.GetUnitDetails(DateFrom, DateTo, BranchId, LeaveCategoryID, (int)userDetail.EmployeeID, out unit_remark);
            DataTable DT = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(unit);
            return Json(new { totalUnit = DT.Rows[0]["totalUnit"], remark = unit_remark }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult _PostLeave()
        {
            log.Info($"EmployeeLeaveController/_PostLeave");
            try
            {
                BindDropdowns(null, 0);
                Model.EmployeeLeave objEmployeeLeave = new Model.EmployeeLeave();
                objEmployeeLeave.FromdayType = Model.DayType.FullDay;
                objEmployeeLeave.TodayType = Model.DayType.FullDay;
                return PartialView("_PostLeave", objEmployeeLeave);
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _ExportLeaveRecords(int statusID, int lCategoryID, string fromdate, string todate)
        {
            log.Info("EmployeeLeaveController/_ExportLeaveRecords");
            //var statusID = 0;
            //var leavecategoryID = 0;
            //var fromdate = "";
            //var todate = "";

            if (fromdate == null)
                fromdate = "";
            if (todate == null)
                todate = "";

            string fileName = string.Empty, msg = string.Empty;
            string fullPath = Server.MapPath("~/FileDownload/");
            fileName = ExtensionMethods.SetUniqueFileName("EmployeeLeaveSheet-", FileExtension.xlsx);
            var res = leaveService.GetLeaveForm((int)userDetail.EmployeeID, Convert.ToInt32(statusID), Convert.ToInt32(lCategoryID), fromdate, todate, fileName, fullPath, userDetail.FullName);

            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath + fileName);
            System.IO.File.Delete(fullPath + fileName); //Delete your local file


            // Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //  stream.Seek(0, SeekOrigin.Begin);
            //   return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee Leave.xlsx");

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee Leave.xlsx");
            //  return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }


        #region Leave Encashment
        public ActionResult EncashmentReport()
        {
            log.Info($"EmployeeLeaveController/EncashmentReport");
            return View();
        }
        [HttpGet]
        public ActionResult _GetEncashmentFilters()
        {
            log.Info($"EmployeeLeaveController/_GetEncashmentFilters/");

            try
            {
                Model.CommonFilter cFilter = new Model.CommonFilter();
                return PartialView("_EncashmentFilter", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult _ExportEncashmentReport(Model.CommonFilter cFilter)
        {

            log.Info($"EmployeeLeaveController/_ExportEncashmentReport/");
            try
            {
                if (ModelState.IsValid)
                {
                    if (!cFilter.StartDate.HasValue || !cFilter.EndDate.HasValue)
                    {
                        ModelState.AddModelError("StartDate", "Please select From and To Date.");
                        return Json(new
                        {
                            message = "error",
                            htmlData = ConvertViewToString("_EncashmentFilter", cFilter)
                        }, JsonRequestBehavior.AllowGet);

                    }
                    string fileName = string.Empty, msg = string.Empty;
                    string fullPath = Server.MapPath("~/FileDownload/");
                    var getEncashList = leaveService.GetLeaveEncashForF_A(cFilter.StartDate.Value, cFilter.EndDate.Value);
                    if (getEncashList != null && getEncashList.Count > 0)
                    {
                        DataTable exportData = new DataTable();
                        string tFilter = string.Empty;
                        fileName = "Leave Encashment Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;
                        tFilter = $"Period : {cFilter.StartDate.Value.ToString("dd-MMM-yyyy")} - {cFilter.EndDate.Value.ToString("dd-MMM-yyyy")}";

                        DataColumn dtc0 = new DataColumn("S.No.");
                        DataColumn dtc1 = new DataColumn("Employee Code");
                        DataColumn dtc2 = new DataColumn("Name");
                        DataColumn dtc3 = new DataColumn("No. of EL");
                        DataColumn dtc4 = new DataColumn("Basic");
                        DataColumn dtc5 = new DataColumn("DA");
                        DataColumn dtc6 = new DataColumn("Gross Amount");
                        DataColumn dtc7 = new DataColumn("TDS");
                        DataColumn dtc8 = new DataColumn("Net Amount");

                        exportData.Columns.Add(dtc0);
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        exportData.Columns.Add(dtc4);
                        exportData.Columns.Add(dtc5);
                        exportData.Columns.Add(dtc6);
                        exportData.Columns.Add(dtc7);
                        exportData.Columns.Add(dtc8);
                        for (int i = 0; i < getEncashList.Count; i++)
                        {
                            DataRow row = exportData.NewRow();
                            row[0] = i + 1;
                            row[1] = getEncashList[i].EmployeeCode;
                            row[2] = getEncashList[i].EmployeeName;
                            row[3] = getEncashList[i].Unit;
                            row[4] = getEncashList[i].CR.Value;
                            row[5] = getEncashList[i].OB.Value;
                            row[6] = getEncashList[i].Bal;
                            exportData.Rows.Add(row);
                        }

                        leaveService.ExportLeaveEncashForF_A(exportData, fullPath, fileName, tFilter);
                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");
                    }
                    else
                    {
                        ModelState.AddModelError("OtherValidation", "No Record Found.");
                    }
                    return PartialView("_EncashmentFilter", cFilter);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return PartialView("_EncashmentFilter", cFilter);
        }
        #endregion
    }
}