using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers
{
    public class PFAccountBalanceController : BaseController
    {
        private readonly IDropdownBindService ddlService;
        private readonly IPFAccountBalanceServices pfAcService;
        public PFAccountBalanceController(IDropdownBindService ddlService, IPFAccountBalanceServices pfAcService)
        {
            this.ddlService = ddlService;
            this.pfAcService = pfAcService;
        }
        // GET: PFAccountBalance
        public ActionResult Index()
        {
            PFAccountBalanceViewModel pfView = new PFAccountBalanceViewModel();
            pfView.EmpPFOPBalance = new EmpPFOpBalance();
            return View(pfView);
        }

        [HttpGet]
        public PartialViewResult _PFAccountBalance()
        {
            log.Info($"PFAccountBalanceController/_PFAccountBalance");
            try
            {
                EmpPFOpBalance empBal = new EmpPFOpBalance();
                empBal.PensionDeduct = false;
                empBal.IsInterestCalculate = false;
                ViewBag.Employees = ddlService.GetAllEmployee();
                return PartialView("_PFAccountBalForm", empBal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public PartialViewResult _PFAccountBalanceGridView()
        {
            log.Info($"PFAccountBalanceController/_PFAccountBalanceGridView");
            try
            {
                EmpPFOpBalance empBal = new EmpPFOpBalance();
                empBal.NonRefundLoan = 0;
                empBal.AdditionEmployeeAc = 0;
                empBal.WithdrawlEmployeeAc = 0;
                empBal.IntWDempl = 0;
                empBal.AdditionEmployerAc = 0;
                empBal.WithdrawlEmployerAc = 0;
                empBal.IntWDemplr = 0;
                return PartialView("_PFAccountBalViewPart", empBal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public JsonResult EmployeePFDetail(int employeeID, int month, int year)
        {
            log.Info($"PFAccountBalanceController/EmployeePFDetail/employeeID={employeeID}");
            try
            {
                var getPFDtl = pfAcService.GetEmployeePFDetail(employeeID, month, year);
                return Json(new { htmlData = ConvertViewToString("_PFAccountBalViewPart", getPFDtl), pfno = getPFDtl.PFAcNo, empOPNxt = getPFDtl.TotalPFOpeningEmpl.ThousandSeprator(), emprOPNxt = getPFDtl.TotalPFOpeningEmplr.ThousandSeprator() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public JsonResult SearchPFAccountBalance(int pfNo, int month, int year)
        {
            log.Info($"PFAccountBalanceController/SearchPFAccountBalance/employeeID={pfNo}");
            try
            {
                var getPFDtl = pfAcService.SearchEmployeePFDetail(pfNo, month, year);
                return Json(new { htmlData = ConvertViewToString("_PFAccountBalViewPart", getPFDtl), emp = getPFDtl.EmployeeID, empOPNxt = getPFDtl.TotalPFOpeningEmpl.ThousandSeprator(), emprOPNxt = getPFDtl.TotalPFOpeningEmplr.ThousandSeprator() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult _PostPFAccountBalance(EmpPFOpBalance pfOpbal, string ButtonType)
        {
            log.Info($"PFAccountBalanceController/_PostPFAccountBalance");
            try
            {
                pfOpbal.CreatedBy = userDetail.UserID;
                pfOpbal.CreatedOn = DateTime.Now;
                if (ButtonType == "Update Interest")
                {
                    int result1 = pfAcService.UpdateInterest(pfOpbal.Salyear, pfOpbal.InterestRate);
                }
                else
                {
                    var result = pfAcService.InsertPFDetail(pfOpbal);
                    if (result)
                    {
                        var getPFDtl = pfAcService.GetEmployeePFDetail(pfOpbal.EmployeeID.Value, pfOpbal.Salmonth, pfOpbal.Salyear);
                        return Json(new { htmlData = ConvertViewToString("_PFAccountBalViewPart", getPFDtl), res = result, pfno = getPFDtl.PFAcNo, empOPNxt = getPFDtl.TotalPFOpeningEmpl.ThousandSeprator(), emprOPNxt = getPFDtl.TotalPFOpeningEmplr.ThousandSeprator() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ViewBag.Employees = ddlService.GetAllEmployee();
                        return PartialView("_PFAccountBalForm", pfOpbal);
                    }
                }
                return PartialView("_PFAccountBalForm", pfOpbal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //private void GetopningBalancefornonRegular(int year)
        //{
        //    DataTable dtemp; string stropbal; string stropbal1; SqlDataAdapter da = new SqlDataAdapter(); DataTable dt = new DataTable(); SqlDataAdapter da1 = new SqlDataAdapter(); DataTable dt1 = new DataTable();
        //    int intr; SqlCommand cmd = new SqlCommand(); int iopbal; string str; string tyear2; SqlDataAdapter datotval = new SqlDataAdapter(); DataTable dttotval = new DataTable();
        //    string fromyear, tyear, tyear1, fromyear1, toYear; int sal, yr; int iemp; int i; string sEmployee;
        //    int opbalemp, opbalemplr; tyear1 = year + 1; tyear2 = year + 2; fromyear = tyear1 + "03"; fromyear1 = tyear2 + "03";
        //    toYear = (tyear2 + 1) + "02"; tyear = year + 1; tyear = tyear + "02"; tyear2 = tyear2 + "02";

        //    if (txtROI.Text == "")
        //    {
        //        MessageBox.Show("Please select Rate");
        //        return;
        //    }
        //    string sEmployee1; SqlDataAdapter daopbal = new SqlDataAdapter(); DataTable dtopbal = new DataTable(); bool pensiondeduct; stropbal = ""; sEmployee1 = "";
        //    stropbal = "select distinct Pfno from TblPFOPBalanceManual where pfno not in (select distinct Pfacno from tblpfopbalance where (CAST(salyear AS VARCHAR) + CASE WHEN LEN(salmonth) < 2 THEN '0' + salmonth ELSE salmonth END)>= '" + fromyear + "' AND (CAST(salyear AS VARCHAR) + CASE WHEN LEN(salmonth) < 2 THEN '0' + salmonth ELSE salmonth END) < = '" + tyear2 + "') ";
        //    daopbal = new SqlDataAdapter(stropbal, Con);
        //    daopbal.Fill(dtopbal);

        //    if (dtopbal.Rows.Count > 0)
        //    {
        //        #region
        //        for (iemp = 0; iemp <= dtopbal.Rows.Count - 1; iemp++)
        //        {
        //            sEmployee = dtopbal.Rows(iemp)("Pfno");
        //            sEmployee1 = sEmployee1 + "," + sEmployee;
        //            str = " select salmonth,salyear  from tblPFOpBalance sal  WHERE (CAST(SAL.SALYEAR AS VARCHAR) + CASE WHEN LEN(SAL.SALMONTH) < 2 THEN '0' + SAL.SALMONTH ELSE SAL.SALMONTH END) >= '" + fromyear + "' AND (CAST(SAL.SALYEAR AS VARCHAR) + CASE WHEN LEN(SAL.SALMONTH) < 2 THEN '0' + SAL.SALMONTH ELSE SAL.SALMONTH END) < = '" + tyear2 + "' group by salmonth,salyear";
        //            datotval = new SqlDataAdapter(str, Con);
        //            dttotval.Rows.Clear();
        //            datotval.Fill(dttotval);

        //            if (dttotval.Rows.Count > 0)
        //            {
        //                sal = 3;
        //                yr = CmbYear.Text;
        //                tyear1 = CmbYear.Text + 2;
        //                for (iopbal = 0; iopbal <= dttotval.Rows.Count - 1; iopbal++)
        //                {
        //                    stropbal = "select Pfno,month,year,sum(EPF) EPF,sum(VPF)VPF,sum(EMPPF)EMPPF,sum(PENSION)PENSION,sum(WithEmp)WithEmp,sum(WithEmplr)WithEmplr,sum(ADDEmp)ADDEmp,sum(AddEMplr)AddEMplr,sum(IntEmp)IntEmp,sum(IntEmplr) IntEmplr from TblPFOPBalanceManual where year='" + tyear1 + "' and month='" + sal + "' and pfno=" + dtopbal.Rows(iemp)("Pfno") + " group by pfno,year,month order by pfno,year,Convert(numeric,month)";
        //                    da = new SqlDataAdapter(stropbal, Con);
        //                    dt.Rows.Clear();
        //                    da.Fill(dt);


        //                    if (dt.Rows.Count > 0)
        //                    {
        //                        #region
        //                        stropbal = "select * from tblPFOpBalance where PFacno='" + dtopbal.Rows(iemp)("Pfno") + "' and salmonth='" + sal + "' and salyear='" + tyear1 + "'";
        //                        SqlDataAdapter daup = new SqlDataAdapter();
        //                        DataTable dtup = new DataTable();
        //                        daup = new SqlDataAdapter(stropbal, Con);
        //                        daup.Fill(dtup);
        //                        if (dtup.Rows.Count > 0)
        //                        {
        //                        }
        //                        else
        //                        {
        //                            stropbal = "select * from tblmstemployee where Pfno='" + dtopbal.Rows(iemp)("Pfno") + "'";
        //                            SqlDataAdapter daopbal1 = new SqlDataAdapter();
        //                            daopbal1 = new SqlDataAdapter(stropbal, Con);
        //                            DataTable dt2 = new DataTable();
        //                            dt2 = new DataTable();
        //                            dt2.Rows.Clear();
        //                            daopbal1.Fill(dt2);
        //                            stropbal = "";
        //                            stropbal = "insert into  tblPFOpBalance (Employeecode,PFAcNo,Salmonth,Salyear,BRANCHCODE,EmplOpBal,EmplrOpBal,EmployeePFCont,VPF,Pension,EmployerPFCont,EmployeeInterest,EmployerInterest,InterestNRPFLoan,InterestRate,NonRefundLoan,TotalPFBalance,InterestTotal,TotalPFOpeningEmpl,TotalPFOpeningEmplr,WithdrawlEmployeeAc,WithdrawlEmployerAc,AdditionEmployeeAc,AdditionEmployerAc,IntWDempl,IntWDemplr,Reason,PF_DAarear,VPF_DAarear,PF_payarear,VPF_payarear,PensionDeduct) values('" + dt2.Rows(i)(0) + "','" + dtopbal.Rows(iemp)("Pfno") + "','" + sal + "','" + tyear1 + "','" + dt2.Rows(0)("BRANCHCODE") + "',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 )";
        //                            // '''''''''''''''

        //                            cmd = new SqlCommand(stropbal, Con);
        //                            cmd.ExecuteNonQuery();
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region
        //                        stropbal = "select * from tblPFOpBalance where PFacno='" + dtopbal.Rows(iemp)("Pfno") + "' and salmonth='" + sal + "' and salyear='" + tyear1 + "'";
        //                        SqlDataAdapter daup = new SqlDataAdapter();
        //                        DataTable dtup = new DataTable();
        //                        daup = new SqlDataAdapter(stropbal, Con);
        //                        daup.Fill(dtup);
        //                        if (dtup.Rows.Count > 0)
        //                        {
        //                        }
        //                        else
        //                        {
        //                            stropbal = "select * from tblmstemployee where Pfno='" + dtopbal.Rows(iemp)("Pfno") + "'";
        //                            SqlDataAdapter daopbal1 = new SqlDataAdapter();
        //                            daopbal1 = new SqlDataAdapter(stropbal, Con);
        //                            DataTable dt2 = new DataTable();
        //                            dt2 = new DataTable();
        //                            dt2.Rows.Clear();
        //                            daopbal1.Fill(dt2);
        //                            stropbal = "";
        //                            stropbal = "insert into  tblPFOpBalance (Employeecode,PFAcNo,Salmonth,Salyear,BRANCHCODE,EmplOpBal,EmplrOpBal,EmployeePFCont,VPF,Pension,EmployerPFCont,EmployeeInterest,EmployerInterest,InterestNRPFLoan,InterestRate,NonRefundLoan,TotalPFBalance,InterestTotal,TotalPFOpeningEmpl,TotalPFOpeningEmplr,WithdrawlEmployeeAc,WithdrawlEmployerAc,AdditionEmployeeAc,AdditionEmployerAc,IntWDempl,IntWDemplr,Reason,PF_DAarear,VPF_DAarear,PF_payarear,VPF_payarear,PensionDeduct) values('" + dt2.Rows(i)(0) + "','" + dtopbal.Rows(iemp)("Pfno") + "','" + sal + "','" + tyear1 + "','" + dt2.Rows(0)("BRANCHCODE") + "',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 )";
        //                            // '''''''''''''''

        //                            cmd = new SqlCommand(stropbal, Con);
        //                            cmd.ExecuteNonQuery();
        //                        }
        //                        #endregion
        //                    }

        //                    if (sal == 12)
        //                    {
        //                        sal = 1;
        //                        tyear1 = tyear1 + 1;
        //                    }
        //                    else
        //                        sal = sal + 1;
        //                }
        //            }
        //        }
        //        #endregion
        //    }
        //    sEmployee1 = sEmployee1.Substring(1, sEmployee1.Length - 1);

        //    // For Update Employee
        //    str = "select distinct PFacno from tblPFOpBalance where (CAST(SALYEAR AS VARCHAR) + CASE WHEN LEN(SALMONTH) < 2 THEN '0' + SALMONTH ELSE SALMONTH END)>= '" + fromyear1 + "' AND (CAST(SALYEAR AS VARCHAR) + CASE WHEN LEN(SALMONTH) < 2 THEN '0' + SALMONTH ELSE SALMONTH END) < = '" + toYear + "' and Pfacno<>0 and Pfacno in  (" + sEmployee1 + ")   Order by PFacno";
        //    // group by employeecode 
        //    datotval = new SqlDataAdapter(str, Con);
        //    dtemp = new DataTable();
        //    dtemp.Rows.Clear();
        //    datotval.Fill(dtemp);

        //    if (dtemp.Rows.Count > 0)
        //    {
        //        for (i = 0; i <= dtemp.Rows.Count - 1; i++)
        //        {
        //            str = " select salmonth,salyear  from tblPFOpBalance sal  WHERE (CAST(SAL.SALYEAR AS VARCHAR) + CASE WHEN LEN(SAL.SALMONTH) < 2 THEN '0' + SAL.SALMONTH ELSE SAL.SALMONTH END) >= '" + fromyear + "' AND (CAST(SAL.SALYEAR AS VARCHAR) + CASE WHEN LEN(SAL.SALMONTH) < 2 THEN '0' + SAL.SALMONTH ELSE SAL.SALMONTH END) < = '" + tyear2 + "' group by salmonth,salyear";
        //            datotval = new SqlDataAdapter(str, Con);
        //            dttotval.Rows.Clear();
        //            datotval.Fill(dttotval);

        //            if (dttotval.Rows.Count > 0)
        //            {
        //                #region
        //                sal = 3;
        //                yr = CmbYear.Text;
        //                tyear1 = CmbYear.Text + 2;
        //                string monyear, monyearprevious;
        //                monyear = tyear1 + "03";
        //                monyearprevious = tyear1 + "02";
        //                for (iopbal = 0; iopbal <= dttotval.Rows.Count - 1; iopbal++)
        //                {
        //                    SqlDataAdapter daopbal1 = new SqlDataAdapter();
        //                    DataTable dtopbal1 = new DataTable();
        //                    bool pensiondeduct1;
        //                    stropbal = "";
        //                    stropbal = "select * from tblmstemployee where Pfno='" + dtemp.Rows(i)(0) + "'";
        //                    daopbal1 = new SqlDataAdapter(stropbal, Con);
        //                    daopbal1.Fill(dtopbal1);
        //                    if (dtopbal.Rows.Count > 0)
        //                    {
        //                        #region
        //                        if (IIf(IsDBNull(dtopbal1.Rows(0)("pensiondeduct")), 0, dtopbal1.Rows(0)("pensiondeduct")) == false)
        //                        {
        //                            stropbal = "select  ((EmplOpBal+EmployeePFCont+isnull(EPF,0)+a.VPF+isnull(b.VPF,0)+PF_DAarear+VPF_DAarear+PF_payarear+VPF_payarear+AdditionEmployeeAc+isnull(AddEmp,0))-(WithdrawlEmployeeAc+isnull(WithEmp,0))) as emptotal,((EmplrOpBal+EmployerPFCont+isnull(EmpPF,0)+PF_DAarear+PF_payarear+AdditionEmployerAc+isnull(AddEmplr,0)+a.Pension+isnull(b.pension,0))-(WithdrawlEmployerAc+isnull(WithEmplr,0))) as emprtotal from (select PFACNO,salmonth,salyear, sum(isnull(EmplOpBal,0)) EmplOpBal, sum(isnull(EmplrOpBal,0)) EmplrOpBal,sum(isnull(EmployeePFCont,0)) EmployeePFCont,sum(isnull(pf.VPF,0)) VPF,sum(isnull(PF_DAarear,0)) PF_DAarear,sum(isnull(VPF_DAarear,0)) VPF_DAarear,sum( isnull(PF_payarear,0))PF_payarear,sum(isnull(VPF_payarear,0)) VPF_payarear,sum(isnull(AdditionEmployeeAc,0)) AdditionEmployeeAc,sum(isnull(Pension,0)) Pension,sum(isnull(WithdrawlEmployeeAc,0)) WithdrawlEmployeeAc,sum(isnull(EmployerPFCont,0))EmployerPFCont,sum(isnull(AdditionEmployerAc,0))AdditionEmployerAc,sum(isnull(WithdrawlEmployerAc,0))WithdrawlEmployerAc from tblPFOpBalance pf where  (CAST(SALYEAR AS VARCHAR) + CASE WHEN LEN(SALMONTH) < 2 THEN '0' + SALMONTH ELSE SALMONTH END)= '" + monyearprevious + "' and pfacno='" + dtemp.Rows(i)(0) + "' group by PFACNO,salmonth,salyear)a Left Join (select PFno,month,year,sum(isnull(EPF,0)) EPF,sum(isnull(VPF,0)) VPF,sum(isnull(EmpPF,0)) EmpPF,sum(isnull(WithEmp,0)) WithEmp,sum(isnull(AddEmp,0)) AddEmp,sum(isnull(WithEmplr,0)) WithEmplr,sum(isnull(AddEmplr,0)) AddEmplr,sum(isnull(Pension,0)) Pension  from TblPFOPBalanceManual where (CAST(SALYEAR AS VARCHAR) + CASE WHEN LEN(MONTH) < 2 THEN '0' + MONTH ELSE MONTH END)= '" + monyearprevious + "'  and PFno='" + dtemp.Rows(i)(0) + "' group by PFno,month,year)b on a.salmonth= b.month and a.salyear= b.year and    b.PFNo=a.pfacno";

        //                            da = new SqlDataAdapter(stropbal, Con);
        //                            dt.Rows.Clear();
        //                            da.Fill(dt);
        //                        }
        //                        else
        //                        {
        //                            stropbal = "select  ((EmplOpBal+EmployeePFCont+isnull(EPF,0)+a.VPF+isnull(b.VPF,0)+PF_DAarear+VPF_DAarear+PF_payarear+VPF_payarear+AdditionEmployeeAc+isnull(AddEmp,0))-(WithdrawlEmployeeAc+isnull(WithEmp,0))) as emptotal,((EmplrOpBal+EmployerPFCont+isnull(EmpPF,0)+PF_DAarear+PF_payarear+AdditionEmployerAc+isnull(AddEmplr,0))-(WithdrawlEmployerAc+isnull(WithEmplr,0))) as emprtotal from (select PFACNO,salmonth,salyear, sum(isnull(EmplOpBal,0)) EmplOpBal, sum(isnull(EmplrOpBal,0)) EmplrOpBal,sum(isnull(EmployeePFCont,0)) EmployeePFCont,sum(isnull(pf.VPF,0)) VPF,sum(isnull(PF_DAarear,0)) PF_DAarear,sum(isnull(VPF_DAarear,0)) VPF_DAarear,sum( isnull(PF_payarear,0))PF_payarear,sum(isnull(VPF_payarear,0)) VPF_payarear,sum(isnull(AdditionEmployeeAc,0)) AdditionEmployeeAc,sum(isnull(WithdrawlEmployeeAc,0)) WithdrawlEmployeeAc,sum(isnull(EmployerPFCont,0))EmployerPFCont,sum(isnull(AdditionEmployerAc,0))AdditionEmployerAc,sum(isnull(WithdrawlEmployerAc,0))WithdrawlEmployerAc from tblPFOpBalance pf where (CAST(SALYEAR AS VARCHAR) + CASE WHEN LEN(SALMONTH) < 2 THEN '0' + SALMONTH ELSE SALMONTH END)= '" + monyearprevious + "'  and pfacno='" + dtemp.Rows(i)(0) + "' group by PFACNO,salmonth,salyear)a Left Join (select PFno,month,year,sum(isnull(EPF,0)) EPF,sum(isnull(VPF,0)) VPF,sum(isnull(EmpPF,0)) EmpPF,sum(isnull(WithEmp,0)) WithEmp,sum(isnull(AddEmp,0)) AddEmp,sum(isnull(WithEmplr,0)) WithEmplr,sum(isnull(AddEmplr,0)) AddEmplr from TblPFOPBalanceManual where  (CAST(YEAR AS VARCHAR) + CASE WHEN LEN(MONTH) < 2 THEN '0' + MONTH ELSE MONTH END)= '" + monyearprevious + "'  and PFno='" + dtemp.Rows(i)(0) + "' group by PFno,month,year)b on a.salmonth= b.month and a.salyear= b.year and b.PFNo=a.pfacno";

        //                            da = new SqlDataAdapter(stropbal, Con);
        //                            dt.Rows.Clear();
        //                            da.Fill(dt);
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                    }

        //                    if (dt.Rows.Count > 0)
        //                    {
        //                        #region
        //                        stropbal = "select * from tblPFOpBalance where PFacno='" + dtemp.Rows(i)(0) + "' and salmonth='" + sal + "' and salyear='" + tyear1 + "'";
        //                        SqlDataAdapter daup = new SqlDataAdapter();
        //                        DataTable dtup = new DataTable();
        //                        daup = new SqlDataAdapter(stropbal, Con);
        //                        daup.Fill(dtup);
        //                        if (dtup.Rows.Count > 0)
        //                        {
        //                            stropbal = "";
        //                            stropbal = "update tblPFOpBalance  set EmplOpBal=" + dt.Rows(0)("emptotal") + ", EmplrOpBal=" + dt.Rows(0)("emprtotal") + " where PFacno='" + dtemp.Rows(i)(0) + "' and salmonth='" + sal + "' and salyear='" + tyear1 + "'";
        //                            cmd = new SqlCommand(stropbal, Con);
        //                            cmd.ExecuteNonQuery();
        //                        }
        //                        else
        //                        {
        //                            stropbal = "select * from tblmstEmployee where PFno='" + dtemp.Rows(i)(0) + "'";
        //                            // stropbal = "select * from tblPFOpBalance where employeecode='" & dtemp.Rows(i)(0) & "' and salmonth=3 and salyear='" & Mid(CmbYear.Text, 1, 4) & "'"
        //                            SqlDataAdapter daup1 = new SqlDataAdapter();
        //                            DataTable dtup1 = new DataTable();
        //                            daup1 = new SqlDataAdapter(stropbal, Con);
        //                            daup1.Fill(dtup1);
        //                            stropbal = "";
        //                            stropbal = "insert into  tblPFOpBalance (Employeecode,PFAcNo,Salmonth,Salyear,BRANCHCODE,EmplOpBal,EmplrOpBal,EmployeePFCont,VPF,Pension,EmployerPFCont,EmployeeInterest,EmployerInterest,InterestNRPFLoan,InterestRate,NonRefundLoan,TotalPFBalance,InterestTotal,TotalPFOpeningEmpl,TotalPFOpeningEmplr,WithdrawlEmployeeAc,WithdrawlEmployerAc,AdditionEmployeeAc,AdditionEmployerAc,IntWDempl,IntWDemplr,Reason,PF_DAarear,VPF_DAarear,PF_payarear,VPF_payarear,PensionDeduct) values('" + dtup1.Rows(0)(0) + "','" + dtup1.Rows(0)("PFNO") + "','" + sal + "','" + tyear1 + "','" + dtup1.Rows(0)("BRANCHCODE") + "','" + dt.Rows(0)("emptotal") + "','" + dt.Rows(0)("emprtotal") + "',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 )";
        //                            // '''''''''''''''
        //                            cmd = new SqlCommand(stropbal, Con);
        //                            cmd.ExecuteNonQuery();
        //                        }
        //                        #endregion
        //                    }
        //                    if (sal == 12)
        //                    {
        //                        sal = 1;
        //                        tyear1 = (CmbYear.Text + 2) + 1;
        //                        monyear = tyear1 + "01";
        //                        monyearprevious = Convert.ToString(Convert.ToInt32(monyearprevious) + 1);
        //                    }
        //                    else
        //                    {
        //                        sal = sal + 1;
        //                        monyear = Convert.ToString(Convert.ToInt32(monyear) + 1);
        //                        if (sal == 2)
        //                            monyearprevious = tyear1 + "01";
        //                        else
        //                            monyearprevious = Convert.ToString(Convert.ToInt32(monyearprevious) + 1);
        //                    }
        //                }
        //                #endregion
        //            }
        //        }
        //    }

        //    Interaction.MsgBox("Record succesfully Updated", MsgBoxStyle.OKOnly);
        //}

    }
}