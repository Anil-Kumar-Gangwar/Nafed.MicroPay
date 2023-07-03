using Nafed.MicroPay.Common;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using System.Data;
using Nafed.MicroPay.Data.Repositories;

namespace Nafed.MicroPay.Services.Salary
{
    public class SalaryHeadRuleService : BaseService, ISalaryHeadRuleService
    {
        private readonly IGenericRepository genericRepo;
        private readonly ISalaryRepository salaryRepo;
        public SalaryHeadRuleService(IGenericRepository genericRepo, ISalaryRepository salaryRepo)
        {
            this.genericRepo = genericRepo;
            this.salaryRepo = salaryRepo;
        }

        #region Salary Head Rules

        public List<SalaryHeadField> GetSalaryHeadFields(int? employeeTypeID = null)
        {
            log.Info($"SalaryHeadRuleService/GetSalaryHeadFields");
            try
            {
                var allSalaryFields = SalaryHeadFields.GetSalaryHeadFields().Select(x => new { FieldName = x }).ToList();
                var fields = genericRepo.Get<DTOModel.SalaryHead>(x => !x.IsDeleted && (employeeTypeID.HasValue ? (x.EmployeeTypeID == employeeTypeID) : (x.EmployeeTypeID == 5))).Distinct().ToList();
                var fieldLists = (from left in allSalaryFields
                                  join right in fields on left.FieldName equals right.FieldName into joinedList
                                  from sub in joinedList.DefaultIfEmpty()
                                  select new SalaryHeadField
                                  {
                                      FieldName = left.FieldName,
                                      FieldDesc = sub == null ? string.Empty : sub.FieldDesc,
                                      SeqNo = sub == null ? 0 : sub.SeqNo
                                  });
                return fieldLists.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SalaryHead GetSalaryHeadFormulaRule(string fieldName, int? employeeTypeID = null)
        {
            log.Info($"SalaryHeadRuleService/GetSalaryHeadFormulaRule/{fieldName}");
            SalaryHead salaryHead = new SalaryHead();
            try
            {
                var dtoSalaryHead = genericRepo.Get<DTOModel.SalaryHead>(x => x.FieldName == fieldName &&
                (employeeTypeID > 0 ? x.EmployeeTypeID == employeeTypeID : 1 > 0)).FirstOrDefault();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SalaryHead, SalaryHead>()
                     .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                     .ForMember(d => d.FieldDesc, o => o.MapFrom(s => s.FieldDesc))
                     .ForMember(d => d.FormulaColumn, o => o.MapFrom(s => s.FormulaColumn))
                     .ForMember(d => d.A, o => o.MapFrom(s => s.A))
                     .ForMember(d => d.Abbreviation, o => o.MapFrom(s => s.Abbreviation))
                     .ForMember(d => d.ActiveField, o => o.MapFrom(s => s.ActiveField))
                     .ForMember(d => d.AttendanceDep, o => o.MapFrom(s => s.AttendanceDep))
                     .ForMember(d => d.C, o => o.MapFrom(s => s.C))
                     .ForMember(d => d.Conditional, o => o.MapFrom(s => s.Conditional))
                     .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                     .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                     .ForMember(d => d.CT, o => o.MapFrom(s => s.CT))
                     .ForMember(d => d.CW, o => o.MapFrom(s => s.CW))
                     .ForMember(d => d.DC, o => o.MapFrom(s => s.DC))
                     .ForMember(d => d.DW, o => o.MapFrom(s => s.DW))
                     .ForMember(d => d.FromMaster, o => o.MapFrom(s => s.FromMaster))
                     .ForMember(d => d.LoanHead, o => o.MapFrom(s => s.LoanHead))
                     .ForMember(d => d.LookUpHead, o => o.MapFrom(s => s.LookUpHead))
                     .ForMember(d => d.LookUpHeadName, o => o.MapFrom(s => s.LookUpHeadName))
                     .ForMember(d => d.LocationDependent, o => o.MapFrom(s => s.LocationDependent))
                     .ForMember(d => d.MonthlyInput, o => o.MapFrom(s => s.MonthlyInput))
                     .ForMember(d => d.MT, o => o.MapFrom(s => s.MT))
                     .ForMember(d => d.RoundingUpto, o => o.MapFrom(s => s.RoundingUpto))
                     .ForMember(d => d.RoundToHigher, o => o.MapFrom(s => s.RoundToHigher))
                     .ForMember(d => d.SeqNo, o => o.MapFrom(s => s.SeqNo))
                     .ForMember(d => d.SpecialField, o => o.MapFrom(s => s.SpecialField))
                     .ForMember(d => d.SpecialFieldMaster, o => o.MapFrom(s => s.SpecialFieldMaster))
                     .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                     .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                     .ForMember(d => d.FixedValue, o => o.MapFrom(s => s.FixedValue))
                     .ForMember(d => d.FixedValueFormula, o => o.MapFrom(s => s.FixedValueFormula))
                     .ForMember(d => d.Slab, o => o.MapFrom(s => s.Slab))
                     .ForMember(d => d.LowerRange, o => o.MapFrom(s => s.LowerRange))
                     .ForMember(d => d.UpperRange, o => o.MapFrom(s => s.UpperRange))
                     .ForMember(d => d.EmployeeTypeID, o => o.MapFrom(s => s.EmployeeTypeID))
                     .ForMember(d => d.CheckHeadInEmpSalTable, o => o.MapFrom(s => s.CheckHeadInEmpSalTable))
                    //.ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                    //.ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))

                    .ForAllOtherMembers(d => d.Ignore());
                });
                salaryHead = Mapper.Map<DTOModel.SalaryHead, Model.SalaryHead>(dtoSalaryHead);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return salaryHead;
        }

        public bool PostSalaryFormData(SalaryHead salHead)
        {
            log.Info($"SalaryHeadRuleService/PostSalaryFormData/");

            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<SalaryHead, DTOModel.SalaryHead>()
                     .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                     .ForMember(d => d.FieldDesc, o => o.MapFrom(s => s.FieldDesc))
                     .ForMember(d => d.FormulaColumn, o => o.MapFrom(s => s.FormulaColumn))
                     .ForMember(d => d.A, o => o.MapFrom(s => s.A))
                     .ForMember(d => d.Abbreviation, o => o.MapFrom(s => s.Abbreviation))
                     .ForMember(d => d.ActiveField, o => o.MapFrom(s => s.ActiveField))
                     .ForMember(d => d.AttendanceDep, o => o.MapFrom(s => s.AttendanceDep))
                     .ForMember(d => d.C, o => o.MapFrom(s => s.C))
                     .ForMember(d => d.Conditional, o => o.MapFrom(s => s.Conditional))
                     .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                     .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                     .ForMember(d => d.FixedValue, o => o.MapFrom(s => s.FixedValue))
                     .ForMember(d => d.FixedValueFormula, o => o.MapFrom(s => s.FixedValueFormula))
                     .ForMember(d => d.Slab, o => o.MapFrom(s => s.Slab))
                     .ForMember(d => d.CT, o => o.MapFrom(s => s.CT))
                     .ForMember(d => d.CW, o => o.MapFrom(s => s.CW))
                     .ForMember(d => d.DC, o => o.MapFrom(s => s.DC))
                     .ForMember(d => d.DW, o => o.MapFrom(s => s.DW))
                     .ForMember(d => d.FromMaster, o => o.MapFrom(s => s.FromMaster))
                     .ForMember(d => d.LoanHead, o => o.MapFrom(s => s.LoanHead))
                     .ForMember(d => d.LookUpHead, o => o.MapFrom(s => s.LookUpHead))
                     .ForMember(d => d.LookUpHeadName, o => o.MapFrom(s => s.LookUpHeadName))
                     .ForMember(d => d.LocationDependent, o => o.MapFrom(s => s.LocationDependent))
                     .ForMember(d => d.MonthlyInput, o => o.MapFrom(s => s.MonthlyInput))
                     .ForMember(d => d.MT, o => o.MapFrom(s => s.MT))
                     .ForMember(d => d.RoundingUpto, o => o.MapFrom(s => s.RoundingUpto))
                     .ForMember(d => d.RoundToHigher, o => o.MapFrom(s => s.RoundToHigher))
                     .ForMember(d => d.SeqNo, o => o.MapFrom(s => s.SeqNo))
                     .ForMember(d => d.SpecialField, o => o.MapFrom(s => s.SpecialField))
                     .ForMember(d => d.SpecialFieldMaster, o => o.MapFrom(s => s.SpecialFieldMaster))
                     .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                     .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                     .ForMember(d => d.LowerRange, o => o.MapFrom(s => s.LowerRange))
                     .ForMember(d => d.UpperRange, o => o.MapFrom(s => s.UpperRange))
                     .ForMember(d => d.EmployeeTypeID, o => o.MapFrom(s => s.EmployeeTypeID))
                     .ForMember(d => d.CheckHeadInEmpSalTable, o => o.MapFrom(s => s.CheckHeadInEmpSalTable))
                     .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoSalaryHead = Mapper.Map<DTOModel.SalaryHead>(salHead);

                if (salHead.ActionType == "Insert")
                    genericRepo.Insert<DTOModel.SalaryHead>(dtoSalaryHead);

                else
                    genericRepo.Update<DTOModel.SalaryHead>(dtoSalaryHead);

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool PostBranchHeadRule(BranchSalaryHeadRule bHeadRule)
        {
            log.Info($"SalaryHeadRuleService/PostBranchHeadRule/");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<BranchSalaryHeadRule, DTOModel.BranchSalaryHeadRule>()
                    .ForMember(d => d.BranchFormulaID, o => o.MapFrom(s => s.BranchFormulaID))
                     .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                     .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                     .ForMember(d => d.FormulaColumn, o => o.MapFrom(s => s.FormulaColumn))
                     .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                     .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                     .ForMember(d => d.FixedValue, o => o.MapFrom(s => s.FixedValue))
                     .ForMember(d => d.FixedValueFormula, o => o.MapFrom(s => s.FixedValueFormula))
                     .ForMember(d => d.LookUpHead, o => o.MapFrom(s => s.LookUpHead))
                     .ForMember(d => d.LookUpHeadName, o => o.MapFrom(s => s.LookUpHeadName))
                     .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                     .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                     .ForMember(d => d.LowerRange, o => o.MapFrom(s => s.LowerRange))
                     .ForMember(d => d.UpperRange, o => o.MapFrom(s => s.UpperRange))
                     .ForMember(d => d.EmployeeTypeID, o => o.MapFrom(s => s.EmployeeTypeID))
                     .ForAllOtherMembers(d => d.Ignore());
                });

                var dtoBranchHeadRule = Mapper.Map<DTOModel.BranchSalaryHeadRule>(bHeadRule);

                if (bHeadRule.ActionType == "Insert")

                    genericRepo.Insert<DTOModel.BranchSalaryHeadRule>(dtoBranchHeadRule);

                else
                    genericRepo.Update<DTOModel.BranchSalaryHeadRule>(dtoBranchHeadRule);

                flag = true;

            }
            catch
            {
                throw;
            }
            return flag;
        }

        public IEnumerable<BranchSalaryHeadRule> GetBranchSalaryHeadRules(string fieldName, int? employeeTypeID)
        {
            log.Info($"SalaryHeadRuleService/GetBranchSalaryHeadRules/{fieldName}");
            IEnumerable<BranchSalaryHeadRule> bHeadRules = Enumerable.Empty<BranchSalaryHeadRule>();
            try
            {
                var dtoBrarchHeadRules = genericRepo.Get<DTOModel.BranchSalaryHeadRule>(x => x.FieldName == fieldName && x.EmployeeTypeID == employeeTypeID && !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.BranchSalaryHeadRule, BranchSalaryHeadRule>()
                     .ForMember(d => d.BranchFormulaID, o => o.MapFrom(s => s.BranchFormulaID))
                     .ForMember(d => d.FieldName, o => o.MapFrom(s => s.FieldName))
                     .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                     .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                     .ForMember(d => d.FormulaColumn, o => o.MapFrom(s => s.FormulaColumn))
                     .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                     .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                     .ForMember(d => d.FixedValue, o => o.MapFrom(s => s.FixedValue))
                     .ForMember(d => d.FixedValueFormula, o => o.MapFrom(s => s.FixedValueFormula))
                     .ForMember(d => d.LookUpHead, o => o.MapFrom(s => s.LookUpHead))
                     .ForMember(d => d.LookUpHeadName, o => o.MapFrom(s => s.LookUpHeadName))
                     .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                     .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                     .ForMember(d => d.LowerRange, o => o.MapFrom(s => s.LowerRange))
                     .ForMember(d => d.UpperRange, o => o.MapFrom(s => s.UpperRange))
                     .ForMember(d => d.EmployeeTypeID, o => o.MapFrom(s => s.EmployeeTypeID))
                     .ForAllOtherMembers(d => d.Ignore());
                });

                bHeadRules = Mapper.Map<IEnumerable<BranchSalaryHeadRule>>(dtoBrarchHeadRules);

                return bHeadRules;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DeleteBranchHeadFieldRule(int branchFormulaID, string fieldName)
        {
            log.Info($"SalaryHeadRuleService/DeleteBranchHeadFieldRule/{branchFormulaID}/{fieldName}");
            bool flag = false;
            try
            {

                var dtoBranchHeadRule = genericRepo.Get<DTOModel.BranchSalaryHeadRule>(x => x.BranchFormulaID == branchFormulaID && x.FieldName == fieldName).FirstOrDefault();
                dtoBranchHeadRule.IsDeleted = true;
                genericRepo.Update<DTOModel.BranchSalaryHeadRule>(dtoBranchHeadRule);
                flag = true;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public BranchSalaryHeadRule GetBranchHeadFieldRule(int bFormulaID)
        {
            log.Info($"SalaryHeadRuleService/GetBranchHeadFieldRule/{bFormulaID}");

            BranchSalaryHeadRule bSalHeadRule = new BranchSalaryHeadRule();

            try
            {
                var dtoSalHeadRule = genericRepo.Get<DTOModel.BranchSalaryHeadRule>(x => x.BranchFormulaID == bFormulaID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.BranchSalaryHeadRule, BranchSalaryHeadRule>();
                });

                bSalHeadRule = Mapper.Map<BranchSalaryHeadRule>(dtoSalHeadRule);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return bSalHeadRule;
        }

        public bool IsBranchHeadRuleExists(string fieldName, int branchID, int? employeeTypeID)
        {
            log.Info($"SalaryHeadRuleService/IsBranchHeadRuleExists/{fieldName}/{branchID}");
            bool flag = false;
            try
            {
                flag = genericRepo.Exists<DTOModel.BranchSalaryHeadRule>(x => !x.IsDeleted
                 && x.FieldName == fieldName && x.BranchID == branchID && x.EmployeeTypeID == employeeTypeID);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        #endregion

        #region  Salary Head Slab Configuration
        public List<SalaryHeadSlab> GetSalaryHeadSlabList(string fieldName)
        {

            log.Info($"SalaryHeadRuleService/GetSalaryHeadSlabList");
            try
            {

                var getSalaryHeadSlab = genericRepo.Get<DTOModel.SlabWiseHeadBreakup>(em => (bool)!em.IsDeleted &&
                (!string.IsNullOrEmpty(fieldName) ? em.FieldName == fieldName : (1 > 0)));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SlabWiseHeadBreakup, Model.SalaryHeadSlab>()
                    .ForMember(c => c.SlabID, c => c.MapFrom(m => m.SlabID))
                    .ForMember(c => c.FieldName, c => c.MapFrom(m => m.FieldName))
                    .ForMember(c => c.LowerRange, c => c.MapFrom(m => m.LowerRange))
                    .ForMember(c => c.UpperRange, c => c.MapFrom(m => m.UpperRange))
                    .ForMember(c => c.Amount, c => c.MapFrom(m => m.Amount))
                    //.ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchID))                  
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listSalaryHeadSlab = Mapper.Map<List<Model.SalaryHeadSlab>>(getSalaryHeadSlab);
                return listSalaryHeadSlab;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public SalaryHeadSlab GetSalaryHeadSlab(SalaryHeadSlab salaryHeadSlab)
        {
            log.Info($"SalaryHeadRuleService/GetSalaryHeadSlab");
            try
            {
                var salaryHeadSlabData = genericRepo.Get<DTOModel.SlabWiseHeadBreakup>(x => x.SlabID == salaryHeadSlab.SlabID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SlabWiseHeadBreakup, Model.SalaryHeadSlab>()
                    .ForMember(c => c.SlabID, c => c.MapFrom(m => m.SlabID))
                    .ForMember(c => c.FieldName, c => c.MapFrom(m => m.FieldName))
                    .ForMember(c => c.LowerRange, c => c.MapFrom(m => m.LowerRange))
                    .ForMember(c => c.UpperRange, c => c.MapFrom(m => m.UpperRange))
                    .ForMember(c => c.Amount, c => c.MapFrom(m => m.Amount))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    //.ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchID))                  
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listSalaryHeadSlab = Mapper.Map<Model.SalaryHeadSlab>(salaryHeadSlabData);
                return listSalaryHeadSlab;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public bool InsertSalaryHeadSlab(SalaryHeadSlab createSalaryHeadSlab)
        {
            log.Info($"BankRatesService/InsertSalaryHeadSlab");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SalaryHeadSlab, DTOModel.SlabWiseHeadBreakup>()
                      .ForMember(c => c.SlabID, c => c.MapFrom(m => m.SlabID))
                     .ForMember(c => c.FieldName, c => c.MapFrom(m => m.FieldName))
                     .ForMember(c => c.LowerRange, c => c.MapFrom(m => m.LowerRange))
                     .ForMember(c => c.UpperRange, c => c.MapFrom(m => m.UpperRange))
                     .ForMember(c => c.Amount, c => c.MapFrom(m => m.Amount))
                    //.ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchID))  
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoHeadSlab = Mapper.Map<DTOModel.SlabWiseHeadBreakup>(createSalaryHeadSlab);
                genericRepo.Insert<DTOModel.SlabWiseHeadBreakup>(dtoHeadSlab);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateSalaryHeadSlab(SalaryHeadSlab updateSalaryHeadSlab)
        {
            log.Info($"BankRatesService/InsertSalaryHeadSlab");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SalaryHeadSlab, DTOModel.SlabWiseHeadBreakup>()
                      .ForMember(c => c.SlabID, c => c.MapFrom(m => m.SlabID))
                     .ForMember(c => c.FieldName, c => c.MapFrom(m => m.FieldName))
                     .ForMember(c => c.LowerRange, c => c.MapFrom(m => m.LowerRange))
                     .ForMember(c => c.UpperRange, c => c.MapFrom(m => m.UpperRange))
                    .ForMember(c => c.Amount, c => c.MapFrom(m => m.Amount))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoHeadSlab = Mapper.Map<DTOModel.SlabWiseHeadBreakup>(updateSalaryHeadSlab);
                genericRepo.Update<DTOModel.SlabWiseHeadBreakup>(dtoHeadSlab);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool SalaryHeadSlabExists(SalaryHeadSlab salaryHeadSlab)
        {
            try
            {
                log.Info($"SalaryHeadRuleService/SalaryHeadSlabExists");
                return genericRepo.Exists<DTOModel.SlabWiseHeadBreakup>(em => em.FieldName == salaryHeadSlab.FieldName && (
                (em.LowerRange == salaryHeadSlab.LowerRange && em.UpperRange == salaryHeadSlab.UpperRange) && em.UpperRange >= salaryHeadSlab.LowerRange));
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(SalaryHeadSlab salaryHeadSlab)
        {
            log.Info($"SalaryHeadRuleService/Delete");

            bool flag = false;
            try
            {
                genericRepo.Delete<DTOModel.SlabWiseHeadBreakup>(salaryHeadSlab.SlabID);
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

        #region Sanction Loan
        public List<Model.SanctionLoan> GetSanctionLoanList()
        {
            log.Info($"SalaryHeadRuleService/GetSanctionLoanList");
            try
            {
                var sanctionLoanList = salaryRepo.GetSanctionLoanList();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetSanctionLoanList_Result, Model.SanctionLoan>()
                    .ForMember(c => c.EmployeeName, c => c.MapFrom(m => m.Name))
                    .ForMember(c => c.PriorityNo, c => c.MapFrom(m => m.PriorityNo))
                    .ForMember(c => c.LoanTypeDesc, c => c.MapFrom(m => m.LoanDesc))
                    .ForMember(c => c.SerialNo, c => c.MapFrom(m => m.SerialNo))
                    .ForMember(c => c.EmpCode, c => c.MapFrom(m => m.EmpCode))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeId))
                    //.ForMember(c => c.LoanSanc, c => c.MapFrom(m => m.LoanSanc == null ? false : m.LoanSanc.Value))
                    .ForMember(c => c.LoanSanction, c => c.MapFrom(m => m.LoanSanc == null ? false : m.LoanSanc.Value))
                    .ForMember(c => c.DateAvailLoan, c => c.MapFrom(m => m.DateAvailLoan))
                      .ForMember(c => c.DateofSanc, c => c.MapFrom(m => m.DateofSanc))
                    .ForMember(c => c.SancAmt, c => c.MapFrom(m => m.SancAmt))
                       .ForMember(c => c.MstLoanID, c => c.MapFrom(m => m.MstLoanID))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listsanctionLoan = Mapper.Map<List<Model.SanctionLoan>>(sanctionLoanList);
                return listsanctionLoan;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public LoanTypeDetail GetLoanTypeDetails(int LoanTypeID)
        {
            log.Info($"SalaryHeadRuleService/GetLoanTypeDetails{LoanTypeID}");
            try
            {
                decimal? LoanRate = 0;
                bool chkRound = false;

                var loanType = genericRepo.Get<DTOModel.tblMstLoanType>(x => x.LoanTypeId == LoanTypeID).FirstOrDefault();

                var MaxLnAmountValue = loanType.MaxLnAmount;
                if (LoanTypeID == 1)
                {
                    LoanRate = genericRepo.Get<DTOModel.tblmstbankrate>(x => !x.IsDeleted).OrderByDescending(x => x.EffectiveDate).FirstOrDefault().PFLoanRate;
                    chkRound = true;
                }
                else
                {
                    LoanRate = genericRepo.Get<DTOModel.tblmstbankrate>(x => !x.IsDeleted).OrderByDescending(x => x.EffectiveDate).FirstOrDefault().BankLoanRate;
                    chkRound = false;
                }
                decimal? ratOfInterest = 0;
                if (genericRepo.Exists<DTOModel.tblmstslab>(x => x.LoanTypeId == LoanTypeID))
                {
                    ratOfInterest = genericRepo.Get<DTOModel.tblmstslab>(x => x.LoanTypeId == LoanTypeID).OrderBy(x => x.EffectiveDate).FirstOrDefault().RateOfInterest;
                }

                LoanTypeDetail loanTypeDetail = new LoanTypeDetail()
                {
                    MaxLnAmount = MaxLnAmountValue.Value,
                    LoanRate = LoanRate.Value,
                    isFloatingRate = loanType.IsOnFloatingRate.Value,
                    isinterestcalc = loanType.IsCalcInterest.Value,
                    isslabdependent = loanType.IsSlabDependent.Value,
                    loanMode = loanType.PaymentType,
                    checkRound = chkRound,
                    rateofInterest = ratOfInterest.Value
                };
                return loanTypeDetail;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public PriorityNoDetails GetLoanNumberDetails(int loanTypeID, string empCode)
        {
            log.Info($"SalaryHeadRuleService/GetLoanNumberDetails{loanTypeID},{empCode}");
            try
            {
                PriorityNoDetails priorityNoDetails = new PriorityNoDetails();
                string strPrefix = "";
                string prno = "";
                int count = 0;
                int result = 0;
                string alertmsg = "";
                string loanName = "";

                var loanTypeDetails = genericRepo.Get<DTOModel.tblMstLoanType>(x => x.LoanTypeId == loanTypeID).FirstOrDefault();

                strPrefix = loanTypeDetails.Abbriviation;
                loanName = loanTypeDetails.LoanDesc;
                prno = strPrefix + "0" + empCode;
                int[] loanType = { 4, 5 };

                count = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.Status == false && x.EmpCode == empCode && loanType.Any(y => y == x.LoanTypeId) && !x.IsDeleted).Count();

                result = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.Status == false && x.PriorityNo == prno && x.LoanTypeId == loanTypeID && !x.IsDeleted).Count();

                if (count > 0 && (loanTypeID == 4 || loanTypeID == 5))
                {
                    alertmsg = "This employee already had a conveyance Loan ! You cannot issue another conveyance loan";
                    priorityNoDetails = new PriorityNoDetails()
                    {
                        Message = alertmsg,
                    };

                    return priorityNoDetails;
                }
                if (result > 0)
                {
                    alertmsg = $"This employee already had {loanName}! You cannot issue a New {loanName} unless and until it is finished";
                    priorityNoDetails = new PriorityNoDetails()
                    {
                        Message = alertmsg,
                    };
                    return priorityNoDetails;
                }
                else
                {
                    priorityNoDetails = new PriorityNoDetails()
                    {
                        PriorityNo = prno,
                        Message = alertmsg,
                        MaxInstallmentP = loanTypeDetails.MaxInstallmentP.Value,
                        MaxInstallmentI = loanTypeDetails.MaxInstallmentI.Value
                    };
                    return priorityNoDetails;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool SaveSanctionLoan(SanctionLoan sanctionLoan)
        {
            log.Info($"SalaryHeadRuleService/SaveSanctionLoan");
            bool flag = false;
            try
            {
                var empCode = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.IsDeleted == false && x.DOLeaveOrg == null && x.EmployeeTypeID == 5 && x.EmployeeId == sanctionLoan.EmployeeId).FirstOrDefault().EmployeeCode;
                var loanType = genericRepo.Get<DTOModel.tblMstLoanType>(x => x.LoanTypeId == sanctionLoan.AssignLoanTypeId).FirstOrDefault().LoanType;
                var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SanctionLoan, DTOModel.tblMstLoanPriority>()
                    //.ForMember(c => c.SerialNo, c => c.UseValue(serialNo))
                    .ForMember(c => c.Branchcode, c => c.MapFrom(m => m.Branchcode))
                    .ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchId))
                    .ForMember(c => c.PriorityNo, c => c.MapFrom(m => m.PriorityNo))
                    .ForMember(c => c.LoanType, c => c.UseValue(loanType))
                    .ForMember(c => c.EmpCode, c => c.UseValue(empCode))
                    .ForMember(c => c.DateofApp, c => c.MapFrom(m => m.DateofApp))
                    .ForMember(c => c.DateRcptApp, c => c.MapFrom(m => m.DateRcptApp))
                    .ForMember(c => c.ReqAmt, c => c.MapFrom(m => m.ReqAmt))
                    .ForMember(c => c.Surety, c => c.MapFrom(m => m.Surety))
                    .ForMember(c => c.LoanSanc, c => c.MapFrom(m => m.LoanSanction))
                    .ForMember(c => c.DateofSanc, c => c.MapFrom(m => m.DateofSanc))
                    .ForMember(c => c.DateAvailLoan, c => c.MapFrom(m => m.DateAvailLoan))
                    .ForMember(c => c.EffDate, c => c.MapFrom(m => m.EffDate))
                    .ForMember(c => c.Reasonref, c => c.MapFrom(m => m.Reasonref))
                    .ForMember(c => c.Dateofref, c => c.MapFrom(m => m.Dateofref))
                    .ForMember(c => c.MaxLoanAmt, c => c.MapFrom(m => m.MaxLoanAmt))
                    .ForMember(c => c.ROI, c => c.MapFrom(m => m.RateOfIntrest))
                    .ForMember(c => c.Asubmitted, c => c.MapFrom(m => m.Asubmitted))
                    .ForMember(c => c.Bsubmitted, c => c.MapFrom(m => m.Bsubmitted))
                    .ForMember(c => c.Csubmitted, c => c.MapFrom(m => m.Csubmitted))
                    .ForMember(c => c.Dsubmitted, c => c.MapFrom(m => m.Dsubmitted))
                    .ForMember(c => c.Esubmitted, c => c.MapFrom(m => m.Esubmitted))
                    .ForMember(c => c.Fsubmitted, c => c.MapFrom(m => m.Fsubmitted))
                    .ForMember(c => c.Gsubmitted, c => c.MapFrom(m => m.Gsubmitted))
                    .ForMember(c => c.Hsubmitted, c => c.MapFrom(m => m.Hsubmitted))
                    .ForMember(c => c.Detail, c => c.MapFrom(m => m.Detail))
                    .ForMember(c => c.LoanMode, c => c.MapFrom(m => m.LoanMode))
                    .ForMember(c => c.IsFloatingRate, c => c.MapFrom(m => m.IsFloatingRate))
                    .ForMember(c => c.IsInterestPayable, c => c.MapFrom(m => m.IsInterestPayable))
                    .ForMember(c => c.OriginalPInstNo, c => c.MapFrom(m => m.OriginalPInstNo))
                    .ForMember(c => c.OriginalIInstNo, c => c.MapFrom(m => m.OriginalIInstNo))
                    .ForMember(c => c.OriginalPinstAmt, c => c.MapFrom(m => m.OriginalPinstAmt))
                    .ForMember(c => c.InterestInstAmt, c => c.MapFrom(m => m.InterestInstAmt))
                    .ForMember(c => c.BalancePAmt, c => c.MapFrom(m => m.BalancePAmt))
                    .ForMember(c => c.BalanceIAmt, c => c.MapFrom(m => m.BalanceIAmt))
                    .ForMember(c => c.TotalBalanceAmt, c => c.MapFrom(m => m.TotalBalanceAmt))
                    .ForMember(c => c.RemainingPInstNo, c => c.MapFrom(m => m.RemainingPInstNo))
                    .ForMember(c => c.RemainingIInstNo, c => c.MapFrom(m => m.RemainingIInstNo))
                    .ForMember(c => c.LastPaidInstDeduDt, c => c.MapFrom(m => m.LastPaidInstDeduDt))
                    .ForMember(c => c.LastPaidPInstAmt, c => c.MapFrom(m => m.LastPaidPInstAmt))
                    .ForMember(c => c.LastPaidPInstNo, c => c.MapFrom(m => m.LastPaidPInstNo))
                    .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                    .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                    .ForMember(c => c.LastMonthInterest, c => c.MapFrom(m => m.LastMonthInterest))
                    .ForMember(c => c.TotalSkippedInst, c => c.MapFrom(m => m.TotalSkippedInst))
                    .ForMember(c => c.CurrentROI, c => c.MapFrom(m => m.CurrentROI))
                    .ForMember(c => c.IsNewLoanAfterDevelop, c => c.MapFrom(m => m.IsNewLoanAfterDevelop))
                    .ForMember(c => c.Status, c => c.MapFrom(m => m.Status))
                    .ForMember(c => c.LastInstAmt, c => c.MapFrom(m => m.LastInstAmt))
                    .ForMember(c => c.AdjustedSancAmt, c => c.MapFrom(m => m.AdjustedSancAmt))
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(m => m.EmployeeId))
                    .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.AssignLoanTypeId))
                    .ForMember(c => c.SancAmt, c => c.MapFrom(m => m.SancAmt))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoSanctionLoan = Mapper.Map<DTOModel.tblMstLoanPriority>(sanctionLoan);
                genericRepo.Insert<DTOModel.tblMstLoanPriority>(dtoSanctionLoan);

                var serialNo = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.SerialNo != null).Max(x => x.SerialNo);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SanctionLoan, DTOModel.tblMstLoanPriorityHistory>()
                    .ForMember(c => c.SerialNo, c => c.UseValue(serialNo))
                    .ForMember(c => c.Branchcode, c => c.MapFrom(m => m.Branchcode))
                    .ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchId))
                    .ForMember(c => c.PriorityNo, c => c.MapFrom(m => m.PriorityNo))
                    .ForMember(c => c.LoanType, c => c.UseValue(loanType))
                    .ForMember(c => c.EmpCode, c => c.UseValue(empCode))
                    .ForMember(c => c.DateofApp, c => c.MapFrom(m => m.DateofApp))
                    .ForMember(c => c.DateRcptApp, c => c.MapFrom(m => m.DateRcptApp))
                    .ForMember(c => c.ReqAmt, c => c.MapFrom(m => m.ReqAmt))
                    .ForMember(c => c.Surety, c => c.MapFrom(m => m.Surety))
                    .ForMember(c => c.LoanSanc, c => c.MapFrom(m => m.LoanSanction))
                    .ForMember(c => c.DateofSanc, c => c.MapFrom(m => m.DateofSanc))
                    .ForMember(c => c.DateAvailLoan, c => c.MapFrom(m => m.DateAvailLoan))
                    .ForMember(c => c.EffDate, c => c.MapFrom(m => m.EffDate))
                    .ForMember(c => c.Reasonref, c => c.MapFrom(m => m.Reasonref))
                    .ForMember(c => c.Dateofref, c => c.MapFrom(m => m.Dateofref))
                    .ForMember(c => c.MaxLoanAmt, c => c.MapFrom(m => m.MaxLoanAmt))
                    .ForMember(c => c.ROI, c => c.MapFrom(m => m.RateOfIntrest))
                    .ForMember(c => c.Asubmitted, c => c.MapFrom(m => m.Asubmitted))
                    .ForMember(c => c.Bsubmitted, c => c.MapFrom(m => m.Bsubmitted))
                    .ForMember(c => c.Csubmitted, c => c.MapFrom(m => m.Csubmitted))
                    .ForMember(c => c.Dsubmitted, c => c.MapFrom(m => m.Dsubmitted))
                    .ForMember(c => c.Esubmitted, c => c.MapFrom(m => m.Esubmitted))
                    .ForMember(c => c.Fsubmitted, c => c.MapFrom(m => m.Fsubmitted))
                    .ForMember(c => c.Gsubmitted, c => c.MapFrom(m => m.Gsubmitted))
                    .ForMember(c => c.Hsubmitted, c => c.MapFrom(m => m.Hsubmitted))
                    .ForMember(c => c.Detail, c => c.MapFrom(m => m.Detail))
                    .ForMember(c => c.LoanMode, c => c.MapFrom(m => m.LoanMode))
                    .ForMember(c => c.IsFloatingRate, c => c.MapFrom(m => m.IsFloatingRate))
                    .ForMember(c => c.IsInterestPayable, c => c.MapFrom(m => m.IsInterestPayable))
                    .ForMember(c => c.OriginalPInstNo, c => c.MapFrom(m => m.OriginalPInstNo))
                    .ForMember(c => c.OriginalIInstNo, c => c.MapFrom(m => m.OriginalIInstNo))
                    .ForMember(c => c.OriginalPinstAmt, c => c.MapFrom(m => m.OriginalPinstAmt))
                    .ForMember(c => c.InterestInstAmt, c => c.MapFrom(m => m.InterestInstAmt))
                    .ForMember(c => c.BalancePAmt, c => c.MapFrom(m => m.BalancePAmt))
                    .ForMember(c => c.BalanceIAmt, c => c.MapFrom(m => m.BalanceIAmt))
                    .ForMember(c => c.TotalBalanceAmt, c => c.MapFrom(m => m.TotalBalanceAmt))
                    .ForMember(c => c.RemainingPInstNo, c => c.MapFrom(m => m.RemainingPInstNo))
                    .ForMember(c => c.RemainingIInstNo, c => c.MapFrom(m => m.RemainingIInstNo))
                    .ForMember(c => c.LastPaidInstDeduDt, c => c.MapFrom(m => m.LastPaidInstDeduDt))
                    .ForMember(c => c.LastPaidPInstAmt, c => c.MapFrom(m => m.LastPaidPInstAmt))
                    .ForMember(c => c.LastPaidPInstNo, c => c.MapFrom(m => m.LastPaidPInstNo))
                    .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                    .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                    .ForMember(c => c.LastMonthInterest, c => c.MapFrom(m => m.LastMonthInterest))
                    .ForMember(c => c.TotalSkippedInst, c => c.MapFrom(m => m.TotalSkippedInst))
                    .ForMember(c => c.CurrentROI, c => c.MapFrom(m => m.CurrentROI))
                    .ForMember(c => c.IsNewLoanAfterDevelop, c => c.MapFrom(m => m.IsNewLoanAfterDevelop))
                    .ForMember(c => c.Status, c => c.MapFrom(m => m.Status))
                    .ForMember(c => c.LastInstAmt, c => c.MapFrom(m => m.LastInstAmt))
                    .ForMember(c => c.AdjustedSancAmt, c => c.MapFrom(m => m.AdjustedSancAmt))
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(m => m.EmployeeId))
                    .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.AssignLoanTypeId))
                    .ForMember(c => c.SancAmt, c => c.MapFrom(m => m.SancAmt))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForMember(c => c.period, c => c.UseValue(period))
                    .ForMember(c => c.RefLoanMstID, c => c.UseValue(dtoSanctionLoan.MstLoanID))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoSanctionLoanHistory = Mapper.Map<DTOModel.tblMstLoanPriorityHistory>(sanctionLoan);
                genericRepo.Insert<DTOModel.tblMstLoanPriorityHistory>(dtoSanctionLoanHistory);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public SanctionLoan GetSanctionLoanByID(string priorityNo, int mstLoanID)
        {
            log.Info($"SalaryHeadRuleService/GetSanctionLoanByID/{priorityNo}");
            try
            {
                var sanctionObj = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == priorityNo && x.MstLoanID == mstLoanID && !x.IsDeleted).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblMstLoanPriority, Model.SanctionLoan>()
                    .ForMember(c => c.SerialNo, c => c.MapFrom(m => m.SerialNo))
                    .ForMember(c => c.Branchcode, c => c.MapFrom(m => m.Branchcode))
                    .ForMember(c => c.BranchId, c => c.MapFrom(m => m.BranchID))
                    .ForMember(c => c.PriorityNo, c => c.MapFrom(m => m.PriorityNo))
                    .ForMember(c => c.EmpCode, c => c.MapFrom(m => m.EmpCode))
                    .ForMember(c => c.DateofApp, c => c.MapFrom(m => m.DateofApp))
                    .ForMember(c => c.DateRcptApp, c => c.MapFrom(m => m.DateRcptApp))
                    .ForMember(c => c.ReqAmt, c => c.MapFrom(m => m.ReqAmt))
                    .ForMember(c => c.Surety, c => c.MapFrom(m => m.Surety))
                    .ForMember(c => c.LoanSanction, c => c.MapFrom(m => m.LoanSanc))
                    .ForMember(c => c.DateofSanc, c => c.MapFrom(m => m.DateofSanc))
                    .ForMember(c => c.DateAvailLoan, c => c.MapFrom(m => m.DateAvailLoan))
                    .ForMember(c => c.EffDate, c => c.MapFrom(m => m.EffDate))
                    .ForMember(c => c.Reasonref, c => c.MapFrom(m => m.Reasonref))
                    .ForMember(c => c.Dateofref, c => c.MapFrom(m => m.Dateofref))
                    .ForMember(c => c.MaxLoanAmt, c => c.MapFrom(m => m.MaxLoanAmt))
                    .ForMember(c => c.ROI, c => c.MapFrom(m => m.ROI))
                    .ForMember(c => c.Asubmitted, c => c.MapFrom(m => m.Asubmitted))
                    .ForMember(c => c.Bsubmitted, c => c.MapFrom(m => m.Bsubmitted))
                    .ForMember(c => c.Csubmitted, c => c.MapFrom(m => m.Csubmitted))
                    .ForMember(c => c.Dsubmitted, c => c.MapFrom(m => m.Dsubmitted))
                    .ForMember(c => c.Esubmitted, c => c.MapFrom(m => m.Esubmitted))
                    .ForMember(c => c.Fsubmitted, c => c.MapFrom(m => m.Fsubmitted))
                    .ForMember(c => c.Gsubmitted, c => c.MapFrom(m => m.Gsubmitted))
                    .ForMember(c => c.Hsubmitted, c => c.MapFrom(m => m.Hsubmitted))
                    .ForMember(c => c.Detail, c => c.MapFrom(m => m.Detail))
                    .ForMember(c => c.LoanMode, c => c.MapFrom(m => m.LoanMode))
                    .ForMember(c => c.IsFloatingRate, c => c.MapFrom(m => m.IsFloatingRate))
                    .ForMember(c => c.IsInterestPayable, c => c.MapFrom(m => m.IsInterestPayable))
                    .ForMember(c => c.OriginalPInstNo, c => c.MapFrom(m => m.OriginalPInstNo == null ? 0 : m.OriginalPInstNo))
                    .ForMember(c => c.OriginalIInstNo, c => c.MapFrom(m => m.OriginalIInstNo == null ? 0 : m.OriginalIInstNo))
                    .ForMember(c => c.OriginalPinstAmt, c => c.MapFrom(m => m.OriginalPinstAmt == null ? 0 : m.OriginalPinstAmt))
                    .ForMember(c => c.OriginalPrincipleInstallments, c => c.MapFrom(m => m.OriginalPInstNo == null ? 0 : m.OriginalPInstNo))
                    .ForMember(c => c.OriginalInterestInstallments, c => c.MapFrom(m => m.OriginalIInstNo == null ? 0 : m.OriginalIInstNo))
                    .ForMember(c => c.InterestInstAmt, c => c.MapFrom(m => m.InterestInstAmt))
                    .ForMember(c => c.BalancePAmt, c => c.MapFrom(m => m.BalancePAmt))
                    .ForMember(c => c.BalanceIAmt, c => c.MapFrom(m => m.BalanceIAmt))
                    .ForMember(c => c.TotalBalanceAmt, c => c.MapFrom(m => m.TotalBalanceAmt))
                    .ForMember(c => c.RemainingPInstNo, c => c.MapFrom(m => m.RemainingPInstNo))
                    .ForMember(c => c.RemainingIInstNo, c => c.MapFrom(m => m.RemainingIInstNo))
                    .ForMember(c => c.LastPaidInstDeduDt, c => c.MapFrom(m => m.LastPaidInstDeduDt))
                    .ForMember(c => c.LastPaidPInstAmt, c => c.MapFrom(m => m.LastPaidPInstAmt))
                    .ForMember(c => c.LastPaidPInstNo, c => c.MapFrom(m => m.LastPaidPInstNo))
                    .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                    .ForMember(c => c.LastPaidIInstNo, c => c.MapFrom(m => m.LastPaidIInstNo))
                    .ForMember(c => c.LastMonthInterest, c => c.MapFrom(m => m.LastMonthInterest))
                    .ForMember(c => c.TotalSkippedInst, c => c.MapFrom(m => m.TotalSkippedInst))
                    .ForMember(c => c.CurrentROI, c => c.MapFrom(m => m.CurrentROI))
                    .ForMember(c => c.RateOfIntrest, c => c.MapFrom(m => m.ROI))
                    .ForMember(c => c.IsNewLoanAfterDevelop, c => c.MapFrom(m => m.IsNewLoanAfterDevelop))
                    .ForMember(c => c.Status, c => c.MapFrom(m => m.Status))
                    .ForMember(c => c.LastInstAmt, c => c.MapFrom(m => m.LastInstAmt))
                    //.ForMember(c => c.BkpLastInstAmt, c => c.MapFrom(m => m.BkpLastInstAmt))
                    .ForMember(c => c.AdjustedSancAmt, c => c.MapFrom(m => m.AdjustedSancAmt))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(m => m.EmployeeID))
                    .ForMember(c => c.LoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                    //.ForMember(c => c.DocAsubmitted, c => c.MapFrom(m => m.DocAsubmitted))
                    //.ForMember(c => c.DocBsubmitted, c => c.MapFrom(m => m.DocBsubmitted))
                    //.ForMember(c => c.DocCsubmitted, c => c.MapFrom(m => m.DocCsubmitted))
                    //.ForMember(c => c.DocDsubmitted, c => c.MapFrom(m => m.DocDsubmitted))
                    //.ForMember(c => c.DocEsubmitted, c => c.MapFrom(m => m.DocEsubmitted))
                    //.ForMember(c => c.DocFsubmitted, c => c.MapFrom(m => m.DocFsubmitted))
                    //.ForMember(c => c.DocGsubmitted, c => c.MapFrom(m => m.DocGsubmitted))
                    //.ForMember(c => c.DocHsubmitted, c => c.MapFrom(m => m.DocHsubmitted)) DateAvailLoan1
                    .ForMember(c => c.AssignLoanTypeId, c => c.MapFrom(m => m.LoanTypeId))
                    .ForMember(c => c.DateAvailLoan1, c => c.MapFrom(m => m.DateAvailLoan.ToString("yyyy-MM-dd")))
                    .ForMember(c => c.SancAmt, c => c.MapFrom(m => m.SancAmt))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblMstLoanPriority, Model.SanctionLoan>(sanctionObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DeleteSanction(string priorityNo, int mstLoanID, int updatedBy)
        {
            log.Info($"SalaryHeadRuleService/Delete/{priorityNo}");
            bool flag = false;
            try
            {
                var getLoanEntry = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.MstLoanID == mstLoanID && !x.IsDeleted).FirstOrDefault();
                if (getLoanEntry != null)
                {
                    getLoanEntry.IsDeleted = true;
                    getLoanEntry.UpdatedBy = updatedBy;
                    getLoanEntry.UpdatedOn = DateTime.Now;
                    getLoanEntry.IsNewLoanAfterDevelop = false;
                    genericRepo.Update(getLoanEntry);
                    var getLoanHtrEntry = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.RefLoanMstID == mstLoanID && !x.IsDeleted).ToList();
                    if (getLoanHtrEntry != null && getLoanHtrEntry.Count > 0)
                    {
                        foreach (var item in getLoanHtrEntry)
                        {
                            item.IsDeleted = true;
                            item.IsNewLoanAfterDevelop = false;
                            item.UpdatedBy = updatedBy;
                            item.UpdatedOn = DateTime.Now;
                            genericRepo.Update(item);
                        }
                    }
                }
                // flag = salaryRepo.DeleteSanction(priorityNo, DateAvailLoan);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateSanctionLoan(SanctionLoan sanctionLoan)
        {
            log.Info($"SalaryHeadRuleService/UpdateSanctionLoan");
            bool flag = false;
            var period = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>().Max(x => x.period);
            try
            {
                var sanctionObj = genericRepo.Get<DTOModel.tblMstLoanPriority>(x => x.PriorityNo == sanctionLoan.PriorityNo /*&& x.DateAvailLoan == sanctionLoan.DateAvailLoan*/&& x.SerialNo == sanctionLoan.SerialNo && !x.IsDeleted).FirstOrDefault();
                if (sanctionObj != null)
                {
                    sanctionObj.Surety = sanctionLoan.Surety;
                    sanctionObj.DateofApp = sanctionLoan.DateofApp;
                    sanctionObj.DateRcptApp = sanctionLoan.DateRcptApp;
                    sanctionObj.Detail = sanctionLoan.Detail;
                    sanctionObj.Asubmitted = sanctionLoan.Asubmitted;
                    sanctionObj.Bsubmitted = sanctionLoan.Bsubmitted;
                    sanctionObj.Csubmitted = sanctionLoan.Csubmitted;
                    sanctionObj.Dsubmitted = sanctionLoan.Dsubmitted;
                    sanctionObj.Esubmitted = sanctionLoan.Esubmitted;
                    sanctionObj.Fsubmitted = sanctionLoan.Fsubmitted;
                    sanctionObj.Gsubmitted = sanctionLoan.Gsubmitted;
                    sanctionObj.Hsubmitted = sanctionLoan.Hsubmitted;
                    sanctionObj.UpdatedBy = sanctionLoan.UpdatedBy;
                    sanctionObj.UpdatedOn = sanctionLoan.UpdatedOn;
                    genericRepo.Update<DTOModel.tblMstLoanPriority>(sanctionObj);
                }
                var sanctionObjHistory = genericRepo.Get<DTOModel.tblMstLoanPriorityHistory>(x => x.PriorityNo == sanctionLoan.PriorityNo && x.SerialNo == sanctionLoan.SerialNo && x.period == period && !x.IsDeleted).FirstOrDefault();
                if (sanctionObjHistory != null)
                {
                    sanctionObjHistory.Surety = sanctionLoan.Surety;
                    sanctionObjHistory.DateofApp = sanctionLoan.DateofApp;
                    sanctionObjHistory.DateRcptApp = sanctionLoan.DateRcptApp;
                    sanctionObjHistory.Detail = sanctionLoan.Detail;
                    sanctionObjHistory.Asubmitted = sanctionLoan.Asubmitted;
                    sanctionObjHistory.Bsubmitted = sanctionLoan.Bsubmitted;
                    sanctionObjHistory.Csubmitted = sanctionLoan.Csubmitted;
                    sanctionObjHistory.Dsubmitted = sanctionLoan.Dsubmitted;
                    sanctionObjHistory.Esubmitted = sanctionLoan.Esubmitted;
                    sanctionObjHistory.Fsubmitted = sanctionLoan.Fsubmitted;
                    sanctionObjHistory.Gsubmitted = sanctionLoan.Gsubmitted;
                    sanctionObjHistory.Hsubmitted = sanctionLoan.Hsubmitted;
                    sanctionObjHistory.UpdatedBy = sanctionLoan.UpdatedBy;
                    sanctionObjHistory.UpdatedOn = sanctionLoan.UpdatedOn;
                    genericRepo.Update<DTOModel.tblMstLoanPriorityHistory>(sanctionObjHistory);
                }
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion Sanction Loan

        #region Monthly Input

        public bool CheckMonthlyInputDetails(int currentYear, int currentMonth, int PreYear, int PreMonth)
        {
            log.Info($"SalaryHeadRuleService/CheckMonthlyInputDetails");
            bool flag = false;
            try
            {
                var monthlyInputCount = genericRepo.Get<DTOModel.TBLMONTHLYINPUT>(x => x.SalYear == currentYear && x.SalMonth == currentMonth);
                if (monthlyInputCount.Count() <= 0)
                {
                    var save = salaryRepo.InsertMonthlyInput(Convert.ToInt16(currentYear), Convert.ToInt16(currentMonth), Convert.ToInt16(PreYear), Convert.ToInt16(PreMonth));
                    UpdateSalaryHeadsInMonthlyInput(Convert.ToInt16(currentMonth), Convert.ToInt16(currentYear));
                }
                // commented by ibrhim, because this code override the professional tax to 0 at export time for the same month,which user provide input
                //   ISalaryRepository gSRepo = new SalaryRepository();
                // var getBranchCode = gSRepo.GetBranchCodeDetails(Convert.ToInt16(PreMonth), Convert.ToInt16(PreYear));


                //foreach (DataRow row in getBranchCode.Rows)
                //{
                //    var EmployeeCode = Convert.ToString(row["employeecode"]);
                //    var BranchId = Convert.ToInt32(row["BranchID"]);
                //    var updateBranchId = genericRepo.Get<DTOModel.TBLMONTHLYINPUT>(x => x.SalYear == currentYear && x.SalMonth == currentMonth && x.EmployeeCode == EmployeeCode).FirstOrDefault();
                //    if (updateBranchId != null)
                //    {
                //        updateBranchId.BranchId = BranchId;
                //        updateBranchId.D_02 = 0;
                //        updateBranchId.E_SP = 0;
                //        genericRepo.Update<DTOModel.TBLMONTHLYINPUT>(updateBranchId);
                //    }
                //}

                //    for (int i = 0; i < getBranchCode.Count; i++)
                //{
                //    var updateBranchId = genericRepo.Get<DTOModel.TBLMONTHLYINPUT>(x => x.SalYear == currentYear && x.SalMonth == currentMonth && x.EmployeeId == getBranchCode[i].EmployeeID).FirstOrDefault();
                //    updateBranchId.BranchId = getBranchCode[i].BranchID;
                //    genericRepo.Update<DTOModel.TBLMONTHLYINPUT>(updateBranchId);
                //}
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateSalaryHeadsInMonthlyInput(Int16 currentMonth, Int16 currentYear)
        {
            log.Info($"SalaryHeadRuleService/UpdateSalaryHeadsInMonthlyInput");
            bool flag = false;
            try
            {
                var salaryHeadCount = genericRepo.Get<DTOModel.SalaryHead>(x => x.ActiveField == true && x.FromMaster == false && x.MonthlyInput == true && !x.IsDeleted).ToList();
                for (int i = 0; i < salaryHeadCount.Count(); i++)
                {
                    salaryRepo.UpdateSalaryHeadsInMonthlyInput(salaryHeadCount[i].FieldName, Convert.ToInt16(currentMonth), Convert.ToInt16(currentYear), "M");
                }
                salaryRepo.UpdateSalaryHeadsInMonthlyInput("", Convert.ToInt16(currentMonth), Convert.ToInt16(currentYear), "I");
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public DataTable GetBranchDetails(string SalaryHead, int EmployeeId, int currentMonth, int currentYear)
        {
            log.Info($"SalaryHeadRuleService/GetBranchDetails");
            try
            {
                var ss = salaryRepo.GetBranchDetails(SalaryHead, Convert.ToInt16(currentMonth), Convert.ToInt16(currentYear), EmployeeId);
                return ss;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateSalaryMonthlyInput(Model.SalaryMonthlyInput salaryMonthlyInput)
        {
            log.Info($"SalaryHeadRuleService/UpdateSalaryMonthlyInput");
            bool flag = false;
            try
            {
                //var currentMonth = DateTime.Now.Month;
                //var currentYear = DateTime.Now.Year;
                if (genericRepo.Exists<DTOModel.TBLMONTHLYINPUT>(x => x.SalMonth == salaryMonthlyInput.monthId && x.SalYear == salaryMonthlyInput.yearId && x.EmployeeId == salaryMonthlyInput.EmployeeId))
                {
                    var result = salaryRepo.UpdateSalaryMonthlyInput(salaryMonthlyInput.EmployeeId, salaryMonthlyInput.MonthlyInputHeadId, salaryMonthlyInput.Amount, salaryMonthlyInput.monthId, salaryMonthlyInput.yearId);
                    flag = true;
                }
                else
                {
                    //==== Insert Row into Monthly Input table ======= added On - 24-Mar-2021

                    var empl = genericRepo.Get<DTOModel.tblMstEmployee>(x =>
                       x.EmployeeId == salaryMonthlyInput.EmployeeId).FirstOrDefault();

                    DTOModel.TBLMONTHLYINPUT mInput = new DTOModel.TBLMONTHLYINPUT();
                    mInput.EmployeeId = salaryMonthlyInput.EmployeeId;
                    mInput.EmployeeCode = empl.EmployeeCode;
                    mInput.BranchId = empl.BranchID;
                    mInput.BranchCode = empl.Branch.BranchCode;
                    mInput.CreatedBy = 1;
                    mInput.CreatedOn = DateTime.Now;
                    mInput.DeductNB = false;
                    mInput.DeductTCS = false;
                    mInput.DeductHouseLoan = false;
                    mInput.DeductFestivalLoan = false;
                    mInput.DeductCarLoan = false;
                    mInput.DeductScooterLoan = false;
                    mInput.DeductSundryAdv = false;

                    mInput.SalMonth = (byte)salaryMonthlyInput.monthId;
                    mInput.SalYear = (short)salaryMonthlyInput.yearId;
                    var mInputProperty = mInput.GetType().GetProperty(salaryMonthlyInput.MonthlyInputHeadId,
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                    mInputProperty.SetValue(mInput, salaryMonthlyInput.Amount, null);


                    genericRepo.Insert<DTOModel.TBLMONTHLYINPUT>(mInput);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public decimal? GetMedicalReimbursement(int EmployeeId, string fromYear, string toYear)
        {
            log.Info($"SalaryHeadRuleService/GetMedicalReimbursement");
            try
            {
                var result = salaryRepo.GetMedicalReimbursement(EmployeeId, fromYear, toYear);
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetEmployeeList(int empTypeID, int salMonth, int salYear)
        {
            log.Info($"SalaryHeadRuleService/GetEmployeeList");

            var dtoEmployees = genericRepo.Get<DTOModel.tblMstEmployee>(x => !x.IsDeleted
              && x.EmployeeTypeID == empTypeID &&
              (x.DOLeaveOrg == null || (x.DOLeaveOrg.Value.Month == salMonth && x.DOLeaveOrg.Value.Year == salYear))
            );

            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<DTOModel.tblMstEmployee, Model.SelectListModel>()
               .ForMember(d => d.id, o => o.MapFrom(s => s.EmployeeId))
               .ForMember(d => d.value, o => o.MapFrom(s => ($"{s.EmployeeCode}-{s.Name}")));
            });

            return Mapper.Map<List<SelectListModel>>(dtoEmployees);
        }

        public bool GetFinancialMonthlyDetails(int selectedMonth, int selectedYear, int employeeId)
        {
            log.Info($"SalaryHeadRuleService/GetFinancialMonthlyDetails");
            bool flag = false;
            try
            {
                var result = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.SalMonth == selectedMonth && x.SalYear == selectedYear && x.EmployeeID == employeeId).FirstOrDefault();
                if (result != null)
                    flag = result.Publish;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region Individual Head Report
        public bool GeneralReports(string branchCode, int month, int year, string headName, string headDesc, string slcode, string payslipno, int employeeTypeId)
        {
            log.Info($"SalaryHeadRuleService/GeneralReports");
            try
            {
                bool flag = false;
                flag = salaryRepo.GeneralReports(branchCode, month, year, headName, headDesc, slcode, payslipno, employeeTypeId);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        public bool GeneralReportsSummation(string branchcode, string salmonthF, string salyearF, string salmonthT, string salyearT, List<string> lst, int empType)
        {
            log.Info($"SalaryHeadRuleService/GeneralReportsSummation");
            try
            {
                bool flag = false;
                flag = salaryRepo.GeneralReportsSummation(branchcode,salmonthF,salyearF,salmonthT,salyearT,lst,empType);
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
