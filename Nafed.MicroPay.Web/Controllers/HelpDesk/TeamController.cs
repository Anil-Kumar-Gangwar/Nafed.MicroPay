using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.HelpDesk;
namespace MicroPay.Web.Controllers.HelpDesk
{
    public class TeamController : BaseController
    {
        private readonly ISupportTeamService SupportTeamServ;
        public TeamController(ISupportTeamService SupportTeamServ)
        {
            this.SupportTeamServ = SupportTeamServ;

        }
        // GET: Team
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _GetSupportTeamGridView()
        {
            log.Info("TeamController/_GetSupportTeamGridView");
            try
            {
                List<SupportTeam> objSupportTeamList = new List<SupportTeam>();
                objSupportTeamList = SupportTeamServ.GetSupportTeamList();
                return PartialView("_SupportTeamList", objSupportTeamList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("TeamController/Create");
            try
            {
                SupportTeam objSupportTeam = new SupportTeam();
                return View(objSupportTeam);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SupportTeam createSupportTeam)
        {
            log.Info("TeamController/Create");
            try
            {

                if (ModelState.IsValid)
                {
                    createSupportTeam.name = createSupportTeam.name.Trim();
                    createSupportTeam.description = createSupportTeam.description.Trim();
                    if (SupportTeamServ.SupportTeamExists(createSupportTeam.name, createSupportTeam.description))
                    {
                        ModelState.AddModelError("SupportTeamAlreadyExist", "Team Already Exist.");
                        return View(createSupportTeam);
                    }
                    createSupportTeam.CreatedOn = DateTime.Now;
                    createSupportTeam.description = createSupportTeam.description.Trim();
                    createSupportTeam.CreatedBy = userDetail.UserID;

                    int acadmicProfessionalID = SupportTeamServ.InsertSupportTeam(createSupportTeam);
                    TempData["Message"] = "Team created successfully.";
                    return RedirectToAction("Index");

                }
                else
                {
                    return View(createSupportTeam);
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }

        }

        [HttpGet]
        public ActionResult Edit(int SupportTeamID)
        {
            log.Info("TeamController/Edit");
            try
            {
                SupportTeam objSupportTeam = new SupportTeam();
                objSupportTeam = SupportTeamServ.GetSupportTeamByID(SupportTeamID);
                return View(objSupportTeam);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SupportTeam updateSupportTeam)
        {
            log.Info("TeamController/Edit");
            try
            {

                if (!string.IsNullOrEmpty(updateSupportTeam.name))
                {
                    updateSupportTeam.name = updateSupportTeam.name.Trim();
                    updateSupportTeam.description = updateSupportTeam.description.Trim();
                    if (SupportTeamServ.SupportTeamExists(updateSupportTeam.name, updateSupportTeam.description))
                        ModelState.AddModelError("SupportTeamAlreadyExist", "Team Already Exist.");
                }
                if (ModelState.IsValid)
                {
                    SupportTeamServ.UpdateSupportTeam(updateSupportTeam);
                    TempData["Message"] = "Team updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(updateSupportTeam);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateSupportTeam);
        }

        public ActionResult Delete(int SupportTeamID)
        {
            log.Info("TeamController/Delete");
            try
            {
                SupportTeamServ.Delete(SupportTeamID);
                TempData["Message"] = "Team deleted successfully.";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }
    }
}