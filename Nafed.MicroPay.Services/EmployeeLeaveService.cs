using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Nafed.MicroPay.Model;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using AutoMapper;
using System.Data;
using Nafed.MicroPay.Common;
using System.IO;
using static Nafed.MicroPay.ImportExport.LeaveForm;
using Nafed.MicroPay.ImportExport.Interfaces;
using System.Threading.Tasks;
using System.Text;
using System.Web;

namespace Nafed.MicroPay.Services
{
    public class EmployeeLeaveService : BaseService, IEmployeeLeaveService
    {
        private readonly IGenericRepository genericRepo;
        private readonly ILeaveRepository empLeaveRepo;
        private readonly IExport export;

        public EmployeeLeaveService(IGenericRepository genericRepo, ILeaveRepository empLeaveRepo, IExport export)
        {
            this.genericRepo = genericRepo;
            this.empLeaveRepo = empLeaveRepo;
            this.export = export;
        }

        public List<Model.LeaveCategory> GetLeaveCategoryGuidlines(int leaveCategoryID)
        {
            log.Info($"EmployeeLeaveService/GetLeaveCategoryGuidlines");
            try
            {
                var getLeaveCatGuidlines = genericRepo.Get<DTOModel.LeaveCategory>(em => em.LeaveCategoryID == leaveCategoryID && !em.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeaveCategory, Model.LeaveCategory>()
                     .ForMember(d => d.leaveGuidelines, o => o.MapFrom(s => s.leaveGuidelines))
                     .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<List<Model.LeaveCategory>>(getLeaveCatGuidlines);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<EmployeeLeave> GetEmployeeLeaveList(Model.EmployeeLeave empLeave)
        {
            log.Info($"EmployeeLeaveService/GetEmployeeLeaveList");

            try
            {

                var result = genericRepo.Get<Data.Models.EmployeeLeave>(x => x.EmployeeId == empLeave.EmployeeId && !x.IsDeleted
                //  && x.StatusID == (empLeave.StatusID > 0 ? empLeave.StatusID : x.StatusID)

                && (empLeave.StatusID > 0 ? (x.StatusID == empLeave.StatusID) : 1 > 0)
                );
                if (empLeave.LeaveCategoryID > 0)
                {
                    result = result.Where(x => x.LeaveCategoryID == empLeave.LeaveCategoryID);
                }
                if (empLeave.DateFrom.HasValue && empLeave.DateTo.HasValue)
                {
                    result = result.Where(x => x.DateFrom >= empLeave.DateFrom.Value && x.DateTo <= empLeave.DateTo.Value);
                }

                //var result = genericRepo.Get<Data.Models.EmployeeLeave>(x => x.IsDeleted == false);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeLeave, Model.EmployeeLeave>()
                    .ForMember(c => c.LeaveID, c => c.MapFrom(s => s.LeaveID))
                    .ForMember(c => c.LeaveCategoryName, c => c.MapFrom(s => s.LeaveCategory.LeaveCategoryName))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.TitleName, c => c.MapFrom(s => s.tblMstEmployee.Title.TitleName))
                    .ForMember(c => c.EmployeeName, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.DateFrom, c => c.MapFrom(s => s.DateFrom))
                    .ForMember(c => c.DateTo, c => c.MapFrom(s => s.DateTo))
                    .ForMember(c => c.Unit, c => c.MapFrom(s => s.Unit))
                    .ForMember(c => c.Reason, c => c.MapFrom(s => s.Reason))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.StatusName, c => c.MapFrom(s => s.LeaveStatu.StatusDesc))
                    ;
                });
                var listGrade = Mapper.Map<List<Model.EmployeeLeave>>(result);
                return listGrade.OrderByDescending(x => x.CreatedOn).ThenBy(x => x.StatusID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool LeaveExists(int? leaveID, DateTime DateFrom, DateTime DateTo)
        {
            return genericRepo.Exists<Data.Models.EmployeeLeave>(x => x.LeaveID != leaveID && x.DateFrom == DateFrom && x.DateTo == DateTo && x.IsDeleted == false);
        }
        public bool UpdateEmployeeLeave(Model.EmployeeLeave editEmployeeLeaveEntity)
        {
            log.Info($"EmployeeLeaveService/UpdateEmployeeLeave");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<Data.Models.EmployeeLeave>(editEmployeeLeaveEntity.LeaveID);

                dtoObj.DateFrom = editEmployeeLeaveEntity.DateFrom;
                dtoObj.DateTo = editEmployeeLeaveEntity.DateTo;
                dtoObj.LeaveCategoryID = editEmployeeLeaveEntity.LeaveCategoryID;

                dtoObj.Reason = editEmployeeLeaveEntity.Reason;
                dtoObj.Unit = editEmployeeLeaveEntity.Unit;
                dtoObj.LeaveBalance = editEmployeeLeaveEntity.LeaveBalance;

                dtoObj.DocumentName = editEmployeeLeaveEntity.DocumentName;
                dtoObj.LeavePurposeID = editEmployeeLeaveEntity.LeavePurposeID;
                dtoObj.UpdatedBy = editEmployeeLeaveEntity.UpdatedBy;
                dtoObj.UpdatedOn = editEmployeeLeaveEntity.UpdatedOn;

                genericRepo.Update<Data.Models.EmployeeLeave>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public int InsertEmployeeLeave(Model.EmployeeLeave createEmployeeLeave)
        {
            log.Info($"EmployeeLeaveService/InsertEmployeeLeave");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmployeeLeave, Data.Models.EmployeeLeave>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.DateFrom, c => c.MapFrom(s => s.DateFrom))
                    .ForMember(c => c.DateTo, c => c.MapFrom(s => s.DateTo))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.Reason, c => c.MapFrom(s => s.Reason))
                    .ForMember(c => c.Unit, c => c.MapFrom(s => s.Unit))
                    .ForMember(c => c.ReportingOfficer, c => c.MapFrom(s => s.ReportingOfficer))
                    .ForMember(c => c.DocumentName, c => c.MapFrom(s => s.DocumentName))
                    .ForMember(c => c.LeavePurposeID, c => c.MapFrom(s => s.LeavePurposeID))
                    .ForMember(c => c.LeavePurposeID, c => c.MapFrom(s => s.LeavePurposeID))
                    .ForMember(c => c.PurposeOthers, c => c.MapFrom(s => s.PurposeOthers))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.ReviewerTo, c => c.MapFrom(s => s.ReviewerTo))
                    .ForMember(c => c.DateFrom_DayType, c => c.MapFrom(s => s.DateFrom_DayType))
                   .ForMember(c => c.DateTo_DayType, c => c.MapFrom(s => s.DateTo_DayType))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoemployeeLeave = Mapper.Map<Data.Models.EmployeeLeave>(createEmployeeLeave);
                genericRepo.Insert<Data.Models.EmployeeLeave>(dtoemployeeLeave);
                createEmployeeLeave._ProcessWorkFlow.ReferenceID = dtoemployeeLeave.LeaveID;
                AddProcessWorkFlow(createEmployeeLeave._ProcessWorkFlow);

                return dtoemployeeLeave.LeaveID;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateEmployeeLeaveDocument(Model.EmployeeLeave editEmployeeLeaveEntity)
        {
            log.Info($"EmployeeLeaveService/UpdateEmployeeLeave");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<Data.Models.EmployeeLeave>(editEmployeeLeaveEntity.LeaveID);

                dtoObj.DocumentName = editEmployeeLeaveEntity.DocumentName;
                dtoObj.UpdatedBy = editEmployeeLeaveEntity.UpdatedBy;
                dtoObj.UpdatedOn = editEmployeeLeaveEntity.UpdatedOn;

                genericRepo.Update<Data.Models.EmployeeLeave>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public Model.EmployeeLeave GetEmployeeLeaveByID(int leaveID)
        {
            log.Info($"EmployeeLeaveService/GetEmployeeLeaveByID {leaveID}");
            try
            {
                var EmpLeaveObj = genericRepo.GetByID<Data.Models.EmployeeLeave>(leaveID);
                Mapper.Initialize((Action<IMapperConfigurationExpression>)(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeLeave, Model.EmployeeLeave>()

                    .ForMember(c => c.DateFrom, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, DateTime?> c) => c.MapFrom(m => m.DateFrom))
                      .ForMember(c => c.DateTo, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, DateTime?> c) => c.MapFrom(m => m.DateTo))
                     .ForMember(c => c.LeaveCategoryID, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, int> c) => c.MapFrom(m => m.LeaveCategoryID))
                     .ForMember(c => c.StatusID, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, int> c) => c.MapFrom(m => m.StatusID))
                     .ForMember(c => c.Reason, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, string> c) => c.MapFrom(m => m.Reason))
                     .ForMember(c => c.Unit, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, decimal> c) => c.MapFrom(m => m.Unit))
                    .ForMember((System.Linq.Expressions.Expression<Func<EmployeeLeave, int?>>)(c => (int?)c.ReportingOfficer), (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, int?> c) => c.MapFrom(m => m.ReportingOfficer))
                     .ForMember(c => c.DocumentName, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, string> c) => c.MapFrom(m => m.DocumentName))
                     .ForMember(c => c.LeavePurposeID, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, int> c) => c.MapFrom(m => m.LeavePurposeID))
                    .ForMember(c => c.PurposeOthers, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, string> c) => c.MapFrom(m => m.PurposeOthers))
                    .ForMember(c => c.DateFrom_DayType, c => c.MapFrom(m => m.DateFrom_DayType))
                    .ForMember(c => c.DateTo_DayType, c => c.MapFrom(m => m.DateTo_DayType))
                    .ForMember(c => c.IsDeleted, (IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, bool> c) => c.MapFrom(m => m.IsDeleted))
                    .ForAllOtherMembers((IMemberConfigurationExpression<Data.Models.EmployeeLeave, EmployeeLeave, object> c) => c.Ignore());
                }));
                var obj = Mapper.Map<Data.Models.EmployeeLeave, Model.EmployeeLeave>(EmpLeaveObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //public bool WithdrawlLeave(int leaveID, int empleavetransID, Model.ProcessWorkFlow workFlow)
        //{
        //    log.Info($"EmployeeLeaveService/Delete{leaveID}");
        //    bool flag = false;
        //    try
        //    {
        //        Data.Models.EmployeeLeave dtoEmployeeLeave = new Data.Models.EmployeeLeave();
        //        dtoEmployeeLeave = genericRepo.GetByID<Data.Models.EmployeeLeave>(leaveID);
        //        dtoEmployeeLeave.StatusID = (int)EmpLeaveStatus.Withdrawl;
        //        dtoEmployeeLeave.Unit = 0;
        //        //  dtoEmployeeLeave.tblLeaveTrans.FirstOrDefault(x=>x.EmpLeaveID== leaveID).Unit= 
        //        genericRepo.Update<Data.Models.EmployeeLeave>(dtoEmployeeLeave);

        //        //// delete from leaveTrans
        //        Data.Models.tblLeaveTran dtoEmployeeLeaveTrans = new Data.Models.tblLeaveTran();
        //        dtoEmployeeLeaveTrans = genericRepo.GetByID<Data.Models.tblLeaveTran>(empleavetransID);
        //        workFlow.ReferenceID = dtoEmployeeLeaveTrans.EmpLeaveID;
        //        genericRepo.Delete<Data.Models.tblLeaveTran>(dtoEmployeeLeaveTrans);

        //        AddProcessWorkFlow(workFlow);
        //        flag = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //    return flag;
        //}


        public bool WithdrawlLeave(int leaveID, int empleavetransID, Model.ProcessWorkFlow workFlow)
        {
            log.Info($"EmployeeLeaveService/Delete{leaveID}");
            bool flag = false;
            try
            {
                Data.Models.EmployeeLeave dtoEmployeeLeave = new Data.Models.EmployeeLeave();
                dtoEmployeeLeave = genericRepo.GetByID<Data.Models.EmployeeLeave>(leaveID);
                dtoEmployeeLeave.StatusID = (int)EmpLeaveStatus.Withdrawl;
                //dtoEmployeeLeave.Unit = 0;
                //  dtoEmployeeLeave.tblLeaveTrans.FirstOrDefault(x=>x.EmpLeaveID== leaveID).Unit=
                genericRepo.Update<Data.Models.EmployeeLeave>(dtoEmployeeLeave);

                //// delete from leaveTrans
                Data.Models.tblLeaveTran dtoEmployeeLeaveTrans = new Data.Models.tblLeaveTran();
                dtoEmployeeLeaveTrans = genericRepo.GetByID<Data.Models.tblLeaveTran>(empleavetransID);
                workFlow.ReferenceID = dtoEmployeeLeaveTrans.EmpLeaveID;
                dtoEmployeeLeaveTrans.StatusID = (int)EmpLeaveStatus.Withdrawl;
                ///genericRepo.Delete<Data.Models.tblLeaveTran>(dtoEmployeeLeaveTrans);
                genericRepo.Update<Data.Models.tblLeaveTran>(dtoEmployeeLeaveTrans);
                AddProcessWorkFlow(workFlow);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool LeaveDataExists(int EmployeeID, DateTime DateFrom, DateTime DateTo, int? leaveCategoryID = null)
        {
            return genericRepo.Exists<Data.Models.EmployeeLeave>(x => x.EmployeeId == EmployeeID &&
            (leaveCategoryID.Value == 18 ? (((DateFrom >= x.DateFrom && DateFrom <= x.DateTo) || (DateTo >= x.DateFrom && DateTo <= x.DateTo)) && x.LeaveCategoryID == 18) : (((DateFrom >= x.DateFrom && DateFrom <= x.DateTo) || (DateTo >= x.DateFrom && DateTo <= x.DateTo))
            && x.StatusID != 6 && x.StatusID != 7 && x.StatusID != 4 && x.StatusID != 2 && x.LeaveCategoryID != 18)));
            // x.LeaveCategoryID  18 (ignore Leave Encashment category)
        }

        public List<Model.EmployeeLeave> GetEmployeeLeaveDetails(string employeecode, string year)
        {
            var Leavelist = new List<Model.EmployeeLeave>();
            DataTable result = null;
            try
            {
                int day = (int)DateTime.Now.Day;



                result = empLeaveRepo.GetEmployeeLeaveDetails(employeecode, year);
                Leavelist = Common.ExtensionMethods.ConvertToList<EmployeeLeave>(result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return Leavelist;
        }

        #region Export leave

        public string GetLeaveForm(int employeeID, int statusID, int leavecategoryID, string fromdate, string todate, string fileName, string sFullPath, string Name)
        {
            log.Info($"MarkAttendanceService/GetLeaveForm/{employeeID}");

            string result = string.Empty;
            if (Directory.Exists(sFullPath))
            {
                sFullPath = $"{sFullPath}{fileName}";
                var empleaveDTO = genericRepo.Get<Data.Models.EmployeeLeave>(x => x.EmployeeId == employeeID && !x.IsDeleted);


                if (statusID > 0)
                {
                    empleaveDTO = empleaveDTO.Where(x => x.StatusID == statusID);
                }
                if (leavecategoryID > 0)
                {
                    empleaveDTO = empleaveDTO.Where(x => x.LeaveCategoryID == leavecategoryID);
                }

                if (fromdate != "" && todate != "")
                {
                    empleaveDTO = empleaveDTO.Where(x => x.DateFrom >= Convert.ToDateTime(fromdate) && x.DateTo <= Convert.ToDateTime(todate));
                }

                var listEmployee = Getemployeedetails(employeeID);
                DataTable DTemployee = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(listEmployee);

                DataTable dtLeaveForm = new DataTable();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.EmployeeLeave, Model.EmployeeLeave>()
                    .ForMember(d => d.LeaveCategoryName, o => o.MapFrom(s => s.LeaveCategory))
                    .ForMember(d => d.LeaveType, o => o.MapFrom(s => s.LeaveCategory.LeaveCategoryName))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.DateFrom, o => o.MapFrom(s => s.DateFrom))
                    .ForMember(d => d.DateTo, o => o.MapFrom(s => s.DateTo))
                    .ForMember(d => d.Unit, o => o.MapFrom(s => s.Unit))
                    .ForMember(d => d.Reason, o => o.MapFrom(s => s.Reason))
                    .ForMember(d => d.StatusName, o => o.MapFrom(s => (s.StatusID == 1 ? "Pending" : (s.StatusID == 2 ? "InProcess" : (s.StatusID == 3 ? "RejectedByReportingOfficer" : (s.StatusID == 4 ? "RejectedByReviwerOfficer" : (s.StatusID == 5 ? "Approved" : "Withdrawl")))))))

                    .ForAllOtherMembers(d => d.Ignore());
                });
                var empList = Mapper.Map<List<Model.EmployeeLeave>>(empleaveDTO);

                var sno = 1;
                empList.ForEach(x =>
                {
                    x.Sno = sno;
                    sno++;
                });

                if (empList.Count > 0)
                {
                    var formData = empList.Select(x => new
                    {
                        x.Sno,
                        x.LeaveType,
                        x.CreatedOn,
                        x.DateFrom,
                        x.DateTo,
                        x.Unit,
                        x.Reason,
                        x.StatusName
                    }).ToList();

                    dtLeaveForm = Common.ExtensionMethods.ToDataTable(formData);
                }

                if (dtLeaveForm != null && dtLeaveForm.Rows.Count > 0)  //====== export attendance form if there is data =========
                {
                    dtLeaveForm.Columns[0].ColumnName = "#";

                    IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                    exportHdr = dtLeaveForm.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                    result = ExportToExcel(exportHdr, dtLeaveForm, "AttendanceForm", sFullPath, Name, DTemployee.Rows[0]["DesignationName"].ToString(), DTemployee.Rows[0]["EmployeeCode"].ToString());
                }
                else
                    result = "norec";
            }
            return result;
        }


        public List<Model.EmployeeLeave> Getemployeedetails(int employeeID)
        {
            try
            {

                var dtoEmployess = genericRepo.Get<Data.Models.tblMstEmployee>(x => x.EmployeeId == employeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblMstEmployee, Model.EmployeeLeave>()
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.EmployeeCode))
                    .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.Designation.DesignationName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var employeedetail = Mapper.Map<List<Model.EmployeeLeave>>(dtoEmployess);

                return employeedetail.OrderBy(x => x.EmployeeCode).ToList();


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        public List<Model.EmployeeLeave> GetUnitDetails(DateTime fromDate, DateTime toDate, int BranchID, int leavecategoryID, int EmployeeID, out string remark)
        {
            var Unit = new List<Model.EmployeeLeave>();
            remark = string.Empty;
            DataTable result = null;
            try
            {
                result = empLeaveRepo.getUnitDetails(fromDate, toDate, BranchID, leavecategoryID, EmployeeID);
                var total_leave_applied_unit = int.Parse(result.Rows[0][0].ToString().Split(new char[] { '|' })[0].ToString());
                remark = result.Rows[0][0].ToString().Split(new char[] { '|' })[1];

                DataTable dt = new DataTable();
                dt.Columns.Add("totalUnit", typeof(System.Int32)); DataRow dr = dt.NewRow();
                dr[0] = total_leave_applied_unit;
                dt.Rows.Add(dr);

                Unit = Common.ExtensionMethods.ConvertToList<EmployeeLeave>(dt);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return Unit;
        }

        public bool InsertLeaveTrans(Model.EmployeeLeave createEmployeeLeave, int LeaveID)
        {
            log.Info($"EmployeeLeaveService/InsertLeaveTrans");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.EmployeeLeave, Data.Models.tblLeaveTran>()
                    .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmployeeCode))
                    .ForMember(c => c.CurrDate, c => c.MapFrom(s => DateTime.Now))
                    .ForMember(c => c.LeaveType, c => c.MapFrom(s => s.LeaveType))
                    .ForMember(c => c.FromDate, c => c.MapFrom(s => s.DateFrom))
                    .ForMember(c => c.ToDate, c => c.MapFrom(s => s.DateTo))
                    .ForMember(c => c.Reason, c => c.MapFrom(s => s.Reason))
                    .ForMember(c => c.Unit, c => c.MapFrom(s => s.Unit))
                    .ForMember(c => c.TransactionType, c => c.MapFrom(s => "DR"))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => 1))
                    .ForMember(c => c.EmpLeaveID, c => c.MapFrom(s => LeaveID))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoemployeeLeave = Mapper.Map<Data.Models.tblLeaveTran>(createEmployeeLeave);
                genericRepo.Insert<Data.Models.tblLeaveTran>(dtoemployeeLeave);
                flag = true;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public List<Model.EmployeeLeave> checkMapLeave(int leavecategoryID, DateTime fromDate, int EmployeeID)
        {
            var mapleave = new List<Model.EmployeeLeave>();
            DataTable result = null;
            try
            {
                result = empLeaveRepo.checkMapLeave(leavecategoryID, fromDate, EmployeeID);
                mapleave = Common.ExtensionMethods.ConvertToList<EmployeeLeave>(result);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return mapleave;
        }

        public EmployeeLeave GetLeaveTransDetails(int LeaveID)
        {
            log.Info($"");
            try
            {
                var dtoLeaveTrans = genericRepo.Get<DTOModel.tblLeaveTran>(x => x.EmpLeaveID == LeaveID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblLeaveTran, Model.EmployeeLeave>()
                    .ForMember(d => d.LeaveID, o => o.MapFrom(s => s.EmpLeaveTransID))
                    .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeLeave.tblMstEmployee.EmployeeId))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.EmployeeLeave.tblMstEmployee.Name))
                    .ForMember(d => d.LeaveCategoryName, o => o.MapFrom(s => s.EmployeeLeave.LeaveCategory.LeaveCategoryName))
                    .ForMember(d => d.Reason, o => o.MapFrom(s => s.Reason))
                    .ForMember(d => d.DateFrom, o => o.MapFrom(s => s.FromDate))
                    .ForMember(d => d.DateTo, o => o.MapFrom(s => s.ToDate))
                    .ForMember(d => d.Unit, o => o.MapFrom(s => s.Unit))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var leaveTrans = Mapper.Map<Model.EmployeeLeave>(dtoLeaveTrans.FirstOrDefault());
                return leaveTrans;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool ExportLeaveBalanceAsOfNow(DataSet dsSource, string sFullPath, string fileName)
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


        private void GetEmailConfiguration(EmailMessage message, EmailConfiguration emailsetting)
        {
            try
            {
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                message.HTMLView = true;
                message.FriendlyName = "NAFED";
                EmailHelper.SendEmail(message);



            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SenderSendMail(int senderID, string leaveType, EmployeeLeave empleave, string mailType)
        {
            log.Info($"EmployeeLeaveService/SendMail");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                //var senderMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == senderID && !x.IsDeleted).FirstOrDefault().OfficialEmail;
                var senderMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == senderID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail
                }).FirstOrDefault();

                if (mailType == "LeaveApply")
                {
                    if (empleave.LeaveCategoryID == 18)
                    {
                        empleave.DateFrom = DateTime.Now.Date;
                        empleave.DateTo = DateTime.Now.Date.AddDays(Convert.ToInt32(empleave.Unit));
                    }
                    if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                    {
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear <b>" + empleave.EmployeeName + "</b>,</p>"
                   + "<p>Your Leave Application has been successfully applied.</p>"
                   + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Leave Type </td><td style='font-weight: bold'> {0} </td> </tr> <tr><td> Period </td> <td style='font-weight: bold'> {1} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {2} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{3}</td></tr>", leaveType, empleave.DateFrom.Value.ToShortDateString() + " To " + empleave.DateTo.Value.ToShortDateString(), empleave.Unit, empleave.Reason);
                        emailBody.AppendFormat("</Table>");

                    }
                    else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                    {
                        emailBody.AppendFormat("Dear Sir/Madam, Leave Application has been successfully applied."
                            + "" + leaveType + ", Period - " + empleave.DateFrom.Value.ToShortDateString() + " To " + empleave.DateTo.Value.ToShortDateString() + "");
                    }
                }
                else if (mailType == "Withdrawal")
                {
                    if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                    {
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear <b>" + empleave.EmployeeName + "</b>,</p>"
                  + "<p>Your Leave Application has been successfully withdrawal.</p>"
                  + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Leave Type </td><td style='font-weight: bold'> {0} </td> </tr> <tr><td> Period </td> <td style='font-weight: bold'> {1} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {2} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{3}</td></tr>", leaveType, empleave.DateFrom.Value.ToShortDateString() + " To " + empleave.DateTo.Value.ToShortDateString(), empleave.Unit, empleave.Reason);
                        emailBody.AppendFormat("</Table>");
                    }
                    else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                    {
                        emailBody.AppendFormat("Dear Sir/Madam, Leave Application has been successfully withdrawal."
                             + "" + leaveType + ", Period - " + empleave.DateFrom.Value.ToShortDateString() + " To " + empleave.DateTo.Value.ToShortDateString() + "");
                    }
                }

                if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                {
                    EmailConfiguration emailsetting = new EmailConfiguration();
                    var emailsettings = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmailConfiguration, Model.EmailConfiguration>();

                    });
                    emailsetting = Mapper.Map<Model.EmailConfiguration>(emailsettings);

                    EmailMessage message = new EmailMessage();
                    message.To = senderMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : Leave Application";
                    Task t1 = Task.Run(() => GetEmailConfiguration(message, emailsetting));
                }
                else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                {
                    SMSConfiguration smssetting = new SMSConfiguration();
                    var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                    });
                    smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);

                    Task t1 = Task.Run(() => SendMessageOnMobile(senderMail.MobileNo, emailBody.ToString(), smssetting));
                }

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool RecieverSendMail(int recieverID, string leaveType, EmployeeLeave empleave, string mailType)
        {
            log.Info($"EmployeeLeaveService/RecieverSendMail");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                // var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).FirstOrDefault().OfficialEmail;
                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail,
                    EmployeeCode = x.EmployeeCode
                }).FirstOrDefault();
                if (mailType == "LeaveApply")
                {
                    if (empleave.LeaveCategoryID == 18)
                    {
                        empleave.DateFrom = DateTime.Now.Date;
                        empleave.DateTo = DateTime.Now.Date.AddDays(Convert.ToInt32(empleave.Unit));
                    }
                    if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                    {
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
              + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Employee </td><td style='font-weight: bold'> {0} </td> </tr><tr><td> Leave Type </td><td style='font-weight: bold'> {1} </td> </tr> <tr><td> Period </td> <td style='font-weight: bold'> {2} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {3} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{4}</td></tr>", empleave.EmployeeCode + " - " + empleave.EmployeeName, leaveType, empleave.DateFrom.Value.ToShortDateString() + " To " + empleave.DateTo.Value.ToShortDateString(), empleave.Unit, empleave.Reason);
                        emailBody.AppendFormat("</Table>");
                        //emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>For details please refer to my request on your Dashboard. Please login to  HRMS portal. </p>");
                        emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>To view this request, Accept or Reject,Click on below link. </p>");
                        emailBody.AppendFormat("<br/>http://182.74.122.83/NafedHRMs/");
                        emailBody.AppendFormat("<div> <p>Regards, <br/> Name : " + empleave.EmployeeName + "-" + empleave.EmployeeCode + " <br/> Post : " + empleave.DesignationName + " </p> </div>");

                    }
                    else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                    {
                        emailBody.AppendFormat("Dear Sir/Madam, To view this request, Accept or Reject,Click on below link. http://182.74.122.83/NafedHRMs/"
                            + "" + leaveType + ", Period - " + empleave.DateFrom.Value.ToShortDateString() + " To " + empleave.DateTo.Value.ToShortDateString() + "");
                    }
                }

                if (!string.IsNullOrEmpty(recieverMail.EmployeeCode))
                {
                    PushNotification notification = new PushNotification
                    {
                        UserName = recieverMail.EmployeeCode,
                        Title = "Leave Approval",
                        Message = $"Dear Sir/Madam, This is to intimate that you have received the Leave Approval, for employee {empleave.EmployeeCode + " - " + empleave.EmployeeName} with period {empleave.DateFrom.Value.ToShortDateString() + " To " + empleave.DateTo.Value.ToShortDateString()}  for further evaluation."

                    };
                    Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                }

                if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                {
                    EmailConfiguration emailsetting = new EmailConfiguration();
                    var emailsettings = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmailConfiguration, Model.EmailConfiguration>();

                    });
                    emailsetting = Mapper.Map<Model.EmailConfiguration>(emailsettings);

                    EmailMessage message = new EmailMessage();
                    message.To = recieverMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : Leave Application";
                    Task t2 = Task.Run(() => GetEmailConfiguration(message, emailsetting));
                }
                else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                {
                    SMSConfiguration smssetting = new SMSConfiguration();
                    var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                    });
                    smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);
                    Task t1 = Task.Run(() => SendMessageOnMobile(recieverMail.MobileNo, emailBody.ToString(), smssetting));

                }
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool SendMessageOnMobile(string mobileNo, string message, SMSConfiguration smssetting)
        {
            try
            {
                string msgRecepient = mobileNo.Length == 10 ? "91" + mobileNo : mobileNo;
                SMSParameter sms = new SMSParameter();
                sms.MobileNo = msgRecepient;
                sms.Message = message;
                sms.URL = smssetting.SMSUrl;
                sms.User = smssetting.UserName;
                sms.Password = smssetting.Password;
                return SMSHelper.SendSMS(sms);
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }

        }

        public List<Model.EmployeeLeave> chkdateforLE(int EmployeeID)
        {
            var dateforEL = new List<Model.EmployeeLeave>();
            DataTable result = null;
            try
            {
                result = empLeaveRepo.chkdateforLE(EmployeeID);
                if (result.Rows.Count > 0 && result.Rows[0][0].ToString() != "")
                {
                    dateforEL = Common.ExtensionMethods.ConvertToList<EmployeeLeave>(result);
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dateforEL;
        }

        public List<EmployeeLeave> GetLeaveEncashForF_A(DateTime fromDate, DateTime toDate)
        {

            log.Info("EmployeeLeaveService/ExportLeaveEncashForF_A");
            var getdata = empLeaveRepo.GetLeaveEncashForF_A(fromDate, toDate);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.SP_GetEncashmentForF_A_Result, EmployeeLeave>()
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Unit, o => o.MapFrom(s => s.unit))
                .ForMember(d => d.CR, o => o.MapFrom(s => s.BasicSal))
                .ForMember(d => d.OB, o => o.MapFrom(s => Math.Round(s.DA.Value)))
                .ForMember(d => d.Bal, o => o.MapFrom(s => (s.BasicSal + s.DA)))
                .ForAllOtherMembers(d => d.Ignore());
            });

            var dtoLeaveEncash = Mapper.Map<List<EmployeeLeave>>(getdata);

            return dtoLeaveEncash;
        }

        public bool ExportLeaveEncashForF_A(DataTable dtTable, string sFullPath, string fileName, string tFilter)
        {
            try
            {
                var flag = false;

                IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                exportHdr = dtTable.Columns.Cast<System.Data.DataColumn>()
                    .Select(x => x.ColumnName).AsEnumerable<string>();
                sFullPath = $"{sFullPath}{fileName}";
                var res = ExportToExcel(exportHdr, dtTable, fileName, sFullPath, tFilter);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

    }
}
