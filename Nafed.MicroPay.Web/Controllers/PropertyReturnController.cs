using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using MicroPay.Web.Models;
using System.Linq;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace MicroPay.Web.Controllers
{
    public class PropertyreturnController : BaseController
    {
        // GET: Propertyreturn
        private readonly IPRService PRService;
        private readonly IDropdownBindService ddlService;

        public PropertyreturnController(IPRService PRService, IDropdownBindService ddlService)
        {
            this.PRService = PRService;
            this.ddlService = ddlService;
        }
        public ActionResult Index(int? EmployeeID, int? PRID, int? Year, int? StatusID = null)
        {
            log.Info($"PropertyreturnController/Index");
            PropertyReturnViewModel PRVM = new PropertyReturnViewModel();
            PRVM.userRights = userAccessRight;
            BindDropdowns();
            PRVM.StatusID = StatusID == null ? 0 : (int)StatusID;
            var CYear = DateTime.Now.Year;
            var cList = new List<Model.SelectListModel>();
            cList.Add(new Model.SelectListModel { id = CYear - 1, value = (CYear - 1).ToString() });
            cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
            cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
            PRVM.CYearList = cList;

            List<Model.SelectListModel> PropertyType = new List<Model.SelectListModel>();
            PropertyType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            PropertyType.Add(new Model.SelectListModel() { value = "Housing", id = 1 });
            PropertyType.Add(new Model.SelectListModel() { value = "Building", id = 2 });
            PropertyType.Add(new Model.SelectListModel() { value = "Land", id = 3 });
            PRVM.PropertyTypeDetails = PropertyType;

            List<Model.SelectListModel> AcquiredType = new List<Model.SelectListModel>();
            AcquiredType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Purchase", id = 1 });
            AcquiredType.Add(new Model.SelectListModel() { value = "lease ", id = 2 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Mortgage ", id = 3 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Inheritance ", id = 4 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Gift or Otherwise   ", id = 5 });
            PRVM.AcquiredTypeDetails = AcquiredType;


            var ddlRelation = ddlService.ddlRelationList();
            Model.SelectListModel selectRelation = new Model.SelectListModel();
            selectRelation.id = 0;
            selectRelation.value = "Select";
            ddlRelation.Insert(0, selectRelation);
            PRVM.RelationDetails = ddlRelation;

            var ddlEmployeeList = PRService.GetAllEmployee();
            ddlEmployeeList.OrderBy(x => x.value);
            Model.SelectListModel employee = new Model.SelectListModel();
            employee.id = 0;
            employee.value = "Select";
            ddlEmployeeList.Insert(0, employee);
            PRVM.EmployeeList = ddlEmployeeList;


            bool flag = false;
            Model.PR uDetail = new Model.PR();
            flag = PRService.getemployeeallDetails(Convert.ToInt32(userDetail.DesignationID), Convert.ToInt32(userDetail.EmployeeID), out uDetail, Year);
            if (flag)
            {
                ViewBag.DesignationName = uDetail.DesignationName;
                ViewBag.BasicPay = uDetail.E_Basic;
            }

            if (EmployeeID.HasValue)
            {
                ViewBag.EmployeeID = EmployeeID.Value;
            }
            ViewBag.Employeename = userDetail.FullName;
            ViewBag.BranchName = userDetail.Location;

            if (Year.HasValue)
            {
                ViewBag.Year = Year.Value;
            }
            if (PRID.HasValue)
            {
                ViewBag.PRID = PRID.Value;
            }
            if (uDetail.UpdatedOn.HasValue)
            {
                ViewBag.submitDate = uDetail.UpdatedOn.Value.ToString("dd/MM/yyyy");
            }
            return View(PRVM);
        }
        [HttpGet]
        public ActionResult _PropertyReturnGridView(int? EmployeeID, int? PRID, int? Year)
        {
            log.Info($"PropertyreturnController/_PropertyReturnGridView");
            try
            {
                PropertyReturnViewModel PRVM = new PropertyReturnViewModel();
                PRVM.userRights = userAccessRight;
                if (PRID == null)
                {
                    PRID = Convert.ToInt32(Session["PRID"]);
                }

                PRVM.PRList = PRService.GetPRGVList(EmployeeID == null ? 0 : (int)EmployeeID, (int)PRID, Year == null ? 0 : (int)Year);
                if (EmployeeID.HasValue)
                {
                    ViewBag.EmployeeID = EmployeeID.Value;
                    Session["EmployeeID"] = EmployeeID.Value;
                }
                if (Year.HasValue)
                {
                    ViewBag.Year = Year.Value;
                    Session["Year"] = Year.Value;
                }
                ViewBag.Employeename = userDetail.FullName;
                if (PRID.HasValue)
                {
                    ViewBag.PRID = PRID.Value;
                    Session["PRID"] = PRID.Value;
                }

                bool flag = false;
                Model.PR uDetail = new Model.PR();
                flag = PRService.getemployeeallDetails(Convert.ToInt32(userDetail.DesignationID), Convert.ToInt32(userDetail.EmployeeID), out uDetail,Year);
                if (flag)
                {
                    ViewBag.DesignationName = uDetail.DesignationName;
                    ViewBag.BasicPay = uDetail.E_Basic;
                }

                return PartialView("_PropertyReturnGridView", PRVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PropertyReturnGridView(PropertyReturnViewModel PrRVM)
        {
            log.Info($"PropertyreturnController/_PropertyReturnGridView");
            try
            {
                PropertyReturnViewModel PRVM = new PropertyReturnViewModel();
                PRVM.userRights = userAccessRight;
                var CYear = DateTime.Now.Year;
                var cList = new List<Model.SelectListModel>();
                cList.Add(new Model.SelectListModel { id = CYear - 1, value = (CYear - 1).ToString() });
                cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
                cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
                PRVM.CYearList = cList;

                if (PrRVM.EmployeeId == null)
                {
                    PRVM.PRList = PRService.GetPRGVList((int)PrRVM.EmployeeId, 0, 0);
                }
                else
                {
                    PRVM.PRList = PRService.GetPRGVList((int)PrRVM.EmployeeId, 0, 0);
                }
                return PartialView("_PropertyReturnGridView", PRVM);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //[HttpGet]
        //public ActionResult Create()
        //{
        //    log.Info("PropertyreturnController/Create");
        //    try
        //    {
        //        BindDropdowns();
        //        var CYear = DateTime.Now.Year;
        //        var cList = new List<Model.SelectListModel>();
        //        cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
        //        cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
        //        Model.PR objLTCPR = new Model.PR();
        //        objLTCPR.CYearList = cList;
        //        return View(objLTCPR);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}

        //[HttpPost]
        //public ActionResult Create(Model.PR createPR)
        //{
        //    log.Info("PropertyreturnController/Create");
        //    try
        //    {


        //        BindDropdowns();
        //        var CYear = DateTime.Now.Year;
        //        var cList = new List<Model.SelectListModel>();
        //        cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
        //        cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
        //        createPR.CYearList = cList;
        //        if (ModelState.IsValid)
        //        {
        //            createPR.CreatedOn = DateTime.Now;
        //            createPR.CreatedBy = userDetail.UserID;
        //            int PRID = PRService.InsertPR(createPR);
        //            TempData["Message"] = "Successfully Created";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        TempData["Error"] = "Error-Some error occurs please contact administrator";
        //        throw ex;
        //    }
        //    return View(createPR);
        //}

        //[HttpGet]
        //public ActionResult Edit(int PRID)
        //{
        //    log.Info("PropertyreturnController/Edit");
        //    try
        //    {
        //        BindDropdowns();
        //        Model.PR objPR = new Model.PR();
        //        objPR = PRService.GetPRList(PRID);

        //        var CYear = DateTime.Now.Year;
        //        var cList = new List<Model.SelectListModel>();
        //        cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
        //        cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
        //        objPR.CYearList = cList;

        //        return View(objPR);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}

        //[HttpPost]
        //public ActionResult Edit(Model.PR editPR)
        //{
        //    log.Info("PropertyreturnController/Edit");
        //    try
        //    {
        //        BindDropdowns();
        //        var CYear = DateTime.Now.Year;
        //        var cList = new List<Model.SelectListModel>();
        //        cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
        //        cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
        //        editPR.CYearList = cList;

        //        if (ModelState.IsValid)
        //        {
        //            editPR.UpdatedBy = userDetail.UserID;
        //            editPR.UpdatedOn = DateTime.Now;
        //            int PRID = PRService.UpdatePR(editPR);
        //            TempData["Message"] = "Succesfully Updated";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        TempData["Error"] = "Error-Some error occurs please contact administrator";
        //    }
        //    return View(editPR);
        //}

        //public ActionResult Delete(int PRID)
        //{
        //    log.Info("PropertyreturnController/Delete");
        //    try
        //    {
        //        PRService.DeletePR(PRID);
        //        TempData["Message"] = "Succesfully Deleted";
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        TempData["Error"] = "Error-Some error occurs please contact administrator";
        //    }
        //    return RedirectToAction("Index");
        //}

        public void BindDropdowns()
        {
            var ddlEmployeeList = PRService.GetAllEmployee();
            ddlEmployeeList.OrderBy(x => x.value);
            Model.SelectListModel employee = new Model.SelectListModel();
            employee.id = 0;
            employee.value = "Select";
            ddlEmployeeList.Insert(0, employee);
            ViewBag.EmployeeDetails = new SelectList(ddlEmployeeList, "id", "value");

        }

        public ActionResult _GetPropertyreturn(int Counter)
        {
            log.Info($"PropertyreturnController/_GetPropertyreturn");
            Model.PR PR = new Model.PR();
            PR.Counter = Counter;
            PR = PRService.GetPRList(Counter);
            var CYear = DateTime.Now.Year;
            var cList = new List<Model.SelectListModel>();
            cList.Add(new Model.SelectListModel { id = CYear - 1, value = (CYear - 1).ToString() });
            cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
            cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
            PR.CYearList = cList;

            var ddlEmployeeList = PRService.GetAllEmployee();
            List<dynamic> EmployeeList = new List<dynamic>();
            foreach (var item in ddlEmployeeList)
            {
                EmployeeList.Add(new
                {
                    value = item.value,
                    id = item.id
                });
            }
            PR.EmployeeList = EmployeeList;

            List<dynamic> PropertyType = new List<dynamic>();
            PropertyType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            PropertyType.Add(new Model.SelectListModel() { value = "Housing", id = 1 });
            PropertyType.Add(new Model.SelectListModel() { value = "Building", id = 2 });
            PropertyType.Add(new Model.SelectListModel() { value = "Land", id = 3 });
            PR.PropertyTypeDetails = PropertyType;

            List<Model.SelectListModel> AcquiredType = new List<Model.SelectListModel>();
            AcquiredType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Purchase", id = 1 });
            AcquiredType.Add(new Model.SelectListModel() { value = "lease ", id = 2 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Mortgage ", id = 3 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Inheritance ", id = 4 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Gift or Otherwise   ", id = 5 });
            PR.AcquiredTypeDetails = AcquiredType;


            var ddlRelation = ddlService.ddlRelationList();
            Model.SelectListModel selectRelation = new Model.SelectListModel();
            selectRelation.id = 0;
            selectRelation.value = "Select";
            ddlRelation.Insert(0, selectRelation);
            PR.RelationDetails = ddlRelation;

            return PartialView("_PropertyReturnPopup", PR);

        }

        [HttpPost]
        public ActionResult _PostPropertyReturn(Model.PR PR, FormCollection frm)
        {
            log.Info($"PropertyreturnController/_PostPropertyReturn");
            try
            {
                var CYear = DateTime.Now.Year;
                var cList = new List<Model.SelectListModel>();
                cList.Add(new Model.SelectListModel { id = CYear - 1, value = (CYear - 1).ToString() });
                cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
                cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
                PR.CYearList = cList;
                PR.SelectedYear = frm.Get("ddlYear");
                PR.Year = PR.Year == null ? Convert.ToInt32(PR.SelectedYear) : PR.Year;

                var ddlEmployeeList = PRService.GetAllEmployee();
                List<dynamic> EmployeeList = new List<dynamic>();
                PR.SelectedEmployeeName = frm.Get("ddlEmployee");
                foreach (var item in ddlEmployeeList)
                {
                    EmployeeList.Add(new
                    {
                        value = item.value,
                        id = item.id
                    });
                }
                PR.EmployeeList = EmployeeList;
                PR.EmployeeId = PR.EmployeeId == null ? Convert.ToInt32(PR.SelectedEmployeeName) : PR.EmployeeId;

                List<dynamic> PropertyType = new List<dynamic>();
                PropertyType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                PropertyType.Add(new Model.SelectListModel() { value = "Housing", id = 1 });
                PropertyType.Add(new Model.SelectListModel() { value = "Building", id = 2 });
                PropertyType.Add(new Model.SelectListModel() { value = "Land", id = 3 });
                PR.PropertyTypeDetails = PropertyType;

                PR.SelectedPropertyType = frm.Get("ddlPropertyType");
                if (PR.SelectedPropertyType != "" && PR.SelectedPropertyType != null)
                {
                    PR.PropertyType = PR.PropertyType == null ? Convert.ToInt32(PR.SelectedPropertyType) : PR.PropertyType;
                }
                List<Model.SelectListModel> AcquiredType = new List<Model.SelectListModel>();
                AcquiredType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
                AcquiredType.Add(new Model.SelectListModel() { value = "Purchase", id = 1 });
                AcquiredType.Add(new Model.SelectListModel() { value = "lease ", id = 2 });
                AcquiredType.Add(new Model.SelectListModel() { value = "Mortgage ", id = 3 });
                AcquiredType.Add(new Model.SelectListModel() { value = "Inheritance ", id = 4 });
                AcquiredType.Add(new Model.SelectListModel() { value = "Gift or Otherwise   ", id = 5 });
                PR.AcquiredTypeDetails = AcquiredType;

                PR.SelectedAcquiredType = frm.Get("ddlAcquiredType");
                if (PR.SelectedAcquiredType != "" && PR.SelectedAcquiredType != null)
                {
                    PR.AcquiredTypeID = PR.AcquiredTypeID == null ? Convert.ToInt32(PR.SelectedAcquiredType) : PR.AcquiredTypeID;
                }

                var ddlRelation = ddlService.ddlRelationList();
                Model.SelectListModel selectRelation = new Model.SelectListModel();
                selectRelation.id = 0;
                selectRelation.value = "Select";
                ddlRelation.Insert(0, selectRelation);
                PR.RelationDetails = ddlRelation;

                PR.SelectedRelationName = frm.Get("ddlRelation");
                if (PR.SelectedRelationName != "" && PR.SelectedRelationName != null)
                {
                    PR.RelationID = PR.RelationID == null ? Convert.ToInt32(PR.SelectedRelationName) : PR.RelationID;
                }
                if (PR.Year == null)
                {
                    ModelState.AddModelError("YearRequired", "Please select year.");
                    // return PartialView("_PropertyReturnPopup", PR);
                }
                if (PR.EmployeeId == 0)
                {
                    ModelState.AddModelError("EmployeeNameRequired", "Please enter employee name.");
                    // return PartialView("_PropertyReturnPopup", PR);
                }
                if (PR.PropertySituated == null)
                {
                    ModelState.AddModelError("PropertySituatedRequired", "Please enter address details.");
                    // return PartialView("_PropertyReturnPopup", PR);
                }
                if (PR.PropertyType == 0)
                {
                    ModelState.AddModelError("PropertypeRequired", "Please enter property type.");
                    //return PartialView("_PropertyReturnPopup", PR);
                }
                if (PR.PropertyDetails == null)
                {
                    ModelState.AddModelError("PropertyDetailsRequired", "Please enter property situated.");
                    //return PartialView("_PropertyReturnPopup", PR);
                }

                if (PR.PresentValue == 0)
                {
                    ModelState.AddModelError("PresentValueRequired", "Please enter present value.");
                    //return PartialView("_PropertyReturnPopup", PR);
                }

                ModelState.Remove("Year");
                ModelState.Remove("PropertyType");
                if (ModelState.IsValid)
                {
                    string msg = "";
                    bool result = true;
                    if (PR.CreatedOn == default(DateTime))
                    {
                        PR.CreatedOn = DateTime.Now;
                        PR.CreatedBy = userDetail.UserID;
                        result = PRService.InsertPR(PR, Convert.ToInt32(Session["PRID"]));
                        msg = "Successfully created";
                    }
                    else
                    {

                        PR.UpdatedBy = userDetail.UserID;
                        PR.UpdatedOn = DateTime.Now;
                        result = PRService.UpdatePR(PR);
                        msg = "Successfully updated";
                    }

                    return Json(new
                    {
                        status = result,
                        fieldName = PR.EmployeeId,
                        type = "success",
                        msg = msg
                    }, JsonRequestBehavior.AllowGet);
                }

                return PartialView("_PropertyReturnPopup", PR);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public ActionResult Delete(int Counter)
        {
            PropertyReturnViewModel PRVM = new PropertyReturnViewModel();
            log.Info("PropertyreturnController/Delete");
            try
            {
                Model.PR PR = new Model.PR();
                bool result = false;
                result = PRService.DeletePR(Counter);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult _GetPRDtls()
        {
            log.Info($"PropertyreturnController/_GetPRDtls");
            Model.PR PR = new Model.PR();
            var CYear = DateTime.Now.Year;
            var cList = new List<Model.SelectListModel>();
            cList.Add(new Model.SelectListModel { id = CYear - 1, value = (CYear - 1).ToString() });
            cList.Add(new Model.SelectListModel { id = CYear, value = CYear.ToString() });
            cList.Add(new Model.SelectListModel { id = CYear + 1, value = (CYear + 1).ToString() });
            PR.CYearList = cList;
            PR.Year = PR.Year == null ? Convert.ToInt32(Session["Year"]) : PR.Year;

            var ddlEmployeeList = PRService.GetAllEmployee();
            List<dynamic> EmployeeList = new List<dynamic>();

            foreach (var item in ddlEmployeeList)
            {
                EmployeeList.Add(new
                {
                    value = item.value,
                    id = item.id
                });
            }
            PR.EmployeeList = EmployeeList;
            PR.EmployeeId = PR.EmployeeId == null ? Convert.ToInt32(Session["EmployeeID"]) : PR.EmployeeId;

            List<dynamic> PropertyType = new List<dynamic>();
            PropertyType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            PropertyType.Add(new Model.SelectListModel() { value = "Housing", id = 1 });
            PropertyType.Add(new Model.SelectListModel() { value = "Building", id = 2 });
            PropertyType.Add(new Model.SelectListModel() { value = "Land", id = 3 });
            PR.PropertyTypeDetails = PropertyType;

            List<Model.SelectListModel> AcquiredType = new List<Model.SelectListModel>();
            AcquiredType.Add(new Model.SelectListModel() { value = "Select", id = 0 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Purchase", id = 1 });
            AcquiredType.Add(new Model.SelectListModel() { value = "lease ", id = 2 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Mortgage ", id = 3 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Inheritance ", id = 4 });
            AcquiredType.Add(new Model.SelectListModel() { value = "Gift or Otherwise   ", id = 5 });
            PR.AcquiredTypeDetails = AcquiredType;


            var ddlRelation = ddlService.ddlRelationList();
            Model.SelectListModel selectRelation = new Model.SelectListModel();
            selectRelation.id = 0;
            selectRelation.value = "Select";
            ddlRelation.Insert(0, selectRelation);
            PR.RelationDetails = ddlRelation;

            ViewBag.EmployeeID = userDetail.EmployeeID;
            ViewBag.UserID = userDetail.UserID;

            return PartialView("_PropertyReturnPopup", PR);
        }

        [HttpPost]
        public ActionResult _SubmitData(FormCollection frm, bool isApplic)
        {
           // string msg = "";
            bool result = true;
            try
            {               
                result = PRService.UpdatePRStatus(Convert.ToInt32(Session["PRID"]), isApplic);
                if (result == true)
                {
                    return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}