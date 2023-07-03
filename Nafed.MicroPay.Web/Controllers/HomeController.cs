using MicroPay.Web.Attributes;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using static Nafed.MicroPay.Common.EncryptDecrypt;

namespace MicroPay.Web.Controllers
{
    public class HomeController : Controller
    {

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDashBoardService dashBoardService;
        private readonly IMenuService menuService;
        // GET: Home
        public HomeController(IDashBoardService dashBoardService, IMenuService menuService)
        {
            this.menuService = menuService;
            this.dashBoardService = dashBoardService;
        }

        [SessionTimeout]
        [CheckRight]
        public ActionResult Index()
        {
          
            if (Session["User"] != null)
            {
                log.Info($"DashboardController/Index");
                Dashboard dashBoard = new Dashboard();
                var userDetail = (UserDetail)Session["User"];

                dashBoard = dashBoardService.GetEmployeeDetailForDashBoard(userDetail.EmployeeID).FirstOrDefault();
                dashBoard.EmployeeTypeId = userDetail.EmployeeTypeId;
                dashBoard.DeviceTypeIsMobile = userDetail.DeviceTypeIsMobile;

                if (!userDetail.DeviceTypeIsMobile)
                {
                    if (dashBoard != null)
                        dashBoard.EmployeeDobDoj = dashBoardService.GetEmployeeDOBWorkAnniversary(userDetail.BranchID, null).ToList();
                    else
                        dashBoard = new Dashboard()
                        {
                            EmployeeDobDoj = dashBoardService.GetEmployeeDOBWorkAnniversary(userDetail.BranchID, null).ToList()

                        };
                    dashBoard.EmployeeDobDoj.Where(w => w.EmpProfilePhotoUNCPath == null).Select(w => { w.EmpProfilePhotoUNCPath = Path.Combine(Nafed.MicroPay.Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png"); return w; });

                    dashBoard.EmployeeDobDoj.Where(w => w.EmpProfilePhotoUNCPath != null).Select(w => { w.EmpProfilePhotoUNCPath = Path.Combine(Nafed.MicroPay.Common.DocumentUploadFilePath.ProfilePicFilePath + "/"+ w.EmpProfilePhotoUNCPath); return w; });


                    var eventList = dashBoardService.GetDashboardDocumentList().ToList();
                    if (eventList != null && eventList.Count > 0)
                        dashBoard.dashbrdDocList = eventList;
                }

                return View(dashBoard);
            }
            else
            {
                return RedirectToAction("Index", "Login");
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
                    byte[] imageByteData = ImageService.GetImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Nafed.MicroPay.Common.DocumentUploadFilePath.ProfilePicFilePath + "/" + imgPath));
                    return File(imageByteData, "image/jpg;base64");
                }
                else
                {
                    byte[] imageByteData = ImageService.GetImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Nafed.MicroPay.Common.DocumentUploadFilePath.ProfilePicFilePath + "/DefaultUser.png"));
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

        public ActionResult GetMobileDashBoard(string accessKey)
        {
            log.Info($"HomeController/GetMobileDashBoard/");

            if (!string.IsNullOrEmpty(accessKey))
            {
                var userName = Decrypt(accessKey, cryptographyKey); ///==== mobile logged in  username...
                var uDetail = dashBoardService.GetUserInfo(userName);

                if (userName.Equals(uDetail.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    uDetail.DeviceTypeIsMobile = true;
                    uDetail.userMenuRights = menuService.GetUserMenuRights(uDetail.UserID, uDetail.DepartmentID, uDetail.UserTypeID);

                    Session["User"] = uDetail;
                    TempData["UserID"] = uDetail.UserID;
                    TempData["DepartmentID"] = uDetail.DepartmentID;
                    return RedirectToAction("Index", "Home");
                }
            }
            return Content("");
        }

        public ActionResult Menu()
        {
            return View();
        }
    }
}