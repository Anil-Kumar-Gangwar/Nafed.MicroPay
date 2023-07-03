using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.HelpDesk;
namespace MicroPay.Web.Controllers.HelpDesk
{
    public class GroupController : BaseController
    {
        private readonly ISupportGroupService supportGroupServ;
        public GroupController(ISupportGroupService supportGroupServ)
        {
            this.supportGroupServ = supportGroupServ;

        }
        // GET: Group
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _GetSupportGroupGridView()
        {
            log.Info("SupportGroupController/_GetSupportGroupGridView");
            try
            {
                List<SupportGroup> objSupportGroupList = new List<SupportGroup>();
                objSupportGroupList = supportGroupServ.GetSupportGroupList();
                return PartialView("_SupportGroupList", objSupportGroupList);
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
            log.Info("SupportGroupController/Create");
            try
            {
                SupportGroup objSupportGroup = new SupportGroup();
                return View(objSupportGroup);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SupportGroup createSupportGroup)
        {
            log.Info("SupportGroupController/Create");
            try
            {

                if (ModelState.IsValid)
                {
                    createSupportGroup.name = createSupportGroup.name.Trim();                
                    if (supportGroupServ.SupportGroupExists(createSupportGroup.name, null))
                    {
                        ModelState.AddModelError("SupportGroupAlreadyExist", "Group Already Exist.");
                        return View(createSupportGroup);
                    }
                    createSupportGroup.CreatedOn = DateTime.Now;
                    createSupportGroup.description = createSupportGroup.description.Trim();
                    createSupportGroup.CreatedBy = userDetail.UserID;

                    int acadmicProfessionalID = supportGroupServ.InsertSupportGroup(createSupportGroup);
                    TempData["Message"] = "Group created successfully.";
                    return RedirectToAction("Index");

                }
                else
                {
                    return View(createSupportGroup);
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
        public ActionResult Edit(int supportGroupID)
        {
            log.Info("SupportGroupController/Edit");
            try
            {
                SupportGroup objSupportGroup = new SupportGroup();
                objSupportGroup = supportGroupServ.GetSupportGroupByID(supportGroupID);
                return View(objSupportGroup);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SupportGroup updateSupportGroup)
        {
            log.Info("SupportGroupController/Edit");
            try
            {

                if (!string.IsNullOrEmpty(updateSupportGroup.name))
                {
                    updateSupportGroup.name = updateSupportGroup.name.Trim();
                    if (supportGroupServ.SupportGroupExists(updateSupportGroup.name, updateSupportGroup.description))
                        ModelState.AddModelError("SupportGroupAlreadyExist", "Group Already Exist.");
                }
                if (ModelState.IsValid)
                {
                    supportGroupServ.UpdateSupportGroup(updateSupportGroup);
                    TempData["Message"] = "Group updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(updateSupportGroup);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(updateSupportGroup);
        }

        public ActionResult Delete(int supportGroupID)
        {
            log.Info("SupportGroupController/Delete");
            try
            {
                supportGroupServ.Delete(supportGroupID);
                TempData["Message"] = "Group deleted successfully.";
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