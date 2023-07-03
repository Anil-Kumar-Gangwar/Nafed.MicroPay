using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MicroPay.Web.Attributes
{
    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string filePath = filterContext.Controller.TempData["filePath"].ToString();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                filterContext.Controller.TempData.Remove("filePath");
            }
        }

    }
}