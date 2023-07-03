using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Common;

using System.Data;

namespace Nafed.MicroPay.Services
{
    public class ActivityLogService : BaseService, IActivityLogRepository, IActivityLogService
    {
        private readonly IActivityLogRepository _activitylogrepository;

        public ActivityLogService(IActivityLogRepository activitylogrepository)
        {
            _activitylogrepository = activitylogrepository;
        }
        
        public DataTable GetActivityLog()
        {
            log.Info($"GetLeaveCategoryList");
            try
            {
                return _activitylogrepository.GetActivityLog();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }      

               
        }
    }
}
