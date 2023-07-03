using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Common;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
namespace Nafed.MicroPay.Services
{
    public class BaseService
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Dispose()
        {

        }

        protected bool AddAppraisalFormHeader(AppraisalFormHdr appFormHdr)
        {
            log.Info($"BaseService/AddAppraisalFormHeader");
            try
            {
                Data.Models.AppraisalFormHdr dtoAppFormHdr = new Data.Models.AppraisalFormHdr();
                IGenericRepository genericRepo = new GenericRepository();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<AppraisalFormHdr, Data.Models.AppraisalFormHdr>();
                });
                dtoAppFormHdr = Mapper.Map<Data.Models.AppraisalFormHdr>(appFormHdr);
                genericRepo.Insert<Data.Models.AppraisalFormHdr>(dtoAppFormHdr);
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public virtual bool AddProcessWorkFlow(ProcessWorkFlow workFlow)
        {
            try
            {
                log.Info($"BaseService/AddProcessWorkFlow");
                Data.Models.ProcessWorkFlow dtoWorkFlow = new Data.Models.ProcessWorkFlow();
                IGenericRepository genericRepo = new GenericRepository();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<ProcessWorkFlow, Data.Models.ProcessWorkFlow>();
                });
                dtoWorkFlow = Mapper.Map<Data.Models.ProcessWorkFlow>(workFlow);

                if (dtoWorkFlow.ReceiverID.HasValue && dtoWorkFlow.ReceiverID.Value > 0)
                {
                    var receiverDtls = genericRepo.GetByID<Data.Models.tblMstEmployee>(dtoWorkFlow.ReceiverID.Value);
                    dtoWorkFlow.ReceiverDesignationID = receiverDtls?.DesignationID ?? null;
                    dtoWorkFlow.ReceiverDepartmentID = receiverDtls?.DepartmentID ?? null;
                }
                genericRepo.Insert<Data.Models.ProcessWorkFlow>(dtoWorkFlow);
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }

        }
        public virtual EmployeeProcessApproval GetProcessApprovalSetting(int employeeID, WorkFlowProcess wrkProcess)
        {
            log.Info($"EmployeeService/GetEmpApprovalProcesses/employeeID={employeeID}");
            try
            {
                EmployeeProcessApproval empProcessApproval = new EmployeeProcessApproval();

                IGenericRepository genericRepo = new GenericRepository();

                var dtoEmpProcessModel = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => x.EmployeeID == employeeID && x.ToDate == null && x.ProcessID == (int)wrkProcess).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeProcessApproval, Model.EmployeeProcessApproval>()
                    .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.ReportingTo))
                    .ForMember(d => d.ReviewingTo, o => o.MapFrom(s => s.ReviewingTo))
                    .ForMember(d => d.AcceptanceAuthority, o => o.MapFrom(s => s.AcceptanceAuthority))
                    .ForAllOtherMembers(d => d.Ignore())
                    ;

                });
                empProcessApproval = Mapper.Map<Model.EmployeeProcessApproval>(dtoEmpProcessModel);

                return empProcessApproval;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public virtual List<SelectListModel> GetProcessApprovalManagers(int employeeID, WorkFlowProcess wrkProcess)
        {
            log.Info($"EmployeeService/GetEmpApprovalProcesses/employeeID={employeeID}");
            try
            {


                IGenericRepository genericRepo = new GenericRepository();

                var dtoEmpProcessModel = genericRepo.Get<DTOModel.EmployeeProcessApproval>(x => x.EmployeeID == employeeID && x.ToDate == null && x.ProcessID == (int)wrkProcess).FirstOrDefault()

                    ;
                if (dtoEmpProcessModel != null)
                {
                    var employess = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == dtoEmpProcessModel.ReportingTo
                || x.EmployeeId == dtoEmpProcessModel.ReviewingTo || x.EmployeeId == dtoEmpProcessModel.AcceptanceAuthority).Select(x => new SelectListModel()
                {

                    id = (x.EmployeeId == dtoEmpProcessModel.ReportingTo ? 1 : x.EmployeeId == dtoEmpProcessModel.ReviewingTo ? 2 :
                   x.EmployeeId == dtoEmpProcessModel.AcceptanceAuthority ? 3 : 0)
                   ,
                    value = x.Name
                }).ToList();
                    return employess;
                }
                else
                {
                    return new List<SelectListModel>();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Notification
        public async Task FirebaseTopicNotification(PushNotification notification)
        {
            try
            {
                string SERVER_API_KEY = "AAAAMBRPDG4:APA91bHUZeRpsFxapBel9NGfaiN-1WdqKKGopUTMqRbkI8gJbjq4u025dY0RsyHLQLYXQmtsDNELjra_oJV7MxDMVMFuUL7F7aaCRDFY--AsvUnEma9bP_2_Tlgo2Vh0OH1Cl8kKZ5al";
                var SENDER_ID = "206499155054";
                //var value = message;
                List<string> topic = new List<string>();
                topic.Add(notification.UserName.ToUpper());
                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach (var item in topic)
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    //serverKey - Key from Firebase cloud messaging server  
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
                    //Sender Id - From firebase project setting  
                    tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                    tRequest.ContentType = "application/json";
                    var payload = new
                    {
                        to = "/topics/" + Convert.ToString(item),
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            body = notification.Message,
                            title = notification.Title,
                            badge = 1
                        },
                        data = data

                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    tRequest.ContentType = "application/json";
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = await tRequest.GetResponseAsync())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        public bool InsertInsufficientLoggingMonitoring(InsufficientLoggingMonitoring insufficientLoggingMonitoring)
        {
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.InsufficientLoggingMonitoring, DTOModel.InsufficientLoggingMonitoring>()
                    .ForMember(c => c.UserName, c => c.MapFrom(m => m.UserName))
                    .ForMember(c => c.SessionId, c => c.MapFrom(m => m.SessionId))
                    .ForMember(c => c.IP_address, c => c.MapFrom(m => m.IP_address))
                    .ForMember(c => c.DateTime, c => c.MapFrom(m => m.DateTime))
                   .ForMember(c => c.Referrer, c => c.MapFrom(m => m.Referrer))
                   .ForMember(c => c.ProcessId, c => c.MapFrom(m => m.ProcessId))
                   .ForMember(c => c.URL, c => c.MapFrom(m => m.URL))
                   .ForMember(c => c.UserAgent, c => c.MapFrom(m => m.UserAgent))
                   .ForMember(c => c.Country, c => c.MapFrom(m => m.Country))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoInsufficientLoggingMonitoring = Mapper.Map<DTOModel.InsufficientLoggingMonitoring>(insufficientLoggingMonitoring);
                IGenericRepository genericRepo = new GenericRepository();
                genericRepo.Insert<DTOModel.InsufficientLoggingMonitoring>(dtoInsufficientLoggingMonitoring);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        
    }
}
