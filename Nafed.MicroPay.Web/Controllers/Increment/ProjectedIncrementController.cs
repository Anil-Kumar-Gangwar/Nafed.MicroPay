using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using MicroPay.Web.Models;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using Nafed.MicroPay.Services.Increment;
using System.Data;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers.Increment
{
    public class ProjectedIncrementController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IIncrement incrementService;
        public ProjectedIncrementController(IDropdownBindService ddlService, IIncrement incrementService)
        {
            this.ddlService = ddlService;
            this.incrementService = incrementService;
        }

        public ActionResult Index()
        {
            log.Info($"ProjectedIncrementController/Index");
            try
            {
                ProjectedIncrementViewModel incrementVM = new ProjectedIncrementViewModel();
                incrementVM.userRights = userAccessRight;
                incrementVM.branchList = ddlService.ddlBranchList();
                List<SelectListItem> incrementMonth = new List<SelectListItem>();

                incrementMonth.Add(new SelectListItem
                { Text = "Select", Value = "0" });

                incrementMonth.Add(new SelectListItem
                { Text = "January", Value = "1" });

                incrementMonth.Add(new SelectListItem
                { Text = "July", Value = "7" });

                incrementVM.incrementMonth = incrementMonth;
                var increment = incrementService.InsertProjectedEmployee();
                var projectedInc = incrementService.GetProjectedIncrementDetails(null, "", "", null);
                TempData["ProjectedIncrementDetails"] = projectedInc;
                return View(incrementVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetProjectedIncrementGridView(ProjectedIncrementViewModel pVM)
        {
            try
            {
                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                pVM.EmployeeCode = pVM.EmployeeCode != null ? pVM.EmployeeCode.Trim() : pVM.EmployeeCode;
                pVM.EmployeeName = pVM.EmployeeName != null ? pVM.EmployeeName.Trim() : pVM.EmployeeName;
                pVM.incrementMonthId = pVM.incrementMonthId == 0 ? null : pVM.incrementMonthId;
                var dataDetails = new List<Model.ProjectedIncrement>();

                if (TempData["ProjectedIncrementDetails"] != null)
                {
                    dataDetails = (List<Model.ProjectedIncrement>)TempData["ProjectedIncrementDetails"];
                    TempData.Keep("ProjectedIncrementDetails");
                    if (pVM.EmployeeCode != null)
                        dataDetails = dataDetails.Where(x => x.EmployeeCode == pVM.EmployeeCode).ToList();
                    if (pVM.EmployeeName != null)
                        dataDetails = dataDetails.Where(x => x.EmployeeName == pVM.EmployeeName).ToList();
                    if (pVM.BranchID != null)
                        dataDetails = dataDetails.Where(x => x.BranchID == pVM.BranchID).ToList();
                    if (pVM.incrementMonthId != null)
                        dataDetails = dataDetails.Where(x => x.IncrementMonth == pVM.incrementMonthId).ToList();
                }

                //var projectedInc = incrementService.GetProjectedIncrementDetails(pVM.BranchID, pVM.EmployeeCode, pVM.EmployeeName, pVM.incrementMonthId);
                //projectedVM.projectedIncrement = projectedInc;
                dataDetails.ForEach(x => { x.CurrentBasic = Convert.ToInt32(x.CurrentBasic); x.OldBasic = Convert.ToInt32(x.OldBasic); x.LastIncrement = Convert.ToInt32(x.LastIncrement); });
                projectedVM.projectedIncrement = dataDetails;
                projectedVM.userRights = userAccessRight;
                return PartialView("_ProjectedIncrementGridView", projectedVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult PostProjectedIncrementDetails(ProjectedIncrementViewModel pVM, string Validate)
        {
            try
            {
                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                List<Model.ProjectedIncrement> UpdateGrid = new List<Model.ProjectedIncrement>();
                if (pVM.projectedIncrement != null)
                {
                    if (pVM.projectedIncrement.Count() > 0)
                    {
                        var projectedIncrementAll = pVM.projectedIncrement;
                        var changedRowCount = 0;
                        var changedrow = pVM.projectedIncrement.Where(x => x.RowChanged == true).ToList();
                        if (Validate.ToLower().Trim() == "export report")
                        {
                            return ExportApplicableIncrement(pVM);
                        }
                        else if (Validate.ToLower().Trim() == "update basic")
                        {
                            return UpdateBasicDetails(pVM);
                        }
                        else if (Validate.ToLower().Trim() == "update grid" && changedrow.Count > 0)
                        {
                            if (changedrow.Count > 0)
                            {
                                changedRowCount = changedrow.Count();
                                pVM.projectedIncrement = changedrow;
                            }
                            else if (changedrow.Count == 0)
                            {
                                changedRowCount = projectedIncrementAll.Count();
                                pVM.projectedIncrement = projectedIncrementAll;
                            }

                            int i = 1;
                            foreach (var item in pVM.projectedIncrement)
                            {
                                string strNewBasicAmount = "";
                                var validate = fnValidateNewBasicAmount(item.CurrentBasic, item.EmployeeID, item.BranchCode, item.BranchID, item.ProceedFurther, out strNewBasicAmount);
                                if (strNewBasicAmount != "" && !validate)
                                {
                                    item.RowChanged = false;
                                    var msg = "Salary could not be updated For that Employee. Reason:" + strNewBasicAmount + " ";
                                    var EmpDetails = projectedIncrementAll.Where(x => x.EmployeeID == item.EmployeeID).FirstOrDefault();
                                    EmpDetails.ProceedFurther = false;
                                    EmpDetails.AlertMessage = msg;
                                    EmpDetails.EmployeeName = item.EmployeeName;
                                    EmpDetails.BranchName = item.BranchName;
                                    EmpDetails.RowChanged = item.RowChanged;
                                    EmpDetails.IncrementMonth = item.IncrementMonth;
                                }
                                else if (validate)
                                {
                                    var EmpDetails = projectedIncrementAll.Where(x => x.EmployeeID == item.EmployeeID).FirstOrDefault();
                                    EmpDetails.EmployeeName = item.EmployeeName;
                                    EmpDetails.BranchName = item.BranchName;
                                    EmpDetails.RowChanged = item.RowChanged;
                                    EmpDetails.ProceedFurther = true;
                                    EmpDetails.IncrementMonth = item.IncrementMonth;
                                    Model.ProjectedIncrement updateGridModel = new Model.ProjectedIncrement();
                                    updateGridModel.EmployeeID = item.EmployeeID;
                                    updateGridModel.EmployeeCode = item.EmployeeCode;
                                    updateGridModel.BranchID = item.BranchID;
                                    updateGridModel.CurrentBasic = item.CurrentBasic;
                                    updateGridModel.OldBasic = item.OldBasic;
                                    updateGridModel.LastIncrement = item.LastIncrement;
                                    UpdateGrid.Add(updateGridModel);
                                }
                                i++;
                            }

                            incrementService.UpdateProjectedEmployeeSalaryDetails(UpdateGrid);
                            pVM.incrementMonthId = pVM.incrementMonthId == 0 ? null : pVM.incrementMonthId;
                            var projectedInc = incrementService.GetProjectedIncrementDetails(pVM.BranchID, pVM.EmployeeCode, pVM.EmployeeName, pVM.incrementMonthId);
                            foreach (var x in projectedInc)
                            {
                                projectedIncrementAll.First(d => d.EmployeeCode == x.EmployeeCode).CurrentBasic = x.CurrentBasic;
                                projectedIncrementAll.First(d => d.EmployeeCode == x.EmployeeCode).OldBasic = x.OldBasic;
                                projectedIncrementAll.First(d => d.EmployeeCode == x.EmployeeCode).LastIncrement = x.LastIncrement;
                            }
                            projectedIncrementAll.ForEach(x => { x.CurrentBasic = Convert.ToInt32(x.CurrentBasic); x.OldBasic = Convert.ToInt32(x.OldBasic); x.LastIncrement = Convert.ToInt32(x.LastIncrement); });
                            projectedVM.projectedIncrement = projectedIncrementAll;
                            projectedVM.userRights = userAccessRight;
                            projectedVM.incrementMonthId = pVM.incrementMonthId;
                            if (projectedIncrementAll.Any(x => x.ProceedFurther == false))
                                ViewBag.Message = "Some Records can not updated. Reason mentioned in grid status column";
                            else
                                ViewBag.Message = "Salary Increment is completed successfully.";
                            TempData["ProjectedIncrementDetails"] = projectedIncrementAll;
                            return PartialView("_ProjectedIncrementGridView", projectedVM);
                        }
                        else
                        {
                            return PartialView("_ProjectedIncrementGridView", pVM);
                        }
                    }
                }
                else
                {
                    return PartialView("_ProjectedIncrementGridView", pVM);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return null;
        }

        public bool fnValidateNewBasicAmount(decimal? NewCurrentBasic, int? EmployeeId, string BranchCode, int BranchID, bool? ProceedFurther, out string strNewBasicAmount)
        {
            strNewBasicAmount = "";
            bool flag = false;
            var validateDetails = incrementService.GetValidateNewBasicAmountDetails(EmployeeId.Value, null);

            if (validateDetails.Count > 0)
            {
                var validation = validateDetails;

                DataTable dataTable = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(validation);

                foreach (DataRow row in dataTable.Rows)
                {
                    int limit = 0;
                    decimal?[] b = new decimal?[40];
                    decimal?[] e = new decimal?[40];
                    string strEmpCode = row["EmployeeCode"].ToString();
                    string empName = row["Name"].ToString();
                    decimal? dblBasic = row["e_Basic"] == null ? 0 : Convert.ToDecimal(row["e_Basic"]);
                    string level = row["Level"].ToString();
                    string payScale = row["payscale"].ToString();

                    if (level == "1" || level == "2" || level == "3" || level == "4" || level == "5" || level == "6" || level == "7" || level == "8" || level == "9" || level == "10")
                        limit = 40;
                    else if (level == "11")
                        limit = 39;
                    else if (level == "12")
                        limit = 34;
                    else if (level == "13")
                        limit = 20;
                    else if (level == "13A")
                        limit = 18;
                    else if (level == "14")
                        limit = 15;
                    else if (level == "15")
                        limit = 8;
                    else if (level == "16")
                        limit = 4;
                    else if (level == "17")
                        limit = 1;
                    else if (level == "18")
                        limit = 1;

                    var scalepart = payScale.Split('-');

                    if (payScale == "0")
                    {
                        for (int x = 1; x <= limit; x++)
                        {
                            b[x - 1] = Convert.ToDecimal(row["B" + x]);
                            e[x - 1] = Convert.ToDecimal(row["E" + x]);
                        }
                    }
                    else
                    {
                        int even = 0;
                        int odd = 0;
                        for (int p = 0; p <= scalepart.Length - 1; p++)
                        {
                            if (Convert.ToInt32(p) % 2 == 0)
                            {
                                b[even] = Convert.ToDecimal(scalepart[Convert.ToInt32(p)]);
                                even++;
                            }
                            else
                            {
                                e[odd] = Convert.ToDecimal(scalepart[Convert.ToInt32(p)]);
                                odd++;
                            }
                        }
                    }

                    for (int cnt = 0; cnt <= b.Length - 1; cnt++)
                    {
                        if (NewCurrentBasic == b[cnt])
                        {
                            flag = true;
                            return flag;
                        }
                    }

                    if (!flag)
                    {
                        strNewBasicAmount = "New Basic Amount is not fall in Current Salary Slab for employee : " + empName + "-" + strEmpCode + "";
                        return flag;
                    }
                }

                //decimal? basic1 = null;
                //decimal? basic2 = null;
                //string paysScale = validation.payscale;
                //if (paysScale == "0")
                //{
                //    basic1 = validation.B1;
                //    if (validation.B3 != 0)
                //        basic2 = validation.B3;
                //    else
                //        basic2 = validation.B2;
                //}
                //else
                //{
                //    var scalepart = paysScale.Split('-');
                //    basic1 = Convert.ToDecimal(scalepart[0]);
                //    if (scalepart.Length < 4)
                //        basic2 = Convert.ToDecimal(scalepart[2]);
                //    else
                //        if (Convert.ToInt32(scalepart[4]) == 0)
                //        basic2 = Convert.ToDecimal(scalepart[2]);
                //    else
                //        basic2 = Convert.ToDecimal(scalepart[4]);
                //}
                //if (NewCurrentBasic < Convert.ToDecimal(basic1) || NewCurrentBasic > Convert.ToDecimal(basic2))
                //{
                //    strNewBasicAmount = "New Basic Amount is either Exceed or Less the Current Salary Slab";
                //    return flag;
                //}
                //decimal? firstB = null;
                //decimal? SecondB = null;
                //decimal? ThirdB = null;
                //decimal? firstI = null;
                //decimal? SecondI = null;

                //if (paysScale == "0")
                //{
                //    firstB = validation.B1;
                //    SecondB = validation.B2;
                //    ThirdB = validation.B3;
                //    firstI = validation.E1;
                //    SecondI = validation.E2;
                //}
                //else
                //{
                //    var scalepart = paysScale.Split('-');
                //    firstB = Convert.ToDecimal(scalepart[0]);
                //    firstI = Convert.ToDecimal(scalepart[1]);
                //    SecondB = Convert.ToDecimal(scalepart[2]);
                //    if (scalepart.Length > 3)
                //    {
                //        SecondI = Convert.ToDecimal(scalepart[3]);
                //        ThirdB = Convert.ToDecimal(scalepart[4]);
                //    }
                //}

                //decimal? EnteredBasic = NewCurrentBasic;
                //decimal? dblIncrement = null;
                //decimal? forMatch = null;
                //string EmpName = validation.Name;
                //string EmpCode = validation.EmployeeCode;

                //if (firstB <= EnteredBasic && EnteredBasic < SecondB)
                //{
                //    dblIncrement = firstI;
                //    forMatch = firstB;
                //    while (forMatch != EnteredBasic)
                //    {
                //        forMatch += dblIncrement;
                //        if (forMatch > SecondB)
                //            break;
                //    }
                //    if (forMatch != EnteredBasic)
                //    {
                //        strNewBasicAmount = "New Basic Amount is not fall in Current Salary Slab for employee : " + EmpName + "-" + EmpCode + "";
                //        flag = false;
                //        return flag;
                //    }
                //}
                //else if ((SecondB <= EnteredBasic) && (ThirdB == 0))
                //{
                //    dblIncrement = firstI;
                //    forMatch = SecondB;
                //    while (forMatch != EnteredBasic)
                //    {
                //        forMatch += dblIncrement;
                //        if (forMatch > (SecondB + (dblIncrement * 15)))
                //            break;
                //    }

                //    strNewBasicAmount = "New Basic Amount is not fall in Current Salary Slab for employee : " + EmpName + "-" + EmpCode + "";
                //    flag = false;
                //    return flag;
                //}
                //else if ((SecondB <= EnteredBasic && EnteredBasic < ThirdB) && (ThirdB != 0))
                //{
                //    dblIncrement = SecondI;
                //    forMatch = SecondB;
                //    while (forMatch != EnteredBasic)
                //    {
                //        forMatch += dblIncrement;
                //        if (forMatch > ThirdB)
                //            break;
                //    }

                //    strNewBasicAmount = "New Basic Amount is not fall in Current Salary Slab for employee : " + EmpName + "-" + EmpCode + "";
                //    flag = false;
                //    return flag;
                //}
                //else if (ThirdB <= EnteredBasic && ThirdB != 0)
                //{
                //    dblIncrement = SecondI;
                //    forMatch = ThirdB;

                //    while (forMatch != EnteredBasic)
                //    {
                //        forMatch += dblIncrement;
                //        if (forMatch > (ThirdB + (dblIncrement * 15)))
                //            break;
                //    }

                //    strNewBasicAmount = "New Basic Amount is not fall in Current Salary Slab for employee : " + EmpName + "-" + EmpCode + "";
                //    flag = false;
                //    return flag;
                //}
                //else
                //{
                //    strNewBasicAmount = "Salary Slab is not Assigned to the Respective Designation!";
                //    flag = false;
                //    return flag;
                //}
            }
            else
            {
                strNewBasicAmount = "Salary Slab is not Assigned to the Respective Designation!";
                flag = false;
                return flag;
            }
            return flag;
        }
        [HttpPost]
        public ActionResult UpdateBasicDetails(ProjectedIncrementViewModel pVM)
        {
            try
            {
                ModelState.Clear();
                //string strEmpCode;
                //decimal? dblBasic;
                decimal? dblNewBasic;
                decimal? dblIncrement;
                //int i = 0;
                //string level = "";
                //int limit = 0;

                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                if (pVM.stopIncrement == 1)
                {
                    projectedVM.Message = "There may be increment stopped for some employee.To see the details of those employee. Please Click on Ok or click on Cancel to Increment the Basic Salary.";
                    projectedVM.stopIncrement = 0;
                    TempData["flgOpenFromIncrement"] = true;
                    TempData.Keep("flgOpenFromIncrement");
                    TempData["gintIncrementMonth"] = pVM.incrementMonthId;
                    TempData.Keep("gintIncrementMonth");
                    var projectedInc = incrementService.GetProjectedIncrementDetails(pVM.BranchID, pVM.EmployeeCode, pVM.EmployeeName, pVM.incrementMonthId);
                    projectedInc.ForEach(x => { x.CurrentBasic = Convert.ToInt32(x.CurrentBasic); x.OldBasic = Convert.ToInt32(x.OldBasic); x.LastIncrement = Convert.ToInt32(x.LastIncrement); });
                    projectedVM.projectedIncrement = projectedInc;
                    projectedVM.incrementMonthId = pVM.incrementMonthId;
                    projectedVM.userRights = userAccessRight;

                    return PartialView("_ProjectedIncrementGridView", projectedVM);
                }
                else
                {
                    var updateBasicEmpDetails = incrementService.GetValidateNewBasicAmountDetails(null, pVM.incrementMonthId).ToList();
                    List<Model.ProjectedIncrement> UpdateBasicAmount = new List<Model.ProjectedIncrement>();

                    DataTable dataTable = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(updateBasicEmpDetails);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int limit = 0;
                        decimal?[] b = new decimal?[40];
                        decimal?[] e = new decimal?[40];
                        string strEmpCode = row["EmployeeCode"].ToString();
                        string empName = row["Name"].ToString();
                        decimal? dblBasic = row["e_Basic"] == null ? 0 : Convert.ToDecimal(row["e_Basic"]);
                        string level = row["Level"].ToString();
                        string payScale = row["payscale"].ToString();

                        if (level == "1" || level == "2" || level == "3" || level == "4" || level == "5" || level == "6" || level == "7" || level == "8" || level == "9" || level == "10")
                            limit = 40;
                        else if (level == "11")
                            limit = 39;
                        else if (level == "12")
                            limit = 34;
                        else if (level == "13")
                            limit = 20;
                        else if (level == "13A")
                            limit = 18;
                        else if (level == "14")
                            limit = 15;
                        else if (level == "15")
                            limit = 8;
                        else if (level == "16")
                            limit = 4;
                        else if (level == "17")
                            limit = 1;
                        else if (level == "18")
                            limit = 1;

                        var scalepart = payScale.Split('-');

                        if (payScale == "0")
                        {
                            for (int x = 1; x <= limit; x++)
                            {
                                b[x - 1] = Convert.ToDecimal(row["B" + x]);
                                e[x - 1] = Convert.ToDecimal(row["E" + x]);
                            }
                        }
                        else
                        {
                            int even = 0;
                            int odd = 0;
                            for (int p = 0; p <= scalepart.Length - 1; p++)
                            {
                                if (Convert.ToInt32(p) % 2 == 0)
                                {
                                    b[even] = Convert.ToDecimal(scalepart[Convert.ToInt32(p)]);
                                    even++;
                                }
                                else
                                {
                                    e[odd] = Convert.ToDecimal(scalepart[Convert.ToInt32(p)]);
                                    odd++;
                                }
                            }
                        }

                        dblIncrement = 0;
                        bool SlabMatch = false;
                        for (int x = 1; x <= limit; x++)
                        {
                            if (b[x - 1] == dblBasic)
                            {
                                dblIncrement = e[x - 1];
                                SlabMatch = true;
                            }
                        }

                        if (SlabMatch)
                        {
                            //Update old basic with new basic in tblEmployeesalary table
                            dblNewBasic = dblBasic + dblIncrement;
                            Model.ProjectedIncrement updateBasicModel = new Model.ProjectedIncrement();

                            updateBasicModel.EmployeeID = Convert.ToInt32(row["EmployeeId"]);
                            updateBasicModel.EmployeeCode = row["EmployeeCode"].ToString();
                            updateBasicModel.BranchID = Convert.ToInt32(row["BranchID"]);
                            updateBasicModel.CurrentBasic = dblNewBasic;
                            updateBasicModel.OldBasic = dblBasic/*DBNull.Value.Equals(row["LastBasic"]) == true ? 0 : Convert.ToDecimal(row["LastBasic"])*/;
                            updateBasicModel.LastIncrement = dblIncrement/*DBNull.Value.Equals(row["LastIncrement"]) == true ? 0 : Convert.ToDecimal(row["LastIncrement"])*/;
                            UpdateBasicAmount.Add(updateBasicModel);
                        }
                    }

                    //for (i = 0; i < updateBasicEmpDetails.Count(); i++)
                    //{
                    //    decimal?[] b = new decimal?[40];
                    //    decimal?[] e = new decimal?[40];
                    //    dblIncrement = 0;
                    //    strEmpCode = updateBasicEmpDetails[i].EmployeeCode;
                    //    dblBasic = updateBasicEmpDetails[i].e_Basic == null ? 0 : updateBasicEmpDetails[i].e_Basic;
                    //    level = updateBasicEmpDetails[i].Level;
                    //    string payscale = updateBasicEmpDetails[i].payscale;

                    //    if (level == "01" || level == "02" || level == "03" || level == "04" || level == "05" || level == "06" || level == "07" || level == "08" || level == "09" || level == "10")
                    //        limit = 40;
                    //    else if (level == "11")
                    //        limit = 39;
                    //    else if (level == "12")
                    //        limit = 34;
                    //    else if (level == "13")
                    //        limit = 20;
                    //    else if (level == "13A")
                    //        limit = 18;
                    //    else if (level == "14")
                    //        limit = 15;
                    //    else if (level == "15")
                    //        limit = 8;
                    //    else if (level == "16")
                    //        limit = 4;
                    //    else if (level == "17")
                    //        limit = 1;
                    //    else if (level == "18")
                    //        limit = 1;

                    //    if (payscale == "0")
                    //    {
                    //        for (int x = 1; x <= limit; x++)
                    //        {
                    //            b[x] = BasicIncrementValue(x, updateBasicEmpDetails, "B");
                    //            e[x] = BasicIncrementValue(x, updateBasicEmpDetails, "E");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var scalepart = payscale.Split('-');
                    //        int even = 0;
                    //        int odd = 0;
                    //        for (int p = 0; p <= scalepart.Length - 1; p++)
                    //        {
                    //            if (Convert.ToInt32(p) % 2 == 0)
                    //            {
                    //                b[even] = Convert.ToDecimal(scalepart[Convert.ToInt32(p)]);
                    //                even++;
                    //            }
                    //            else
                    //            {
                    //                e[odd] = Convert.ToDecimal(scalepart[Convert.ToInt32(p)]);
                    //                odd++;
                    //            }
                    //        }
                    //    }
                    //    dblIncrement = 0;
                    //    bool SlabMatch = false;
                    //    for (int x = 1; x <= limit; x++)
                    //    {
                    //        if (b[x] == dblBasic)
                    //        {
                    //            dblIncrement = e[x];
                    //            SlabMatch = true;
                    //        }
                    //    }
                    //    if (SlabMatch)
                    //    {
                    //        //Update old basic with new basic in tblEmployeesalary table
                    //        dblNewBasic = dblBasic + dblIncrement;
                    //        Model.ProjectedIncrement updateBasicModel = new Model.ProjectedIncrement();
                    //        updateBasicModel.EmployeeID = updateBasicEmpDetails[i].EmployeeId;
                    //        updateBasicModel.EmployeeCode = updateBasicEmpDetails[i].EmployeeCode;
                    //        updateBasicModel.BranchID = updateBasicEmpDetails[i].BranchID;
                    //        updateBasicModel.CurrentBasic = dblNewBasic;
                    //        updateBasicModel.OldBasic = updateBasicEmpDetails[i].LastBasic;
                    //        updateBasicModel.LastIncrement = updateBasicEmpDetails[i].LastIncrement;
                    //        UpdateBasicAmount.Add(updateBasicModel);
                    //    }
                    //}
                    //Add Update Function 
                    var insertUpdate = incrementService.UpdateProjectedEmployeeSalaryDetails(UpdateBasicAmount);
                    var projectedInc = incrementService.GetProjectedIncrementDetails(pVM.BranchID, pVM.EmployeeCode, pVM.EmployeeName, pVM.incrementMonthId);
                    projectedInc.ForEach(x => { x.CurrentBasic = Convert.ToInt32(x.CurrentBasic); x.OldBasic = Convert.ToInt32(x.OldBasic); x.LastIncrement = Convert.ToInt32(x.LastIncrement); });
                    TempData["ProjectedIncrementDetails"] = projectedInc;
                    projectedVM.projectedIncrement = projectedInc;
                    projectedVM.userRights = userAccessRight;
                    projectedVM.incrementMonthId = pVM.incrementMonthId;
                    ViewBag.Message = "Salary Increment is completed successfully!";
                    return PartialView("_ProjectedIncrementGridView", projectedVM);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //public decimal? BasicIncrementValue(int x, List<Model.ValidateNewBasicAmount> updateBasicEmpDetails, string type)
        //{
        //    decimal? value;
        //    if (type == "B")
        //    {
        //        switch (x)
        //        {
        //            case 1:
        //                value = updateBasicEmpDetails[x].B1;
        //                break;
        //            case 2:
        //                value = updateBasicEmpDetails[x].B2;
        //                break;
        //            case 3:
        //                value = updateBasicEmpDetails[x].B3;
        //                break;
        //            case 4:
        //                value = updateBasicEmpDetails[x].B4;
        //                break;
        //            case 5:
        //                value = updateBasicEmpDetails[x].B5;
        //                break;
        //            case 6:
        //                value = updateBasicEmpDetails[x].B6;
        //                break;
        //            case 7:
        //                value = updateBasicEmpDetails[x].B7;
        //                break;
        //            case 8:
        //                value = updateBasicEmpDetails[x].B8;
        //                break;
        //            case 9:
        //                value = updateBasicEmpDetails[x].B9;
        //                break;
        //            case 10:
        //                value = updateBasicEmpDetails[x].B10;
        //                break;
        //            case 11:
        //                value = updateBasicEmpDetails[x].B11;
        //                break;
        //            case 12:
        //                value = updateBasicEmpDetails[x].B12;
        //                break;
        //            case 13:
        //                value = updateBasicEmpDetails[x].B13;
        //                break;
        //            case 14:
        //                value = updateBasicEmpDetails[x].B14;
        //                break;
        //            case 15:
        //                value = updateBasicEmpDetails[x].B15;
        //                break;
        //            case 16:
        //                value = updateBasicEmpDetails[x].B16;
        //                break;
        //            case 17:
        //                value = updateBasicEmpDetails[x].B17;
        //                break;
        //            case 18:
        //                value = updateBasicEmpDetails[x].B18;
        //                break;
        //            case 19:
        //                value = updateBasicEmpDetails[x].B19;
        //                break;
        //            case 20:
        //                value = updateBasicEmpDetails[x].B20;
        //                break;
        //            case 21:
        //                value = updateBasicEmpDetails[x].B21;
        //                break;
        //            case 22:
        //                value = updateBasicEmpDetails[x].B22;
        //                break;
        //            case 23:
        //                value = updateBasicEmpDetails[x].B23;
        //                break;
        //            case 24:
        //                value = updateBasicEmpDetails[x].B24;
        //                break;
        //            case 25:
        //                value = updateBasicEmpDetails[x].B25;
        //                break;
        //            case 26:
        //                value = updateBasicEmpDetails[x].B26;
        //                break;
        //            case 27:
        //                value = updateBasicEmpDetails[x].B27;
        //                break;
        //            case 28:
        //                value = updateBasicEmpDetails[x].B28;
        //                break;
        //            case 29:
        //                value = updateBasicEmpDetails[x].B29;
        //                break;
        //            case 30:
        //                value = updateBasicEmpDetails[x].B30;
        //                break;
        //            case 31:
        //                value = updateBasicEmpDetails[x].B31;
        //                break;
        //            case 32:
        //                value = updateBasicEmpDetails[x].B32;
        //                break;
        //            case 33:
        //                value = updateBasicEmpDetails[x].B33;
        //                break;
        //            case 34:
        //                value = updateBasicEmpDetails[x].B34;
        //                break;
        //            case 35:
        //                value = updateBasicEmpDetails[x].B35;
        //                break;
        //            case 36:
        //                value = updateBasicEmpDetails[x].B36;
        //                break;
        //            case 37:
        //                value = updateBasicEmpDetails[x].B37;
        //                break;
        //            case 38:
        //                value = updateBasicEmpDetails[x].B38;
        //                break;
        //            case 39:
        //                value = updateBasicEmpDetails[x].B39;
        //                break;
        //            case 40:
        //                value = updateBasicEmpDetails[x].B40;
        //                break;
        //            default:
        //                value = 0;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        switch (x)
        //        {
        //            case 1:
        //                value = updateBasicEmpDetails[x].B1;
        //                break;
        //            case 2:
        //                value = updateBasicEmpDetails[x].B2;
        //                break;
        //            case 3:
        //                value = updateBasicEmpDetails[x].B3;
        //                break;
        //            case 4:
        //                value = updateBasicEmpDetails[x].B4;
        //                break;
        //            case 5:
        //                value = updateBasicEmpDetails[x].B5;
        //                break;
        //            case 6:
        //                value = updateBasicEmpDetails[x].B6;
        //                break;
        //            case 7:
        //                value = updateBasicEmpDetails[x].B7;
        //                break;
        //            case 8:
        //                value = updateBasicEmpDetails[x].B8;
        //                break;
        //            case 9:
        //                value = updateBasicEmpDetails[x].B9;
        //                break;
        //            case 10:
        //                value = updateBasicEmpDetails[x].B10;
        //                break;
        //            case 11:
        //                value = updateBasicEmpDetails[x].B11;
        //                break;
        //            case 12:
        //                value = updateBasicEmpDetails[x].B12;
        //                break;
        //            case 13:
        //                value = updateBasicEmpDetails[x].B13;
        //                break;
        //            case 14:
        //                value = updateBasicEmpDetails[x].B14;
        //                break;
        //            case 15:
        //                value = updateBasicEmpDetails[x].B15;
        //                break;
        //            case 16:
        //                value = updateBasicEmpDetails[x].B16;
        //                break;
        //            case 17:
        //                value = updateBasicEmpDetails[x].B17;
        //                break;
        //            case 18:
        //                value = updateBasicEmpDetails[x].B18;
        //                break;
        //            case 19:
        //                value = updateBasicEmpDetails[x].B19;
        //                break;
        //            case 20:
        //                value = updateBasicEmpDetails[x].B20;
        //                break;
        //            case 21:
        //                value = updateBasicEmpDetails[x].B21;
        //                break;
        //            case 22:
        //                value = updateBasicEmpDetails[x].B22;
        //                break;
        //            case 23:
        //                value = updateBasicEmpDetails[x].B23;
        //                break;
        //            case 24:
        //                value = updateBasicEmpDetails[x].B24;
        //                break;
        //            case 25:
        //                value = updateBasicEmpDetails[x].B25;
        //                break;
        //            case 26:
        //                value = updateBasicEmpDetails[x].B26;
        //                break;
        //            case 27:
        //                value = updateBasicEmpDetails[x].B27;
        //                break;
        //            case 28:
        //                value = updateBasicEmpDetails[x].B28;
        //                break;
        //            case 29:
        //                value = updateBasicEmpDetails[x].B29;
        //                break;
        //            case 30:
        //                value = updateBasicEmpDetails[x].B30;
        //                break;
        //            case 31:
        //                value = updateBasicEmpDetails[x].B31;
        //                break;
        //            case 32:
        //                value = updateBasicEmpDetails[x].B32;
        //                break;
        //            case 33:
        //                value = updateBasicEmpDetails[x].B33;
        //                break;
        //            case 34:
        //                value = updateBasicEmpDetails[x].B34;
        //                break;
        //            case 35:
        //                value = updateBasicEmpDetails[x].B35;
        //                break;
        //            case 36:
        //                value = updateBasicEmpDetails[x].B36;
        //                break;
        //            case 37:
        //                value = updateBasicEmpDetails[x].B37;
        //                break;
        //            case 38:
        //                value = updateBasicEmpDetails[x].B38;
        //                break;
        //            case 39:
        //                value = updateBasicEmpDetails[x].B39;
        //                break;
        //            case 40:
        //                value = updateBasicEmpDetails[x].B40;
        //                break;
        //            default:
        //                value = 0;
        //                break;
        //        }
        //    }
        //    return value;
        //}

        public ActionResult StopIncrementDetails(ProjectedIncrementViewModel pVM)
        {
            try
            {
                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                List<int?> monthArray = new List<int?>();
                if (pVM.incrementMonthId == 1)
                {
                    projectedVM.January = true;
                    monthArray.Add(1);
                }
                else if (pVM.incrementMonthId == 7)
                {
                    projectedVM.July = true;
                    monthArray.Add(7);
                }
                int?[] incrmentMonth = monthArray.ToArray();
                bool onlystop = TempData["flgOpenFromIncrement"] == null ? false : (bool)TempData["flgOpenFromIncrement"];
                TempData.Keep("flgOpenFromIncrement");
                var validateincrement = onlystop;
                projectedVM.OnlyStopped = onlystop;
                if (incrmentMonth.Count() > 0)
                {
                    var stopIncrementDetails = incrementService.GetStopIncrementDetails(incrmentMonth, validateincrement);
                    projectedVM.stopIncrementDetails = stopIncrementDetails;
                }
                return PartialView("_ShowStopIncrementWindow", projectedVM);
                //return PartialView("_StopIncrementDetails", projectedVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _PostStopIncrement(ProjectedIncrementViewModel pVM)
        {
            try
            {
                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                ModelState.Clear();
                var flag = incrementService.UpdateStopIncrementDetails(pVM.stopIncrementDetails);
                if (flag)
                {
                    int? month = TempData["gintIncrementMonth"] == null ? 0 : (int?)TempData["gintIncrementMonth"];
                    TempData.Keep("gintIncrementMonth");
                    bool onlyStopped = TempData["flgOpenFromIncrement"] == null ? false : (bool)TempData["flgOpenFromIncrement"];
                    TempData.Keep("flgOpenFromIncrement");
                    pVM.OnlyStopped = onlyStopped;
                    if (month == 1)
                        pVM.January = true;
                    else if (month == 7)
                        pVM.July = true;
                    List<int?> monthArray = new List<int?>();
                    if (pVM.January)
                        monthArray.Add(1);
                    else if (pVM.July)
                        monthArray.Add(7);
                    else if (pVM.January && pVM.July)
                    {
                        monthArray.Add(1);
                        monthArray.Add(7);
                    }
                    int?[] incrmentMonth = monthArray.ToArray();
                    var validateincrement = pVM.OnlyStopped;
                    var stopIncrementDetails = incrementService.GetStopIncrementDetails(incrmentMonth, validateincrement);
                    projectedVM.stopIncrementDetails = stopIncrementDetails;
                    projectedVM.January = pVM.January;
                    projectedVM.July = pVM.July;
                    projectedVM.OnlyStopped = pVM.OnlyStopped;
                    ViewBag.Message = "Stop Increment updated successfully!";
                }
                else
                {
                    projectedVM.stopIncrementDetails = pVM.stopIncrementDetails;
                    projectedVM.January = pVM.January;
                    projectedVM.July = pVM.July;
                    projectedVM.OnlyStopped = pVM.OnlyStopped;
                }
                return PartialView("_ShowStopIncrementWindow", projectedVM);
                //return PartialView("_StopIncrementDetails", projectedVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult ListStopIncrementDetails(ProjectedIncrementViewModel prVM)
        {
            log.Info($"ProjectedIncrementController/ListStopIncrementDetails");
            try
            {
                ProjectedIncrementViewModel projectedVM = new ProjectedIncrementViewModel();
                projectedVM.userRights = userAccessRight;
                int? month = TempData["gintIncrementMonth"] == null ? 0 : (int?)TempData["gintIncrementMonth"];
                TempData.Keep("gintIncrementMonth");
                bool onlyStopped = TempData["flgOpenFromIncrement"] == null ? false : (bool)TempData["flgOpenFromIncrement"];
                TempData.Keep("flgOpenFromIncrement");
                prVM.OnlyStopped = onlyStopped;
                if (month == 1)
                    prVM.January = true;
                else if (month == 7)
                    prVM.July = true;

                List<int?> monthArray = new List<int?>();
                if (prVM.January)
                    monthArray.Add(1);
                else if (prVM.July)
                    monthArray.Add(7);
                else if (prVM.January && prVM.July)
                {
                    monthArray.Add(1);
                    monthArray.Add(7);
                }
                int?[] incrmentMonth = monthArray.ToArray();
                bool validateincrement = prVM.OnlyStopped;

                if (incrmentMonth.Count() > 0)
                {
                    var stopIncrementDetails = incrementService.GetStopIncrementDetails(incrmentMonth, validateincrement);
                    projectedVM.stopIncrementDetails = stopIncrementDetails;
                    projectedVM.January = prVM.January;
                    projectedVM.July = prVM.July;
                    projectedVM.OnlyStopped = prVM.OnlyStopped;
                }
                return PartialView("_ShowStopIncrementWindow", projectedVM);
                //return PartialView("_StopIncrementDetails", projectedVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult ExportApplicableIncrement(ProjectedIncrementViewModel pVM)
        {
            log.Info($"ProjectedIncrementController/ExportApplicableIncrement");
            try
            {
                pVM.EmployeeCode = pVM.EmployeeCode != null ? pVM.EmployeeCode.Trim() : pVM.EmployeeCode;
                pVM.EmployeeName = pVM.EmployeeName != null ? pVM.EmployeeName.Trim() : pVM.EmployeeName;
                pVM.incrementMonthId = pVM.incrementMonthId == 0 ? null : pVM.incrementMonthId;
                string fileName = string.Empty, msg = string.Empty;
                string fullPath = Server.MapPath("~/FileDownload/");
                fileName = ExtensionMethods.SetUniqueFileName("ApplicableIncr-", FileExtension.xlsx);
                var res = incrementService.ExportIncrement(pVM.BranchID, pVM.incrementMonthId, pVM.EmployeeName, pVM.EmployeeCode, fileName, fullPath, "1");
                return Json(new { fileName = fileName, fullPath = fullPath + fileName, htmlData = ConvertViewToString("_ProjectedIncrementGridView", pVM), message = "success" });
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}