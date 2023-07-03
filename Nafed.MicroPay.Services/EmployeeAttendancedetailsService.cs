using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Common;
using System.Data;
using Nafed.MicroPay.ImportExport.Interfaces;

namespace Nafed.MicroPay.Services
{
    public class EmployeeAttendancedetailsService : BaseService, IEmployeeAttendancedetailsService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeAttendanceRepository empAttendaceRepo;
        private readonly IExport export;
        public EmployeeAttendancedetailsService(IGenericRepository genericRepo, IEmployeeAttendanceRepository empAttendaceRepo, IExport export)
        {
            this.genericRepo = genericRepo;
            this.empAttendaceRepo = empAttendaceRepo;
            this.export = export;
        }

        public List<EmpAttendance> GetAttendanceDetails(Model.EmpAttendance tabletProxyAttendanceDetails)
        {
            log.Info($"MarkAttendanceService/GetAttendanceForm/");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>()
                    .ForMember(c => c.EmpAttendanceID, c => c.MapFrom(s => s.EmpAttendanceID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                    .ForMember(c => c.InTime, c => c.MapFrom(s => s.InTime))
                    .ForMember(c => c.OutTime, c => c.MapFrom(s => s.OutTime))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.Attendancestatus, c => c.MapFrom(s => s.Attendancestatus))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoModel = Mapper.Map<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>(tabletProxyAttendanceDetails);

                var listproxyAttendance = empAttendaceRepo.GetAttendanceDetails(dtoModel);

                var getList = listproxyAttendance.Select(x => new Model.EmpAttendance()
                {
                    EmployeeId = x.EmployeeId,
                    EmpAttendanceID = x.EmpAttendanceID,
                    ProxydateIn = x.ProxydateIn,
                    ProxyOutDate = x.ProxyOutDate,
                    InTime = x.InTime,
                    OutTime = x.OutTime,
                    Remarks = x.Remarks,
                    Attendancestatus = (int)x.EmpAttendanceHdr.Attendancestatus

                });

                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<DTOModel.EmpAttendanceHdr, Model.EmpAttendance>()
                //    .ForMember(c => c.EmpAttendanceID, c => c.MapFrom(s => s.EmpAttendanceID))
                //    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                //    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                //    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                //    .ForMember(c => c.InTime, c => c.MapFrom(s => s.InTime))
                //    .ForMember(c => c.OutTime, c => c.MapFrom(s => s.OutTime))
                //    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                //    .ForMember(c => c.Attendancestatus, c => c.MapFrom(s => s.Attendancestatus))
                //    .ForAllOtherMembers(c => c.Ignore());
                //});

                //var attendanceList = Mapper.Map<List<DTOModel.EmpAttendanceHdr>, List<Model.EmpAttendance>>(listproxyAttendance);
                return getList.ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<EmployeeAttendanceDetails> GetEmployeeAttendanceList(int? branchId, int? employeeID, string month, string year, int usertypeID, int EmployeeID,int? departmentId=null)
        {

            var attendanceList = new List<Model.EmployeeAttendanceDetails>();
            DataTable result = null;
            try
            {
                log.Info($"EmployeeAttendancedetailsService/GetEmployeeAttendanceList");
                log.Info($"month:{month},year:{year}");
                //if (usertypeID != (int)UserType.Employee)
                //{
                int day = DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month));
                result = empAttendaceRepo.GetEmployeeAttendanceList(branchId > 0 ? branchId : null, employeeID > 0 ? employeeID : null, (month != "0" ? month : DateTime.Now.Month.ToString()), (year != "0" ? year : DateTime.Now.Year.ToString()), day.ToString(), 3,departmentId);
                //  }
                //else if (usertypeID == (int)UserType.Employee)
                //{
                //    int day = DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month));
                //    result = empAttendaceRepo.GetEmployeeAttendanceList(branchId > 0 ? branchId : 0, employeeID > 0 ? employeeID : EmployeeID, (month != "0" ? month : DateTime.Now.Month.ToString()), (year != "0" ? year : DateTime.Now.Year.ToString()), day.ToString(), 1);
                //}
                attendanceList = Common.ExtensionMethods.ConvertToList<EmployeeAttendanceDetails>(result);

                #region ==== Append InTime /Out Time List for ===============

                if (attendanceList?.Count > 0)
                {
                    var InTimeOutTimeMatrix =
                        empAttendaceRepo.GetEmpAttendanceInTimeOutTime(
                            branchId.Value, employeeID,
                          month,
                        year);

                    var list = Common.ExtensionMethods.ConvertToList<MyAttendanceDetails>(InTimeOutTimeMatrix);

                    attendanceList.ForEach(x =>
                    {

                        x.InTimeOutTimes = list.Where(x1 => x1.EmployeeId == x.EmployeeId).ToList();
                    });

                }
                #endregion



            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return attendanceList;
        }

        public List<MyAttendanceDetails> GetMyAttendanceList(int branchId, int employeeID, string month, string year, int usertypeID, int LoggedInEmployeeID)
        {

            var attendanceList = new List<Model.MyAttendanceDetails>();
            DataTable result = null;
            try
            {

                log.Info($"EmployeeAttendancedetailsService/GetEmployeeAttendanceList");
                if (usertypeID == (int)UserType.Employee)
                {
                    int day = (int)DateTime.Now.Day;
                    result = empAttendaceRepo.GetEmployeeAttendanceList(branchId > 0 ? branchId : 0, employeeID > 0 ? employeeID : LoggedInEmployeeID, (month != "0" ? month : DateTime.Now.Month.ToString()), (year != "0" ? year : DateTime.Now.Year.ToString()), day.ToString(), 1);
                }
                attendanceList = Common.ExtensionMethods.ConvertToList<MyAttendanceDetails>(result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return attendanceList;
        }

        public List<EmpAttendance> GetEmployeeAttendanceByManagerID(Model.EmpAttendance tabletProxyAttendanceDetails, out int attendanceCount)
        {
            log.Info($"MarkAttendanceService/AttendanceDetailsManagerWise");
            try
            {

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>()
                    .ForMember(c => c.EmpAttendanceID, c => c.MapFrom(s => s.EmpAttendanceID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.ProxydateIn, c => c.MapFrom(s => s.ProxydateIn))
                    .ForMember(c => c.ProxyOutDate, c => c.MapFrom(s => s.ProxyOutDate))
                    .ForMember(c => c.InTime, c => c.MapFrom(s => s.InTime))
                    .ForMember(c => c.OutTime, c => c.MapFrom(s => s.OutTime))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.Attendancestatus, c => c.MapFrom(s => s.Attendancestatus))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoModel = Mapper.Map<Model.EmpAttendance, DTOModel.EmpAttendanceHdr>(tabletProxyAttendanceDetails);

                var listproxyAttendance = empAttendaceRepo.GetEmployeeAttendanceByManagerID(dtoModel, 4);

                var getList = listproxyAttendance.Select(x => new Model.EmpAttendance()
                {
                    EmployeeId = x.EmployeeId,
                    EmpAttendanceID = x.EmpAttendanceID,
                    ProxydateIn = x.ProxydateIn,
                    ProxyOutDate = x.ProxyOutDate,
                    InTime = x.InTime,
                    OutTime = x.OutTime,
                    Remarks = x.Remarks,
                    Attendancestatus = (int)x.EmpAttendanceHdr.Attendancestatus,
                    EmpAttendanceApprovalSettings = x.tblMstEmployee.EmployeeProcessApprovals.Where(y => y.ProcessID == 4 && y.ToDate == null)
                    .Select(em => new EmployeeProcessApproval()
                    {
                        ReportingTo = em.ReportingTo,
                        ReviewingTo = em.ReviewingTo,
                        AcceptanceAuthority = em.AcceptanceAuthority,
                        EmployeeID = em.EmployeeID,
                        ProcessID = em.ProcessID
                    }).FirstOrDefault(),
                    EmployeeName = x.tblMstEmployee.Name,
                    ReportingToRemark = x.EmpAttendanceHdr.ReportingToRemark,
                    ReviewerToRemark = x.EmpAttendanceHdr.ReviewerToRemark,
                    AcceptanceAuthRemark = x.EmpAttendanceHdr.AcceptanceAuthRemark

                });

                attendanceCount = getList.Where(x => x.Attendancestatus == (int)EmpAttendanceStatus.Pending).Count();
                //   var ss = getList.Where(x => x.Attendancestatus == (int)EmpAttendanceStatus.Pending);
                return getList.OrderBy(x => x.Attendancestatus).ThenBy(x => x.ProxydateIn).ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.EmployeeAttendanceDetails> GetReportingTo(int ReportingTo)
        {
            try
            {

                //var dtoRegularEmployess = genericRepo.Get<Data.Models.tblMstEmployee>(x => !x.IsDeleted && x.ReportingTo == ReportingTo);

                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<DTOModel.tblMstEmployee, Model.EmployeeAttendanceDetails>()
                //    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))

                //   .ForAllOtherMembers(c => c.Ignore());
                //});
                //var reportingTO = Mapper.Map<List<Model.EmployeeAttendanceDetails>>(dtoRegularEmployess);

                //return reportingTO.OrderBy(x => x.EmployeeId).ToList();

                return null;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool ExportAttendance(DataSet dsSource, string sFullPath, string fileName)
        {
            try
            {
                var flag = false;
                sFullPath = $"{sFullPath}{fileName}";
                flag = export.ExportToExcel(dsSource, sFullPath, fileName);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public int getAttendancemanually(DateTime fromdate, DateTime todate)
        {
            int flag = 0;
            log.Info($"EmployeeAttendancedetailsService/getAttendancemanually");
            try
            {
                flag = empAttendaceRepo.getAttendancemanually(fromdate, todate);
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
