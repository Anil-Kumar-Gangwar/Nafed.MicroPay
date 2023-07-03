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
    public class LeaveService : BaseService, ILeaveService
    {
        private readonly IGenericRepository genericRepo;
        private readonly ILeaveRepository leaveRepo;

        public LeaveService(IGenericRepository genericRepo, ILeaveRepository leaveRepo)
        {
            this.genericRepo = genericRepo;
            this.leaveRepo = leaveRepo;
        }

        #region Leave Category
        public List<Model.LeaveCategory> GetLeaveCategoryList()
        {
            log.Info($"GetLeaveCategoryList");
            try
            {
                var result = genericRepo.Get<DTOModel.LeaveCategory>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeaveCategory, Model.LeaveCategory>()
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.LeaveCode, c => c.MapFrom(s => s.LeaveCode))
                    .ForMember(c => c.LeaveCategoryName, c => c.MapFrom(s => s.LeaveCategoryName))
                    .ForMember(c => c.DeductFromSalary, c => c.MapFrom(s => s.DeductFromSalary))
                    .ForMember(c => c.CarryForward, c => c.MapFrom(s => s.CarryForward))
                    .ForMember(c => c.MaxCFUnit, c => c.MapFrom(s => s.MaxCFUnit))
                    .ForMember(c => c.MaxLeave, c => c.MapFrom(s => s.MaxLeave))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForMember(c => c.MinLeave, c => c.MapFrom(s => s.MinLeave))
                    .ForMember(c => c.AllowLevelUpto, c => c.MapFrom(s => s.AllowLevelUpto))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listLeaveCategory = Mapper.Map<List<Model.LeaveCategory>>(result);

                return listLeaveCategory.OrderBy(x => x.LeaveCategoryName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int LeaveCategoryExists(Model.LeaveCategory createLeaveCategoryDetails, out int exists)
        {
            exists = 0;
            if (genericRepo.Exists<DTOModel.LeaveCategory>(x => x.LeaveCategoryID != createLeaveCategoryDetails.LeaveCategoryID && x.LeaveCategoryName == createLeaveCategoryDetails.LeaveCategoryName && !x.IsDeleted))
                exists = 1;
            if (genericRepo.Exists<DTOModel.LeaveCategory>(x => x.LeaveCategoryID != createLeaveCategoryDetails.LeaveCategoryID && x.LeaveCode == createLeaveCategoryDetails.LeaveCode && !x.IsDeleted))
                exists = 2;
            if (genericRepo.Exists<DTOModel.LeaveCategory>(x => x.LeaveCategoryID != createLeaveCategoryDetails.LeaveCategoryID && x.LeaveCategoryName == createLeaveCategoryDetails.LeaveCategoryName && !x.IsDeleted) && genericRepo.Exists<DTOModel.LeaveCategory>(x => x.LeaveCategoryID != createLeaveCategoryDetails.LeaveCategoryID && x.LeaveCode == createLeaveCategoryDetails.LeaveCode && !x.IsDeleted))
                exists = 3;
            return exists;
        }

        public int InsertLeaveCategoryDetails(Model.LeaveCategory createLeaveCategoryDetails)
        {
            log.Info($"InsertLeaveCategoryDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveCategory, DTOModel.LeaveCategory>()
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.LeaveCode, c => c.MapFrom(s => s.LeaveCode))
                    .ForMember(c => c.LeaveCategoryName, c => c.MapFrom(s => s.LeaveCategoryName))
                    .ForMember(c => c.DeductFromSalary, c => c.MapFrom(s => s.DeductFromSalary))
                    .ForMember(c => c.CarryForward, c => c.MapFrom(s => s.CarryForward))
                    .ForMember(c => c.MaxCFUnit, c => c.MapFrom(s => s.MaxCFUnit))
                    .ForMember(c => c.MaxLeave, c => c.MapFrom(s => s.MaxLeave))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.SeqNo, c => c.MapFrom(s => s.SeqNo))
                    .ForMember(c => c.IsSanwichSystem, c => c.MapFrom(s => s.IsSanwichSystem))
                    .ForMember(c => c.leaveGuidelines, c => c.MapFrom(s => s.leaveGuidelines))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLeaveCategory = Mapper.Map<DTOModel.LeaveCategory>(createLeaveCategoryDetails);
                genericRepo.Insert<DTOModel.LeaveCategory>(dtoLeaveCategory);
                return dtoLeaveCategory.LeaveCategoryID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.LeaveCategory GetLeaveCategoryByID(int leaveCategoryID)
        {
            log.Info($"GetLeaveCategoryByID {leaveCategoryID}");
            try
            {
                var leaveCategoryObj = genericRepo.GetByID<DTOModel.LeaveCategory>(leaveCategoryID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.LeaveCategory, Model.LeaveCategory>()
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.LeaveCode, c => c.MapFrom(s => s.LeaveCode))
                    .ForMember(c => c.LeaveCategoryName, c => c.MapFrom(s => s.LeaveCategoryName))
                    .ForMember(c => c.DeductFromSalary, c => c.MapFrom(s => s.DeductFromSalary))
                    .ForMember(c => c.CarryForward, c => c.MapFrom(s => s.CarryForward))
                    .ForMember(c => c.MaxCFUnit, c => c.MapFrom(s => s.MaxCFUnit))
                    .ForMember(c => c.MaxLeave, c => c.MapFrom(s => s.MaxLeave))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.SeqNo, c => c.MapFrom(s => s.SeqNo))
                    .ForMember(c => c.IsSanwichSystem, c => c.MapFrom(s => s.IsSanwichSystem))
                    .ForMember(c => c.leaveGuidelines, c => c.MapFrom(s => s.leaveGuidelines))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                     .ForMember(c => c.EmployeeTypeID, c => c.MapFrom(s => s.EmployeeTypeID))
                     .ForMember(c => c.ApprovalLevel, c => c.MapFrom(s => s.ApprovalRequiredUpto))
                     .ForMember(c => c.MinLeave, c => c.MapFrom(s => s.MinLeave))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.LeaveCategory, Model.LeaveCategory>(leaveCategoryObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLeaveCategoryDetails(Model.LeaveCategory updateLeaveCategoryDetails)
        {
            log.Info($"UpdateLeaveCategoryDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.LeaveCategory>(updateLeaveCategoryDetails.LeaveCategoryID);
                dtoObj.LeaveCategoryName = updateLeaveCategoryDetails.LeaveCategoryName;
                dtoObj.LeaveCode = updateLeaveCategoryDetails.LeaveCode;
                dtoObj.DeductFromSalary = updateLeaveCategoryDetails.DeductFromSalary;
                dtoObj.DeductFromSalary = updateLeaveCategoryDetails.DeductFromSalary;
                dtoObj.CarryForward = updateLeaveCategoryDetails.CarryForward;
                dtoObj.MaxCFUnit = updateLeaveCategoryDetails.MaxCFUnit;
                dtoObj.MaxLeave = updateLeaveCategoryDetails.MaxLeave;
                dtoObj.Remarks = updateLeaveCategoryDetails.Remarks;
                dtoObj.IsSanwichSystem = updateLeaveCategoryDetails.IsSanwichSystem;
                dtoObj.leaveGuidelines = updateLeaveCategoryDetails.leaveGuidelines;
                dtoObj.UpdatedBy = updateLeaveCategoryDetails.UpdateBy;
                dtoObj.UpdatedOn = DateTime.Now;
                dtoObj.EmployeeTypeID = updateLeaveCategoryDetails.EmployeeTypeID == 0 ? null : updateLeaveCategoryDetails.EmployeeTypeID;
                dtoObj.ApprovalRequiredUpto = (int)updateLeaveCategoryDetails.ApprovalLevel;
                genericRepo.Update<DTOModel.LeaveCategory>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool DeleteLeaveCategory(int leaveCategoryID)
        {
            log.Info($"DeleteByID {leaveCategoryID}");
            bool flag = false;
            try
            {
                DTOModel.LeaveCategory dtoleaveCategory = new DTOModel.LeaveCategory();
                dtoleaveCategory = genericRepo.GetByID<DTOModel.LeaveCategory>(leaveCategoryID);
                dtoleaveCategory.IsDeleted = true;
                genericRepo.Update<DTOModel.LeaveCategory>(dtoleaveCategory);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        #endregion

        #region Leave Rule
        public List<Model.LeaveRule> GetLeaveRuleList()
        {
            log.Info($"GetLeaveRuleList");
            try
            {
                var result = genericRepo.Get<DTOModel.LeaveRule>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeaveRule, Model.LeaveRule>()
                    .ForMember(c => c.LeaveRuleID, c => c.MapFrom(s => s.LeaveRuleID))
                    .ForMember(c => c.C_YearID, c => c.MapFrom(s => s.C_YearID))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.LeaveLimit, c => c.MapFrom(s => s.LeaveLimit))
                    .ForMember(c => c.IndividualLeaveLimit, c => c.MapFrom(s => s.IndividualLeaveLimit))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForMember(c => c.FinancialYear, c => c.MapFrom(s => s.CalendarYear.C_YearName))
                    .ForMember(c => c.LeaveCategory, c => c.MapFrom(s => s.LeaveCategory.LeaveCategoryName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listLeaveRule = Mapper.Map<List<Model.LeaveRule>>(result);

                return listLeaveRule.OrderBy(x => x.C_YearID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool LeaveRuleExists(int leaveRuleID, int LeaveCategoryID)
        {
            return genericRepo.Exists<DTOModel.LeaveRule>(x => x.LeaveRuleID != leaveRuleID && x.LeaveCategoryID == LeaveCategoryID && !x.IsDeleted);
        }

        public int InsertLeaveRuleDetails(Model.LeaveRule createLeaveRuleDetails)
        {
            log.Info($"InsertLeaveRuleDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveRule, DTOModel.LeaveRule>()
                    //.ForMember(c => c.LeaveRuleID, c => c.MapFrom(s => s.LeaveRuleID))
                    .ForMember(c => c.C_YearID, c => c.MapFrom(s => s.C_YearID))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.LeaveLimit, c => c.MapFrom(s => s.LeaveLimit))
                    .ForMember(c => c.IndividualLeaveLimit, c => c.MapFrom(s => s.IndividualLeaveLimit))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLeaveRule = Mapper.Map<DTOModel.LeaveRule>(createLeaveRuleDetails);
                genericRepo.Insert<DTOModel.LeaveRule>(dtoLeaveRule);
                return dtoLeaveRule.LeaveRuleID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }


        public Model.LeaveRule GetLeaveRuleByID(int leaveRuleID)
        {
            log.Info($"GetLeaveRuleByID {leaveRuleID}");
            try
            {
                var leaveRuleObj = genericRepo.GetByID<DTOModel.LeaveRule>(leaveRuleID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.LeaveRule, Model.LeaveRule>()
                    .ForMember(c => c.LeaveRuleID, c => c.MapFrom(s => s.LeaveRuleID))
                    .ForMember(c => c.C_YearID, c => c.MapFrom(s => s.C_YearID))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.LeaveLimit, c => c.MapFrom(s => s.LeaveLimit))
                    .ForMember(c => c.IndividualLeaveLimit, c => c.MapFrom(s => s.IndividualLeaveLimit))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.LeaveRule, Model.LeaveRule>(leaveRuleObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLeaveRuleDetails(Model.LeaveRule updateLeaveRuleDetails)
        {
            log.Info($"UpdateLeaveRuleDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.LeaveRule>(updateLeaveRuleDetails.LeaveRuleID);
                dtoObj.C_YearID = updateLeaveRuleDetails.C_YearID;
                dtoObj.LeaveCategoryID = updateLeaveRuleDetails.LeaveCategoryID;
                dtoObj.LeaveLimit = updateLeaveRuleDetails.LeaveLimit;
                dtoObj.IndividualLeaveLimit = updateLeaveRuleDetails.IndividualLeaveLimit;
                dtoObj.UpdatedBy = updateLeaveRuleDetails.UpdateBy;
                dtoObj.UpdatedOn = DateTime.Now;
                genericRepo.Update<DTOModel.LeaveRule>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool DeleteLeaveRule(int leaveRuleId)
        {
            log.Info($"DeleteByID {leaveRuleId}");
            bool flag = false;
            try
            {
                DTOModel.LeaveRule dtoleaveRule = new DTOModel.LeaveRule();
                dtoleaveRule = genericRepo.GetByID<DTOModel.LeaveRule>(leaveRuleId);
                dtoleaveRule.IsDeleted = true;
                genericRepo.Update<DTOModel.LeaveRule>(dtoleaveRule);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        #endregion

        #region Leave Credit Rule

        public List<Model.LeaveCreditRule> GetLeaveCreditList()
        {
            log.Info($"GetLeaveCreditList");
            try
            {
                var result = genericRepo.Get<DTOModel.LeaveCreditRule>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeaveCreditRule, Model.LeaveCreditRule>()
                   .ForMember(c => c.CreditRuleID, c => c.MapFrom(s => s.CreditRuleID))
                   .ForMember(c => c.C_YearID, c => c.MapFrom(s => s.C_YearID))
                   .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                   .ForMember(c => c.FromMonth, c => c.MapFrom(s => s.FromMonth))
                   .ForMember(c => c.ToMonth, c => c.MapFrom(s => s.ToMonth))
                   .ForMember(c => c.Credit, c => c.MapFrom(s => s.Credit))
                   .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForMember(c => c.FinancialYear, c => c.MapFrom(s => s.CalendarYear.C_YearName))
                   .ForMember(c => c.LeaveCategory, c => c.MapFrom(s => s.LeaveCategory.LeaveCategoryName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listLeaveCredit = Mapper.Map<List<Model.LeaveCreditRule>>(result);
                listLeaveCredit.ForEach(x =>
                {
                    x.FMonth = GetMonthName(x.FromMonth);
                    x.TMonth = GetMonthName(x.ToMonth);
                });

                return listLeaveCredit.OrderBy(x => x.C_YearID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public string GetMonthName(int monthId)
        {
            string monthName = "";
            switch (monthId)
            {
                case 1:
                    monthName = "Jan";
                    break;
                case 2:
                    monthName = "Feb";
                    break;
                case 3:
                    monthName = "Mar";
                    break;
                case 4:
                    monthName = "Apr";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "Jun";
                    break;
                case 7:
                    monthName = "Jul";
                    break;
                case 8:
                    monthName = "Aug";
                    break;
                case 9:
                    monthName = "Sep";
                    break;
                case 10:
                    monthName = "Oct";
                    break;
                case 11:
                    monthName = "Nov";
                    break;
                case 12:
                    monthName = "Dec";
                    break;
                default:
                    monthName = "";
                    break;
            }

            return monthName;
        }

        public int LeaveCreditRuleExists(Model.LeaveCreditRule createLeaveCreditRule, out int exists)
        {
            exists = 0;
            if (genericRepo.Exists<DTOModel.LeaveCreditRule>(x => x.CreditRuleID != createLeaveCreditRule.CreditRuleID && x.C_YearID == createLeaveCreditRule.C_YearID && x.LeaveCategoryID == createLeaveCreditRule.LeaveCategoryID && x.FromMonth == createLeaveCreditRule.FromMonth && !x.IsDeleted))
                exists = 1;
            if (genericRepo.Exists<DTOModel.LeaveCreditRule>(x => x.CreditRuleID != createLeaveCreditRule.CreditRuleID && x.C_YearID == createLeaveCreditRule.C_YearID && x.LeaveCategoryID == createLeaveCreditRule.LeaveCategoryID && x.ToMonth == createLeaveCreditRule.ToMonth && !x.IsDeleted))
                exists = 2;
            if (genericRepo.Exists<DTOModel.LeaveCreditRule>(x => x.CreditRuleID != createLeaveCreditRule.CreditRuleID && x.C_YearID == createLeaveCreditRule.C_YearID && x.LeaveCategoryID == createLeaveCreditRule.LeaveCategoryID && x.FromMonth == createLeaveCreditRule.FromMonth && !x.IsDeleted) && genericRepo.Exists<DTOModel.LeaveCreditRule>(x => x.CreditRuleID != createLeaveCreditRule.CreditRuleID && x.C_YearID == createLeaveCreditRule.C_YearID && x.LeaveCategoryID == createLeaveCreditRule.LeaveCategoryID && x.ToMonth == createLeaveCreditRule.ToMonth && !x.IsDeleted))
                exists = 3;
            return exists;

            //return genericRepo.Exists<DTOModel.LeaveCreditRule>(x => x.CreditRuleID != leaveCreditRuleID && x.LeaveCategoryID == leaveCategoryID && !x.IsDeleted);
        }

        public int InsertLeaveCreditRule(Model.LeaveCreditRule createLeaveCreditRule)
        {
            log.Info($"InsertLeaveCreditRule");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveCreditRule, DTOModel.LeaveCreditRule>()
                    .ForMember(c => c.CreditRuleID, c => c.MapFrom(s => s.CreditRuleID))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.C_YearID, c => c.MapFrom(s => s.C_YearID))
                    .ForMember(c => c.FromMonth, c => c.MapFrom(s => s.FromMonth))
                    .ForMember(c => c.ToMonth, c => c.MapFrom(s => s.ToMonth))
                    .ForMember(c => c.Credit, c => c.MapFrom(s => s.Credit))
                    .ForMember(c => c.EmployeeTypeID, c => c.MapFrom(s => s.EmployeeTypeID))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLeaveCreditRule = Mapper.Map<DTOModel.LeaveCreditRule>(createLeaveCreditRule);
                genericRepo.Insert<DTOModel.LeaveCreditRule>(dtoLeaveCreditRule);
                return dtoLeaveCreditRule.CreditRuleID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.LeaveCreditRule GetLeaveCreditRuleByID(int leaveCreditRuleID)
        {
            log.Info($"GetLeaveCreditRuleByID {leaveCreditRuleID}");
            try
            {
                var leaveCreditObj = genericRepo.GetByID<DTOModel.LeaveCreditRule>(leaveCreditRuleID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.LeaveCreditRule, Model.LeaveCreditRule>()
                    .ForMember(c => c.CreditRuleID, c => c.MapFrom(s => s.CreditRuleID))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.C_YearID, c => c.MapFrom(s => s.C_YearID))
                    .ForMember(c => c.FromMonth, c => c.MapFrom(s => s.FromMonth))
                    .ForMember(c => c.ToMonth, c => c.MapFrom(s => s.ToMonth))
                    .ForMember(c => c.Credit, c => c.MapFrom(s => s.Credit))
                    .ForMember(c => c.EmployeeTypeID, c => c.MapFrom(s => s.EmployeeTypeID))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.LeaveCreditRule, Model.LeaveCreditRule>(leaveCreditObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLeaveCreditRuleDetails(Model.LeaveCreditRule updateLeaveType)
        {
            log.Info($"UpdateLeaveCreditRuleDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.LeaveCreditRule>(updateLeaveType.CreditRuleID);
                dtoObj.C_YearID = updateLeaveType.C_YearID;
                dtoObj.LeaveCategoryID = updateLeaveType.LeaveCategoryID;
                dtoObj.FromMonth = updateLeaveType.FromMonth;
                dtoObj.ToMonth = updateLeaveType.ToMonth;
                dtoObj.Credit = updateLeaveType.Credit;
                dtoObj.UpdatedOn = DateTime.Now;
                dtoObj.EmployeeTypeID = updateLeaveType.EmployeeTypeID;
                genericRepo.Update<DTOModel.LeaveCreditRule>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }


        public bool DeleteLeaveCreditRule(int leaveCreditRuleID)
        {
            log.Info($"DeleteByID {leaveCreditRuleID}");
            bool flag = false;
            try
            {
                DTOModel.LeaveCreditRule dtoleaveCreditRule = new DTOModel.LeaveCreditRule();
                dtoleaveCreditRule = genericRepo.GetByID<DTOModel.LeaveCreditRule>(leaveCreditRuleID);
                dtoleaveCreditRule.IsDeleted = true;
                genericRepo.Update<DTOModel.LeaveCreditRule>(dtoleaveCreditRule);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        #endregion

        #region Leave Type
        public List<Model.LeaveType> GetLeaveTypeList()
        {
            log.Info($"GetLeaveTypeList");
            try
            {
                var result = genericRepo.Get<DTOModel.LeaveType>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeaveType, Model.LeaveType>()
                   .ForMember(c => c.LeaveTypeID, c => c.MapFrom(s => s.LeaveTypeID))
                   .ForMember(c => c.LeaveCode, c => c.MapFrom(s => s.LeaveCode))
                   .ForMember(c => c.LeaveDesc, c => c.MapFrom(s => s.LeaveDesc))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listLeaveType = Mapper.Map<List<Model.LeaveType>>(result);
                return listLeaveType.OrderBy(x => x.LeaveCode).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int LeaveTypeExists(Model.LeaveType leaveType, out int exists)
        {
            exists = 0;
            if (genericRepo.Exists<DTOModel.LeaveType>(x => x.LeaveTypeID != leaveType.LeaveTypeID && x.LeaveCode == leaveType.LeaveCode && !x.IsDeleted))
                exists = 1;
            if (genericRepo.Exists<DTOModel.LeaveType>(x => x.LeaveTypeID != leaveType.LeaveTypeID && x.LeaveDesc == leaveType.LeaveDesc && !x.IsDeleted))
                exists = 2;
            if (genericRepo.Exists<DTOModel.LeaveType>(x => x.LeaveTypeID != leaveType.LeaveTypeID && x.LeaveDesc == leaveType.LeaveDesc && !x.IsDeleted) && genericRepo.Exists<DTOModel.LeaveType>(x => x.LeaveTypeID != leaveType.LeaveTypeID && x.LeaveCode == leaveType.LeaveCode && !x.IsDeleted))
                exists = 3;
            return exists;
        }

        public int InsertLeaveTypeDetails(Model.LeaveType createLeaveType)
        {
            log.Info($"InsertLeaveTypeDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveType, DTOModel.LeaveType>()
                    .ForMember(c => c.LeaveCode, c => c.MapFrom(s => s.LeaveCode))
                    .ForMember(c => c.LeaveDesc, c => c.MapFrom(s => s.LeaveDesc))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLeaveType = Mapper.Map<DTOModel.LeaveType>(createLeaveType);
                genericRepo.Insert<DTOModel.LeaveType>(dtoLeaveType);
                return dtoLeaveType.LeaveTypeID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.LeaveType GetLeaveTypeByID(int leaveTypeID)
        {
            log.Info($"GetLeaveTypeByID {leaveTypeID}");
            try
            {
                var leaveTypeObj = genericRepo.GetByID<DTOModel.LeaveType>(leaveTypeID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.LeaveType, Model.LeaveType>()
                    .ForMember(c => c.LeaveTypeID, c => c.MapFrom(s => s.LeaveTypeID))
                    .ForMember(c => c.LeaveCode, c => c.MapFrom(s => s.LeaveCode))
                    .ForMember(c => c.LeaveDesc, c => c.MapFrom(s => s.LeaveDesc))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.LeaveType, Model.LeaveType>(leaveTypeObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public bool UpdateLeaveTypeDetails(Model.LeaveType updateLeaveType)
        {
            log.Info($"UpdateLeaveTypeDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.LeaveType>(updateLeaveType.LeaveTypeID);
                dtoObj.LeaveTypeID = updateLeaveType.LeaveTypeID;
                dtoObj.LeaveCode = updateLeaveType.LeaveCode;
                dtoObj.LeaveDesc = updateLeaveType.LeaveDesc;
                dtoObj.UpdatedOn = DateTime.Now;
                genericRepo.Update<DTOModel.LeaveType>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool DeleteLeaveType(int leaveTypeID)
        {
            log.Info($"DeleteByID {leaveTypeID}");
            bool flag = false;
            try
            {
                DTOModel.LeaveType dtoleaveType = new DTOModel.LeaveType();
                dtoleaveType = genericRepo.GetByID<DTOModel.LeaveType>(leaveTypeID);
                dtoleaveType.IsDeleted = true;
                genericRepo.Update<DTOModel.LeaveType>(dtoleaveType);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        #endregion

        #region Leave Purpose 
        public List<Model.LeavePurpose> GetleavePurposeList()
        {
            log.Info($"GetleavePurposeList");
            try
            {
                var result = genericRepo.Get<DTOModel.LeavePurpose>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.LeavePurpose, Model.LeavePurpose>()
                   .ForMember(c => c.LeavePurposeID, c => c.MapFrom(s => s.LeavePurposeID))
                   .ForMember(c => c.LeavePurposeName, c => c.MapFrom(s => s.LeavePurposeName))
                   //   .ForMember(c => c.CategoryName, c => c.MapFrom(s => s.LeaveCategory.LeaveCategoryName))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listLeavePurpose = Mapper.Map<List<Model.LeavePurpose>>(result);
                return listLeavePurpose.OrderBy(x => x.LeavePurposeName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool LeavePurposeExists(int leavePurposeID, string leavePurposeName)
        {
            return genericRepo.Exists<DTOModel.LeavePurpose>(x => x.LeavePurposeID != leavePurposeID && x.LeavePurposeName == leavePurposeName && !x.IsDeleted);
        }

        public int InsertLeavePurposeDetails(Model.LeavePurpose createLeavePurpose)
        {
            log.Info($"InsertLeavePurposeDetails");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeavePurpose, DTOModel.LeavePurpose>()
                    .ForMember(c => c.LeavePurposeName, c => c.MapFrom(s => s.LeavePurposeName))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLeavePurpose = Mapper.Map<DTOModel.LeavePurpose>(createLeavePurpose);
                genericRepo.Insert<DTOModel.LeavePurpose>(dtoLeavePurpose);
                return dtoLeavePurpose.LeavePurposeID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.LeavePurpose GetLeavePurposeByID(int leavePurposeID)
        {
            log.Info($"GetLeavePurposeByID {leavePurposeID}");
            try
            {
                var leavePurposeObj = genericRepo.GetByID<DTOModel.LeavePurpose>(leavePurposeID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.LeavePurpose, Model.LeavePurpose>()
                    .ForMember(c => c.LeavePurposeID, c => c.MapFrom(s => s.LeavePurposeID))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.LeaveCategoryID))
                    .ForMember(c => c.LeavePurposeName, c => c.MapFrom(s => s.LeavePurposeName))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.LeavePurpose, Model.LeavePurpose>(leavePurposeObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLeavePurposeDetails(Model.LeavePurpose updateLeavePurpose)
        {
            log.Info($"UpdateLeavePurposeDetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.LeavePurpose>(updateLeavePurpose.LeavePurposeID);
                dtoObj.LeavePurposeID = updateLeavePurpose.LeavePurposeID;
                dtoObj.LeaveCategoryID = updateLeavePurpose.LeaveCategoryID;
                dtoObj.LeavePurposeName = updateLeavePurpose.LeavePurposeName;
                dtoObj.UpdatedOn = DateTime.Now;
                genericRepo.Update<DTOModel.LeavePurpose>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool DeleteLeavePurpose(int leavePurposeID)
        {
            log.Info($"DeleteLeavePurpose {leavePurposeID}");
            bool flag = false;
            try
            {
                DTOModel.LeavePurpose dtoLeavePurpose = new DTOModel.LeavePurpose();
                dtoLeavePurpose = genericRepo.GetByID<DTOModel.LeavePurpose>(leavePurposeID);
                dtoLeavePurpose.IsDeleted = true;
                genericRepo.Update<DTOModel.LeavePurpose>(dtoLeavePurpose);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        #endregion

        #region Leave accrued 

        public List<Model.LeaveAccruedDetails> GetEmployeeLeaveAccruedList(Model.LeaveAccruedDetails leaveaccrued)
        {
            try
            {
                int day = (int)DateTime.Now.DayOfWeek;
                var result = leaveRepo.GetEmployeeLeaveAccruedList(leaveaccrued.leavecategoryId, Convert.ToString(leaveaccrued.Month != 0 ? leaveaccrued.Month : Convert.ToInt32(DateTime.Now.Month.ToString())), Convert.ToString(leaveaccrued.Year != 0 ? leaveaccrued.Year : Convert.ToInt32(DateTime.Now.Year.ToString())));
                var LeaveAccruedList = Common.ExtensionMethods.ConvertToList<LeaveAccruedDetails>(result);
                return LeaveAccruedList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public List<Model.LeaveAccruedDetails> GetFillDropdown(string flag, int leavecategoryId)
        {
            try
            {

                var result = leaveRepo.GetFillDropdown(flag, leavecategoryId);
                var ddlList = Common.ExtensionMethods.ConvertToList<LeaveAccruedDetails>(result);
                return ddlList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.LeaveAccruedDetails> GetRegularEmployee()
        {
            try
            {

                var dtoRegularEmployess = genericRepo.Get<Data.Models.tblMstEmployee>(x => !x.IsDeleted && x.EmployeeTypeID == 5 && x.DOLeaveOrg == null);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.LeaveAccruedDetails>()
                    .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.EmployeeCode))
                    .ForMember(c => c.Pr_Loc_DOJ, c => c.MapFrom(s => s.Pr_Loc_DOJ))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var regularEmployees = Mapper.Map<List<Model.LeaveAccruedDetails>>(dtoRegularEmployess);

                return regularEmployees.OrderBy(x => x.EmployeeCode).ToList();


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.LeaveAccruedDetails> GetLeavevalidatemonth(Model.LeaveAccruedDetails leaveacureddetails)
        {
            try
            {
                var dtogetvalidatemonth = genericRepo.Get<Data.Models.tblLeaveUpdate>(x => x.LeaveCategoryID == leaveacureddetails.leavecategoryId && x.Year == leaveacureddetails.Year && x.Month == leaveacureddetails.Month);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblLeaveUpdate, Model.LeaveAccruedDetails>()
                    .ForMember(c => c.LeaveType, c => c.MapFrom(s => s.LeaveType))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var getdata = Mapper.Map<List<Model.LeaveAccruedDetails>>(dtogetvalidatemonth);

                return getdata.OrderBy(x => x.Year).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.LeaveAccruedDetails> GetELMLAccumulationdata(string EmpCode, int prevYear, int curryear, string flag)
        {
            try
            {
                var result = leaveRepo.GetELMLAccumulationdata(EmpCode, prevYear, curryear, flag);
                var ddlaccudata = Common.ExtensionMethods.ConvertToList<LeaveAccruedDetails>(result);
                return ddlaccudata;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int Checkdataalreadyexist(Model.LeaveAccruedDetails leaveacureddetails)
        {
            try
            {
                var dtocountofrecordforUpdate = genericRepo.Get<Data.Models.tblLeaveBal>(x => x.LeaveYear == (leaveacureddetails.Year).ToString() && x.EmpCode == leaveacureddetails.EmployeeCode);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblLeaveBal, Model.LeaveAccruedDetails>()
                     .ForMember(c => c.EmployeeCode, c => c.MapFrom(s => s.EmpCode))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var employeefromLeaveBal = Mapper.Map<List<Model.LeaveAccruedDetails>>(dtocountofrecordforUpdate);

                return employeefromLeaveBal.Count;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Updateleavetrans(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveAccruedDetails, DTOModel.tblLeaveTran>()
                   .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmployeeCode))
                   .ForMember(c => c.LeaveType, c => c.MapFrom(s => s.LeaveTypeName))
                   .ForMember(c => c.CurrDate, c => c.MapFrom(s => DateTime.Now))
                   .ForMember(c => c.AccruedLeave, c => c.MapFrom(s => s.AccruedLeave))
                   .ForMember(c => c.TransactionType, c => c.MapFrom(s => "CR"))
                   .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.leavecategoryId))
                   .ForMember(c => c.Unit, c => c.MapFrom(s => s.AccruedLeave))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => userID))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => DateTime.Now))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoleavetrans = Mapper.Map<DTOModel.tblLeaveTran>(leaveacureddetails);
                return leaveRepo.AddLeaveTrans(dtoleavetrans);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateLeaveUpdate(Model.LeaveAccruedDetails leaveacureddetails)
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveAccruedDetails, DTOModel.tblLeaveUpdate>()
                   .ForMember(c => c.LeaveType, c => c.MapFrom(s => leaveacureddetails.LeaveTypeName))
                   .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                   .ForMember(c => c.Year, c => c.MapFrom(s => DateTime.Now.Year))
                    .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => s.leavecategoryId))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoleaveUpdate = Mapper.Map<DTOModel.tblLeaveUpdate>(leaveacureddetails);
                return leaveRepo.AddLeaveUpdate(dtoleaveUpdate);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.LeaveAccruedDetails> GetEmpLeaveBalID(Model.LeaveAccruedDetails leaveacureddetails)
        {
            try
            {
                var dtEmpLeaveBalID = genericRepo.Get<Data.Models.tblLeaveBal>(x => x.EmpCode == leaveacureddetails.EmployeeCode && x.LeaveYear == (leaveacureddetails.Year).ToString());
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblLeaveBal, Model.LeaveAccruedDetails>()
                    .ForMember(c => c.EmpLeaveBalID, c => c.MapFrom(s => s.EmpLeaveBalID))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var empLeaveBalID = Mapper.Map<List<Model.LeaveAccruedDetails>>(dtEmpLeaveBalID);
                return empLeaveBalID.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region EL update

        public int ELAccumulation(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            double dblCF = 0;
            DateTime doj;

            var listRegularEmployee = GetRegularEmployee();
            DataTable DTregularemployee = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(listRegularEmployee);

            string CurrentDate = DateTime.Now.ToShortDateString();
            int curryear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            int prevYear = curryear - 1;
            leaveacureddetails.LeaveTypeName = "EL";
            try
            {

                #region Check month of process
                //

                int caseSwitch = leaveacureddetails.Month;

                switch (caseSwitch)
                {
                    case 1:
                        leaveacureddetails.AccruedLeave = 17;
                        break;
                    case 7:
                        leaveacureddetails.AccruedLeave = 16;
                        break;
                    default:
                        // TempData["Message"] = "User can only process this activity in Janauary or July";
                        break;
                }
                #endregion

                #region Loop Begin for regular employee

                for (int i = 0; i < DTregularemployee.Rows.Count - 1; i++)
                {
                    leaveacureddetails.EmployeeCode = DTregularemployee.Rows[i]["EmployeeCode"].ToString();
                    doj = Convert.ToDateTime(DTregularemployee.Rows[i]["pr_loc_doj"]);

                    if (doj <= Convert.ToDateTime("01/01/" + DateTime.Now.Year))
                    {
                        //Carry Forward EL
                        dblCF = 0.0;
                        if (leaveacureddetails.Month == 1)
                        {
                            var getaccumulationdate = GetELMLAccumulationdata(leaveacureddetails.EmployeeCode, prevYear, curryear, "1");
                            DataTable dtaccdata = Common.ExtensionMethods.ToDataTable(getaccumulationdate);
                            if (dtaccdata.Rows.Count > 0)
                            {
                                dblCF = Convert.ToDouble(dtaccdata.Rows[0]["BalanceEL"].ToString());
                            }
                        }
                        else if (leaveacureddetails.Month == 7)
                        {
                            var getaccumulationdate = GetELMLAccumulationdata(leaveacureddetails.EmployeeCode, prevYear, curryear, "7");
                            DataTable dtaccdata = Common.ExtensionMethods.ToDataTable(getaccumulationdate);
                            if (dtaccdata.Rows.Count > 0)
                            {
                                dblCF = Convert.ToDouble(dtaccdata.Rows[0]["BalanceEL"].ToString());
                            }
                        }

                        //if balance is > 300 since last 6 month then it will elaps to 300 
                        leaveacureddetails.EL_OpBal = dblCF;
                        if (dblCF > 300)
                        {
                            dblCF = 300;
                        }
                        leaveacureddetails.EL = leaveacureddetails.AccruedLeave + dblCF;
                        leaveacureddetails.EL = Math.Round(leaveacureddetails.EL, 0);
                    }
                    else //if date of joinining is after 1 jan 
                    {
                        double missedDays = 0;
                        DateTime startdate = System.DateTime.Parse("01/" + "01/" + curryear);
                        DateTime lasttdate = doj;
                        missedDays = (lasttdate - startdate).Days;
                        var currELdays = leaveacureddetails.AccruedLeave * (365 - missedDays);
                        currELdays = currELdays / 365;
                        leaveacureddetails.EL = Math.Round(currELdays, 0);
                    }

                    //'Here Balance update in tblLeaveBal

                    int chkdata = Checkdataalreadyexist(leaveacureddetails);

                    if (chkdata == 1)
                    {
                        var getEmpLeaveBalID = GetEmpLeaveBalID(leaveacureddetails);
                        DataTable dtgetEmpLeaveBalID = Common.ExtensionMethods.ToDataTable(getEmpLeaveBalID);
                        leaveacureddetails.EmpLeaveBalID = Convert.ToInt32(dtgetEmpLeaveBalID.Rows[0]["EmpLeaveBalID"]);
                        UpdateELLeaves(leaveacureddetails, userID);
                    }
                    else
                    {
                        InsertELCreditLeaves(leaveacureddetails, userID);
                    }

                    //Leave Trans Table It will insert an entry as accured Leave days with date

                    Updateleavetrans(leaveacureddetails, userID);
                }
                #endregion

                //Update tblLeaveUpdate Table with Year, month and categoryID

                UpdateLeaveUpdate(leaveacureddetails);
                return 1;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateELLeaves(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblLeaveBal>(leaveacureddetails.EmpLeaveBalID);
                dtoObj.EL = leaveacureddetails.EL;
                dtoObj.EL_OpBal = leaveacureddetails.EL_OpBal;
                dtoObj.UpdatedOn = DateTime.Now;
                dtoObj.UpdatedBy = userID;
                genericRepo.Update<DTOModel.tblLeaveBal>(dtoObj);
                flag = true;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool InsertELCreditLeaves(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveAccruedDetails, DTOModel.tblLeaveBal>()
                   .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmployeeCode))
                   .ForMember(c => c.LeaveYear, c => c.MapFrom(s => DateTime.Now.Year))
                   .ForMember(c => c.EL, c => c.MapFrom(s => s.EL))
                   .ForMember(c => c.EL_OpBal, c => c.MapFrom(s => s.EL_OpBal))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => userID))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => DateTime.Now))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoleavebal = Mapper.Map<DTOModel.tblLeaveBal>(leaveacureddetails);
                return leaveRepo.AddLeaveBalance(dtoleavebal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region MedicalLeaves update

        public int PuttingMedicalLeave(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            int countNoOfEmp = 0;
            int curryear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            leaveacureddetails.prevYear = curryear - 1;
            leaveacureddetails.LeaveTypeName = "ML";


            var listRegularEmployee = GetELMLAccumulationdata("", 0, curryear, "8");
            DataTable DTregularemployee = Common.ExtensionMethods.ToDataTable(listRegularEmployee);
            countNoOfEmp = DTregularemployee.Rows.Count;
            string CurrentDate = DateTime.Now.ToShortDateString();


            try
            {

                #region Loop Begin for regular employee

                for (int i = 0; i < DTregularemployee.Rows.Count - 1; i++)
                {
                    leaveacureddetails.EmployeeCode = DTregularemployee.Rows[i]["EmployeeCode"].ToString();

                    //count of record for update

                    var medicalLeaves = GetmedicalLeaves(leaveacureddetails);
                    DataTable dtmedicalLeaves = Common.ExtensionMethods.ToDataTable(medicalLeaves);
                    if (dtmedicalLeaves.Rows.Count > 0)
                    {
                        leaveacureddetails.Medical = Convert.ToDouble(dtmedicalLeaves.Rows[0]["Medical"].ToString());
                        leaveacureddetails.Medical_Extra = Convert.ToDouble(dtmedicalLeaves.Rows[0]["Medical_OpBal"].ToString());
                        leaveacureddetails.Medical_OpBal = Convert.ToDouble(dtmedicalLeaves.Rows[0]["Medical_Extra"].ToString());
                    }
                    //if max accumulation is  360 days on halfpay
                    //as more than 360 can be credited and encashed at the rate of 25 % of the fullday pay

                    double dblOpBal = 0.0;
                    double Medical_Extra = 0.0;

                    dblOpBal = leaveacureddetails.Medical;
                    leaveacureddetails.Medical_OpBal = leaveacureddetails.Medical;
                    int currMLdays = 40;
                    leaveacureddetails.AccruedLeave = 40;
                    leaveacureddetails.Medical = Convert.ToInt32(currMLdays + dblOpBal);
                    leaveacureddetails.Medical = Math.Round(leaveacureddetails.Medical, 0);

                    if (leaveacureddetails.Medical > 360)
                    {
                        Medical_Extra = Math.Round(leaveacureddetails.Medical - 360, 0);
                        leaveacureddetails.Medical_Extra = Math.Round(Medical_Extra + leaveacureddetails.Medical_Extra);
                        leaveacureddetails.Medical = 360;
                    }
                    else
                    {
                        leaveacureddetails.Medical_Extra = leaveacureddetails.Medical_Extra;

                    }
                    int chkdata = Checkdataalreadyexist(leaveacureddetails);
                    if (chkdata == 1)
                    {
                        var getEmpLeaveBalID = GetEmpLeaveBalID(leaveacureddetails);
                        DataTable dtgetEmpLeaveBalID = Common.ExtensionMethods.ToDataTable(getEmpLeaveBalID);
                        leaveacureddetails.EmpLeaveBalID = Convert.ToInt32(dtgetEmpLeaveBalID.Rows[0]["EmpLeaveBalID"]);
                        UpdateMedicalLeaves(leaveacureddetails, userID);
                    }
                    else
                    {
                        InsertMedicalLeaves(leaveacureddetails, userID);
                    }
                    Updateleavetrans(leaveacureddetails, userID);

                }
                #endregion

                //Update tblLeaveUpdate Table with Year, month and categoryID
                UpdateMedicalUpdate(leaveacureddetails);
                return 1;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateMedicalLeaves(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            log.Info($"LeaveService/UpdateMedicalLeaves");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblLeaveBal>(leaveacureddetails.EmpLeaveBalID);
                dtoObj.Medical = leaveacureddetails.Medical;
                dtoObj.Medical_OpBal = leaveacureddetails.Medical_OpBal;
                dtoObj.Medical_Extra = leaveacureddetails.Medical_Extra;
                dtoObj.MedicalUpdatedate = DateTime.Now;
                dtoObj.UpdatedBy = userID;
                dtoObj.UpdatedOn = DateTime.Now;
                genericRepo.Update<DTOModel.tblLeaveBal>(dtoObj);
                flag = true;


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool InsertMedicalLeaves(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveAccruedDetails, DTOModel.tblLeaveBal>()
                   .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmployeeCode))
                   .ForMember(c => c.LeaveYear, c => c.MapFrom(s => DateTime.Now.Year))
                   .ForMember(c => c.Medical, c => c.MapFrom(s => s.Medical))
                    .ForMember(c => c.Medical_OpBal, c => c.MapFrom(s => s.Medical_OpBal))
                     .ForMember(c => c.Medical_Extra, c => c.MapFrom(s => s.Medical_Extra))
                    .ForMember(c => c.MedicalUpdatedate, c => c.MapFrom(s => DateTime.Now))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => DateTime.Now))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => userID))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoleavebal = Mapper.Map<DTOModel.tblLeaveBal>(leaveacureddetails);
                return leaveRepo.AddLeaveBalance(dtoleavebal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.LeaveAccruedDetails> GetmedicalLeaves(Model.LeaveAccruedDetails leaveacureddetails)
        {
            try
            {
                var dtomedicalLeaves = genericRepo.Get<Data.Models.tblLeaveBal>(x => x.LeaveYear == leaveacureddetails.prevYear.ToString() && x.EmpCode == leaveacureddetails.EmployeeCode);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblLeaveBal, Model.LeaveAccruedDetails>()
                    .ForMember(c => c.Medical, c => c.MapFrom(s => s.Medical))
                    .ForMember(c => c.Medical_Extra, c => c.MapFrom(s => s.Medical_Extra))
                    .ForMember(c => c.Medical_OpBal, c => c.MapFrom(s => s.Medical_OpBal))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var employeemedicalLeaves = Mapper.Map<List<Model.LeaveAccruedDetails>>(dtomedicalLeaves);
                return employeemedicalLeaves.ToList();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateMedicalUpdate(Model.LeaveAccruedDetails leaveacureddetails)
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveAccruedDetails, DTOModel.tblMedicalLeaveUpdate>()
                   .ForMember(c => c.LeaveCategoryID, c => c.MapFrom(s => 8))
                   .ForMember(c => c.Day, c => c.MapFrom(s => DateTime.Now.Day))
                   .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                   .ForMember(c => c.Year, c => c.MapFrom(s => DateTime.Now.Year))
                    .ForMember(c => c.fulldate, c => c.MapFrom(s => DateTime.Now))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtomedicalleaveUpdate = Mapper.Map<DTOModel.tblMedicalLeaveUpdate>(leaveacureddetails);
                return leaveRepo.AddMedicalLeaveUpdate(dtomedicalleaveUpdate);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        #endregion

        #region CL Update

        public int CLAccumulation(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            double dblCF = 0;
            DateTime doj;

            var listRegularEmployee = GetRegularEmployee();
            DataTable DTregularemployee = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(listRegularEmployee);

            string CurrentDate = DateTime.Now.ToShortDateString();
            int curryear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;
            int prevYear = curryear - 1;
            leaveacureddetails.LeaveTypeName = "CL";
            try
            {

                #region Check month of process
                //
                int caseSwitch = leaveacureddetails.Month;
                #endregion

                #region Loop Begin for regular employee

                leaveacureddetails.AccruedLeave = 14;

                for (int i = 0; i < DTregularemployee.Rows.Count - 1; i++)
                {
                    leaveacureddetails.EmployeeCode = DTregularemployee.Rows[i]["EmployeeCode"].ToString();
                    doj = Convert.ToDateTime(DTregularemployee.Rows[i]["pr_loc_doj"]);

                    if (doj.ToString() != "")
                    {
                        if (doj <= Convert.ToDateTime("01/01/" + DateTime.Now.Year))
                        {
                            //Credit CL
                            dblCF = leaveacureddetails.AccruedLeave;
                            leaveacureddetails.CL = dblCF;
                        }
                        else //if date of joinining is after 1 jan 
                        {
                            double missedDays = 0;
                            DateTime startdate = System.DateTime.Parse("01/" + "01/" + curryear);
                            DateTime lasttdate = doj;
                            missedDays = (lasttdate - startdate).Days;
                            var currELdays = leaveacureddetails.AccruedLeave * (365 - missedDays);
                            currELdays = currELdays / 365;
                            leaveacureddetails.CL = Math.Round(currELdays, 0);
                            if (leaveacureddetails.CL < 0)
                            {
                                leaveacureddetails.CL = 0;
                            }
                        }
                        //'Here Balance update in tblLeaveBal

                        int chkdata = Checkdataalreadyexist(leaveacureddetails);

                        if (chkdata == 1)
                        {
                            var getEmpLeaveBalID = GetEmpLeaveBalID(leaveacureddetails);
                            DataTable dtgetEmpLeaveBalID = Common.ExtensionMethods.ToDataTable(getEmpLeaveBalID);
                            leaveacureddetails.EmpLeaveBalID = Convert.ToInt32(dtgetEmpLeaveBalID.Rows[0]["EmpLeaveBalID"]);
                            UpdateCLLeaves(leaveacureddetails, userID);
                        }
                        else
                        {
                            InsertCLCreditLeaves(leaveacureddetails, userID);
                        }

                        //Leave Trans Table It will insert an entry as accured Leave days with date

                        Updateleavetrans(leaveacureddetails, userID);

                    }
                }
                #endregion

                //Update tblLeaveUpdate Table with Year, month and categoryID

                UpdateLeaveUpdate(leaveacureddetails);
                return 1;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertCLCreditLeaves(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LeaveAccruedDetails, DTOModel.tblLeaveBal>()
                   .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmployeeCode))
                   .ForMember(c => c.LeaveYear, c => c.MapFrom(s => DateTime.Now.Year))
                   .ForMember(c => c.CL, c => c.MapFrom(s => s.CL))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => userID))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(s => DateTime.Now))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoleavebal = Mapper.Map<DTOModel.tblLeaveBal>(leaveacureddetails);
                return leaveRepo.AddLeaveBalance(dtoleavebal);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateCLLeaves(Model.LeaveAccruedDetails leaveacureddetails, int userID)
        {
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblLeaveBal>(leaveacureddetails.EmpLeaveBalID);
                dtoObj.CL = leaveacureddetails.CL;
                dtoObj.UpdatedOn = DateTime.Now;
                dtoObj.UpdatedBy = userID;
                genericRepo.Update<DTOModel.tblLeaveBal>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }
        #endregion

        #endregion

        #region Leave Balance
        public IList<Model.EmployeeLeaveBalance> GetEmployeeLeaveBal(string empCode)
        {
            log.Info($"LeaveService/GetEmployeeLeaveBal/{empCode}");
            try
            {
                var leaveBalance = leaveRepo.GetEmployeeLeaveBal(empCode).ToList();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetEmployeeLeaveBal_Result, Model.EmployeeLeaveBalance>()
                    .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmpCode))
                    .ForMember(c => c.LeaveType, c => c.MapFrom(s => s.LeaveType))
                    .ForMember(c => c.OpeningBal, c => c.MapFrom(s => s.OpeningBal))
                    .ForMember(c => c.Accrued, c => c.MapFrom(s => s.Accrued))
                    .ForMember(c => c.availed, c => c.MapFrom(s => s.availed))
                    .ForMember(c => c.Balance, c => c.MapFrom(s => s.Balance))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var leaveBal = Mapper.Map<List<Model.EmployeeLeaveBalance>>(leaveBalance);
                return leaveBal;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region Leave Applied
        public IList<Model.EmployeeLeave> GetLeaveApplied(int reportingTo, out int leaveCount)
        {
            var leaveApplied = leaveRepo.GetLeaveApplied(reportingTo).ToList();
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeLeave, Model.EmployeeLeave>()
                    .ForMember(c => c.LeaveID, c => c.MapFrom(s => s.LeaveID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.DateFrom, c => c.MapFrom(s => s.DateFrom))
                    .ForMember(c => c.DateTo, c => c.MapFrom(s => s.DateTo))
                    .ForMember(c => c.StatusID, c => c.MapFrom(s => s.StatusID))
                    .ForMember(c => c.LeaveCode, o => o.MapFrom(s => s.LeaveCategory.LeaveCode))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var leaveBal = Mapper.Map<List<Model.EmployeeLeave>>(leaveApplied);
                leaveCount = leaveBal.Where(x => x.StatusID == 1).Count();
                return leaveBal;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        public bool UpdateLeaveStatus(EmployeeLeave empLeave, int UserID)
        {
            log.Info($"UpdateLeaveStatus");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.EmployeeLeave>(empLeave.LeaveID);
                dtoObj.StatusID = empLeave.StatusID;
                dtoObj.ReportingToRemark = !string.IsNullOrEmpty(empLeave.ReporotingToRemark) ? empLeave.ReporotingToRemark : dtoObj.ReportingToRemark;
                dtoObj.ReviewerToRemark = !string.IsNullOrEmpty(empLeave.ReviewerToRemark) ? empLeave.ReviewerToRemark : dtoObj.ReviewerToRemark;
                dtoObj.PurposeOthers = !string.IsNullOrEmpty(empLeave.AcceptanceAuthorityRemark) ? empLeave.AcceptanceAuthorityRemark : dtoObj.PurposeOthers;
                dtoObj.UpdatedOn = DateTime.Now;
                dtoObj.UpdatedBy = UserID;
                // dtoObj.EmployeeId = empLeave.EmployeeId;
                genericRepo.Update<DTOModel.EmployeeLeave>(dtoObj);
                empLeave._ProcessWorkFlow.ReferenceID = dtoObj.LeaveID;
                AddProcessWorkFlow(empLeave._ProcessWorkFlow);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool UpdateLeaveTransStatus(EmployeeLeave empLeave, int UserID)
        {
            log.Info($"UpdateLeaveStatus");
            bool flag = false;
            try
            {
                Nafed.MicroPay.Data.Models.tblLeaveTran dtoEmployeeLeaveTrans = new Nafed.MicroPay.Data.Models.tblLeaveTran();
                var leavetrans = genericRepo.Get<Data.Models.tblLeaveTran>(x => x.EmpLeaveID == empLeave.LeaveID).Select(x => new { x.EmpLeaveTransID }).FirstOrDefault();
                var dtoObj = genericRepo.GetByID<DTOModel.tblLeaveTran>(Convert.ToInt32(leavetrans.EmpLeaveTransID));
                dtoObj.StatusID = empLeave.StatusID;
                dtoObj.UpdatedOn = DateTime.Now;
                dtoObj.UpdatedBy = UserID;
                genericRepo.Update<DTOModel.tblLeaveTran>(dtoObj);
                flag = true;



            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool SenderSendMail(int senderID, string leaveType, EmployeeLeave empLeave, string mailType)
        {
            log.Info($"LeaveService/SenderSendMail");
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

                if (mailType == "Approval")
                {
                    if (empLeave.ActionType == "accept")
                    {
                        if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                        {
                            emailBody.AppendFormat("<div> <p><b><span style='font-family:Tahoma;font-size:12pt;color:green'>Approved</span></b> - "
                       + "" + empLeave.EmployeeName + " (" + empLeave.EmployeeCode + ")" + "</p>"
                       + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Leave Type </td><td style='font-weight: bold'> {0} </td> </tr> <tr><td> Period </td> <td style='font-weight: bold'> {1} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {2} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{3}</td></tr>", leaveType, empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString(), empLeave.Unit, empLeave.Reason);
                            emailBody.AppendFormat("</Table>");
                            emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");
                        }
                        else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                        {
                            emailBody.AppendFormat("Approved " + empLeave.EmployeeName + " - " + empLeave.EmployeeCode + ""
                                + "" + leaveType + ", Period - " + empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString() + "");
                        }
                    }
                    else if (empLeave.ActionType == "reject")
                    {
                        if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                        {
                            emailBody.AppendFormat("<div> <p><b> <span style='font-family:Tahoma;font-size:12pt;color:red'>Rejected</span></b> - "
                       + "" + empLeave.EmployeeName + " - " + empLeave.EmployeeCode + "</p>"
                       + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Leave Type </td><td style='font-weight: bold'> {0} </td> </tr> <tr><td> Period </td> <td style='font-weight: bold'> {1} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {2} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{3}</td></tr>", leaveType, empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString(), empLeave.Unit, empLeave.Reason);
                            emailBody.AppendFormat("</Table>");
                            emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");
                        }
                        else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                        {
                            emailBody.AppendFormat("Rejected " + empLeave.EmployeeName + " - " + empLeave.EmployeeCode + ""
                                + "" + leaveType + ", Period - " + empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString() + "");
                        }
                    }
                }
                if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                {
                    EmailMessage message = new EmailMessage();
                    message.To = senderMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : Leave Application";
                    Task t1 = Task.Run(() => GetEmailConfiguration(message));
                }
                else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                {

                    Task t1 = Task.Run(() => SendMessageOnMobile(senderMail.MobileNo, emailBody.ToString()));
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

        public bool RecieverSendMail(int recieverID, string leaveType, EmployeeLeave empLeave, string mailType)
        {
            log.Info($"LeaveService/RecieverSendMail");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                // var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).FirstOrDefault().OfficialEmail;
                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail
                }).FirstOrDefault();

                int mailRceiverID = 0;
                if (empLeave.StatusID == (int)EmpLeaveStatus.Approved || empLeave.StatusID == (int)EmpLeaveStatus.RejectedByReportingOfficer || empLeave.StatusID == (int)EmpLeaveStatus.RejectedByReviwerOfficer || empLeave.StatusID == (int)EmpLeaveStatus.RejectedByAcceptanceAuthority)
                {
                    mailRceiverID = empLeave._ProcessWorkFlow.EmployeeID;
                }
                else
                {
                    mailRceiverID = (int)empLeave._ProcessWorkFlow.ReceiverID;
                }

                if (mailType == "Approval")
                {
                    if (empLeave.ActionType == "accept")
                    {
                        if (empLeave.StatusID == (int)EmpLeaveStatus.Approved)
                        {
                            if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                            {
                                emailBody.AppendFormat("<div> <p><b><span style='font-family:Tahoma;font-size:12pt;color:green'>Approved</span></b></p>"
                      + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Employee </td><td style='font-weight: bold'> {0} </td> </tr><tr><td> Leave Type </td><td style='font-weight: bold'> {1} </td> </tr> <tr><td> Period </td> <td style='font-weight: bold'> {2} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {3} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{4}</td></tr>", empLeave.EmployeeCode + " - " + empLeave.EmployeeName, leaveType, empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString(), empLeave.Unit, empLeave.Reason);
                                emailBody.AppendFormat("</Table>");
                                emailBody.AppendFormat("<div> <p>Regards, <br/> Name : " + empLeave.ReportingOfficerName + " <br/> Post : " + empLeave.DesignationName + " </p> </div>");

                            }
                            else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                            {
                                emailBody.AppendFormat("Dear Sir/Madam, Your Leave Application has been Approved."
                                     + "" + leaveType + ", Period - " + empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString() + "");
                            }
                        }

                        else
                        {
                            if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                            {

                                emailBody.AppendFormat("<div> <p><b><span style='font-family:Tahoma;font-size:12pt;'> Approval Recommendation </span></b> </p>"
                                    + "<p>Dear Sir/Madam,</p>"
                      + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Employee </td><td style='font-weight: bold'> {0} </td> </tr><tr><td> Leave Type </td><td style='font-weight: bold'> {1} </td> </tr> <tr><td> Period </td> <td style='font-weight: bold'> {2} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {3} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{4}</td></tr>", empLeave.EmployeeCode + " - " + empLeave.EmployeeName, leaveType, empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString(), empLeave.Unit, empLeave.Reason);
                                emailBody.AppendFormat("</Table>");
                                emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>For details please login to HRMS portal. </p>");
                                emailBody.AppendFormat("<div> <p>Regards, <br/> Name : " + empLeave.ReportingOfficerName + " <br/> Post : " + empLeave.DesignationName + " </p> </div>");

                            }
                            else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                            {
                                emailBody.AppendFormat("Approval Recommendation - Dear Sir/Madam, For details please login to HRMS portal."
                                    + " ." + "" + leaveType + ", Period - " + empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString() + "");
                            }
                        }
                    }
                    if (empLeave.ActionType == "reject")
                    {
                        if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                        {
                            emailBody.AppendFormat("<div> <p><b><span style='font-family:Tahoma;font-size:12pt;color:red'> Rejected</span> </b> due to - " + empLeave._ProcessWorkFlow.Scomments + " </p>"
                 + "<Table border='1' cellspacing='0' cellpadding='3' width='400' style='font-family:Tahoma'><tr><td> Employee </td><td style='font-weight: bold'> {0} </td> </tr><tr><td> Leave Type </td><td style='font-weight: bold'> {1} </td> </tr> <tr><td> Period </td> <td style='font-weight: bold'> {2} </td> <tr> <td> #Days </td> <td style='font-weight: bold'> {3} </td> </tr> <tr><td> Reason </td><td style='font-weight: bold'>{4}</td></tr>", empLeave.EmployeeCode + " - " + empLeave.EmployeeName, leaveType, empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString(), empLeave.Unit, empLeave.Reason);
                            emailBody.AppendFormat("</Table>");
                            emailBody.AppendFormat("<div> <p>Regards, <br/> Name : " + empLeave.ReportingOfficerName + " <br/> Post : " + empLeave.DesignationName + " </p> </div>");

                        }
                        else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                        {
                            emailBody.AppendFormat("Dear Sir/Madam, Your Leave Application has been Rejected."
                                 + "" + leaveType + ", Period - " + empLeave.DateFrom.Value.ToShortDateString() + " To " + empLeave.DateTo.Value.ToShortDateString() + "");
                        }
                    }
                }
                if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                {
                    EmailMessage message = new EmailMessage();
                    message.To = recieverMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : Leave Application";
                    Task t2 = Task.Run(() => GetEmailConfiguration(message));
                }
                else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                {

                    Task t1 = Task.Run(() => SendMessageOnMobile(recieverMail.MobileNo, emailBody.ToString()));
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

        void GetEmailConfiguration(EmailMessage message)
        {
            try
            {
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                // EmailMessage message = new EmailMessage();
                //   message.To = ToEmailID;
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                //  message.Subject = subject;
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                //   message.Body = mailbody;
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

        public bool SendMessageOnMobile(string mobileNo, string message)
        {
            try
            {
                using (var web = new System.Net.WebClient())
                {
                    var smssetting = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                    string msgRecepient = mobileNo.Length == 10 ? "91" + mobileNo : mobileNo;
                    string msgText = message;

                    string url = smssetting.SMSUrl +
                        "&mobileno=" + msgRecepient +
                        "&message=" + message;
                    string result = web.DownloadString(url);
                    if (result.Contains("successfully"))
                    {
                        return true;
                    }
                    else
                    {
                        log.Error("Some issue delivering-");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }

        }

    }
}
