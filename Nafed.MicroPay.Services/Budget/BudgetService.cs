using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using DTOModel = Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Nafed.MicroPay.Services.Budget
{
    public class BudgetService : BaseService, IBudgetService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IBudgetRepository budgetRepo;
        public BudgetService(IGenericRepository genericRepo, IBudgetRepository budgetRepo)
        {
            this.genericRepo = genericRepo;
            this.budgetRepo = budgetRepo;
        }

        #region Configure Budget
        public PromotionCota GetPromotionDetails(int designationID)
        {
            log.Info($"BudgetService/GetPromotionDetails");
            try
            {
                var promotionDetails = genericRepo.Get<DTOModel.promotioncota1>(x => x.DesignationID == designationID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.promotioncota1, Model.PromotionCota>()
                    .ForMember(d => d.PromotionCotaId, o => o.MapFrom(s => s.PromotionCotaId))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                    .ForMember(d => d.Promotion, o => o.MapFrom(s => s.Promotion == null ? 0 : s.Promotion))
                    .ForMember(d => d.Direct, o => o.MapFrom(s => s.Direct == null ? 0 : s.Direct))
                    .ForMember(d => d.LCT, o => o.MapFrom(s => s.LCT == null ? 0 : s.LCT))
                    .ForMember(d => d.NPromotion, o => o.MapFrom(s => s.NPromotion == null ? 0 : s.NPromotion))
                   .ForMember(d => d.NDirect, o => o.MapFrom(s => s.NDirect == null ? 0 : s.NDirect))
                   .ForMember(d => d.NLCT, o => o.MapFrom(s => s.NLCT == null ? 0 : s.NLCT))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var promotionDetailsByDesignation = Mapper.Map<Model.PromotionCota>(promotionDetails);
                return promotionDetailsByDesignation;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdatePromotionDetails(PromotionCota promotionCota)
        {
            log.Info($"BudgetService/UpdatePromotionDetails");
            bool flag = false;
            try
            {
                var dtopromotionDetails = genericRepo.Get<DTOModel.promotioncota1>(x => x.DesignationID == promotionCota.DesignationID).FirstOrDefault();
                if (dtopromotionDetails != null)
                {
                    dtopromotionDetails.Promotion = promotionCota.Promotion;
                    dtopromotionDetails.Direct = promotionCota.Direct;
                    dtopromotionDetails.LCT = promotionCota.LCT;
                    dtopromotionDetails.UpdatedBy = promotionCota.UpdatedBy;
                    dtopromotionDetails.UpdatedOn = DateTime.Now;
                    dtopromotionDetails.NPromotion = promotionCota.NPromotion;
                    dtopromotionDetails.NDirect = promotionCota.NDirect;
                    dtopromotionDetails.NLCT = promotionCota.NLCT;
                    genericRepo.Update<DTOModel.promotioncota1>(dtopromotionDetails);
                    flag = true;
                }
                else
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.PromotionCota, DTOModel.promotioncota1>()
                        .ForMember(c => c.DesignationID, c => c.MapFrom(m => m.DesignationID))
                        .ForMember(c => c.Promotion, c => c.MapFrom(m => m.Promotion))
                        .ForMember(c => c.Direct, c => c.MapFrom(m => m.Direct))
                        .ForMember(c => c.LCT, c => c.MapFrom(m => m.LCT))
                        .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.UpdatedBy))
                        .ForMember(c => c.CreatedOn, c => c.UseValue(DateTime.Now))
                         .ForMember(c => c.NPromotion, c => c.MapFrom(m => m.NPromotion))
                       .ForMember(c => c.NDirect, c => c.MapFrom(m => m.NDirect))
                       .ForMember(c => c.NLCT, c => c.MapFrom(m => m.NLCT))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    var dtoPromotion = Mapper.Map<DTOModel.promotioncota1>(promotionCota);
                    genericRepo.Insert<DTOModel.promotioncota1>(dtoPromotion);
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
        #endregion

        #region Staff Budget
        public List<StaffBudget> GetStaffBudgetList(string year, int? designationID)
        {
            log.Info($"BudgetService/GetStaffBudgetList");
            try
            {
                var staffBudgetDetails = budgetRepo.GetStaffBudgetList(year, designationID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetStaffBudgetDetailsList_Result, Model.StaffBudget>()
                    .ForMember(d => d.StaffBudgetId, o => o.MapFrom(s => s.StaffBudgetId))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation))
                    .ForMember(d => d.Year, o => o.MapFrom(s => s.year))
                    .ForMember(d => d.Month, o => o.MapFrom(s => s.month))
                    .ForMember(d => d.BudgetedStaff, o => o.MapFrom(s => s.Budgeted_Staff))
                    .ForMember(d => d.PresentStaff, o => o.MapFrom(s => s.Present_Staff))
                    .ForMember(d => d.Vac, o => o.MapFrom(s => s.Vacancies))
                    .ForMember(d => d.FLTC, o => o.MapFrom(s => s.LTC))
                    .ForMember(d => d.FPromotion, o => o.MapFrom(s => s.Promotion))
                    .ForMember(d => d.FDirect, o => o.MapFrom(s => s.Direct))
                    .ForMember(d => d.VRS, o => o.MapFrom(s => s.vrs))
                    .ForMember(d => d.Creation, o => o.MapFrom(s => s.creation))
                    .ForMember(d => d.Upgrade, o => o.MapFrom(s => s.Upgrade))
                    .ForMember(d => d.ForceFully, o => o.MapFrom(s => s.ForceFully))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var lstStaffBudgetDetails = Mapper.Map<List<Model.StaffBudget>>(staffBudgetDetails);
                return lstStaffBudgetDetails.OrderBy(x => x.DesignationName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int GetPresentStaff(int? designationID)
        {
            log.Info($"BudgetService/GetPresentStaff");
            try
            {
                var presentStaffCount = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DesignationID == designationID.Value && x.DOLeaveOrg == null).Count();
                return presentStaffCount;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int SaveStaffBudget(Model.StaffBudget staffBudget)
        {
            log.Info($"BudgetService/SaveStaffBudget");
            try
            {
                Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Model.StaffBudget, DTOModel.tblStaffBudget>()
                .ForMember(c => c.DesignationID, c => c.MapFrom(m => m.DesignationID))
                .ForMember(c => c.Year, c => c.MapFrom(m => m.Year))
                .ForMember(c => c.Month, c => c.MapFrom(m => m.Month))
                .ForMember(c => c.PresentStaff, c => c.MapFrom(m => m.PresentStaff))
                .ForMember(c => c.BudgetedStaff, c => c.MapFrom(m => m.BudgetedStaff))
                .ForMember(c => c.Vac, c => c.MapFrom(m => m.Vac))
                .ForMember(c => c.VRS, c => c.MapFrom(m => m.VRS))
                .ForMember(c => c.FPromotion, c => c.MapFrom(m => m.FPromotion))
                .ForMember(c => c.FLTC, c => c.MapFrom(m => m.FLTC))
                .ForMember(c => c.FDirect, c => c.MapFrom(m => m.FDirect))
                .ForMember(c => c.Upgrade, c => c.MapFrom(m => m.Upgrade))
                .ForMember(c => c.Creation, c => c.MapFrom(m => m.Creation))
                .ForMember(c => c.ForceFully, c => c.MapFrom(m => m.ForceFully))
                .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                .ForMember(c => c.CreatedOn, c => c.UseValue(DateTime.Now))
                .ForAllOtherMembers(c => c.Ignore());
            });
                var dtoStaffBudget = Mapper.Map<DTOModel.tblStaffBudget>(staffBudget);
                genericRepo.Insert<DTOModel.tblStaffBudget>(dtoStaffBudget);
                return dtoStaffBudget.StaffBudgetId;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public StaffBudget GetStaffBudgetById(int staffBudgetId)
        {
            log.Info($"BudgetService/GetStaffBudgetById");
            try
            {
                var staffBudegtDetails = genericRepo.Get<DTOModel.tblStaffBudget>(x => x.StaffBudgetId == staffBudgetId).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblStaffBudget, Model.StaffBudget>()
                    .ForMember(d => d.StaffBudgetId, o => o.MapFrom(s => s.StaffBudgetId))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                    .ForMember(d => d.Year, o => o.MapFrom(s => s.Year))
                    .ForMember(d => d.Month, o => o.MapFrom(s => s.Month))
                    .ForMember(d => d.BudgetedStaff, o => o.MapFrom(s => s.BudgetedStaff))
                    .ForMember(d => d.PresentStaff, o => o.MapFrom(s => s.PresentStaff))
                    .ForMember(d => d.Vac, o => o.MapFrom(s => s.Vac))
                    .ForMember(d => d.VRS, o => o.MapFrom(s => s.VRS))
                    .ForMember(d => d.FPromotion, o => o.MapFrom(s => s.FPromotion))
                    .ForMember(d => d.FDirect, o => o.MapFrom(s => s.FDirect))
                    .ForMember(d => d.FLTC, o => o.MapFrom(s => s.FLTC))
                    .ForMember(d => d.Upgrade, o => o.MapFrom(s => s.Upgrade))
                    .ForMember(d => d.Creation, o => o.MapFrom(s => s.Creation))
                    .ForMember(d => d.ForceFully, o => o.MapFrom(s => s.ForceFully))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var staffBudgetById = Mapper.Map<Model.StaffBudget>(staffBudegtDetails);
                return staffBudgetById;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool UpdateStaffBudget(Model.StaffBudget staffBudget)
        {
            log.Info($"BudgetService/UpdateStaffBudget");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.tblStaffBudget>(staffBudget.StaffBudgetId);
                dtoObj.Year = staffBudget.Year;
                dtoObj.Month = staffBudget.Month;
                dtoObj.BudgetedStaff = staffBudget.BudgetedStaff;
                dtoObj.PresentStaff = staffBudget.PresentStaff;
                dtoObj.VRS = staffBudget.VRS;
                dtoObj.Vac = staffBudget.Vac;
                dtoObj.FPromotion = staffBudget.FPromotion;
                dtoObj.FLTC = staffBudget.FLTC;
                dtoObj.FDirect = staffBudget.FDirect;
                genericRepo.Update<DTOModel.tblStaffBudget>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool Delete(int staffBudgetId)
        {
            log.Info($"BudgetService/Delete");
            bool flag = false;
            try
            {
                Data.Models.tblStaffBudget dtoStaffBudget = new Data.Models.tblStaffBudget();
                dtoStaffBudget = genericRepo.GetByID<Data.Models.tblStaffBudget>(staffBudgetId);
                dtoStaffBudget.IsDeleted = true;
                genericRepo.Update<Data.Models.tblStaffBudget>(dtoStaffBudget);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool StaffBudgetExist(Model.StaffBudget staffBudget)
        {
            log.Info($"BudgetService/StaffBudgetExist");
            try
            {
                var staffBudegtDetails = genericRepo.Exists<DTOModel.tblStaffBudget>(x => x.Year == staffBudget.Year && x.DesignationID == staffBudget.DesignationID);
                return staffBudegtDetails;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int GenerateStaffBudget(string forYear, string fromYear)
        {
            log.Info($"BudgetService/GenerateStaffBudget");
            try
            {
                var staffBudgetDetails = budgetRepo.GenerateStaffBudget(forYear, fromYear);
                return staffBudgetDetails;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

    }
}
