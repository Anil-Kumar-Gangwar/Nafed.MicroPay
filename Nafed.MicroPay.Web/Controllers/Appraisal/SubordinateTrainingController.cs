using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using MicroPay.Web.Models;
using System.Data;

namespace MicroPay.Web.Controllers.Appraisal
{
    public class SubordinateTrainingController : BaseController
    {
        private readonly IDropdownBindService ddlServices;
        private readonly IAppraisalFormService appraisalService;

        public SubordinateTrainingController(IAppraisalFormService appraisalService, IDropdownBindService ddlServices)
        {
            this.appraisalService = appraisalService;
            this.ddlServices = ddlServices;
        }
        // GET: SubordinateTraining
        public ActionResult Index()
        {
            log.Info("SubordinateTrainingController/Index");
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

        public ActionResult GetFilters()
        {
            int[] designationLits = new int[] { 309, 5, 27, 29, 70 };
            if (!designationLits.Contains((int)userDetail.DesignationID))
            {
                return Content(@"<div class='col-lg-12 col-md-12 col-sm-12 col-xs-12 font-weight-bold text-center alert alert-warning'>
                           You are not authorized to access this page.
                      </div>");
            }
            else
            {
                CommonFilter cFilter = new CommonFilter();
                ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                return PartialView("_Filters", cFilter);
            }
        }

        public ActionResult GetSubordTrnList()
        {
            int[] designationLits = new int[] { 309, 5, 27, 29, 70 };
            if (designationLits.Contains((int)userDetail.DesignationID))
            {
                List<FormTrainingDtls> objTraining = new List<FormTrainingDtls>();
                return PartialView("_SubordTrainingGridView", objTraining);
            }
            else
                return Content("");
        }
        public ActionResult _GetAPAREmpTraining(CommonFilter cfilter)
        {
            log.Info("SubordinateTrainingController/_GetAPAREmpTraining");
            try
            {
                if (string.IsNullOrEmpty(cfilter.ReportingYear))
                {
                    ModelState.AddModelError("ReportingYear", "Please select Reporting Year");
                    ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                    return Json(new { success = false, htmlData = ConvertViewToString("_Filters", cfilter) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<FormTrainingDtls> objTraining = new List<FormTrainingDtls>();
                    objTraining = appraisalService.GetSubordinateTrainings(userDetail.EmployeeID, cfilter.ReportingYear);
                    return Json(new { success = true, htmlData = ConvertViewToString("_SubordTrainingGridView", objTraining) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpGet]
        public ViewResult Create()
        {
            log.Info("SubordinateTrainingController/Create");
            try
            {
                ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                ViewBag.Employee = ddlServices.GetEmployeeByManager((int)userDetail.EmployeeID).ToList();
                SubordinateTrainingVM objVM = new Models.SubordinateTrainingVM();
                objVM.TrainingDtls = new List<FormTrainingDtls>();
                return View(objVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(SubordinateTrainingVM subVM, string ButtonType)
        {
            log.Info("SubordinateTrainingController/Create");
            try
            {
                if (ButtonType == "Add New Training Row")
                {
                    ViewBag.Employee = ddlServices.GetEmployeeByManager((int)userDetail.EmployeeID).ToList();
                    if (subVM.TrainingDtls == null)
                        subVM.TrainingDtls = new List<FormTrainingDtls>()
                        { new FormTrainingDtls() { sno = 1  } };
                    else
                    {
                        if (subVM.TrainingDtls.Count == 1)
                            subVM.TrainingDtls.FirstOrDefault().sno = 1;

                        if (subVM.TrainingDtls.Count < 5)
                            subVM.TrainingDtls.Add(new FormTrainingDtls()
                            {
                                sno = subVM.TrainingDtls.Count + 1
                            });
                    }
                    TempData["frmGroupTrainingData"] = subVM;
                    return Json(new { part = 1, htmlData = ConvertViewToString("_Trainings", subVM) }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        AppraisalForm obj = new AppraisalForm();
                        obj.formGroupATrainingDtls = subVM.TrainingDtls;                      
                        obj.ReportingYr = subVM.ReportingYr;
                        var res = appraisalService.InsertAPARTrainingforEmployee(obj, userDetail.UserID);
                        if (res)
                        {
                            return Json(new {success=true,msg= "Training added successfully." }, JsonRequestBehavior.AllowGet); 
                        }
                        else
                        {
                            ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                            ViewBag.Employee = ddlServices.GetEmployeeByManager((int)userDetail.EmployeeID).ToList();
                            TempData["Error"] = "Error while adding training.";
                            return View(subVM);
                        }
                    }
                    else
                    {
                        ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                        ViewBag.Employee = ddlServices.GetEmployeeByManager((int)userDetail.EmployeeID).ToList();
                        return View(subVM);

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult _RemoveTrainingRow(int sNo)
        {
            var modelData = (SubordinateTrainingVM)TempData["frmGroupTrainingData"];
            if (modelData != null)
            {
                var deletedRow = modelData.TrainingDtls.Where(x => x.sno == sNo).FirstOrDefault();
                modelData.TrainingDtls.Remove(deletedRow);
                TempData["frmGroupTrainingData"] = modelData;
                return PartialView("_Trainings", modelData);
            }
            return Content("");

        }


        public ActionResult SubTrainingsFilter()
        {
            log.Info("SubordinateTrainingController/SubTrainingsFilter");
            try
            {
                CommonFilter cFilter = new CommonFilter();
                ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                return PartialView("_SuborinateFilters", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult APAREmpTraining(CommonFilter cfilter, string ButtonType)
        {
            log.Info("AppraisalController/APAREmpTraining");
            try
            {
                if (ButtonType == "Search")
                {
                    if (string.IsNullOrEmpty(cfilter.ReportingYear))
                    {
                        ModelState.AddModelError("ReportingYear", "Please select Reporting Year");
                        ViewBag.reportingYrs = ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                        return Json(new { success = false, htmlData = ConvertViewToString("_SuborinateFilters", cfilter) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        List<FormTrainingDtls> objTraining = new List<FormTrainingDtls>();
                        objTraining = appraisalService.GetSubordinateTrainings(null, cfilter.ReportingYear);
                        TempData["tempSubordTraining"] = objTraining;
                        return Json(new { success = true, htmlData = ConvertViewToString("_SubEmpTrnGridView", objTraining) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (ButtonType == "Export")
                {
                    string tFilter = string.Empty;
                    List<FormTrainingDtls> empList = (List<FormTrainingDtls>)TempData["tempSubordTraining"];
                    if (empList != null)
                    {
                        DataTable exportData = new DataTable();
                        string fileName = string.Empty, msg = string.Empty;
                        string fullPath = Server.MapPath("~/FileDownload/");
                        fileName = "Training Need From GM and above Report" + '.' + Nafed.MicroPay.Common.FileExtension.xlsx;

                        DataColumn dtc0 = new DataColumn("S.No.");
                        DataColumn dtc1 = new DataColumn("Reporting Year");
                        DataColumn dtc2 = new DataColumn("Reporting Manager");
                        DataColumn dtc3 = new DataColumn("Name");
                        DataColumn dtc4 = new DataColumn("Branch");
                        DataColumn dtc5 = new DataColumn("Designation");
                        DataColumn dtc6 = new DataColumn("Training");
                        DataColumn dtc7 = new DataColumn("Comments");
                        exportData.Columns.Add(dtc0);
                        exportData.Columns.Add(dtc1);
                        exportData.Columns.Add(dtc2);
                        exportData.Columns.Add(dtc3);
                        exportData.Columns.Add(dtc4);
                        exportData.Columns.Add(dtc5);
                        exportData.Columns.Add(dtc6);
                        exportData.Columns.Add(dtc7);
                        for (int i = 0; i < empList.Count; i++)
                        {
                            DataRow row = exportData.NewRow();
                            row["S.No."] = i + 1;
                            row["Reporting Year"] = empList[i].ReportingYr;
                            row["Reporting Manager"] = empList[i].ReportingTo;
                            row["Name"] = empList[i].Name;
                            row["Branch"] = empList[i].BranchName;
                            row["Designation"] = empList[i].DesignationName;
                            row["Training"] = empList[i].FormTraining;
                            row["Comments"] = empList[i].Remark;
                            exportData.Rows.Add(row);
                        }
                        DataSet ds = new DataSet();
                        ds.Tables.Add(exportData);
                        appraisalService.ExportEmpAPARTraining(ds, fullPath, fileName);

                        fullPath = $"{fullPath}{fileName}";
                        return JavaScript("window.location = '" + Url.Action("DownloadAndDelete", "Base", new { @sFileName = fileName, @sFileFullPath = fullPath }) + "'");

                    }
                    return new EmptyResult();
                }
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}