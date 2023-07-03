using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System.Web.Script.Serialization;
using System.Data;

namespace MicroPay.Web.Controllers.Reports
{
    public class CRReportController : BaseController
    {
        // GET: CRReport
        private readonly IDropdownBindService dropDownService;
        private readonly ICRReportService crReportService;
        private readonly IEmployeeService employeeService;
        public CRReportController(IDropdownBindService dropDownService, ICRReportService crReportService, IEmployeeService employeeService)
        {
            this.dropDownService = dropDownService;
            this.crReportService = crReportService;
            this.employeeService = employeeService;
        }
        public ActionResult Index()
        {
            log.Info($"CRReportController/Index");
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

        public ActionResult _ReportFilter()
        {
            log.Info($"CRReportController/_ReportFilter");
            try
            {
                CRReportFilters crReportFilter = new CRReportFilters();
                crReportFilter.employeeList = new List<SelectListModel>()
                {
                    new SelectListModel { id = 0, value = "Select" }
                };
                crReportFilter.employeeList1 = new List<SelectListModel>()
                {
                    new SelectListModel { id = 0, value = "Select" }
                };
                return PartialView("_ReportFilter", crReportFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult GetEmployeeDetails(string formId)
        {
            log.Info($"CRReportController/GetEmployeeDetails");
            try
            {
                DataTable result = new DataTable();
                string query = string.Empty;
                if (formId == "FormA")
                    query = @"SELECT e.EmployeeId, e.Name,e.employeecode,D.DesignationName Description, b.BranchName,e.orderofpromotion, e.ConfirmationDate,e.pr_loc_doj,e.DOJ from tblmstemployee e inner join Branch b on e.BranchID = b.BranchID inner join Designation d on e.DesignationID = d.DesignationID where d.ISOFFICER = 1 and e.doleaveorg is null  order by d.rank asc";
                else if (formId == "FormB")
                    query = @"SELECT EmployeeId, Name,EmployeeCode from tblmstemployee e
	                          inner join Branch b on e.BranchID=b.BranchID
	                          inner Join
	                          (select * from Designation where rank BETWEEN (select MIN(rank)-1  from Designation where DesignationName like 'AM-II%') AND
	                          (select max(rank)+1 from Designation where DesignationName like 'JR.F.REP./CCT%')
	                          and CadreID not in (5) and IsDeleted=0 AND ISOFFICER=0 AND DesignationCode NOT IN ('20','19','18','78','58','312')) d
	                          on e.DesignationID=d.DesignationID where e.doleaveorg is null and e.EmployeeId=5 order by Name";
                else if (formId == "FormC")
                    query = @"SELECT e.EmployeeId,e.Name,e.EmployeeCode 
	                          from tblmstemployee e
	                          inner join Branch b on e.BranchID=b.BranchID
                              inner Join
	                          (select * from Designation where
	                          rank between (select min(rank) from Designation where DesignationName like 'SR DRIVER%') AND
	                          (select max(rank) from Designation) AND RANK<> 43 and CadreID  in (5, 1) and IsDeleted=0 AND ISOFFICER=0) d
	                          on e.DesignationID=d.DesignationID where e.doleaveorg is null and e.EmployeeTypeID=5 order by Name";
                else if (formId == "FormD")
                    query = @"SELECT e.EmployeeId,e.Name,e.employeecode,D.DesignationName Description,b.BranchName 
	                          from tblmstemployee e
	                          inner join Branch b on e.BranchID=b.BranchID
                              inner join Designation d
	                          on e.DesignationID=d.DesignationID order by Name";
                else if (formId == "FormBOrder")
                    query = @"SELECT e.EmployeeId,e.Name,e.employeecode,D.DesignationName Description,
                              b.BranchName,e.orderofpromotion,e.ConfirmationDate,e.pr_loc_doj,e.DOJ 
	                          from tblmstemployee e
	                          inner join Branch b on e.BranchID=b.BranchID
                              inner join  Designation  d
	                          on e.DesignationID=d.DesignationID  where d.ISOFFICER<>1 and e.IsDeleted=0 AND e.doleaveorg is null  order by Name";
                else if (formId == "CoveringLetterHO")
                    query = @"select e.EmployeeId,e.employeecode ,e.name  from tblmstemployee e where doleaveorg is null and 
                                BranchID = 44 order by e.employeecode";
                else if (formId == "CoveringLetterBranch")
                    query = @"select e.EmployeeId, e.employeecode ,e.name from tblmstemployee e where doleaveorg is null and 
                                BranchID <> 44 order by e.employeecode";


                result = crReportService.GetEmployeeDetails(query);
                List<SelectListModel> list = new List<SelectListModel>();
                foreach (DataRow row in result.Rows)
                {
                    list.Add(new SelectListModel()
                    {
                        id = Convert.ToInt32(row["EmployeeId"].ToString()),
                        value = Convert.ToString(row["employeecode"].ToString() + '-' + row["Name"].ToString())
                    });
                }
                return Json(new { employees = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult CRReportDetails(CRReportFilters crReportFilter, string buttonType)
        {
            log.Info($"CRReportController/CRReportDetails");
            try
            {
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();
                string rptName = string.Empty;
                if (buttonType == "Print")
                {
                    if (crReportFilter.yearID == null)
                        ModelState.AddModelError("yearRequired", "Please Select Year.");
                    if ((!crReportFilter.AllEmployee) && crReportFilter.employeeID == 0)
                        ModelState.AddModelError("employeeRequired", "Please Select Employee.");
                    if (!(crReportFilter.crFormRadio == CRFormRadios.FormB) && !(crReportFilter.crFormRadio == CRFormRadios.FormC) && !(crReportFilter.crFormRadio == CRFormRadios.FormD))
                        ModelState.AddModelError("PrintRequired", "Please select one of them.");

                    if (ModelState.IsValid)
                    {
                        var year = crReportFilter.yearID;
                        string employeeCode = string.Empty;
                        if (crReportFilter.employeeID != null)
                            employeeCode = employeeService.GetEmployeeByID(crReportFilter.employeeID.Value).EmployeeCode;
                        else
                            employeeCode = null;
                        if (crReportFilter.crFormRadio == CRFormRadios.FormB)
                        {
                            rptName = "FormBReport.rpt";
                            parameterList.Add(new ReportParameter { name = "year", value = year });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                        }
                        else if (crReportFilter.crFormRadio == CRFormRadios.FormC)
                        {
                            rptName = "FormCReport.rpt";
                            parameterList.Add(new ReportParameter { name = "year", value = year });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                        }
                        else if (crReportFilter.crFormRadio == CRFormRadios.FormD)
                        {
                            rptName = "FormDReport1.rpt";
                            parameterList.Add(new ReportParameter { name = "year", value = year });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                        }
                    }
                    else
                    {
                        crReportFilter.employeeList = new List<SelectListModel>()
                        {
                            new SelectListModel { id = 0, value = "Select" }
                        };
                        crReportFilter.employeeList1 = new List<SelectListModel>()
                        {
                            new SelectListModel { id = 0, value = "Select" }
                        };
                        return PartialView("_ReportFilter", crReportFilter);
                    }
                }
                else if (buttonType == "Order Print")
                {
                    string employeeCode = string.Empty;
                    if ((!crReportFilter.AllEmployee) && crReportFilter.employeeID == 0)
                        ModelState.AddModelError("employeeRequired", "Please Select Employee.");
                    if (!(crReportFilter.crFormRadio == CRFormRadios.FormBOrder) && !(crReportFilter.crFormRadio == CRFormRadios.FormA))
                        ModelState.AddModelError("OrderRequired", "Please select one of them.");
                    if (ModelState.IsValid)
                    {
                        if (crReportFilter.employeeID != null)
                            employeeCode = employeeService.GetEmployeeByID(crReportFilter.employeeID.Value).EmployeeCode;
                        else
                            employeeCode = null;

                        if (crReportFilter.crFormRadio == CRFormRadios.FormBOrder)
                        {
                            rptName = "rptOrderform_b.rpt";
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                        }
                        else if (crReportFilter.crFormRadio == CRFormRadios.FormA)
                        {
                            rptName = "rptOrderform_a.rpt";
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                        }
                    }
                    else
                    {
                        crReportFilter.employeeList = new List<SelectListModel>()
                        {
                            new SelectListModel { id = 0, value = "Select" }
                        };
                        crReportFilter.employeeList1 = new List<SelectListModel>()
                        {
                            new SelectListModel { id = 0, value = "Select" }
                        };
                        return PartialView("_ReportFilter", crReportFilter);
                    }
                }
                else if (buttonType == "Print Covering Letter")
                {
                    if (crReportFilter.yearID1 == null)
                        ModelState.AddModelError("year1Required", "Please Select Year.");
                    if (crReportFilter.employeeID1 == 0)
                        ModelState.AddModelError("employee1Required", "Please Select Employee.");
                    if (!(crReportFilter.crFormRadio == CRFormRadios.CoveringLetterHO) && !(crReportFilter.crFormRadio == CRFormRadios.CoveringLetterBranch))
                        ModelState.AddModelError("CoveringRequired", "Please select one of them.");

                    if (ModelState.IsValid)
                    {
                        var year = crReportFilter.yearID1;
                        string employeeCode = string.Empty;
                        if (crReportFilter.employeeID1 != 0)
                            employeeCode = employeeService.GetEmployeeByID(crReportFilter.employeeID1.Value).EmployeeCode;
                        else
                            employeeCode = null;

                        if (crReportFilter.crFormRadio == CRFormRadios.CoveringLetterHO)
                        {
                            rptName = "confirmcovletter_Branch.rpt";
                            parameterList.Add(new ReportParameter { name = "year", value = year });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                        }
                        else if (crReportFilter.crFormRadio == CRFormRadios.CoveringLetterBranch)
                        {
                            rptName = "confirmcovletter_HO.rpt";
                            parameterList.Add(new ReportParameter { name = "year", value = year });
                            parameterList.Add(new ReportParameter { name = "empcode", value = employeeCode });
                        }
                    }
                    else
                    {
                        crReportFilter.employeeList = new List<SelectListModel>()
                        {
                            new SelectListModel { id = 0, value = "Select" }
                        };
                        crReportFilter.employeeList1 = new List<SelectListModel>()
                        {
                            new SelectListModel { id = 0, value = "Select" }
                        };
                        return PartialView("_ReportFilter", crReportFilter);
                    }
                }
                reportModel.reportParameters = parameterList;
                reportModel.rptName = rptName;
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}