using AutoMapper;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using static Nafed.MicroPay.Common.FileHelper;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers.Form12BB
{
    public class TaxDeductionController : BaseController
    {
        private readonly IForm12BBService form12BBService;

        public TaxDeductionController(IForm12BBService form12BBService)
        {
            this.form12BBService = form12BBService;
        }
        // GET: TaxDeduction
        public ActionResult Index()
        {
            log.Info($"TaxDeduction/Index");
            return View();
        }

        [HttpPost]
        public ActionResult __Form12BBFilters(Form12BBFilterVM frmFilters)
        {
            log.Info($"TaxDeduction/__Form12BBFilters");

            try
            {
                var frm12BBs = form12BBService.GetForm12BBList((int)userDetail.EmployeeID, frmFilters.selectedFYear);
                return PartialView("_Form12BBGridView", frm12BBs);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public ActionResult __Form12BBFilters()
        {
            log.Info($"TaxDeduction/__Form12BBFilters");
            try
            {
                Form12BBFilterVM frm12BBFilter = new Form12BBFilterVM();
                var fYears = ExtensionMethods.GetFinancialYrList(2017, 2021)
                .Select(x => new SelectListItem { Text = x, Value = x }).OrderByDescending(x => x.Value).ToList();
                frm12BBFilter.fYears = fYears;
                return PartialView("_Form12BBFilter", frm12BBFilter);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult _EditForm12BB(int? view, int? employeeID, string fYear)
        {
            log.Info($"TaxDeduction/_EditForm12BB/view:{view}&employeeID:{employeeID}&fYear:{fYear}");
            try
            {

                Form12BBInfo form12BBInfo = new Form12BBInfo();

                if (!employeeID.HasValue)
                {
                    // fYear = ExtensionMethods.GetFinancialYrList(DateTime.Now.Year, DateTime.Now.Year).FirstOrDefault();

                    form12BBInfo = form12BBService.GetEmployeeForm12BB(userDetail.EmployeeID.Value, fYear);

                }
                else
                    form12BBInfo = form12BBService.GetEmployeeForm12BB(employeeID.Value, fYear);

                if (form12BBInfo.taxDeductions.sections.Count == 0)    /// ======  Default case, when there is no record 
                    form12BBInfo.taxDeductions = form12BBService.GetDeductionSection(fYear);

                form12BBInfo.FYear = fYear;
                form12BBInfo.view = (short?)view;

                return PartialView("_EditForm12BB", form12BBInfo);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult _GetForm12BB(int? employeeID, string fYear)
        {
            log.Info($"TaxDeduction/_GetForm12BB/employeeID:{employeeID}&fYear:{fYear}");
            try
            {
                Form12BBInfo form12BBInfo = new Form12BBInfo();

                if (!employeeID.HasValue)
                {
                    fYear = ExtensionMethods.GetFinancialYrList(DateTime.Now.Year, DateTime.Now.Year).FirstOrDefault();
                    form12BBInfo = form12BBService.GetEmployeeForm12BB(userDetail.EmployeeID.Value, fYear);
                }
                else
                    form12BBInfo = form12BBService.GetEmployeeForm12BB(employeeID.Value, fYear);

                if (form12BBInfo.taxDeductions.sections.Count == 0)    /// ======  Default case, when there is no record 
                    form12BBInfo.taxDeductions = form12BBService.GetDeductionSection(fYear);
                form12BBInfo.FYear = fYear;

                return PartialView("_Form12BB", form12BBInfo);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _PostForm12BB(Form12BBInfo frm12BB, string ButtonType)
        {
            log.Info($"TaxDeduction/_PostForm12BB");
            try
            {
                if (ModelState.IsValid)
                {
                    if (ButtonType == "Save")
                    {
                        if (frm12BB.EmpFormID == 0)
                        {
                            frm12BB.CreatedBy = userDetail.UserID;
                            frm12BB.CreatedOn = DateTime.Now;
                            // frm12BB.FormState = 1;
                        }
                        else
                        {
                            frm12BB.UpdatedBy = userDetail.UserID;
                            frm12BB.UpdatedOn = DateTime.Now;
                        }

                        var res = form12BBService.PostForm12BB(frm12BB);
                        if (res)
                        {
                            if (frm12BB.EmpFormID == 0)
                                return Json(new { success = "1", msgType = "success", msg = "Data saved successfully." }, JsonRequestBehavior.AllowGet);
                            else
                                return Json(new { success = "1", msgType = "success", msg = "Data updated successfully." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (ButtonType == "Save & Submit")
                    {
                        frm12BB.FormState = 2;
                        frm12BB.UpdatedBy = userDetail.UserID;
                        frm12BB.UpdatedOn = DateTime.Now;
                        frm12BB.SubmittedOn = DateTime.Now;
                        var res = form12BBService.PostForm12BB(frm12BB);
                        if (res)
                            return Json(new { success = "1", msgType = "success", msg = "Data submitted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return PartialView("_Form12BB", frm12BB);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetUploadDocumentForm(int? empFormID, string fYear)
        {
            log.Info($"TaxDeduction/_GetUploadDocumentForm");
            try
            {
                UploadForm12BBDocument frmDocument = new UploadForm12BBDocument();
                List<SelectListItem> claimList = new List<SelectListItem>();

                claimList.Add(new SelectListItem { Value = "House Rent Allowance", Text = "HRA" });
                claimList.Add(new SelectListItem { Value = "Leave travel concessions", Text = "LTC" });
                claimList.Add(new SelectListItem { Value = "Deduction of interest", Text = "DOI" });
                claimList.Add(new SelectListItem { Value = "Deduction under Chapter VI-A", Text = "DeductionUnderChapterVI-A" });

                frmDocument.EmpFormID = empFormID ?? 0;
                frmDocument.claimList = claimList;

                var sections = form12BBService.GetSectionList(fYear);
                if (sections.Count > 0)
                    frmDocument.sections = sections;

                return PartialView("_UploadDocument", frmDocument);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult _GetUploadDocument(int empFormID)
        {
            log.Info($"TaxDeduction/_GetUploadDocument");
            try
            {
                var form12BBDocumentList = form12BBService.GetForm12BBDocumentList(empFormID);

                return PartialView("_DocumentList", form12BBDocumentList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult DownloadDocument(string fileName)
        {
            string fullPath = Request.MapPath("~/Document/Form12BBDocument/" + fileName);
            if (!System.IO.File.Exists(fullPath))
                return Content($"file not found.");
            return DownloadFile(fileName, fullPath);
        }

        public ActionResult DeleteDocument(int form12BBDocumentID, int empFormID, string fileName)
        {
            bool flag = form12BBService.DeleteDocument(form12BBDocumentID);
            if (flag)
            {
                string fullPath = Request.MapPath("~/Document/Form12BBDocument/" + fileName);
                if (System.IO.File.Exists(fullPath))
                    ExtensionMethods.DeleteFile(fullPath);

            }
            return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        // [ValidateInput(false)]
        public ActionResult _UploadDocument(UploadForm12BBDocument doc, FormCollection frm)
        {
            log.Info($"TaxDeduction/_UploadDocument");
            try
            {
                if (doc.selectedClaim == "HRA" || doc.selectedClaim == "LTC")
                {
                    ModelState.Remove("sectionID");
                    ModelState.Remove("subSectionID");
                    ModelState.Remove("subSectionDescriptionID");
                }
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        string fileName = string.Empty; string filePath = string.Empty;
                        string fname = "";
                        var DocNames = frm.GetValue("FileName").RawValue;
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;

                        #region Check Mime Type
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            if (!IsValidFileName(file.FileName))
                            {
                                stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                                stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                                stringBuilder.Append($"I. a to z characters.");
                                stringBuilder.Append($"II. numbers(0 to 9).");
                                stringBuilder.Append($"III. - and _ with space.");
                            }

                            var contentType = GetFileContentType(file.InputStream);
                            var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                            if (dicValue != contentType)
                            {
                                stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                                stringBuilder.Append("<br>");
                            }
                        }
                        if (stringBuilder.ToString() != "")
                        {
                            return Json(new
                            {
                                status = 1,
                                type = "error",
                                msg = stringBuilder.ToString()
                            }, JsonRequestBehavior.AllowGet);
                        }
                        #endregion

                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            fname = ExtensionMethods.SetUniqueFileName("Document-",
                            Path.GetExtension(file.FileName));
                            fileName = fname;
                            string fullPath = Request.MapPath("~/Document/Form12BBDocument/" + fileName);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                            fname = Path.Combine(Server.MapPath("~/Document/Form12BBDocument/"), fname);
                            file.SaveAs(fname);

                        }
                    }

                    return Json(new
                    {
                        status = 1,
                        type = "success",
                        msg = "File Forwarded Successfully."
                    }, JsonRequestBehavior.AllowGet);
                }

                List<SelectListItem> claimList = new List<SelectListItem>();
                claimList.Add(new SelectListItem { Value = "House Rent Allowance", Text = "HRA" });
                claimList.Add(new SelectListItem { Value = "Leave travel concessions", Text = "LTC" });
                claimList.Add(new SelectListItem { Value = "Deduction of interest", Text = "DOI" });
                claimList.Add(new SelectListItem { Value = "Deduction under Chapter VI-A", Text = "DeductionUnderChapterVI-A" });
                doc.claimList = claimList;

                return PartialView("_UploadDocument", doc);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }


        [HttpPost]
        public ActionResult UploadFiles(FormCollection frm)
        {
            log.Info($"TaxDeduction/UploadFiles");

            if (Request.Files.Count > 0)
            {
                int? sectionID = null, subSectionID = null, subSectionDescriptionID = null;

                string fileName = string.Empty; string filePath = string.Empty;
                string fname = "";

                string fileDescription = Request["FileDescription"];
                string natureOfCliam = Request["NatureOfCliam"];

                if (natureOfCliam == "DeductionUnderChapterVI-A")
                {
                    sectionID = Request["sectionID"].ToString() != "" ? Convert.ToInt32(Request["sectionID"].ToString()) : (int?)null;
                    subSectionID = Request["subSectionID"].ToString() != "" ? Convert.ToInt32(Request["subSectionID"].ToString()) : (int?)null;
                    subSectionDescriptionID =
                        Request["subSectionDescriptionID"].ToString() != "" ? Convert.ToInt32(Request["subSectionDescriptionID"].ToString()) : (int?)null;

                    if (subSectionID == 0)
                        subSectionID = (int?)null;

                    if (subSectionDescriptionID == 0)
                        subSectionDescriptionID = (int?)null;
                }
                int empFormID = Convert.ToInt32(Request["EmpFormID"]);

                UploadForm12BBDocument frmDocument = new UploadForm12BBDocument();

                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;

                    #region Check Mime Type
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (!IsValidFileName(file.FileName))
                        {
                            stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                            stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                            stringBuilder.Append($"I. a to z characters.");
                            stringBuilder.Append($"II. numbers(0 to 9).");
                            stringBuilder.Append($"III. - and _ with space.");
                        }
                        var contentType = GetFileContentType(file.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                        if (dicValue != contentType)
                        {
                            stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                            stringBuilder.Append("<br>");
                        }
                    }
                    if (stringBuilder.ToString() != "")
                    {
                        return Json(new { success = "0", msgType = "error", msg = stringBuilder.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];

                        fname = ExtensionMethods.SetUniqueFileName("Document-",
                     Path.GetExtension(file.FileName));
                        fileName = fname;

                        //filePath = "~/" + DocumentUploadFilePath.CandidatePhoto;

                        string fullPath = Request.MapPath("~/Document/Form12BBDocument/" + fileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        fname = Path.Combine(Server.MapPath("~/Document/Form12BBDocument/"), fname);
                        file.SaveAs(fname);
                    }

                    #region Save Form12BB Document

                    frmDocument.CreatedBy = userDetail.UserID;
                    frmDocument.CreatedOn = DateTime.Now;
                    frmDocument.FileDescription = fileDescription;
                    frmDocument.selectedClaim = natureOfCliam;
                    frmDocument.sectionID = sectionID;
                    frmDocument.subSectionID = subSectionID;
                    frmDocument.FileName = fileName;
                    frmDocument.subSectionDescriptionID = subSectionDescriptionID;
                    frmDocument.EmpFormID = empFormID;

                    Mapper.Initialize(cfg =>
                    {

                        cfg.CreateMap<UploadForm12BBDocument, EmployeeForm12BBDocument>()
                        .ForMember(d => d.NatureOfClaim, o => o.MapFrom(s => s.selectedClaim))
                        .ForMember(d => d.EmpFormID, o => o.MapFrom(s => s.EmpFormID))
                        .ForMember(d => d.SectionID, o => o.MapFrom(s => s.sectionID))
                        .ForMember(d => d.SubSectionID, o => o.MapFrom(s => s.subSectionID))
                        .ForMember(d => d.SubSectionDescriptionID, o => o.MapFrom(s => s.subSectionDescriptionID))
                        .ForMember(d => d.FileName, o => o.MapFrom(s => s.FileName))
                        .ForMember(d => d.FileDescription, o => o.MapFrom(s => s.FileDescription))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(userDetail.UserID))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))

                        ;
                    });
                    var empForm12BBDoc = Mapper.Map<EmployeeForm12BBDocument>(frmDocument);

                    var flag = form12BBService.UploadForm12BBDocument(empForm12BBDoc);
                    if (flag)
                        return Json(new { success = "1", msgType = "success", msg = "Document uploaded successfully." }, JsonRequestBehavior.AllowGet);

                    return Content("");

                    #endregion
                }
                catch (Exception ex)
                {
                    log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                    throw ex;
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
    }
}
