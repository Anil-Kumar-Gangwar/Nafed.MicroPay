using AutoMapper;
using MicroPay.Web.Models;
using Nafed.MicroPay.Common;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers
{
    public class ActivityLogController : BaseController
    {
        private readonly IActivityLogService _activitylogservice;

        public ActivityLogController(IActivityLogService activitylogservice)
        {
            _activitylogservice = activitylogservice;
        }

        // GET: ActivityLog
        public ActionResult Index()
        {
          DataTable dt =  _activitylogservice.GetActivityLog();
            
            List <ActivityLog> activityloglist = ExtensionMethods.ConvertToList<ActivityLog>(dt);
            
            return View(activityloglist);
        }
    }
}