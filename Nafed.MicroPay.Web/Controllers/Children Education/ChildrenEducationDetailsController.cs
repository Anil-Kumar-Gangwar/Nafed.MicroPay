using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Children_Education
{
    public class ChildrenEducationDetailsController : BaseController
    {
        private readonly IChildrenEducationService childrenEduService;
        private readonly IDropdownBindService ddlServices;

        public ChildrenEducationDetailsController(IChildrenEducationService childrenEduService, IDropdownBindService ddlServices)
        {
            this.childrenEduService = childrenEduService;
            this.ddlServices = ddlServices;
        }

        // GET: ChildrenEducationDetails
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _AdminChildrenEducationFilters()
        {
            log.Info($"ChildrenEducationDetailsController/_AdminChildrenEducationFilters");
            try
            {
                AppraisalFormApprovalFilter filters = new AppraisalFormApprovalFilter();
                filters.employees = ddlServices.GetAllEmployee();
                filters.reportingYrs = new List<SelectListModel>()
                {
                    new SelectListModel { id = 1, value = "2019-2020" },
                    new SelectListModel { id = 2, value = "2020-2021" },
                    new SelectListModel { id = 2, value = "2021-2022" },
                    new SelectListModel { id = 2, value = "2023-2024" }
                };
                return PartialView("_AdminChildrenEducationFilters", filters);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostChildrenEducationFilters(AppraisalFormApprovalFilter filters)
        {
            log.Info($"ChildrenEducationDetailsController/_PostChildrenEducationFilters");
            try
            {
                var conveyanceForms = childrenEduService.GetChildrenEducationForAdmin(filters);
                return PartialView("_ChildrenEducationForAdmin", conveyanceForms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}