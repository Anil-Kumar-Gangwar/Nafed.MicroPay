using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using System;
using MicroPay.Web.Models;
using Nafed.MicroPay.Services.Recruitment;

namespace MicroPay.Web.Controllers.Recruitment
{
    [EncryptedActionParameter]
    public class JobListController : Controller
    {
        private readonly IRecruitmentService recruitmentService;
        public JobListController(IRecruitmentService recruitmentService)
        {
            this.recruitmentService = recruitmentService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _GetJobListGridView()
        {
            //log.Info($"JobListController/_GetJobListGridView");
            try
            {
                RequirementViewModel requirVM = new RequirementViewModel();
                requirVM.RequirementList = recruitmentService.GetVacanciesList();
                //requirVM.userRights = userAccessRight;
                return PartialView("_GetJobListGridView", requirVM);
            }
            catch (Exception ex)
            {
                //log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult ViewDetails(int requirementId)
        {
            try
            {
                Model.Requirement objRequirement = new Model.Requirement();
                objRequirement = recruitmentService.GetRequirementByID(requirementId);
                return View(objRequirement);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}