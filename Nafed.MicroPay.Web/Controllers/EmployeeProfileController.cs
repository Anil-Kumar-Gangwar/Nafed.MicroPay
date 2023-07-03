using MicroPay.Web.Attributes;
using Nafed.MicroPay.Services;
using System.Web.Mvc;
using Nafed.MicroPay.Services.IServices;
using Model = Nafed.MicroPay.Model;
using System;
using System.IO;
using static Nafed.MicroPay.Services.ImageService;
using Common = Nafed.MicroPay.Common;
using static Nafed.MicroPay.Common.FileHelper;
using System.Web;
using System.Collections.Generic;
using System.Linq;
namespace MicroPay.Web.Controllers
{
    [EncryptedActionParameter]
    public class EmployeeProfileController : Controller
    {
        // GET: EmployeeProfile

        private readonly IEmployeeService employeeService;
        private readonly IDropdownBindService ddlService;


        public EmployeeProfileController(IEmployeeService employeeService, IDropdownBindService ddlService)
        {
            this.employeeService = employeeService;
            this.ddlService = ddlService;
        }

        [SessionTimeout]
        //  [CheckRight]
        public ActionResult Index(int? employeeID)
        {
            try
            {
                var userDetail = (Model.UserDetail)Session["User"];

                if (userDetail.EmployeeID != employeeID)
                {
                    return RedirectToAction("UnAuthorizeAccess", "MenuRender");
                }

                var emp = employeeService.GetEmployeeProfile((int)employeeID);
                emp.EmpProfilePhotoUNCPath =
                   emp.EmpProfilePhotoUNCPath == null ?
                   Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png")
                   :

                  System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + emp.EmpProfilePhotoUNCPath)) ?

                   Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + emp.EmpProfilePhotoUNCPath) :
                   Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");


                if (emp?.achievements.Count > 0)
                {
                    var dItems = emp.achievements.Where(x => x.documents.Count > 0).ToList();

                    foreach (var item in dItems)
                    {
                        item.documents.ToList().ForEach(x =>
                        {
                            x.DocumentUNCPath =
                             System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                             Common.DocumentUploadFilePath.EmployeeAchievement + "/" + x.DocumentFilePath)) ?

                           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.EmployeeAchievement + "/" + x.DocumentFilePath) :
                           "#";
                        });
                    }
                }


                if (emp?.certifications.Count > 0)
                {
                    var dItems = emp.certifications.Where(x => x.documents.Count > 0).ToList();

                    foreach (var item in dItems)
                    {
                        item.documents.ToList().ForEach(x =>
                        {
                            x.DocumentUNCPath =
                             System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                             Common.DocumentUploadFilePath.EmployeeCertification + "/" + x.DocumentFilePath)) ?

                           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.EmployeeCertification + "/" + x.DocumentFilePath) :
                           "#";
                        });
                    }
                }


                return View(emp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [HttpGet]
        public FileResult GetImage(string imgPath)
        {
            try
            {
                if (imgPath != null)
                {
                    log.Info($"Get Item image path {imgPath}");
                    byte[] imageByteData = ImageService.GetImage(imgPath);
                    return File(imageByteData, "image/jpg;base64");
                }
                else
                {
                    byte[] imageByteData = ImageService.GetImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png"));
                    return File(imageByteData, "image/jpg;base64");
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }


        [SessionTimeout]
        [HttpGet]
        [EncryptedActionParameter]
        public ActionResult Edit(int employeeID)
        {
            var userDetail = (Model.UserDetail)Session["User"];

            if (userDetail.EmployeeID != employeeID)
            {
                return RedirectToAction("UnAuthorizeAccess", "MenuRender");
            }

            Model.EmployeeProfile employee = new Model.EmployeeProfile();
            log.Info("EmployeeProfileController/Edit");
            var emp = employeeService.GetEmployeeProfile(employeeID);

            var qAcademic = ddlService.ddlAcedamicAndProfDtls(2);
            emp.ddlQAcademic = qAcademic;
            var qProfessional = ddlService.ddlAcedamicAndProfDtls(2);
            emp.ddlQProfessional = qProfessional;
            var bloodGroup = ddlService.ddlBloodGroupList();
            emp.ddlBloodGroup = bloodGroup;

            var PresentState = ddlService.ddlStateList();
            emp.ddlPresentState = PresentState;

            var PermanentState = ddlService.ddlStateList();
            emp.ddlPermanentState = PermanentState;
            //emp.EmpProfilePhotoUNCPath = employeeService.GetEmployeeProfilePath(emp.EmpCode);
            try
            {

                emp.EmpProfilePhotoUNCPath =
                  emp.EmpProfilePhotoUNCPath == null ?
                  Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png")
                  :

                 System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + emp.EmpProfilePhotoUNCPath)) ?

                  Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + emp.EmpProfilePhotoUNCPath) :
                  Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");
                //if (string.IsNullOrEmpty(emp.AadhaarCardFilePath))
                //{
                //    emp.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");
                //}
                //else
                //{
                //    emp.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, emp.AadhaarCardFilePath);
                //    if (!System.IO.File.Exists(emp.AadhaarCardUNCFilePath))
                //    {
                //        emp.AadhaarCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.AadhaarCardFilePath + "/AadhaarNo.jpg");

                //    }
                //}
                //if (string.IsNullOrEmpty(emp.PanCardFilePath))

                //    emp.PanCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.PanCardFilePath + "/SamplePAN_Card.jpg");
                //else
                //{
                //    emp.PanCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, emp.PanCardFilePath);

                //    if (!System.IO.File.Exists(emp.PanCardUNCFilePath))
                //    {
                //        emp.PanCardUNCFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.PanCardFilePath + "/SamplePAN_Card.jpg");
                //    }
                //}

                return PartialView("_EmployeeProfileEdit", emp);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        [SessionTimeout]
        [HttpPost]
        public ActionResult _EditPersonalDetails(Model.EmployeeProfile employee, IEnumerable<HttpPostedFileBase> files)
        {
            log.Info("EmployeeProfileController/_EditPersonalDetails");
            try
            {
                //string panCardFilePath = string.Empty, aadharCardFilePath = string.Empty;
                //var panCardFile = files == null ? null : files.FirstOrDefault();
                //var aadhaarCardFile = files == null ? null : files.LastOrDefault();

                string profileImgFilePath = string.Empty;
                var profileImgFile = files == null ? null : files.FirstOrDefault();

                employee.EmployeeID = employee.EmployeeID == 0 ? Convert.ToInt32(TempData["EmployeeID"]) : employee.EmployeeID;

                //if (panCardFile != null)
                //{
                //    string fileExtension = Path.GetExtension(panCardFile.FileName);
                //    var contentType = GetFileContentType(panCardFile.InputStream);
                //    var dicValue = GetDictionaryValueByKeyName(".jpg");
                //    if (employee.PANNo =="")
                //    {
                //        ModelState.AddModelError("InValidPAN", "Please enter PAN No.");
                //        return View("_EmployeeProfileEdit", employee);
                //    }

                //    if (((fileExtension != ".jpg" && fileExtension != ".jpeg")))
                //    {
                //        ModelState.AddModelError("InValidPAN", "Please select valid image file.");
                //        return View("_EmployeeProfileEdit", employee);
                //    }
                //    else
                //    {
                //        panCardFilePath = Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(panCardFile.FileName),
                //         Path.GetExtension(panCardFile.FileName));

                //        employee.PanCardFilePath = Common.DocumentUploadFilePath.PanCardFilePath + "/" + panCardFilePath;
                //        //  employee.PanCardFilePath = Common.DocumentUploadFilePath.PanCardFilePath + "/" + panCardFilePath;
                //    }
                //}

                //if (aadhaarCardFile != null)
                //{
                //    string fileExtension = Path.GetExtension(aadhaarCardFile.FileName);
                //    var contentType = GetFileContentType(aadhaarCardFile.InputStream);
                //    var dicValue = GetDictionaryValueByKeyName(".jpg");
                //    if (employee.AadhaarNo == "")
                //    {
                //        ModelState.AddModelError("InValidPAN", "Please enter Aadhaar No");
                //        return View("_EmployeeProfileEdit", employee);
                //    }
                //    if (!((dicValue == contentType) || (fileExtension == ".jpg" || fileExtension == ".jpeg")))
                //    {
                //        ModelState.AddModelError("InValidAadhar", "Please select valid image file.");
                //        return View("_EmployeeProfileEdit", employee);
                //    }
                //    else
                //    {
                //        aadharCardFilePath = Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(aadhaarCardFile.FileName),
                //        Path.GetExtension(aadhaarCardFile.FileName));
                //        employee.AadhaarCardFilePath = Common.DocumentUploadFilePath.AadhaarCardFilePath + "/" + aadharCardFilePath;
                //    }
                //}


                if (profileImgFile != null)
                {
                    string fileExtension = Path.GetExtension(profileImgFile.FileName);
                    var contentType = GetFileContentType(profileImgFile.InputStream);
                    var dicValue = GetDictionaryValueByKeyName(".jpg");
                    //if (employee.PANNo == "")
                    //{
                    //    ModelState.AddModelError("InValidPAN", "Please enter PAN No.");
                    //    return View("_EmployeeProfileEdit", employee);
                    //}
                    if (!IsValidFileName(profileImgFile.FileName))
                    {
                        TempData["Error"] = $"File name must not contain special characters.In File Name,Only following characters are allowed.<br />1. a to z characters.<br/>2. numbers(0 to 9). <br />3. - and _ with space.";
                        return View("_EmployeeProfileEdit", employee);
                    }


                    if (((fileExtension != ".jpg" && fileExtension != ".jpeg")))
                    {
                        return View("_EmployeeProfileEdit", employee);
                    }
                    else
                    {
                        profileImgFilePath = Common.ExtensionMethods.SetUniqueFileName("Pic-",
                         Path.GetExtension(profileImgFile.FileName));

                        employee.EmpProfilePhotoUNCPath = Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + profileImgFilePath;
                        //  employee.PanCardFilePath = Common.DocumentUploadFilePath.PanCardFilePath + "/" + panCardFilePath;
                    }


                }

                if ((employee.MobileNo == "" || employee.MobileNo == null) && (employee.EmailID == "" || employee.EmailID == null))
                {
                    TempData["Error"] = "Email ID and Mobile No. can not be blank, either provide Email Id or Mobile No.";
                    return View("_EmployeeProfileEdit", employee);
                }

                bool isUpdated = employeeService.UpdatetEmployeeProfile(employee);

                if (isUpdated)
                {

                    //if (panCardFile != null)
                    //    SaveFile(panCardFile, Nafed.MicroPay.Common.DocumentType.PanCard, panCardFilePath);
                    //if (aadhaarCardFile != null)
                    // SaveFile(aadhaarCardFile, Nafed.MicroPay.Common.DocumentType.AadhaarCard, aadharCardFilePath);

                    if (profileImgFile != null)
                    {
                        employeeService.ChangeProfilePicture(employee.EmployeeID, profileImgFilePath, employee.MobileNo, employee.EmailID);
                        SaveFile(profileImgFile, Common.DocumentType.ProfileImage, profileImgFilePath);
                    }
                }
                TempData.Keep("EmployeeID");
                TempData["Message"] = "Successfully Updated";
                //return PartialView("Index", employee);
                return RedirectToAction("Index", new { employee.EmployeeID });
                // return Json(new {msg= "Successfully Updated",type="success", employeeID =employee.EmployeeID }, JsonRequestBehavior.AllowGet);
                //  return JavaScript("window.location = '" + Url.Action("Index", "EmployeeProfile",new { @employeeID =employee.EmployeeID}) + "'");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
        }

        private bool SaveFile(HttpPostedFileBase file, Common.DocumentType docType, string documentFile)
        {
            log.Info($"EmployeeController/SaveFile");

            string fileName = string.Empty; string filePath = string.Empty;
            try
            {
                if (file != null)
                {
                    fileName = Path.GetFileName(file.FileName);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        #region Check Mime Type
                        if (!IsValidFileName(file.FileName))
                        {
                            return false;
                        }
                        var contentType = GetFileContentType(file?.InputStream);
                        //var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                        //if (dicValue != contentType)
                        //{
                        //    return false;
                        //}
                        #endregion

                        if (docType == Common.DocumentType.PanCard)
                        {
                            fileName = documentFile;
                            // Common.ExtensionMethods.SetUniqueFileName(Path.GetFileNameWithoutExtension(file.FileName),
                            //Path.GetExtension(file.FileName));
                            filePath = "~/" + Common.DocumentUploadFilePath.PanCardFilePath;
                        }

                        if (docType == Common.DocumentType.AadhaarCard)
                        {
                            fileName = documentFile;
                            filePath = "~/" + Common.DocumentUploadFilePath.AadhaarCardFilePath;
                        }

                        if (docType == Common.DocumentType.ProfileImage)
                        {

                            fileName = documentFile;
                            filePath = "~/" + Common.DocumentUploadFilePath.ProfilePicFilePath;
                        }
                        string uploadedFilePath = Server.MapPath(filePath);
                        string sPhysicalPath = Path.Combine(uploadedFilePath, fileName);

                        file.SaveAs(sPhysicalPath);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
            return true;
        }

        //[HttpGet]
        //public ActionResult Create()
        //{
        //    log.Info($"EmployeeProfileController/_Create");
        //    try
        //    {
        //        var emp = employeeService.GetEmployeeProfile(1);
        //        //emp.EmpProfilePhotoUNCPath = employeeService.GetEmployeeProfilePath(emp.EmpCode);
        //        return View(emp);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}

        public ActionResult _UpdateMobileNo(int employeeID, string newMobileNo)
        {
            return new EmptyResult();
        }

    }
}