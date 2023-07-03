using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services
{
   public class CRReportService: BaseService, ICRReportService
    {
        private readonly ICRReportRepository crReportRepo;
        private readonly IGenericRepository genericRepo;

        public CRReportService(ICRReportRepository crReportRepo, IGenericRepository genericRepo)
        {
            this.crReportRepo = crReportRepo;
            this.genericRepo = genericRepo;
        }
        public DataTable GetEmployeeDetails(string query)
        {
            log.Info($"CRReportService/GetEmployeeDetails");
            try
            {
                var result = crReportRepo.GetEmployeeDetails(query);
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
