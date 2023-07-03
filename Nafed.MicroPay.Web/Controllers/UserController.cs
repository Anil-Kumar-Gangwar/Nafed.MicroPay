using MicroPay.Web.Models;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common = Nafed.MicroPay.Common;
using Model = Nafed.MicroPay.Model;
using static Nafed.MicroPay.Common.FileHelper;
using MicroPay.Web.Attributes;

namespace MicroPay.Web.Controllers
{

    public class UserController : BaseController
    {
        private readonly IUserService userService;
        private readonly IDropdownBindService ddlService;
        private readonly IEmployeeService employeeService;
        public UserController(IUserService userService, IDropdownBindService ddlService, IEmployeeService employeeService)
        {
            this.userService = userService;
            this.ddlService = ddlService;
            this.employeeService = employeeService;
        }
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Index()
        //{
        //    var emp = employeeService.GetEmployeeList();
        //    emp.EmpProfilePhotoUNCPath = employeeService.GetEmployeeProfilePath();
        //    return View(emp);
        //}
        //public FileResult GetImage(string imgPath)
        //{
        //    log.Info($"Get Item image path {imgPath}");
        //    byte[] imageByteData = ImageService.GetImage(imgPath);
        //    return File(imageByteData, "image/jpg;base64");
        //}

        public ActionResult UserGridView(FormCollection formCollection)
        {

            log.Info($"UserController/UserGridView");
            try
            {
                UserViewModel userVM = new UserViewModel();
                userVM.userList = userService.GetUserList();

                userVM.userList.ForEach(x =>
                {
                    //x.ImageName =
                    //   x.ImageName == null ?
                    //   Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png")
                    //   :
                    //   Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{x.ImageName}");

                    x.UserProfilePhotoUNCPath =
                    x.ImageName == null ?
                    Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png")
                    :
                    System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{x.ImageName}")) ?

                    Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{x.ImageName}") :
                    Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");
                });

                return PartialView("_UserGridView", userVM);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public void BindDropdowns()
        {
            var ddlDepartmentList = ddlService.ddlDepartmentList();
            Model.SelectListModel selectDepartment = new Model.SelectListModel();
            selectDepartment.id = 0;
            selectDepartment.value = "Select";
            ddlDepartmentList.Insert(0, selectDepartment);
            ViewBag.DepartmentList = new SelectList(ddlDepartmentList, "id", "value");


            var ddlUserTypeList = ddlService.ddlUserTypeList();
            Model.SelectListModel selectUserType = new Model.SelectListModel();
            selectUserType.id = 0;
            selectUserType.value = "Select";
            ddlUserTypeList.Insert(0, selectUserType);
            ViewBag.UserTypeList = new SelectList(ddlUserTypeList, "id", "value");

        }
        [HttpGet]
        public ActionResult Create()
        {
            log.Info("UserController/Create");
            try
            {
                BindDropdowns();
                Model.User objUser = new Model.User();
                objUser.UserProfilePhotoUNCPath = Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");
                objUser.EmployeeList = userService.GetUnMappedEmployees(null);

                return View(objUser);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.User createUser, HttpPostedFileBase files)
        {
            log.Info("UserController/Create");
            try
            {
                if (string.IsNullOrEmpty(createUser.hdUserName))
                    ModelState.AddModelError("UserNameAlreadyExist", "Please enter user name.");
                if (string.IsNullOrEmpty(createUser.hdPassword))
                    ModelState.AddModelError("Password", "Please enter password.");
                if (string.IsNullOrEmpty(createUser.hdConfirmPassword))
                    ModelState.AddModelError("ConfirmPassword", "Please enter confirm password.");

                if (!string.IsNullOrEmpty(createUser.hdUserName) && !string.IsNullOrEmpty(createUser.hdPassword) && !string.IsNullOrEmpty(createUser.hdConfirmPassword))
                {
                    createUser.UserName = Password.DecryptStringAES(createUser.hdUserName);
                    createUser.Password = Password.DecryptStringAES(createUser.hdPassword);
                    createUser.ConfirmPassword = Password.DecryptStringAES(createUser.hdConfirmPassword);
                }

                if (!Password.CheckPasswordAgainstPolicy(createUser.UserName, createUser.Password))
                {
                    ModelState.AddModelError("", "Password should contain atleast one upper case letter, one lower case letter, one digit, one special character, 8-50 characters long and should not contain username");

                }
                BindDropdowns();
                string ext = "";
                if (files != null)
                {
                    ext = Path.GetExtension(files.FileName);
                    ModelState.Remove("ImageName");
                }
                if (ModelState.IsValid)
                {
                    createUser.UserName = createUser.UserName.Trim();
                    if (userService.UserNameExists(createUser.UserID, createUser.UserName))
                        ModelState.AddModelError("UserNameAlreadyExist", "User Name Already Exist");
                    else if (createUser.DepartmentID == 0)
                        ModelState.AddModelError("DepartmentID", "Please select department");
                    else if (createUser.UserTypeID == 0)
                        ModelState.AddModelError("UserTypeID", "Please select user type");
                    else if (String.Compare(createUser.Password, createUser.ConfirmPassword) != 0)
                        ModelState.AddModelError("ConfirmPassword", "Confirm Password not matched");


                    else
                    {
                        createUser.Password = Password.CreatePasswordHash(createUser.Password.Trim(), Password.CreateSalt(Password.PASSWORD_SALT));
                        createUser.CreatedOn = DateTime.Now;
                        createUser.CreatedBy = userDetail.UserID;
                        int userID = userService.InsertUser(createUser, ext);
                        SaveProfilePic(files, userID);
                        TempData["Message"] = "Successfully Created";
                        return RedirectToAction("Index");
                    }
                }
                createUser.EmployeeList = userService.GetUnMappedEmployees(null);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
                throw ex;
            }
            return View(createUser);
        }

        [HttpGet]
        public ActionResult Edit(int userID)
        {
            log.Info($"UserController/Edit/{userID}");
            try

            {
                BindDropdowns();
                Model.User objUser = new Model.User();
                objUser = userService.GetUserByID(userID);

                objUser.UserProfilePhotoUNCPath =
                objUser.ImageName == null ?
                Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png")
                :
                System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{objUser.ImageName}")) ?

                Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{objUser.ImageName}") :
                Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");

                objUser.EmployeeList = userService.GetUnMappedEmployees(null);
                return View(objUser);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Model.User editUser, HttpPostedFileBase files)
        {
            log.Info("UserController/Edit");
            try
            {
                BindDropdowns();
                if (files != null)
                {
                    string ext = Path.GetExtension(files.FileName);
                    string fileName = editUser.UserID + ext;
                    editUser.ImageName = fileName;
                    ModelState.Remove("ImageName");
                }
                ModelState.Remove("EmployeeID");
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                if (ModelState.IsValid)
                {
                    editUser.UserName = editUser.UserName.Trim();

                    if (userService.UserNameExists(editUser.UserID, editUser.UserName))
                        ModelState.AddModelError("UserNameAlreadyExist", "User Name Already Exist");
                    else if (editUser.DepartmentID == 0)
                        ModelState.AddModelError("DepartmentID", "Please select department");
                    else if (editUser.UserTypeID == 0)
                        ModelState.AddModelError("UserTypeID", "Please select user type");
                    else if (String.Compare(editUser.Password, editUser.ConfirmPassword) != 0)
                        ModelState.AddModelError("ConfirmPassword", "Confirm Password not matched");
                    else
                    {
                        // editUser.Password = Password.CreatePasswordHash(editUser.Password.Trim(), Password.CreateSalt(Password.PASSWORD_SALT));
                        editUser.UpdatedOn = DateTime.Now;
                        editUser.UpdatedBy = userDetail.UserID;
                        userService.UpdateUser(editUser);
                        SaveProfilePic(files, editUser.UserID);
                        TempData["Message"] = "Succesfully Updated";
                        return RedirectToAction("Index");
                    }
                }
                editUser.UserProfilePhotoUNCPath =
                editUser.ImageName == null ?
                Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png")
                :
                System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{editUser.ImageName}")) ?

                Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + $"/{editUser.ImageName}") :
                Path.Combine(Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png");

            }
            catch (Exception ex)
            {
                log.Error("Error-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return View(editUser);

        }

        public ActionResult Delete(int userID)
        {
            log.Info($"UserController/Delete/{userID}");
            try
            {
                userService.Delete(userID);
                TempData["Message"] = "Succesfully Deleted";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                TempData["Error"] = "Error-Some error occurs please contact administrator";
            }
            return RedirectToAction("Index");
        }

        public bool SaveProfilePic(HttpPostedFileBase UserImg, int userID)
        {
            log.Info($"UserController/SaveProfilePic/{userID}");

            string sFileName = string.Empty;
            try
            {
                if (UserImg != null)
                {
                    sFileName = Path.GetFileName(UserImg.FileName);

                    if (!string.IsNullOrEmpty(sFileName))
                    {
                        #region Check Mime Type
                        var contentType = GetFileContentType(UserImg.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(UserImg.FileName));
                        if (dicValue != contentType)
                        {
                            return false;
                        }
                        #endregion

                        string ext = Path.GetExtension(UserImg.FileName);
                        string fileName = userID + ext;
                        string filePath = Server.MapPath("~/Images/Profile");
                        string sPhysicalPath = Path.Combine(filePath, fileName);
                        DeleteFile(userID.ToString());
                        UserImg.SaveAs(sPhysicalPath);

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

        private void DeleteFile(string fileNameWithoutExt)
        {
            log.Info($"UserController/DeleteFile");
            try
            {
                string filePath = Server.MapPath("~/Images/Profile");
                DirectoryInfo oDI = new DirectoryInfo(filePath);
                FileInfo[] aryFile = oDI.GetFiles(fileNameWithoutExt + "*");
                foreach (FileInfo oFileInfo in aryFile)
                {
                    string sPath = Path.Combine(filePath, oFileInfo.Name);
                    if (System.IO.File.Exists(sPath))
                    {
                        System.IO.File.Delete(sPath);
                    }
                }
            }
            catch (Exception ex)
            {

                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult GeneratePassword()
        {
            log.Info($"UserController/GeneratePassword");
            try
            {
                var eDAndmDDesignationIDs = new[] { 27, 309, 330 };
                var users = userService.GetUserList().Where(x => x.UserName.ToLower() != "admin"
                && (!eDAndmDDesignationIDs.Any(y => y == x.DesignationID))).ToList();

                users.ForEach(x =>
                {
                    x.Password = Password.CreatePasswordHash($"Nafed{x.UserName}@{x.DOB?.Month.ToString("00")}{x.DOB?.Year.ToString()}",
                        Password.CreateSalt(Password.PASSWORD_SALT));
                });


                userService.GeneratePasswordforAllEmployees(users);
                TempData["Message"] = "Password Generated successfully.";
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
        }

        public ActionResult LockUnlockUser(int userId, bool ulock)
        {
            log.Info($"UserController/LockUnlockUser");
            bool flag = false;
            try
            {
                flag = userService.LockUnlockUser(userId, ulock);
                if (flag)
                    TempData["Message"] = "User " + (ulock == true ? "locked successfully." : "unlocked successfully.");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Error: " + ex.Message + " ,StackTrace: " + ex.StackTrace + " ,DateTimeStamp :" + DateTime.Now);
                throw ex;
            }
        }
    }
}