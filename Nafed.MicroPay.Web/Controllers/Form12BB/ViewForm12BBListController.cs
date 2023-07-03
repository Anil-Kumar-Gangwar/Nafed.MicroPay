using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Form12BB
{
    public class ViewForm12BBListController : BaseController
    {
        private readonly IForm12BBService form12BBService;

        public ViewForm12BBListController(IForm12BBService form12BBService)
        {
            this.form12BBService = form12BBService;
        }
        // GET: ViewForm12BBList
        public ActionResult Index()
        {
            log.Info($"ViewForm12BBListController/Index");
            
                return View();
        }

        public ActionResult __Form12BBFilters()
        {
            log.Info($"ViewForm12BBListController/__Form12BBFilters");
            try
            {
                var loggedInUserName = userDetail.UserName;

                if (!loggedInUserName.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    return Content(@"<div class='col-lg-12 col-md-12 col-sm-12 col-xs-12 font-weight-bold text-center alert alert-warning'>
                           You are not authorized to access this page.
                      </div>");
                else
                {
                    Form12BBFilterVM frm12BBFilter = new Form12BBFilterVM();
                    var fYears = ExtensionMethods.GetFinancialYrList(2017, DateTime.Now.Year)
                    .Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
                    frm12BBFilter.fYears = fYears;
                    return PartialView("_Form12BBFilter", frm12BBFilter);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult __Form12BBFilters(Form12BBFilterVM frmFilters)
        {
            log.Info($"ViewForm12BBListController/__Form12BBFilters");
            try
            {
                List<Form12BBInfo> frm12BBs = new List<Form12BBInfo>();

                DataTable dt = form12BBService.GetForm12BBDetails(frmFilters.selectedFYear);
                foreach (DataRow item in dt.Rows)
                {
                    frm12BBs.Add(new Form12BBInfo
                    {
                        EmpFormID = item.Field<int>("EmpFormID"),
                        FYear = item.Field<string>("FYear"),
                        EmployeeID = item.Field<int>("EmployeeID"),
                        EmployeeName = item.Field<string>("EmployeeName"),
                        FormState = item.Field<byte>("FormState"),
                        EmployeeCode = item.Field<string>("EmployeeCode"),
                        Designation = item.Field<string>("Designation"),
                        PANNo = item.Field<string>("PANNo"),
                        SubmittedOn = item.Field<DateTime?>("SubmittedOn"),
                        UploadDocument = item.Field<int>("UploadDocument")
                    });
                }

                return PartialView("_Form12BBGridView", frm12BBs);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //[HttpPost]
        //public ActionResult __Form12BBFilters(Form12BBFilterVM frmFilters)
        //{
        //    log.Info($"ViewForm12BBListController/__Form12BBFilters");
        //    try
        //    {
        //        var frm12BBs = form12BBService.GetForm12BBList(frmFilters.selectedFYear);
        //        return PartialView("_Form12BBGridView", frm12BBs);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}
    }
}