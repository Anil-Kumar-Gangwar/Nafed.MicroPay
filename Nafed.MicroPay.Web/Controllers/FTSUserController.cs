using MicroPay.Web.Models;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using Model = Nafed.MicroPay.Model;
namespace MicroPay.Web.Controllers
{
    public class FTSUserController : BaseController
    {
        private readonly IFTSUserService userService;
        private readonly IDropdownBindService ddlService;
        public FTSUserController(IFTSUserService userService, IDropdownBindService ddlService)
        {
            this.userService = userService;
            this.ddlService = ddlService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserGridView()
        {

            log.Info($"FTSUserController/UserGridView");
            try
            {
                List<Model.FTSUser> userList = new List<Model.FTSUser>();
                userList = userService.GetUserList();
                return PartialView("_UserGridView", userList);
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
            log.Info("FTSUserController/Create");
            try
            {
                Model.FTSUser objUser = new Model.FTSUser();

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
        public ActionResult Create(Model.FTSUser createUser)
        {
            log.Info("FTSUserController/Create");
            try
            {
                if (string.IsNullOrEmpty(createUser.UserName))
                    ModelState.AddModelError("UserNameAlreadyExist", "Please enter user name.");
                if (string.IsNullOrEmpty(createUser.hdPassword))
                    ModelState.AddModelError("Password", "Please enter password.");
                if (string.IsNullOrEmpty(createUser.hdCPassword))
                    ModelState.AddModelError("ConfirmPassword", "Please confirm enter password.");



                if (ModelState.IsValid)
                {
                    if (!this.IsCaptchaValid("Captcha is not valid"))
                    {
                        ViewBag.LoginMessage = "Captcha is not valid";
                        return View(createUser);
                    }

                    createUser.Password = Password.DecryptStringAES(createUser.hdPassword).Replace(createUser.hdCp, "");
                    createUser.ConfirmPassword = Password.DecryptStringAES(createUser.hdCPassword).Replace(createUser.hdCp, "");

                    createUser.UserName = createUser.UserName.Trim();
                    if (userService.UserNameExists(createUser.UserId, createUser.UserName))
                        ModelState.AddModelError("UserNameAlreadyExist", "User Name Already Exist");
                    else if (String.Compare(createUser.Password, createUser.ConfirmPassword) != 0)
                        ModelState.AddModelError("ConfirmPassword", "Confirm Password not matched");
                    else if (!Password.CheckPasswordAgainstPolicy(createUser.UserName, createUser.Password))
                    {
                        ViewBag.LoginMessage = "Password should contain atleast one upper case letter, one lower case letter, one digit, one special character, 8-50 characters long and should not contain username";
                        return View(createUser);
                    }
                    else
                    {
                        createUser.Password = Password.CreatePasswordHash(createUser.Password.Trim(), Password.CreateSalt(Password.PASSWORD_SALT));
                        createUser.CreatedOn = DateTime.Now;
                        createUser.CreatedBy = userDetail.UserID;
                        int userID = userService.InsertUser(createUser);
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
            return View(createUser);
        }

        [HttpGet]
        public ActionResult Edit(int userId, string userName)
        {
            log.Info($"FTSUserController/Edit/{userId}");
            try
            {

                Model.FTSUser objUser = new Model.FTSUser();
                objUser = userService.GetUserByID(userId);
                objUser.Password = "";
                objUser.hdCPassword = "";
                objUser.hdPassword = "";
                objUser.hdCp = "";
                log.Info($"After userService.GetUserByID(userId)/{userId}");
                return View(objUser);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult EditUser(int userId, string userName)
        {
            log.Info($"FTSUserController/Edit/{userId}");
            try
            {
                ViewBag.userId = userId;
                ViewBag.userName = userName;
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Model.FTSUser editUser)
        {
            log.Info("FTSUserController/Edit");
            try
            {

                ModelState.Remove("UserName");
                ModelState.Remove("ConfirmPassword");

                if (string.IsNullOrEmpty(editUser.hdPassword))
                    ModelState.AddModelError("Password", "Please enter password.");
                if (string.IsNullOrEmpty(editUser.hdCPassword))
                    ModelState.AddModelError("ConfirmPassword", "Please confirm enter password.");

                if (ModelState.IsValid)
                {
                    editUser.UserName = editUser.UserName.Trim();

                    if (!this.IsCaptchaValid("Captcha is not valid"))
                    {
                        ViewBag.LoginMessage = "Captcha is not valid";
                        return View(editUser);
                    }

                    editUser.Password = Password.DecryptStringAES(editUser.hdPassword).Replace(editUser.hdCp, "");
                    editUser.ConfirmPassword = Password.DecryptStringAES(editUser.hdCPassword).Replace(editUser.hdCp, "");

                    if (String.Compare(editUser.Password, editUser.ConfirmPassword) != 0)
                        ModelState.AddModelError("ConfirmPassword", "Confirm Password not matched");
                    else if (!Password.CheckPasswordAgainstPolicy(editUser.UserName, editUser.Password))
                    {
                        ViewBag.LoginMessage = "Password should contain atleast one upper case letter, one lower case letter, one digit, one special character, 8-50 characters long and should not contain username";
                        return View(editUser);
                    }
                    else
                    {
                        editUser.Password = Password.CreatePasswordHash(editUser.Password.Trim(), Password.CreateSalt(Password.PASSWORD_SALT));
                        userService.UpdateUser(editUser);
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
            return View(editUser);

        }

        [HttpGet]
        public ActionResult MapDepartment(int userId, string userName)
        {
            log.Info($"FTSUserController/MapDepartment");
            try
            {
                Model.FTSUserDepartment vm = new Model.FTSUserDepartment();
                vm.UserId = userId;
                vm.UserName = userName;
                vm.lstDepartmentList = ddlService.ddlDepartmentHavingEmployee();
                var data = userService.GetMappedDepartment(userId);
                vm.intDepartmentId = data;
                return View(vm);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        [HttpPost]
        public ActionResult MapDepartment(Model.FTSUserDepartment ftsUDepartment)
        {
            log.Info($"FTSUserController/MapDepartment");
            try
            {

                if (ftsUDepartment.intDepartmentId == null)
                    ModelState.AddModelError("DepartmentValidator", "Please select Department");

                if (ModelState.IsValid)
                {
                    if (!userService.UserExistInUserDepartment(ftsUDepartment.UserId))
                    {
                        userService.InsertMapDepartment(ftsUDepartment);
                        TempData["Message"] = "Successfully Assign";
                        return RedirectToAction("Index");
                    }
                    else if (userService.UserExistInUserDepartment(ftsUDepartment.UserId))
                    {
                        userService.UpdateMapDepartment(ftsUDepartment);
                        TempData["Message"] = "Successfully Assign";
                        return RedirectToAction("Index");
                    }
                    return View(ftsUDepartment);
                }
                else
                {
                    ftsUDepartment.lstDepartmentList = ddlService.ddlDepartmentHavingEmployee();
                    return View(ftsUDepartment);
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

    }
}