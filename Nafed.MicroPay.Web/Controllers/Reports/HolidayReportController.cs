using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPay.Web.Models;
using AutoMapper;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Controllers.Reports
{
    public class HolidayReportController : BaseController
    {
        // GET: HolidayReport
        public ActionResult Index()
        {
            log.Info($"HolidayReportController/Index");
            try
            {
                return View(userAccessRight);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GetHolidayReportFilters()
        {
            log.Info($"HolidayReportController/GetHolidayReportFilters");
            try
            {
                HolidayViewModel holidayVM = new HolidayViewModel();
                var yrs = Enumerable.Range(2012,( DateTime.Now.Year + 1)- 2012).ToList();

                Mapper.Initialize(cfg => {

                    cfg.CreateMap<int, SelectListModel>().
                    ForMember(d => d.id, o => o.MapFrom(s => s))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.ToString()));
                });
                holidayVM.CYearList = Mapper.Map<List<SelectListModel>>(yrs);
                return PartialView("_ReportFilter", holidayVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult ShowHolidayReport(HolidayViewModel holiVM)
        {
            log.Info($"HolidayReportController/ShowHolidayReport");

            var yrs = Enumerable.Range(2012, (DateTime.Now.Year + 1) - 2012).ToList();
            Mapper.Initialize(cfg => {

                cfg.CreateMap<int, SelectListModel>().
                ForMember(d => d.id, o => o.MapFrom(s => s))
                .ForMember(d => d.value, o => o.MapFrom(s => s.ToString()));
            });
            holiVM.CYearList = Mapper.Map<List<SelectListModel>>(yrs);

            if (!holiVM.CYear.HasValue)
            {
                ModelState.AddModelError("CYearRequired", "Please Select Calendar Year.");
                return PartialView("_ReportFilter", holiVM);
            }

            if (ModelState.IsValid)
            {
                BaseReportModel reportModel = new BaseReportModel();
                List<ReportParameter> parameterList = new List<ReportParameter>();
                parameterList.Add(new ReportParameter() { name = "year", value = holiVM.CYear });
                reportModel.reportParameters = parameterList;
                reportModel.rptName = "HolidayReport.rpt";
                Session["ReportModel"] = reportModel;
                return Json(new { IsValidFilter = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
              
                return PartialView("_ReportFilter", holiVM);
            }
        }
    }
}