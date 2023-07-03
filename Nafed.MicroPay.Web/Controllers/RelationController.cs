using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using MicroPay.Web.Models;
  


namespace MicroPay.Web.Controllers
{
    public class RelationController : BaseController
    {

        private readonly IRelationService relationService;
        public RelationController(IRelationService relationService)
        {
            this.relationService = relationService;
        }

        public ActionResult Index()
        {
            log.Info($"RelationController/Index");
            return View(userAccessRight);
        }

        public ActionResult RelationGridView(FormCollection formCollection)
        {
            log.Info($"RelationController/RelationGridView");
            try
            {
                RelationViewModel RelVM = new RelationViewModel();
                List<Model.Relation> objRelationList = new List<Model.Relation>();
                RelVM.listRelation = relationService.GetRelationList();
                RelVM.userRights = userAccessRight;
                return PartialView("_RelationGridView", RelVM);
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
            log.Info("RelationController/Create");
            try
            {
                Model.Relation objRelation = new Model.Relation();
                return View(objRelation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Create(Model.Relation createRelation)
        {
            log.Info("RelationController/Create");
            try
            {
                if (ModelState.IsValid)
                {
                    createRelation.RelationName = createRelation.RelationName.Trim();
                    createRelation.RelationCode = createRelation.RelationCode.Trim();

                    createRelation.CreatedBy = userDetail.UserID;
                    createRelation.CreatedOn = System.DateTime.Now;

                    if (relationService.RelationNameExists(createRelation.RelationID, createRelation.RelationName))
                    {
                        ModelState.AddModelError("RelationNameAlreadyExist", "Relation Name Already Exist");
                        return View(createRelation);
                    }
                    else if (relationService.RelationCodeExists(createRelation.RelationID, createRelation.RelationCode))
                    {
                        ModelState.AddModelError("RelationCodeAlreadyExist", "Relation Code Already Exist");
                        return View(createRelation);
                    }
                    else
                    {
                        relationService.InsertRelation(createRelation);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createRelation);
        }

        [HttpGet]
        public ActionResult Edit(int relationID)
        {
            log.Info("RelationItemController/Edit");
            try
            {
                Model.Relation objRelation = new Model.Relation();
                objRelation = relationService.GetRelationByID(relationID);
                return View(objRelation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Edit(Model.Relation editRelation)
        {
            log.Info("RelationItemController/Edit");
            try
            {
                if (ModelState.IsValid)
                {
                    editRelation.RelationName = editRelation.RelationName.Trim();
                    editRelation.RelationCode = editRelation.RelationCode.Trim();

                    editRelation.UpdatedBy = userDetail.UserID;
                    editRelation.UpdatedOn = System.DateTime.Now;

                    if (relationService.RelationNameExists(editRelation.RelationID, editRelation.RelationName))
                    {
                        ModelState.AddModelError("RelationNameAlreadyExist", "Relation Name Already Exist");
                        return View(editRelation);
                    }
                    else if (relationService.RelationCodeExists(editRelation.RelationID, editRelation.RelationCode))
                    {
                        ModelState.AddModelError("RelationCodeAlreadyExist", "Relation Code Already Exist");
                        return View(editRelation);
                    }
                    else
                    {
                        relationService.UpdateRelation(editRelation);
                        TempData["Message"] = "Succesfully Updated";
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editRelation);

        }


        public ActionResult Delete(int relationID)
        {
            log.Info("Delete");
            try
            {
                relationService.Delete(relationID);
                TempData["Message"] = "Succesfully Deleted";
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