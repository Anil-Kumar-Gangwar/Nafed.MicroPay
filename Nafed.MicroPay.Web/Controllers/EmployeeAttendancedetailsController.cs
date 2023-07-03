using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using System.Reflection;
using Nafed.MicroPay.Common;

namespace MicroPay.Web.Controllers
{
    public class EmployeeAttendancedetailsController : BaseController
    {
        private readonly IEmployeeAttendancedetailsService employeeattendancedetailsservice;
        private readonly IDropdownBindService ddlService;
        public EmployeeAttendancedetailsController(IEmployeeAttendancedetailsService employeeattendancedetailsservice, IDropdownBindService ddlService)
        {
            this.employeeattendancedetailsservice = employeeattendancedetailsservice;
            this.ddlService = ddlService;
        }


        public ActionResult Index()
        {
            log.Info($"EmployeeAttendancedetailsController/Index");
            ViewBag.EmployeeID = userDetail.EmployeeID;
            ViewBag.BranchID = userDetail.BranchID;
         //   List<Model.EmployeeAttendanceDetails> reportingTo = new List<Model.EmployeeAttendanceDetails>();
           // reportingTo = employeeattendancedetailsservice.GetReportingTo(Convert.ToInt32(userDetail.EmployeeID));
         var reportingTo=   GetEmpProcessApprovalSetting((int)userDetail.EmployeeID, WorkFlowProcess.AttendanceApproval);
         //   DataTable DT = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(reportingTo);
            if (reportingTo !=null)
            {
                ViewBag.ReportingTo = "1";
            }
            else
            {
                ViewBag.ReportingTo = "0";
            }

            //if (userDetail.UserTypeID != (int)UserType.Employee)
            //{
            //    ViewBag.UserID = "1";
            //}
            //else
            //{
            //    ViewBag.UserID = "2";

            //}
            ViewBag.UserID = "1";
            return View(userAccessRight);
        }

        public ActionResult EmployeeAttendancedetailsGridView(Model.EmployeeAttendanceDetails empattendance, FormCollection frm)
        {
            log.Info($"EmployeeAttendancedetailsController/EmployeeAttendancedetailsGridView");
            try
            {              
                int branchId = Convert.ToInt32(frm.Get("ddlBranchList1"));              
                List<Model.EmployeeAttendanceDetails> attendanceList = new List<Model.EmployeeAttendanceDetails>();

                attendanceList = employeeattendancedetailsservice.GetEmployeeAttendanceList(empattendance.BranchId, empattendance.EmployeeId, empattendance.Month, empattendance.Year, userDetail.UserTypeID, Convert.ToInt16(userDetail.EmployeeID),empattendance.DepartmentId);
                DataTable DT = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(attendanceList);
                ViewBag.Month = empattendance.Month;
                ViewBag.Year = empattendance.Year;
                ViewBag.BranchID = empattendance.BranchId;
              //  DT.Columns.Remove("EmployeeID");
                Session["dtdata"] = DT;

                if (attendanceList?.Count > 0)
                {
                    var inTimeOutTimeList =
                      attendanceList.Where(y => y.InTimeOutTimes.Count > 0).
                      Select(y => new Model.EmployeeAttendanceDetails
                      {
                          EmployeeId = y.EmployeeId,
                          InTimeOutTimes = y.InTimeOutTimes
                      }).ToList();
                    Session["InTimeOutTime"] = inTimeOutTimeList;
                }


                // if (userDetail.UserTypeID != (int)UserType.Employee)
                // {

                return PartialView("_EmployeeAttendancedetailsGridView", attendanceList);
               // }
                //else
                //{
                //    if (empattendance.DisplayType == 1 || empattendance.DisplayType == 0)
                //    {
                //        return PartialView("_EmployeeAttendanceGridViewDetail", attendanceList);
                //    }
                //   else 
                //    {
                //        return PartialView("_EmployeeAttendancedetailsGridView", attendanceList);
                //    }
                //}
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        
        public ActionResult SearchEmployeeAttendancedetails(FormCollection formCollection)
        {
            log.Info($"EmployeeAttendancedetailsController/SearchEmployeeAttendancedetails");
            try
            {
                BindDropdowns();
                EmployeeAttendancedetailsViewModel employeeAttendanceVM = new EmployeeAttendancedetailsViewModel();
                var userTypeID = userDetail.UserTypeID;
                List<Model.SelectListModel> selectType = new List<Model.SelectListModel>();
                ViewBag.EmployeeDetails = new SelectList(selectType, "id", "value");
           
                return PartialView("_SearchEmployeeAttendancedetails", employeeAttendanceVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void BindDropdowns()
        {
            var ddlBranchList = ddlService.ddlBranchList();
            Model.SelectListModel selectBranch = new Model.SelectListModel();
            selectBranch.id = 0;
            selectBranch.value = "Select";
            ddlBranchList.Insert(0, selectBranch);
            ViewBag.Branch = new SelectList(ddlBranchList, "id", "value");

            var ddlDepartmentList = ddlService.ddlDepartmentList();
            //Model.SelectListModel selectDepartment = new Model.SelectListModel();
            //selectDepartment.id = 0;
            //selectDepartment.value = "Select";
            //ddlDepartmentList.Insert(0, selectDepartment);
            ViewBag.Department = new SelectList(ddlDepartmentList, "id", "value");

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
                return Json(new { employees = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        public void ExportPrinttableExcel()
        {
            if (Session["dtdata"] != null)
            {
                DataTable dtExport = (DataTable)Session["dtdata"];
                string name = "EmployeeAttendanceDetails.xls";
                var Dates = dtExport.Rows[0]["Date"].ToString();
                var Branch = dtExport.Rows[0]["BranchName"].ToString();

                List<Model.EmployeeAttendanceDetails> intTimeOutTime = new List<Model.EmployeeAttendanceDetails>();

                if (Session["InTimeOutTime"] != null)
                    intTimeOutTime = (List<Model.EmployeeAttendanceDetails>)Session["InTimeOutTime"];

                this.Response.Clear();
                this.Response.Charset = "";
                this.Response.ContentType = "application/vnd.ms-excel";
                this.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + name + "\"");
                System.Text.StringBuilder sbb = new System.Text.StringBuilder();
                sbb.Append("<html>");
                sbb.Append("<style>");
                sbb.Append("#table1 tr td { height: 60 px !important;}");
                sbb.Append("</style>");
                //Header information...........................................
                sbb.Append("<Table border= 1>");
                sbb.Append("<tr>");
                sbb.Append("<td align=center valign=middle rowspan=2 colspan=" + 16 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;vertical-align:middle;'><b>");
                sbb.Append($"Employee Attendance Details : {Dates} - Branch : {Branch}");
                sbb.Append("</b></td>");
                sbb.Append("</tr >");
                sbb.Append("</Table>");

                sbb.Append("<Table colspan="+16+" id='table1'>");
                sbb.Append("<tr>");
                sbb.Append("<td>");
                sbb.Append("");
                sbb.Append("</b></td>");
                sbb.Append("</tr >");
                sbb.Append("</Table>");
                for (int i = 0; i < dtExport.Rows.Count; i++)
                {
                    sbb.Append("<Table border= 1 id='table1'>");
                    sbb.Append("<tr>");
                    sbb.Append("<td colspan=" + 16 + " >");
                    sbb.Append($"Code : {dtExport.Rows[i]["Code"].ToString()}");
                    sbb.Append("</td>");
                    sbb.Append("</tr>");
                    sbb.Append("<tr>");
                    sbb.Append("<td colspan=" + 16 + " >");
                    sbb.Append($"Employee : {dtExport.Rows[i]["Name"].ToString()}");
                    sbb.Append("</td>");
                    sbb.Append("</tr>");
                    sbb.Append("<tr>");
                    sbb.Append("<td colspan=" + 16 + " >");
                    sbb.Append($"Department : {dtExport.Rows[i]["DepartmentName"].ToString()}");
                    sbb.Append("</td>");
                    sbb.Append("</tr>");
                    sbb.Append("</Table>");

                    ///------------------ First row of each employee
                    sbb.Append("<Table border= 1>");
                    sbb.Append("<tr>");
                    for (int k = 4; k < 20; k++)
                    {
                        sbb.Append("<td align=center valign=middle  style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");

                        if (k > 3)
                        {
                            if (dtExport.Columns[k].ColumnName.Contains("D"))
                                sbb.Append(dtExport.Columns[k].ColumnName.Replace("D", ""));

                            else
                                sbb.Append(dtExport.Columns[k].ColumnName);
                        }
                        else
                            sbb.Append(dtExport.Columns[k].ColumnName);

                        sbb.Append("</b></td>");
                    }
                    sbb.Append("</tr>");
                    var employeeID = Convert.ToInt32(dtExport.Rows[i]["EmployeeId"].ToString());
                    sbb.Append("<tr>");
                    for (int j = 4; j < 20; j++)
                    {
                        #region
                        if (dtExport.Rows[i][j].ToString().Trim() == "NA")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; background-color: yellow; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "W")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; background-color: gray;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "WP")
                        {
                            sbb.Append("<td align=center  valign=middle style=' white-space:nowrap ; color: red;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "CS")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; color: orange; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "EL")
                        {
                            sbb.Append("<td align=center style=' white-space:nowrap ; color: blue; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim().ToLower() == "p"
                            || dtExport.Rows[i][j].ToString().Trim().ToLower() == "sp"
                             || dtExport.Rows[i][j].ToString().Trim().ToLower() == "cla"
                              || dtExport.Rows[i][j].ToString().Trim().ToLower() == "ts"
                            )
                        {
                            sbb.Append("<td align=center  valign=middle style=' white-space:nowrap ; vertical-align:middle;'>");

                            var dayNumber = int.Parse(dtExport.Columns[j].ColumnName.Remove(0, 1));

                            if (intTimeOutTime.Any(y => y.EmployeeId == employeeID
                                 && y.InTimeOutTimes.Any(y1 => y1.Day == dayNumber)
                            ))
                            {
                                #region  ======== Append In-Time /Out-Time ===============

                                var attendanceOfSelectedDay =
                                    intTimeOutTime.FirstOrDefault(m => m.EmployeeId == employeeID).InTimeOutTimes.
                                    Where(y => y.Day == dayNumber).FirstOrDefault();

                                var inTime = Convert.ToDateTime(attendanceOfSelectedDay.InTime).ToString("HH:mm");

                                var outTime = attendanceOfSelectedDay.OutTime != string.Empty ? Convert.ToDateTime(attendanceOfSelectedDay.OutTime).ToString("HH:mm") : " ";


                                sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                                sbb.Append("<div class='p-2 text-center' style='font-size:1em;'>");
                                sbb.Append($"In-{inTime}");
                                sbb.Append("<br/>");
                                //sbb.Append($"-");
                                //sbb.Append("<br/>");
                                sbb.Append($"Out-{outTime}");
                                sbb.Append("</div>");

                                #endregion
                            }
                            else
                                sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else
                        {
                            sbb.Append("<td align=center style='white-space:nowrap ;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        sbb.Append("</td>");
                        #endregion
                    }
                    sbb.Append("</tr>");                   
                    sbb.Append("</Table>");

                    //// ---------------- Second row of each employee
                    sbb.Append("<Table border= 1>");
                    sbb.Append("<tr>");
                    for (int k = 20; k < 35; k++)
                    {
                        sbb.Append("<td align=center valign=middle  style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");

                        if (k > 3)
                        {
                            if (dtExport.Columns[k].ColumnName.Contains("D"))
                                sbb.Append(dtExport.Columns[k].ColumnName.Replace("D", ""));

                            else
                                sbb.Append(dtExport.Columns[k].ColumnName);
                        }
                        else
                            sbb.Append(dtExport.Columns[k].ColumnName);

                        sbb.Append("</b></td>");
                    }
                    sbb.Append("</tr>");
                   
                    sbb.Append("<tr>");
                    for (int j = 20; j < 35; j++)
                    {
                        #region
                        if (dtExport.Rows[i][j].ToString().Trim() == "NA")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; background-color: yellow; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "W")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; background-color: gray;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "WP")
                        {
                            sbb.Append("<td align=center  valign=middle style=' white-space:nowrap ; color: red;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "CS")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; color: orange; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "EL")
                        {
                            sbb.Append("<td align=center style=' white-space:nowrap ; color: blue; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim().ToLower() == "p"
                            || dtExport.Rows[i][j].ToString().Trim().ToLower() == "sp"
                             || dtExport.Rows[i][j].ToString().Trim().ToLower() == "cla"
                              || dtExport.Rows[i][j].ToString().Trim().ToLower() == "ts"
                            )
                        {
                            sbb.Append("<td align=center  valign=middle style=' white-space:nowrap ; vertical-align:middle;'>");

                            var dayNumber = int.Parse(dtExport.Columns[j].ColumnName.Remove(0, 1));

                            if (intTimeOutTime.Any(y => y.EmployeeId == employeeID
                                 && y.InTimeOutTimes.Any(y1 => y1.Day == dayNumber)
                            ))
                            {
                                #region  ======== Append In-Time /Out-Time ===============

                                var attendanceOfSelectedDay =
                                    intTimeOutTime.FirstOrDefault(m => m.EmployeeId == employeeID).InTimeOutTimes.
                                    Where(y => y.Day == dayNumber).FirstOrDefault();

                                var inTime = Convert.ToDateTime(attendanceOfSelectedDay.InTime).ToString("HH:mm");

                                var outTime = attendanceOfSelectedDay.OutTime != string.Empty ? Convert.ToDateTime(attendanceOfSelectedDay.OutTime).ToString("HH:mm") : " ";


                                sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                                sbb.Append("<div class='p-2 text-center' style='font-size:1em;'>");
                                sbb.Append($"In-{inTime}");
                                sbb.Append("<br/>");
                                //sbb.Append($"-");
                                //sbb.Append("<br/>");
                                sbb.Append($"Out-{outTime}");
                                sbb.Append("</div>");

                                #endregion
                            }
                            else
                                sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else
                        {
                            sbb.Append("<td align=center style='white-space:nowrap ;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        sbb.Append("</td>");
                        #endregion
                    }
                    sbb.Append("</tr>");
                    sbb.Append("</Table>");
                    
                    ///------------ Blank row for sepration 
                    sbb.Append("<Table colspan=" + 16 +">");
                    sbb.Append("<tr>");
                    sbb.Append("<td>");
                    sbb.Append("");
                    sbb.Append("</b></td>");
                    sbb.Append("</tr >");
                    sbb.Append("</Table>");
                }               
                sbb.Append("</html>");
                this.Response.Write(sbb.ToString());
                this.Response.End();
            }
        }

        public void ExportExcel()
        {
            if (Session["dtdata"] != null)
            {
                DataTable dtExport = (DataTable)Session["dtdata"];
                string name = "EmployeeAttendanceDetails.xls";
                var Dates = dtExport.Rows[0]["Date"].ToString();
                var Branch = dtExport.Rows[0]["BranchName"].ToString();

                List<Model.EmployeeAttendanceDetails> intTimeOutTime = new List<Model.EmployeeAttendanceDetails>();

                if (Session["InTimeOutTime"] != null)
                    intTimeOutTime = (List<Model.EmployeeAttendanceDetails>)Session["InTimeOutTime"];

                this.Response.Clear();
                this.Response.Charset = "";
                this.Response.ContentType = "application/vnd.ms-excel";
                this.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + name + "\"");
                System.Text.StringBuilder sbb = new System.Text.StringBuilder();
                sbb.Append("<html>");
                sbb.Append("<style>");
                sbb.Append("#table1 tr td { height: 60 px !important;}");
                sbb.Append("</style>");
                //Header information...........................................
                sbb.Append("<Table border= 1>");
                sbb.Append("<tr>");
                sbb.Append("<td align=center valign=middle rowspan=2 colspan=" + 35 + " style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;vertical-align:middle;'><b>");
                sbb.Append($"Employee Attendance Details : {Dates} - Branch : {Branch}");
                sbb.Append("</b></td>");
                sbb.Append("</tr >");
                sbb.Append("</Table>");

                sbb.Append("<Table border= 1 id='table1'>");
                sbb.Append("<tr>");
                sbb.Append("<td>");
                sbb.Append("");
                sbb.Append("</b></td>");
                sbb.Append("</tr >");
                sbb.Append("</Table>");

                sbb.Append("<Table border= 1>");
                sbb.Append("<tr>");
                for (int k = 0; k < 35; k++)
                {
                    sbb.Append("<td align=center valign=middle  style=' white-space:nowrap ; ;background-color: #17A2B8; color: #ffffff;'><b>");

                    if (k > 3)
                    {
                        if (dtExport.Columns[k].ColumnName.Contains("D"))
                            sbb.Append(dtExport.Columns[k].ColumnName.Replace("D", ""));

                        else
                            sbb.Append(dtExport.Columns[k].ColumnName);
                    }
                    else
                        sbb.Append(dtExport.Columns[k].ColumnName);

                    sbb.Append("</b></td>");
                }
                //   --- rows ---   //
                for (int i = 0; i < dtExport.Rows.Count; i++)
                {
                    var employeeID = Convert.ToInt32(dtExport.Rows[i]["EmployeeId"].ToString());

                    sbb.Append("<tr>");
                    for (int j = 0; j < 35; j++)
                    {
                        if (dtExport.Rows[i][j].ToString().Trim() == "NA")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; background-color: yellow; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "W")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; background-color: gray;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "WP")
                        {
                            sbb.Append("<td align=center  valign=middle style=' white-space:nowrap ; color: red;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "CS")
                        {
                            sbb.Append("<td align=center valign=middle style=' white-space:nowrap ; color: orange; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim() == "EL")
                        {
                            sbb.Append("<td align=center style=' white-space:nowrap ; color: blue; vertical-align:middle;'>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else if (dtExport.Rows[i][j].ToString().Trim().ToLower() == "p"
                            || dtExport.Rows[i][j].ToString().Trim().ToLower() == "sp"
                             || dtExport.Rows[i][j].ToString().Trim().ToLower() == "cla"
                              || dtExport.Rows[i][j].ToString().Trim().ToLower() == "ts"
                            )
                        {
                            sbb.Append("<td align=center  valign=middle style=' white-space:nowrap ; vertical-align:middle;'>");

                            var dayNumber = int.Parse(dtExport.Columns[j].ColumnName.Remove(0, 1));

                            if (intTimeOutTime.Any(y => y.EmployeeId == employeeID
                                 && y.InTimeOutTimes.Any(y1 => y1.Day == dayNumber)
                            ))
                            {
                                #region  ======== Append In-Time /Out-Time ===============

                                var attendanceOfSelectedDay =
                                    intTimeOutTime.FirstOrDefault(m => m.EmployeeId == employeeID).InTimeOutTimes.
                                    Where(y => y.Day == dayNumber).FirstOrDefault();

                                var inTime = Convert.ToDateTime(attendanceOfSelectedDay.InTime).ToString("HH:mm");

                                var outTime = attendanceOfSelectedDay.OutTime != string.Empty ? Convert.ToDateTime(attendanceOfSelectedDay.OutTime).ToString("HH:mm") : " ";


                                sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                                sbb.Append("<div class='p-2 text-center' style='font-size:1em;'>");
                                sbb.Append($"In-{inTime}");
                                sbb.Append("<br/>");
                                //sbb.Append($"-");
                                //sbb.Append("<br/>");
                                sbb.Append($"Out-{outTime}");
                                sbb.Append("</div>");

                                #endregion
                            }
                            else
                                sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        else
                        {
                            sbb.Append("<td align=center style='white-space:nowrap ;vertical-align:middle; '>");
                            sbb.Append(Convert.ToString(dtExport.Rows[i][j]));
                        }
                        sbb.Append("</td>");
                    }
                }
                sbb.Append("</tr >");
                sbb.Append("</Table>");
                sbb.Append("</html>");
                this.Response.Write(sbb.ToString());
                this.Response.End();
            }
        }


        #region GetAttendance
        [HttpGet]
        public ActionResult Create()
        {
            log.Info("EmployeeAttendancedetailsController/Create");
            try
            {
                Model.EmployeeAttendanceDetails employeeAttendanceVM = new Model.EmployeeAttendanceDetails();
                return View(employeeAttendanceVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.EmployeeAttendanceDetails createAttendance)
        {
            log.Info("CalculateBonusController/Create");
            try
            {
                BindDropdowns();

                if (createAttendance.fromdate == null)
                {
                    ModelState.AddModelError("fromdate", "Please select from date");
                    return View(createAttendance);
                }
                if (createAttendance.todate == null)
                {
                    ModelState.AddModelError("todate", "Please select to date");
                    return View(createAttendance);
                }

                if (ModelState.IsValid)
                {

                    int res = employeeattendancedetailsservice.getAttendancemanually((DateTime)createAttendance.fromdate, (DateTime)createAttendance.todate);

                    TempData["Message"] = "Successfully Created";
                    return RedirectToAction("Index");
                }
                return PartialView("Create", createAttendance);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        #endregion

    }
}