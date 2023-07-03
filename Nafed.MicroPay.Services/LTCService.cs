using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using Nafed.MicroPay.Model;



namespace Nafed.MicroPay.Services
{
    public class LTCService : BaseService, ILTCService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IAppraisalRepository apprRepo;

        public LTCService(IGenericRepository genericRepo, IAppraisalRepository apprRepo)
        {
            this.genericRepo = genericRepo;
            this.apprRepo = apprRepo;
        }

        public List<Model.LTC> GetLTCList(int? employeeID)
        {
            log.Info($"GetLTCList");
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstLTC>(x => x.IsDeleted==false && (employeeID.HasValue ? x.EmployeeId == employeeID : (1 > 0)));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstLTC, Model.LTC>()
                    .ForMember(c => c.LTCID, c => c.MapFrom(s => s.LTCID))
                    .ForMember(c => c.LTCNo, c => c.MapFrom(s => s.LTCNo))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.WhetherSelf, c => c.MapFrom(s => s.WhetherSelf))
                    .ForMember(c => c.HomeTown, c => c.MapFrom(s => s.HomeTown))
                    .ForMember(c => c.WhereDetail, c => c.MapFrom(s => s.WhereDetail))
                    .ForMember(c => c.Distance, c => c.MapFrom(s => s.Distance))
                    .ForMember(c => c.InitialcalAmount, c => c.MapFrom(s => s.InitialcalAmount))
                    .ForMember(c => c.DateAvailLTC, c => c.MapFrom(s => s.DateAvailLTC))
                    .ForMember(c => c.DateofReturn, c => c.MapFrom(s => s.DateofReturn))
                    .ForMember(c => c.DateofApplication, c => c.MapFrom(s => s.DateofApplication))
                    .ForMember(c => c.TentativeAdvance, c => c.MapFrom(s => s.TentativeAdvance))
                    .ForMember(c => c.LTCBillAmt, c => c.MapFrom(s => s.LTCBillAmt))
                    .ForMember(c => c.Settlementdone, c => c.MapFrom(s => s.Settlementdone))
                    .ForMember(c => c.Natureofleave, c => c.MapFrom(s => s.Natureofleave))
                    .ForMember(c => c.Detail, c => c.MapFrom(s => s.Detail))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.LTCRefrenceNumber, c => c.MapFrom(s => s.LTCReferenceNumber))
                    .ForMember(d => d.approvalSetting, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.LTC && y.ToDate == null && y.EmployeeID == s.EmployeeId)))
                    ;

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                });
                var listLTC = Mapper.Map<List<Model.LTC>>(result);
                return listLTC.OrderBy(x => x.LTCID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public Model.LTC GetLTCByID(int LTCID, int employeeID)
        {
            log.Info($"GetLTCByID {LTCID}/{employeeID}");
            try
            {
                var LTCObj = apprRepo.GetLTDDetail(LTCID, employeeID); //genericRepo.GetByID<DTOModel.tblMstLTC>(LTCID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.GetLTCDetail_Result, Model.LTC>()
                    .ForMember(c => c.LTCID, c => c.MapFrom(s => s.LTCID))
                    .ForMember(c => c.LTCNo, c => c.MapFrom(s => s.LTCNo))
                    .ForMember(c => c.BranchId, c => c.MapFrom(s => s.BranchID))
                    .ForMember(c => c.Dependents, c => c.MapFrom(s => s.DependentsList))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.EmployeeCode))
                    .ForMember(c => c.Employeename, c => c.MapFrom(s => s.Name))
                    .ForMember(c => c.WhetherSelf, c => c.MapFrom(s => s.WhetherSelf))
                    .ForMember(c => c.BranchName, c => c.MapFrom(s => s.BranchName))
                    .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.DesignationName))
                     .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.EMPDepartment));
                });
                var obj = Mapper.Map<DTOModel.GetLTCDetail_Result, Model.LTC>(LTCObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetDependentlist(int EmployeeId)
        {
            log.Info($"LTCService/GetDependentlist/{EmployeeId}");
            try
            {

                var result = genericRepo.Get<DTOModel.EmployeeDependent>(x => x.EmployeeId == EmployeeId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeDependent, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.EmpDependentID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DependentName));
                });
                var employeeByDependent = Mapper.Map<List<Model.SelectListModel>>(result);
                return employeeByDependent.OrderBy(x => x.value).ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int GetLastLTCNo(int employeeID, bool? WhetherSelf)
        {
            if (WhetherSelf == false)
            {
                var lastLTCNo = genericRepo.Get<DTOModel.tblMstLTC>(x => x.EmployeeId == employeeID).FirstOrDefault();
                if (lastLTCNo != null)
                    return lastLTCNo.LTCNo;
                else
                    return 0;
            }
            else
            {
                var lastLTCNo = genericRepo.Get<DTOModel.tblMstLTC>(x => x.EmployeeId == employeeID && x.WhetherSelf == WhetherSelf).FirstOrDefault();
                if (lastLTCNo != null)
                    return lastLTCNo.LTCNo;
                else
                    return 0;
            }
        }

        public List<SelectListModel> GetAllEmployee()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted && x.DOLeaveOrg == null);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeId))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.EmployeeCode + "-" + s.Name));
                });
                var employeedetail = Mapper.Map<List<Model.SelectListModel>>(result);
                return employeedetail.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelectListModel> GetAlldependent(int? employeeID)
        {
            try
            {

                if (employeeID == 0)
                {
                    var result = genericRepo.Get<DTOModel.EmployeeDependent>(x => !x.IsDeleted);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmployeeDependent, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmpDependentID))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.DependentName + '-' + s.Relation.RelationName + '-' + Nafed.MicroPay.Common.ExtensionMethods.CalculateAge(s.DOB.Value) + " Yr(s)"));
                    });
                    var dependentdetail = Mapper.Map<List<Model.SelectListModel>>(result);
                    return dependentdetail.OrderBy(x => x.value).ToList();
                }
                else
                {
                    var result = genericRepo.Get<DTOModel.EmployeeDependent>(x => !x.IsDeleted && x.EmployeeId == employeeID);
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmployeeDependent, Model.SelectListModel>()
                        .ForMember(c => c.id, c => c.MapFrom(m => m.EmpDependentID))
                        .ForMember(d => d.value, o => o.MapFrom(s => s.DependentName + '-' + s.Relation.RelationName + '-' + Nafed.MicroPay.Common.ExtensionMethods.CalculateAge(s.DOB.Value) + " Yr(s)"));
                    });
                    var dependentdetail = Mapper.Map<List<Model.SelectListModel>>(result);
                    return dependentdetail.OrderBy(x => x.value).ToList();
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime GetLastDateOfreturn(int employeeID)
        {
            DateTime dt = new DateTime(0);
            int maxLTCNo = GetLastLTCNo(employeeID, true);
            var getLTCLastReturn = genericRepo.Get<DTOModel.tblMstLTC>(x => x.EmployeeId == employeeID && x.WhetherSelf == true && x.LTCNo == maxLTCNo && x.IsDeleted==false).FirstOrDefault();
            if (getLTCLastReturn != null)
                return getLTCLastReturn.DateofReturn.Value;
            else
                return dt;

        }

        //public double GetELLeaveBal(int employeeID, DateTime DateAvailLTC)
        //{
        //    var ElClBal = genericRepo.Get<DTOModel.tblLeaveBal>(x => x.EmployeeID == employeeID && x.LeaveYear == DateAvailLTC.Year.ToString()).FirstOrDefault();
        //    if (ElClBal != null)
        //        return ElClBal.EL.Value;
        //    else
        //        return 0;

        //}
        //public double GetCLLeaveBal(int employeeID, DateTime DateAvailLTC)
        //{
        //    var ElClBal = genericRepo.Get<DTOModel.tblLeaveBal>(x => x.EmployeeID == employeeID && x.LeaveYear == DateAvailLTC.Year.ToString()).FirstOrDefault();
        //    if (ElClBal != null)
        //        return ElClBal.CL.Value;
        //    else
        //        return 0;

        //}


        public int InsertLTC(Model.LTC createLTC, ProcessWorkFlow workFlow)
        {
            log.Info($"InsertLTC");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LTC, DTOModel.tblMstLTC>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchId))
                     .ForMember(c => c.WhetherSelf, c => c.MapFrom(s => s.WhetherSelf))
                    .ForMember(c => c.DependentsList, c => c.MapFrom(s => s.Dependents))
                    .ForMember(c => c.LTCNo, c => c.MapFrom(s => s.LTCNo))
                    .ForMember(c => c.HomeTown, c => c.MapFrom(s => s.HomeTown))
                    .ForMember(c => c.WhereDetail, c => c.MapFrom(s => s.WhereDetail))
                     .ForMember(c => c.Distance, c => c.MapFrom(s => s.Distance))
                    .ForMember(c => c.InitialcalAmount, c => c.MapFrom(s => s.InitialcalAmount))
                    .ForMember(c => c.DateAvailLTC, c => c.MapFrom(s => s.DateAvailLTC))
                     .ForMember(c => c.DateofReturn, c => c.MapFrom(s => s.DateofReturn))
                    .ForMember(c => c.DateofApplication, c => c.MapFrom(s => s.DateofApplication))
                     .ForMember(c => c.TentativeAdvance, c => c.MapFrom(s => s.TentativeAdvance))
                    .ForMember(c => c.LTCBillAmt, c => c.MapFrom(s => s.LTCBillAmt))
                     .ForMember(c => c.Settlementdone, c => c.MapFrom(s => s.Settlementdone))
                    .ForMember(c => c.Natureofleave, c => c.MapFrom(s => s.Natureofleave))
                     .ForMember(c => c.Detail, c => c.MapFrom(s => s.Detail))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                     .ForMember(c => c.SpouseOrg, c => c.MapFrom(s => s.SpouseOrg))
                     .ForMember(c => c.LeaveFrom, c => c.MapFrom(s => s.LeaveFrom))
                     .ForMember(c => c.LeaveTo, c => c.MapFrom(s => s.LeaveTo))
                     .ForMember(c => c.FormStatus, c => c.MapFrom(s => s.FormStatus))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLTC = Mapper.Map<DTOModel.tblMstLTC>(createLTC);
                genericRepo.Insert<DTOModel.tblMstLTC>(dtoLTC);
                if (dtoLTC.LTCID > 0)
                {
                    if (createLTC.FormStatus == (short)AppraisalFormState.SubmitedByEmployee)
                    {
                        workFlow.ReferenceID = dtoLTC.LTCID;
                        AddProcessWorkFlow(workFlow);
                    }
                }
                return dtoLTC.LTCID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }


        public int UpdateLTC(LTC createLTC, ProcessWorkFlow workFlow)
        {
            log.Info($"LTCService/UpdateLTC");
            int res = 0;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblMstLTC>(createLTC.LTCID);
                dtoObj.WhetherSelf = createLTC.WhetherSelf;
                dtoObj.DependentsList = createLTC.Dependents;
                dtoObj.LTCNo = createLTC.LTCNo;
                dtoObj.HomeTown = createLTC.HomeTown;
                dtoObj.WhereDetail = createLTC.WhereDetail;
                dtoObj.Distance = createLTC.Distance;
                dtoObj.InitialcalAmount = createLTC.InitialcalAmount;
                dtoObj.DateAvailLTC = DateTime.Parse(createLTC.DateAvailLTC.ToString());
                dtoObj.DateofReturn = createLTC.DateofReturn;
                dtoObj.DateofApplication = createLTC.DateofApplication;
                dtoObj.TentativeAdvance = createLTC.TentativeAdvance;
                dtoObj.LTCBillAmt = createLTC.LTCBillAmt;
                dtoObj.Settlementdone = createLTC.Settlementdone;
                dtoObj.Natureofleave = createLTC.Natureofleave;
                dtoObj.Detail = createLTC.Detail;
                dtoObj.UpdatedBy = createLTC.UpdatedBy;
                dtoObj.UpdatedOn = createLTC.UpdatedOn;
                dtoObj.FormStatus = createLTC.FormStatus;
                dtoObj.SpouseOrg = createLTC.SpouseOrg;
                dtoObj.LeaveFrom = createLTC.LeaveFrom;
                dtoObj.LeaveTo = createLTC.LeaveTo;
                dtoObj.SHComment = createLTC.SHComment;
                dtoObj.DHComment = createLTC.DHComment;
                dtoObj.DealingAssistant = createLTC.DealingAssistant;
                genericRepo.Update<DTOModel.tblMstLTC>(dtoObj);
                res = 1;
                if (res > 0)
                {
                    if (createLTC.FormStatus == (short)AppraisalFormState.SubmitedByEmployee || createLTC.FormStatus == (short)AppraisalFormState.SubmitedByReporting ||
                        createLTC.FormStatus == (short)AppraisalFormState.SubmitedByReviewer || createLTC.FormStatus == (short)AppraisalFormState.SubmitedByAcceptanceAuth)
                    {
                        workFlow.ReferenceID = createLTC.LTCID;
                        AddProcessWorkFlow(workFlow);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return res;
        }

        public bool DeleteLTC(int LTCID)
        {
            log.Info($"LTCService/Delete/{LTCID}");
            bool flag = false;
            try
            {
                DTOModel.tblMstLTC dtoLTC = new DTOModel.tblMstLTC();
                dtoLTC = genericRepo.GetByID<DTOModel.tblMstLTC>(LTCID);
                dtoLTC.IsDeleted = true;
                genericRepo.Update<DTOModel.tblMstLTC>(dtoLTC);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        //public int InsertEmployeeLeave(Model.LTC createLTC, double totaldays)
        //{
        //    log.Info($"LTCService/InsertEmployeeLeave");
        //    try
        //    {
        //        Mapper.Initialize(cfg =>
        //        {
        //            cfg.CreateMap<Model.LTC, Data.Models.EmployeeLeave>()
        //            .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
        //            .ForMember(c => c.DateFrom, c => c.MapFrom(s => s.DateAvailLTC))
        //            .ForMember(c => c.DateTo, c => c.MapFrom(s => s.DateofReturn))
        //            .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.Natureofleave))
        //            .ForMember(c => c.StatusID, c => c.MapFrom(s => 1))
        //            .ForMember(c => c.Reason, c => c.MapFrom(s => "These Leaves are of LTC"))
        //            .ForMember(c => c.Unit, c => c.MapFrom(s => totaldays))
        //            .ForMember(c => c.PurposeOthers, c => c.MapFrom(s => 10))
        //             .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
        //            .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
        //           .ForAllOtherMembers(c => c.Ignore());
        //        });
        //        var dtoemployeeLeave = Mapper.Map<Data.Models.EmployeeLeave>(createLTC);
        //        genericRepo.Insert<Data.Models.EmployeeLeave>(dtoemployeeLeave);
        //        return dtoemployeeLeave.LeaveID;

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}
        //public bool InsertLeaveTrans(Model.LTC createLTC, double totaldays,int leaveID)
        //{
        //    log.Info($"LTCService/InsertLeaveTrans");
        //    bool flag = false;
        //    try
        //    {
        //        Mapper.Initialize(cfg =>
        //        {
        //            cfg.CreateMap<Model.LTC, Data.Models.tblLeaveTran>()
        //            .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.Employeecode))
        //            .ForMember(c => c.CurrDate, c => c.MapFrom(s => DateTime.Now))
        //            .ForMember(c => c.LeaveType, c => c.MapFrom(s => "LTC"))
        //            .ForMember(c => c.FromDate, c => c.MapFrom(s => s.DateAvailLTC))
        //            .ForMember(c => c.ToDate, c => c.MapFrom(s => s.DateofReturn))
        //            .ForMember(c => c.Reason, c => c.MapFrom(s => "These Leaves are of LTC"))
        //            .ForMember(c => c.Unit, c => c.MapFrom(s => totaldays))
        //            .ForMember(c => c.TransactionType, c => c.MapFrom(s => "DR"))
        //            .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.Natureofleave))
        //            .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
        //            .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
        //            .ForMember(c => c.StatusID, c => c.MapFrom(s => 1))
        //            .ForMember(c => c.EmpLeaveID, c => c.MapFrom(s => leaveID))
        //           .ForAllOtherMembers(c => c.Ignore());
        //        });
        //        var dtoemployeeLeave = Mapper.Map<Data.Models.tblLeaveTran>(createLTC);
        //        genericRepo.Insert<Data.Models.tblLeaveTran>(dtoemployeeLeave);
        //        flag = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //    return flag;
        //}

        public IEnumerable<LTC> GetLTCListForApproval(LTC filters)
        {
            log.Info($"AppraisalFormService/GetSkillAssessmentFormHdr/");
            try
            {

                var ltcData = genericRepo.GetIQueryable<DTOModel.tblMstLTC>
                    (x => x.tblMstEmployee.EmployeeProcessApprovals
                       .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.LTC
                       && y.ToDate == null
                       && (y.ReportingTo == filters.loggedInEmpID
                       || y.ReviewingTo == filters.loggedInEmpID
                       || y.AcceptanceAuthority == filters.loggedInEmpID))
                );

                //if (filters != null)
                //    dtoLTC = ltcData.Where(x =>
                //    (filters.selectedEmployeeID == 0 ? (1 > 0) : x.EmployeeId == filters.selectedEmployeeID)).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstLTC, Model.LTC>()
                    .ForMember(d => d.LTCID, o => o.MapFrom(s => s.LTCID))
                    .ForMember(d => d.LTCNo, o => o.MapFrom(s => s.LTCNo))
                    .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.Employeecode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.Employeename, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.approvalSetting, o => o.MapFrom(s =>
                     s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y =>
                     y.ProcessID == (int)Common.WorkFlowProcess.LTC && y.ToDate == null && y.EmployeeID == s.EmployeeId)))
                    .ForMember(d => d.FormStatus, o => o.MapFrom(s => s.FormStatus))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();

                });
                return Mapper.Map<List<Model.LTC>>(ltcData.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region LTC Status Report
        public Model.LTCReport GetLTCReportByID(int LTCID, int employeeID)
        {
            log.Info($"GetLTCReportByID");
            try
            {
                var result = genericRepo.Get<DTOModel.tblMstLTC>(x => (bool)!x.IsDeleted && x.EmployeeId == employeeID && x.LTCID == LTCID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstLTC, Model.LTCReport>()
                    .ForMember(c => c.LTCID, c => c.MapFrom(s => s.LTCID))
                    .ForMember(c => c.LTCNo, c => c.MapFrom(s => s.LTCNo))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Employeecode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Employeename, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(c => c.DepartmentName, c => c.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(c => c.BranchName, c => c.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                    .ForMember(c => c.HomeTown, c => c.MapFrom(s => s.HomeTown))
                    .ForMember(c => c.HomeTownDetails, c => c.MapFrom(s => s.HomeTown == 1 ? "Home" : "AnyWhere In India"))
                    .ForMember(c => c.WhereDetail, c => c.MapFrom(s => s.WhereDetail))
                    .ForMember(c => c.DateAvailLTC, c => c.MapFrom(s => s.DateAvailLTC.ToString("dd-MM-yyyy")))
                    .ForMember(c => c.StartDate, c => c.MapFrom(s => s.DateAvailLTC))
                    .ForMember(c => c.ReturnDate, c => c.MapFrom(s => s.DateofReturn))
                    .ForMember(c => c.DateofReturn, c => c.MapFrom(s => (s.DateofReturn.HasValue ? s.DateofReturn.Value.ToString("dd-MM-yyyy") : "NA")))
                    .ForMember(c => c.DateofApplication, c => c.MapFrom(s => (s.DateofApplication.HasValue ? s.DateofApplication.Value.ToString("dd-MM-yyyy") : "NA")))
                    .ForMember(c => c.leaveType, c => c.MapFrom(s => s.Natureofleave == 2 ? "Casual Leave" : "Earned Leave"))
                    .ForMember(c => c.FormStatus, c => c.MapFrom(s => s.FormStatus))
                    .ForMember(c => c.Dependent, c => c.MapFrom(s => s.DependentsList))
                    .ForMember(c => c.EmployeeDOB, c => c.MapFrom(s => s.tblMstEmployee.DOB))
                    .ForMember(c => c.LTCReferenceNumber, c => c.MapFrom(s => s.LTCReferenceNumber));
                });
                var listLTCReport = Mapper.Map<Model.LTCReport>(result);
                return listLTCReport;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool PostLTCReferenceNumber(LTCReport LTCReport)
        {
            log.Info($"LTCService/PostLTCReferenceNumber");
            bool flag = false;
            try
            {
                var dtoLTCReference = genericRepo.Get<DTOModel.tblMstLTC>(x => x.LTCID == LTCReport.LTCID && x.EmployeeId == LTCReport.EmployeeId).FirstOrDefault();
                if (dtoLTCReference != null)
                {
                    dtoLTCReference.LTCReferenceNumber = LTCReport.LTCReferenceNumber;
                    genericRepo.Update<DTOModel.tblMstLTC>(dtoLTCReference);
                    if (dtoLTCReference.LTCID > 0)
                        flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Holiday> GetHolidayList(DateTime? StartDate, DateTime? EndDate)
        {
            log.Info($"LTCService/GetHolidayList");
            try
            {
                var dtoHolidayList = apprRepo.GetHolidayList(StartDate, EndDate);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Holiday, Model.Holiday>()
                    .ForMember(c => c.CYear, c => c.MapFrom(s => s.CYear))
                   .ForMember(c => c.HolidayID, c => c.MapFrom(s => s.HolidayID))
                   .ForMember(c => c.HolidayDate, c => c.MapFrom(s => s.HolidayDate))
                   .ForMember(c => c.HolidayName, c => c.MapFrom(s => s.HolidayName))
                   .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listHoliday = Mapper.Map<List<Model.Holiday>>(dtoHolidayList);
                return listHoliday.OrderBy(x => x.HolidayDate).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion LTC Status Report
    }
}
