using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using System.Data;

namespace Nafed.MicroPay.Services
{
    public class BonusWagesService : BaseService, IBonusWagesService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IBonusRepository bonusRepo;
        public BonusWagesService(IGenericRepository genericRepo ,IBonusRepository bonusRepo)
        {
            this.genericRepo = genericRepo;
            this.bonusRepo = bonusRepo;
        }

        #region BonusMaster

        public List<Model.BonusWages> GetBonusWagesList()
        {
            log.Info($"BonusWagesService/GetBonusWagesList");
            try
            {
                var result = genericRepo.Get<DTOModel.BonusWage>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.BonusWage, Model.BonusWages>()
                     .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.Restricted_Salary_as_Per_bonus, c => c.MapFrom(s => s.Restricted_Salary_as_Per_bonus))
                    .ForMember(c => c.Minimum_monthly_wages, c => c.MapFrom(s => s.Minimum_monthly_wages))
                    .ForMember(c => c.From_date, c => c.MapFrom(s => s.From_date))
                      .ForMember(c => c.To_date, c => c.MapFrom(s => s.To_date))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBW = Mapper.Map<List<Model.BonusWages>>(result);
                return listBW.OrderBy(x => x.ID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool BonusWagesExists(DateTime? fromdate, DateTime? todate)
        {
            log.Info($"BonusWagesService/BonusWagesExists/{fromdate}/{todate}");
            return genericRepo.Exists<DTOModel.BonusWage>(x => ((fromdate >= x.From_date && todate <= x.To_date) || (todate >= x.From_date && todate <= x.To_date)));
        }
        public bool UpdateBonusWages(Model.BonusWages editBonusWages)
        {
            log.Info($"BonusWagesService/UpdateBonusWages");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.BonusWage>(editBonusWages.ID);
                dtoObj.Restricted_Salary_as_Per_bonus = editBonusWages.Restricted_Salary_as_Per_bonus;
                dtoObj.UpdatedBy = editBonusWages.UpdatedBy;
                dtoObj.UpdatedOn = editBonusWages.UpdatedOn;
                genericRepo.Update<DTOModel.BonusWage>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool InsertBonusWages(Model.BonusWages createBonusWages)
        {
            log.Info($"BonusWagesService/InsertBonusWages");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.BonusWages, DTOModel.BonusWage>()
                     .ForMember(c => c.From_date, c => c.MapFrom(m => m.From_date))
                    .ForMember(c => c.To_date, c => c.MapFrom(m => m.To_date))
                    .ForMember(c => c.Restricted_Salary_as_Per_bonus, c => c.MapFrom(m => m.Restricted_Salary_as_Per_bonus))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                   .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoBW = Mapper.Map<DTOModel.BonusWage>(createBonusWages);
                genericRepo.Insert<DTOModel.BonusWage>(dtoBW);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Model.BonusWages GetBonusWagesByID(int ID)
        {
            log.Info($"BonusWagesService/GetBonusWagesByID/ {ID}");
            try
            {
                var Obj = genericRepo.GetByID<DTOModel.BonusWage>(ID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.BonusWage, Model.BonusWages>()
                      .ForMember(c => c.From_date, c => c.MapFrom(m => m.From_date))
                    .ForMember(c => c.To_date, c => c.MapFrom(m => m.To_date))
                     .ForMember(c => c.Restricted_Salary_as_Per_bonus, c => c.MapFrom(m => m.Restricted_Salary_as_Per_bonus))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.BonusWage, Model.BonusWages>(Obj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int ID)
        {
            log.Info($"BonusWagesService/Delete/{ID}");
            bool flag = false;
            try
            {
                DTOModel.BonusWage dtoBW = new DTOModel.BonusWage();
                dtoBW = genericRepo.GetByID<DTOModel.BonusWage>(ID);
                dtoBW.IsDeleted = true;
                genericRepo.Update<DTOModel.BonusWage>(dtoBW);
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


        #region BonusMinimumWages

        public List<Model.BonusWages> GetBonusMiimumWagesList()
        {
            log.Info($"BonusWagesService/GetBonusMiimumWagesList");
            try
            {
                var result = genericRepo.Get<DTOModel.BonusMinimumMonthlyWage>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.BonusMinimumMonthlyWage, Model.BonusWages>()
                     .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.designationName, c => c.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(c => c.Minimum_monthly_wages, c => c.MapFrom(s => s.Minimum_monthly_wages))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBW = Mapper.Map<List<Model.BonusWages>>(result);
                return listBW.OrderBy(x => x.ID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public Model.BonusWages GetBonusminimumWagesByID(int ID)
        {
            log.Info($"BonusWagesService/GetBonusminimumWagesByID/ {ID}");
            try
            {
                var Obj = genericRepo.GetByID<DTOModel.BonusMinimumMonthlyWage>(ID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.BonusMinimumMonthlyWage, Model.BonusWages>()
                      .ForMember(c => c.designationID, c => c.MapFrom(m => m.DesignationID))
                        .ForMember(c => c.designationName, c => c.MapFrom(m => m.Designation.DesignationName))
                     .ForMember(c => c.Minimum_monthly_wages, c => c.MapFrom(m => m.Minimum_monthly_wages))
                     .ForMember(c => c.IsDeleted, c => c.MapFrom(m => m.IsDeleted))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.BonusMinimumMonthlyWage, Model.BonusWages>(Obj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateBonusMinimumWages(Model.BonusWages editBonusWages)
        {
            log.Info($"BonusWagesService/UpdateBonusMinimumWages");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.BonusMinimumMonthlyWage>(editBonusWages.ID);
                dtoObj.Minimum_monthly_wages = editBonusWages.Minimum_monthly_wages;
                dtoObj.UpdatedBy = editBonusWages.UpdatedBy;
                dtoObj.UpdatedOn = editBonusWages.UpdatedOn;
                genericRepo.Update<DTOModel.BonusMinimumMonthlyWage>(dtoObj);
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

        #region CalculateBonus


        public List<Model.BonusWages> GetBonusList(int fyear,int? branchID,int EmpTypeID)
        {
            log.Info($"BonusWagesService/GetExgratiaList");
            try
            {
                var result = (dynamic)null;
                if (fyear != 0)
                {
                    result = genericRepo.Get<DTOModel.TblBonusAmt>(x => !x.IsDeleted &&  x.Year== fyear && (branchID > 0 ? x.BranchID == branchID : 1 > 0) && (EmpTypeID > 0 ? x.EmployeeTypeID == EmpTypeID : 1 > 0));
                }
                else
                {
                    result = genericRepo.Get<DTOModel.TblBonusAmt>(x => !x.IsDeleted);
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblBonusAmt, Model.BonusWages>()

                      .ForMember(c => c.ID, c => c.MapFrom(s => s.BonusAmtID))
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.Empcode))
                    .ForMember(c => c.Name, c => c.MapFrom(s => s.tblMstEmployee.Name))
                      .ForMember(c => c.branchName, c => c.MapFrom(s => s.Branch.BranchName))
                    .ForMember(c => c.ActBonus_Amt, c => c.MapFrom(s => s.ActBonus_Amt))
                     .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                      .ForMember(c => c.Days, c => c.MapFrom(s => s.Days))
                    .ForMember(c => c.FinancialYear, c => c.MapFrom(s => s.Year))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var list = Mapper.Map<List<Model.BonusWages>>(result);
                return list;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int calculateBonus(decimal bonusrate, string fromYear, string toYear, int branchID, int fYear,int empType)
        {
            int flag = 0;
            log.Info($"BonusWagesService/calculateBonus");
            try
            {
                flag = bonusRepo.calculateBonus(bonusrate, fromYear, toYear, branchID, fYear, empType);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;

        }

        public bool DeleteBonusAmount(int ID,int UserID)
        {
            log.Info($"ExgratiaService/Delete/{ID}");
            bool flag = false;
            try
            {
                DTOModel.TblBonusAmt dtoBonus = new DTOModel.TblBonusAmt();
                dtoBonus = genericRepo.GetByID<DTOModel.TblBonusAmt>(ID);
                dtoBonus.IsDeleted = true;
                dtoBonus.UpdatedBy = UserID;
                dtoBonus.UpdatedOn = DateTime.Now;
                genericRepo.Update<DTOModel.TblBonusAmt>(dtoBonus);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public DataTable GetBonusExport(string fromYear, string toYear, int fyear, int branchID, int emptype)
        {
            DataTable dt = new DataTable();
            log.Info($"ExgratiaService/GetBonusExport");
            try
            {
                dt = bonusRepo.GetBonusExport(fromYear, toYear, fyear, branchID, emptype);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dt;

        }


        #endregion
    }
}
