using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers
{
    public class EPFNominationController : BaseController
    {
        private readonly IEPFNominationService epfNominationService;
        private readonly IDropdownBindService ddlService;
        public EPFNominationController(IEPFNominationService epfNominationService, IDropdownBindService ddlService)
        {
            this.epfNominationService = epfNominationService;
            this.ddlService = ddlService;
        }
        // GET: EPFNomination
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult Create(int? empID = null, int? epfNo = null)
        {
            log.Info("EPFNominationController/Create");
            try
            {

                EPFNomination epfNom = new EPFNomination();


                var employeeID = empID.HasValue ? empID.Value : (int)userDetail.EmployeeID;
                var epfsNo = epfNo.HasValue ? epfNo.Value : 0;
                epfNom = epfNominationService.GetEPFNominationDetail(epfsNo, employeeID);
                epfNom.loggedInEmpID = (int)userDetail.EmployeeID;
                epfNom.EPFDetailList = epfNominationService.GetEPFNominee(employeeID, 1, epfNo); // FOR EPF
                epfNom.EPSDetailList = epfNominationService.GetEPSNominee(employeeID, 2, epfNo); // FOR EPS 
                epfNom.EPSMaleEmpNomList = epfNominationService.GetMaleEmployeeEPSNominee(employeeID); // FOR Male employee nominee, those are 18 year and above 
                var ddlRelationList = ddlService.ddlRelationList();
                ddlRelationList.OrderBy(x => x.value);
                epfNom.RelationList = ddlRelationList;

                var approvalSettings = GetEmpProcessApprovalSetting(employeeID, WorkFlowProcess.EPFNomination);
                if (approvalSettings != null)
                {
                    // epfNom.approvalSettings = approvalSettings;
                    epfNom.ReportingTo = approvalSettings.ReportingTo;
                }
                TempData["EPFNomDtl"] = epfNom;
                TempData.Keep("EPFNomDtl");
                return View("EPFNominationForm", epfNom);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(EPFNomination epfNom)
        {
            log.Info("EPFNominationController/Create");
            try
            {
                var ddlRelationList = ddlService.ddlRelationList();
                ddlRelationList.OrderBy(x => x.value);
                epfNom.RelationList = ddlRelationList;
                if (epfNom.ReportingTo == 0)
                {
                    TempData["Error"] = "You can not apply for EPF Nomination now, because your Reporting Manager is not set.";
                    epfNom = (EPFNomination)TempData["EPFNomDtl"];
                    TempData.Keep("EPFNomDtl");
                    return View("EPFNominationForm", epfNom);
                }

                if (epfNom.EPFDetailList == null || epfNom.EPFDetailList.Count == 0)
                {
                    TempData["Error"] = "EPF Nominee not set, please set EPF Nominee from Family Details-Depenedents Menu.";
                    epfNom = (EPFNomination)TempData["EPFNomDtl"];
                    TempData.Keep("EPFNomDtl");
                    return View("EPFNominationForm", epfNom);
                }

                if (ModelState.IsValid)
                {
                    ProcessWorkFlow workFlow = null;
                    if (epfNom.EPFNoID == 0)
                    {
                        workFlow = new ProcessWorkFlow()
                        {
                            SenderID = userDetail.EmployeeID,
                            ReceiverID = epfNom.ReportingTo,
                            SenderDepartmentID = userDetail.DepartmentID,
                            SenderDesignationID = userDetail.DesignationID,
                            CreatedBy = userDetail.UserID,
                            EmployeeID = epfNom.EmployeeID,
                            Scomments = "EPF Nomination Submitted",
                            ProcessID = (int)WorkFlowProcess.EPFNomination,
                            StatusID = (int)LoanApplicationStatus.Pending
                        };
                        epfNom._ProcessWorkFlow = workFlow;

                        epfNom.CreatedBy = userDetail.UserID;
                        epfNom.CreatedOn = DateTime.Now;
                        epfNom.StatusID = (int)LoanApplicationStatus.Pending;
                        int result = epfNominationService.InsertEPFNominee(epfNom);
                        if (result > 0)
                        {
                            TempData["Message"] = "EPF Nomination Submitted Successfully.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Error"] = "Problem while submitting EPF Nomination - Please contact your application administrator.";
                            epfNom = (EPFNomination)TempData["EPFNomDtl"];
                            TempData.Keep("EPFNomDtl");
                            return View("EPFNominationForm", epfNom);
                        }
                    }
                    else if (epfNom.EPFNoID > 0)
                    {
                        if (epfNom.loggedInEmpID == epfNom.EmployeeID)
                        {
                            epfNom.UpdatedBy = userDetail.UserID;
                            epfNom.UpdatedOn = DateTime.Now;
                            epfNom.StatusID = (int)LoanApplicationStatus.Pending;
                            int result = epfNominationService.UpdateEPFNominee(epfNom);
                            if (result > 0)
                            {
                                TempData["Message"] = "EPF Nomination Updated Successfully.";
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["Error"] = "Problem while Updating EPF Nomination - Please contact your application administrator.";
                                epfNom = (EPFNomination)TempData["EPFNomDtl"];
                                TempData.Keep("EPFNomDtl");
                                return View("EPFNominationForm", epfNom);
                            }
                        }
                        else if (epfNom.loggedInEmpID == epfNom.ReportingTo)
                        {
                            workFlow = new ProcessWorkFlow()
                            {
                                SenderID = userDetail.EmployeeID,
                                ReceiverID = epfNom.ReportingTo,
                                SenderDepartmentID = userDetail.DepartmentID,
                                SenderDesignationID = userDetail.DesignationID,
                                CreatedBy = userDetail.UserID,
                                EmployeeID = epfNom.EmployeeID,
                                Scomments = $"EPF Nomination Approved By {epfNom.ReportingTo}",
                                ProcessID = (int)WorkFlowProcess.EPFNomination,
                                StatusID = (int)LoanApplicationStatus.Accept
                            };
                            epfNom._ProcessWorkFlow = workFlow;
                            epfNom.UpdatedBy = userDetail.UserID;
                            epfNom.UpdatedOn = DateTime.Now;
                            epfNom.StatusID = (int)LoanApplicationStatus.Accept;
                            int result = epfNominationService.UpdateEPFNominee(epfNom);
                            if (result > 0)
                            {
                                TempData["Message"] = "EPF Nomination Approved Successfully.";
                                return RedirectToAction("Index", "ApprovalRequest");
                            }
                            else
                            {
                                TempData["Error"] = "Problem while Approving EPF Nomination - Please contact your application administrator.";
                                epfNom = (EPFNomination)TempData["EPFNomDtl"];
                                TempData.Keep("EPFNomDtl");
                                return View("EPFNominationForm", epfNom);
                            }

                        }

                    }
                    return View("EPFNominationForm", epfNom);
                }
                else
                {
                    epfNom = (EPFNomination)TempData["EPFNomDtl"];
                    TempData.Keep("EPFNomDtl");
                    return View("EPFNominationForm", epfNom);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetEPFNominationList()
        {
            log.Info("EPFNominationController/GetEPFNominationList");
            try
            {
                List<EPFNomination> epfNom = new List<EPFNomination>();
                var getList = epfNominationService.GetEPFNomineeList(userDetail.EmployeeID);
                if (getList != null && getList.Count > 0)
                    epfNom = getList;
                return PartialView("_EPFNominationGridView", epfNom);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetNominationList(CommonFilter cFilter)
        {
            log.Info("EPFNominationController/GetNominationList");
            try
            {
                //if (ModelState.IsValid)
                //{
                    List<EPFNomination> epfNom = new List<EPFNomination>();
                    var getList = epfNominationService.GetEPFNomineeList(null, cFilter.FromDate, cFilter.ToDate);
                    if (getList != null && getList.Count > 0)
                        epfNom = getList;
                    return Json(new { status = true, htmlData = ConvertViewToString("_EPFNominationList", epfNom) }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //    return PartialView("_EPFFilter", cFilter);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult View()
        {
            log.Info("EPFNominationController/View");
            try
            {
                CommonFilter cFilter = new CommonFilter();
                return View(cFilter);

                //EPFNomination epfNom = new EPFNomination();
                //var employeeID = empID.HasValue ? empID.Value : (int)userDetail.EmployeeID;
                //var epfsNo = epfNo.HasValue ? epfNo.Value : 0;
                //epfNom = epfNominationService.GetEPFNominationDetail(epfsNo, employeeID);
                //epfNom.loggedInEmpID = (int)userDetail.EmployeeID;
                //epfNom.EPFDetailList = epfNominationService.GetEPFNominee(employeeID, 1, epfNo); // FOR EPF
                //epfNom.EPSDetailList = epfNominationService.GetEPSNominee(employeeID, 2, epfNo); // FOR EPS 
                //epfNom.EPSMaleEmpNomList = epfNominationService.GetMaleEmployeeEPSNominee(employeeID); // FOR Male employee nominee, those are 18 year and above 
                //var ddlRelationList = ddlService.ddlRelationList();
                //ddlRelationList.OrderBy(x => x.value);
                //epfNom.RelationList = ddlRelationList;
                //return View("EPFNominationForm", epfNom);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult ViewReport(int? empID = null, int? epfNo = null)
        {
            log.Info("EPFNominationController/ViewReport");
            try
            {
                EPFNomination epfNom = new EPFNomination();
                var employeeID = empID.HasValue ? empID.Value : (int)userDetail.EmployeeID;
                var epfsNo = epfNo.HasValue ? epfNo.Value : 0;
                epfNom = epfNominationService.GetEPFNominationDetail(epfsNo, employeeID);
                epfNom.loggedInEmpID = (int)userDetail.EmployeeID;
                epfNom.EPFDetailList = epfNominationService.GetEPFNominee(employeeID, 1, epfNo); // FOR EPF
                epfNom.EPSDetailList = epfNominationService.GetEPSNominee(employeeID, 2, epfNo); // FOR EPS 
                epfNom.EPSMaleEmpNomList = epfNominationService.GetMaleEmployeeEPSNominee(employeeID); // FOR Male employee nominee, those are 18 year and above 
                var ddlRelationList = ddlService.ddlRelationList();
                ddlRelationList.OrderBy(x => x.value);
                epfNom.RelationList = ddlRelationList;
                return View("ViewEPFNominationForm", epfNom);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}