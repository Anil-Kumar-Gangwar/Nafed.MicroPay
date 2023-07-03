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
    public class CalculateBonusController : BaseController
    {
        private readonly IBonusWagesService bonuswagesService;
        private readonly IDropdownBindService ddlService;

        public CalculateBonusController(IBonusWagesService bonuswagesService, IDropdownBindService ddlService)
        {
            this.bonuswagesService = bonuswagesService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            BonusWagesViewModel BWVM = new BonusWagesViewModel();
            List<Model.BonusWages> objBonusList = new List<Model.BonusWages>();
            var reportingYr = ExtensionMethods.GetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
            BWVM.yearList = reportingYr;
            BindDropdowns();
            BWVM.userRights = userAccessRight;
            return View(BWVM);
        }

        public ActionResult BonusWagesGridView(BonusWagesViewModel BonusWagesDataVM)
        {
            log.Info($"CalculateBonusController/BonusWagesGridView");
            try
            {
                BindDropdowns();
                BonusWagesViewModel BWVM = new BonusWagesViewModel();
                List<Model.BonusWages> objBonusList = new List<Model.BonusWages>();
                var reportingYr = ExtensionMethods.GetFinancialYrList(2002, DateTime.Now.Year).Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
                BWVM.yearList = reportingYr;


                if(BonusWagesDataVM.EmpTypeID==0)
                {
                    ModelState.AddModelError("EmpTypeID", "Please select Employee Type");
                    return View(BonusWagesDataVM);
                }
                if (BonusWagesDataVM.selectYearID == null)
                {
                    ModelState.AddModelError("selectYearID", "Please select Year");
                    return View(BonusWagesDataVM);
                }
                if (BonusWagesDataVM.branchID == 0)
                {
                    ModelState.AddModelError("branchID", "Please select Branch");
                    return View(BonusWagesDataVM);
                }
                BWVM.listBonusWages = bonuswagesService.GetBonusList(Convert.ToInt32(BonusWagesDataVM.selectYearID.Substring(0, 4)), (int)BonusWagesDataVM.branchID, BonusWagesDataVM.EmpTypeID);
                string fromYear = BonusWagesDataVM.selectYearID.Substring(0, 4) + "04";
                string toYear = BonusWagesDataVM.selectYearID.Substring(5, 4) + "03";
                DataTable dtExport = bonuswagesService.GetBonusExport(fromYear, toYear, Convert.ToInt32(BonusWagesDataVM.selectYearID.Substring(0, 4)), (int)BonusWagesDataVM.branchID, BonusWagesDataVM.EmpTypeID);
                Session["dtdata"] = dtExport;
                BWVM.userRights = userAccessRight;
                return PartialView("_CalculateBonusGridView", BWVM);
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

            var ddlEmployeeTypeList = ddlService.ddlEmployeeTypeList();
            ddlEmployeeTypeList.OrderBy(x => x.value);
            Model.SelectListModel selectEmployeeType = new Model.SelectListModel();
            selectEmployeeType.id = 0;
            selectEmployeeType.value = "Select";
            ddlEmployeeTypeList.Insert(0, selectEmployeeType);
            ViewBag.EmpType = new SelectList(ddlEmployeeTypeList, "id", "value");

            var ddlBranchList = ddlService.ddlBranchList();
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("CalculateBonusController/Create");
            try
            {
                BindDropdowns();
                Model.BonusWages objBonusWages = new Model.BonusWages();
                objBonusWages.branchList = ddlService.ddlBranchList();
                return View(objBonusWages);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.BonusWages createBonusWages)
        {
            log.Info("CalculateBonusController/Create");
            try
            {
                BindDropdowns();

                if (createBonusWages.EmpTypeID == 0)
                {
                    ModelState.AddModelError("EmpTypeID", "Please select Employee Type");
                    return View(createBonusWages);
                }
                if (createBonusWages.FinancialYear == null)
                {
                    ModelState.AddModelError("FinancialYear", "Please select Financial Year");
                    return View(createBonusWages);
                }
                if (createBonusWages.branchID == 0)
                {
                    ModelState.AddModelError("branchID", "Please select Branch");
                    return View(createBonusWages);
                }
                if (createBonusWages.BonusRate == 0)
                {
                    ModelState.AddModelError("BonusRate", "Please enter Bonus Rate");
                    return View(createBonusWages);
                }
                ModelState.Remove("EmployeeId");
                ModelState.Remove("HeadName");
                ModelState.Remove("From_date");
                ModelState.Remove("To_date");
                ModelState.Remove("selectYearID");
                if (ModelState.IsValid)
                {
                    string fromYear = createBonusWages.FinancialYear.Substring(0, 4) + "04";
                    string toYear =  createBonusWages.FinancialYear.Substring(5, 4) + "03";
                    int res = bonuswagesService.calculateBonus(createBonusWages.BonusRate, fromYear, toYear, Convert.ToInt32(createBonusWages.branchID), Convert.ToInt32(createBonusWages.FinancialYear.Substring(0, 4)), createBonusWages.EmpTypeID);

                    TempData["Message"] = "Successfully Created";
                    return RedirectToAction("Index");
                }
                return PartialView("Create", createBonusWages);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult Delete(int ID)
        {
            log.Info("Delete");
            try
            {
                bonuswagesService.DeleteBonusAmount(ID, userDetail.UserID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        public void ExportExcel()
        {
            log.Info("CalculateExgratiaController/Export");
            try
            {

                DataTable dtExport = (DataTable)Session["dtdata"];

                if (dtExport.Rows.Count > 0)
                {
                    string name = "Bonus.xls";

                    this.Response.Clear();
                    this.Response.Charset = "";
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + name + "\"");
                    System.Text.StringBuilder sbb = new System.Text.StringBuilder();
                    sbb.Append("<html>");

                    //Header information...........................................
                    sbb.Append("<Table border= 1>");
                    sbb.Append("<tr>");
                    sbb.Append("<td align=center valign=top rowspan=2 colspan=" + 24 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");
                    sbb.Append(" CALCULATION OF BONUS RESTRICTED SALARY FOR THE YEAR");
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
                    for (int k = 0; k < 23; k++)
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
                        for (int j = 0; j < 23; j++)
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
    }
}