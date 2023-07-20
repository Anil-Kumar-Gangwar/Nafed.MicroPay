using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.ExtensionMethods;
using static Nafed.MicroPay.Common.FileHelper;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Children_Education
{
    public class ChildrenEducationAllowanceController : BaseController
    {
        private readonly IDropdownBindService _ddlServices;
        private readonly IChildrenEducationService _childrenService;
        public ChildrenEducationAllowanceController(IDropdownBindService dropdownService, IChildrenEducationService childrenService)
        {
            _ddlServices = dropdownService;
            _childrenService = childrenService;
        }

        // GET: ChildrenEducationAllowance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Filter()
        {
            log.Info("ChildrenEducationAllowanceController/_Filter");
            try
            {
                Model.CommonFilter cFilter = new Model.CommonFilter();
                var reportingYrs = _ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                ViewBag.reportingYrs = reportingYrs;
                cFilter.ReportingYear = reportingYrs.First().value; // set default as current year
                return PartialView("_Filters", cFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult _List()
        {
            log.Info($"ChildrenEducationAllowanceController/_List");
            try
            {
                ChildrenEducationViewModel empChildrenEducationVM = new ChildrenEducationViewModel();
                var reportingYrs = _ddlServices.GetFinanceYear().OrderByDescending(x => x.value).ToList();
                var childAllow = _childrenService.GetEmployeeChildrenEducationYearWise(reportingYrs.First().value);
                empChildrenEducationVM.childrenEducationList = childAllow;
                return PartialView("_List", empChildrenEducationVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult _GetList(Model.CommonFilter filter)
        {
            log.Info($"ChildrenEducationAllowanceController/_List");
            try
            {
                ChildrenEducationViewModel empChildrenEducationVM = new ChildrenEducationViewModel();
                var childAllow = _childrenService.GetEmployeeChildrenEducationYearWise(filter.ReportingYear);
                empChildrenEducationVM.childrenEducationList = childAllow;
                return PartialView("_List", empChildrenEducationVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public ActionResult View(int empID, int childrenEduHdrId, bool updateOrNot = true)
        {
            log.Info($"ChildrenEducationAllowance/View/{ empID}/{childrenEduHdrId}");
            try
            {
                Model.ChildrenEducationHdr childrenEduData = new Model.ChildrenEducationHdr();
                var reportingYr = DateTime.Now.GetFinancialYr();
                childrenEduData = _childrenService.GetChildrenEducation(empID, childrenEduHdrId);
                childrenEduData.ChildrenEducationDetailsList = _childrenService.GetChildrenEducationDetails(empID, childrenEduHdrId);
                childrenEduData.DependentList = _childrenService.GetDependentList(empID);
                childrenEduData.ChildrenEducationDocumentsList = _childrenService.GetChildrenEducationDocumentsList(empID, childrenEduHdrId);

                if (childrenEduData.ChildrenEducationDetailsList.Count > 0)
                {
                    var sno = 1;
                    childrenEduData.ChildrenEducationDetailsList.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }

                childrenEduData.IsDependentMatch = true;
                return View("ChildrenEducationContainer", childrenEduData);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetChildrenEducationDocuments(int employeeId, int childrenEduHdrID)
        {
            log.Info($"ChildrenEducationAllowance/GetChildrenEducationDocuments");
            try
            {
                ChildrenEducationViewModel documentViewModel = new ChildrenEducationViewModel();
                documentViewModel.childrenEducationDocuments = _childrenService.GetChildrenEducationDocumentsList(employeeId, childrenEduHdrID);
                return PartialView("_ReceiptDocumentPopUp", documentViewModel);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}