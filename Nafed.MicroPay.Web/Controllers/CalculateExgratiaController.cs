using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using System.Data;

namespace MicroPay.Web.Controllers
{
    public class CalculateExgratiaController : BaseController
    {
        private readonly IExgratiaService exgratiaService;
        private readonly IDropdownBindService ddlService;

        public CalculateExgratiaController(IExgratiaService exgratiaService, IDropdownBindService ddlService)
        {
            this.exgratiaService = exgratiaService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            ExgratiaViewModel EXVM = new ExgratiaViewModel();
            List<Model.ExgratiaList> objExgratiaList = new List<Model.ExgratiaList>();
            var reportingYr = ExtensionMethods.GetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
            EXVM.yearList = reportingYr;
            BindDropdowns();
            EXVM.userRights = userAccessRight;
            return View(EXVM);
        }

        public ActionResult ExgratiaGridView(ExgratiaViewModel ExgratiaDataVM)
        {
            log.Info($"CalculateExgratiaController/ExgratiaGridView");
            try
            {
                BindDropdowns();
                ExgratiaViewModel EXVM = new ExgratiaViewModel();

                List<Model.ExgratiaList> objExgratiaList = new List<Model.ExgratiaList>();
                var reportingYr = ExtensionMethods.GetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
                EXVM.yearList = reportingYr;
                EXVM.listExgratia = exgratiaService.GetExgratiaList(ExgratiaDataVM.selectYearID, (int)ExgratiaDataVM.branchID, ExgratiaDataVM.EmpTypeID);
                string fromYear = ExgratiaDataVM.selectYearID.Substring(0, 4) + "04";
                string toYear = ExgratiaDataVM.selectYearID.Substring(5, 4) + "03";
                DataTable dtExport = exgratiaService.GetExgratiaExport(fromYear, toYear, ExgratiaDataVM.selectYearID, (int)ExgratiaDataVM.branchID, ExgratiaDataVM.EmpTypeID);
                Session["dtdata"] = dtExport;
                EXVM.userRights = userAccessRight;
                return PartialView("_ExgratiaGridView", EXVM);


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult PublishExgratia(ExgratiaViewModel ExgratiaDataVM)
        {
            log.Info($"CalculateExgratiaController/PublishExgratia");
            try
            {
                exgratiaService.PublishExGratia(ExgratiaDataVM.selectYearID, ExgratiaDataVM.EmpTypeID, Convert.ToInt32(ExgratiaDataVM.branchID));

                return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult GetBranchEmployee(int branchID)
        {
            log.Info($"DAArrearReportsController/GetBranchEmployee");
            try
            {
                var employeedetails = ddlService.employeeByBranchID(branchID);
                Model.SelectListModel selectemployeeDetails = new Model.SelectListModel();
                selectemployeeDetails.id = 0;
                selectemployeeDetails.value = "All";
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
        public void BindDropdowns()
        {

            var reportingYr = ExtensionMethods.GetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
            ViewBag.ddlYear = new SelectList(reportingYr, "Value", "Text");

            List<SelectListItem> selectEmployee = new List<SelectListItem>();
            selectEmployee.Add(new SelectListItem
            { Text = "Select", Value = "0" });
            ViewBag.Employee = selectEmployee;

            //ExgratiaViewModel exgratiaVM = new ExgratiaViewModel();
            //List<Model.SelectListModel> emptype = new List<Model.SelectListModel>();
            //emptype.Add(new Model.SelectListModel
            //{ value = "Select", id = 0 });
            //emptype.Add(new Model.SelectListModel
            //{ value = "Regular", id = 1 });
            //emptype.Add(new Model.SelectListModel
            //{ value = "Non Regular", id = 2 });
            //ViewBag.EmpType = new SelectList(emptype, "id", "value");

            var ddlBranchList = ddlService.ddlBranchList();
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlEmployeeTypeList = ddlService.ddlEmployeeTypeList();
            ddlEmployeeTypeList.OrderBy(x => x.value);
            Model.SelectListModel selectEmployeeType = new Model.SelectListModel();
            selectEmployeeType.id = 0;
            selectEmployeeType.value = "Select";
            ddlEmployeeTypeList.Insert(0, selectEmployeeType);
            ViewBag.EmpType = new SelectList(ddlEmployeeTypeList, "id", "value");

        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("CalculateExgratiaController/Create");
            try
            {
                BindDropdowns();
                Model.ExgratiaList objExgratia = new Model.ExgratiaList();
                objExgratia.branchList = ddlService.ddlBranchList();
                return View(objExgratia);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.ExgratiaList createExgratia)
        {
            log.Info("CalculateExgratiaController/Create");
            try
            {
                BindDropdowns();

                if (createExgratia.EmpTypeID == 0)
                {
                    ModelState.AddModelError("EmpTypeID", "Please select Employee Type");
                    return View(createExgratia);
                }
                if (createExgratia.FinancialYear == null)
                {
                    ModelState.AddModelError("FinancialYear", "Please select Financial Year");
                    return View(createExgratia);
                }
                if (createExgratia.branchID == 0)
                {
                    ModelState.AddModelError("branchID", "Please select Branch");
                    return View(createExgratia);
                }
                ModelState.Remove("EmployeeId");
                ModelState.Remove("HeadName");
                ModelState.Remove("noofdays");
                if (ModelState.IsValid)
                {
                    string fromYear = createExgratia.FinancialYear.Substring(0, 4) + "04";
                    string toYear = createExgratia.FinancialYear.Substring(5, 4) + "03";

                    int res = exgratiaService.calculateExgratia(createExgratia.noofdays, fromYear, toYear, Convert.ToInt32(createExgratia.branchID), createExgratia.FinancialYear, createExgratia.flag, createExgratia.EmpTypeID);
                    TempData["Message"] = "Successfully Created";

                    return RedirectToAction("Index");
                }
                return PartialView("Create", createExgratia);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void ExportExcel()
        {
            log.Info("CalculateExgratiaController/Export");
            try
            {
                ExgratiaViewModel EXVM = new ExgratiaViewModel();

                DataTable dtExport = (DataTable)Session["dtdata"];

                if (dtExport.Rows.Count > 0)
                {
                    string name = "Ex-gratia.xls";

                    this.Response.Clear();
                    this.Response.Charset = "";
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + name + "\"");
                    System.Text.StringBuilder sbb = new System.Text.StringBuilder();
                    sbb.Append("<html>");

                    //Header information...........................................
                    sbb.Append("<Table border= 1>");
                    sbb.Append("<tr>");
                    sbb.Append("<td align=center valign=top rowspan=2 colspan=" + 36 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Ex-gratia Calculation");
                    sbb.Append("</b></td>");
                    sbb.Append("</tr >");
                    sbb.Append("</Table>");

                    sbb.Append("<Table border= 1>");
                    sbb.Append("<tr>");
                    sbb.Append("<td>");
                    sbb.Append("");
                    sbb.Append("</b></td>");
                    sbb.Append("</tr >");
                    sbb.Append("</Table>");


                    sbb.Append("<Table border= 1>");
                    sbb.Append("<tr>");
                    sbb.Append("<td></td>");
                    sbb.Append("<td></td>");
                    sbb.Append("<td></td>");
                    sbb.Append("<td></td>");
                    sbb.Append("<td></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("April");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("May");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("June");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("July");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Aug");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Sep");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Oct");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Nov");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Dec");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Jan");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Feb");
                    sbb.Append("</b></td>");
                    sbb.Append("<td align=center valign=top  colspan=" + 2 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append("Mar");
                    sbb.Append("</b></td>");
                    sbb.Append("</tr >");
                    sbb.Append("</Table>");

                    sbb.Append("<Table border= 1>");

                    sbb.Append("<tr>");
                    for (int k = 0; k < 36; k++)
                    {
                        sbb.Append("<td align=left valign=top  style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                        sbb.Append(dtExport.Columns[k].ColumnName);
                        sbb.Append("</b></td>");
                    }
                    sbb.Append("</tr >");
                    //   --- rows ---   //
                    for (int i = 0; i < dtExport.Rows.Count; i++)
                    {
                        sbb.Append("<tr>");
                        for (int j = 0; j < 36; j++)
                        {
                            sbb.Append("<td align=left style=' white-space:nowrap ; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));

                        }
                        sbb.Append("</tr >");
                    }

                    sbb.Append("</Table>");
                    sbb.Append("</html>");
                    this.Response.Write(sbb.ToString());
                    this.Response.End();
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
      
        public ActionResult Delete(int ExgratiaID)
        {
            log.Info("Delete");
            try
            {
                exgratiaService.Delete(ExgratiaID, userDetail.UserID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
    }
}