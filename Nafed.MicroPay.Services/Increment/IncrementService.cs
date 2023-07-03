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
using System.IO;
using Nafed.MicroPay.ImportExport.Interfaces;

namespace Nafed.MicroPay.Services.Increment
{
    public class IncrementService : BaseService, IIncrement
    {
        private readonly IGenericRepository genericRepo;
        private readonly IIncrementRepository incrementRepo;
        private readonly IIncrementExport export;

        public IncrementService(IGenericRepository genericRepo, IIncrementRepository incrementRepo, IIncrementExport export)
        {
            this.genericRepo = genericRepo;
            this.incrementRepo = incrementRepo;
            this.export = export;
        }

        public int InsertProjectedEmployee()
        {
            return incrementRepo.InsertProjectedEmployee();
        }

        public List<Model.ProjectedIncrement> GetProjectedIncrementDetails(int? BranchID, string EmployeeCode, string EmployeeName, int? incrementMonthId)
        {
            var projectedIncrementDetails = incrementRepo.GetProjectedIncrementDetails(BranchID, EmployeeCode, EmployeeName, incrementMonthId);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.GetProjectedIncrementDetails_Result, Model.ProjectedIncrement>()
                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.EmployeeName))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.CurrentBasic, o => o.MapFrom(s => s.CurrentBasic))
                .ForMember(d => d.OldBasic, o => o.MapFrom(s => s.OldBasic))
                .ForMember(d => d.LastIncrement, o => o.MapFrom(s => s.LastIncrement))
                .ForMember(d => d.IncrementMonth, o => o.MapFrom(s => s.IncrementMonth))
                //.ForMember(d => d.Designation, o => o.MapFrom(s => s.Designation))
                .ForAllOtherMembers(d => d.Ignore());
            });
            var lstProjectedIncrement = Mapper.Map<List<Model.ProjectedIncrement>>(projectedIncrementDetails);
            return lstProjectedIncrement.OrderBy(x => x.EmployeeCode).ToList();
        }


        public List<Model.ProjectedIncrement> GetUpdateIncrementDetails(int? BranchID, string EmployeeCode, string EmployeeName, int? incrementMonthId)
        {
            var updatedIncrementDetails = incrementRepo.GetUpdateIncrementDetails(BranchID, EmployeeCode, EmployeeName, incrementMonthId);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.GetUpdateIncrementDetails_Result, Model.ProjectedIncrement>()
                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.EmployeeName))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.CurrentBasic, o => o.MapFrom(s => s.CurrentBasic))
                .ForMember(d => d.OldBasic, o => o.MapFrom(s => s.OldBasic))
                .ForMember(d => d.LastIncrement, o => o.MapFrom(s => s.LastIncrement))
                .ForMember(d => d.IncrementMonth, o => o.MapFrom(s => s.IncrementMonth))
                //.ForMember(d => d.Designation, o => o.MapFrom(s => s.Designation))
                .ForAllOtherMembers(d => d.Ignore());
            });
            var lstProjectedIncrement = Mapper.Map<List<Model.ProjectedIncrement>>(updatedIncrementDetails);
            return lstProjectedIncrement.OrderBy(x => x.EmployeeCode).ToList();
        }

        public bool UpdateProjectedEmployeeSalaryDetails(List<Model.ProjectedIncrement> projectedIncrement)
        {
            List<DTOModel.tblmstProjectedEmployeeSalary> lstProjectedEmployeeSalary = new List<DTOModel.tblmstProjectedEmployeeSalary>();
            bool flag = false;
            foreach (var item in projectedIncrement)
            {
                lstProjectedEmployeeSalary.Add(new DTOModel.tblmstProjectedEmployeeSalary { EmployeeID = item.EmployeeID, EmployeeCode = item.EmployeeCode, BranchID = item.BranchID, BranchCode = item.BranchCode, E_Basic = item.CurrentBasic, LastBasic = item.OldBasic, LastIncrement = item.LastIncrement });
            }

            flag = incrementRepo.UpdateProjectedEmployeeSalaryDetails(lstProjectedEmployeeSalary);

            return flag;
        }

        public bool UpdateEmployeeSalaryDetails(List<Model.ProjectedIncrement> projectedIncrement)
        {
            List<DTOModel.TblMstEmployeeSalary> lstEmployeeSalary = new List<DTOModel.TblMstEmployeeSalary>();
            bool flag = false;
            foreach (var item in projectedIncrement)
            {
                lstEmployeeSalary.Add(new DTOModel.TblMstEmployeeSalary { EmployeeID = item.EmployeeID, EmployeeCode = item.EmployeeCode, BranchID = item.BranchID, BranchCode = item.BranchCode, E_Basic = item.CurrentBasic, LastBasic = item.OldBasic, LastIncrement = item.LastIncrement });
            }

            flag = incrementRepo.UpdateEmployeeSalaryDetails(lstEmployeeSalary);

            return flag;
        }

        public List<ValidateNewBasicAmount> GetValidateNewBasicAmountDetails(int? employeeId, int? incrementMonth)
        {
            var validateNewBasicAmountDetails = incrementRepo.GetValidateNewBasicAmountDetails(employeeId, incrementMonth);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.GetValidateNewBasicAmountDetails_Result, Model.ValidateNewBasicAmount>()
                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.e_Basic, o => o.MapFrom(s => s.e_Basic))
                .ForMember(d => d.LastBasic, o => o.MapFrom(s => s.LastBasic))
                .ForMember(d => d.LastIncrement, o => o.MapFrom(s => s.LastIncrement))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                .ForMember(d => d.payscale, o => o.MapFrom(s => s.payscale))
                .ForMember(d => d.Level, o => o.MapFrom(s => s.Level))
                .ForMember(d => d.B1, o => o.MapFrom(s => s.B1))
                .ForMember(d => d.E1, o => o.MapFrom(s => s.E1))
                .ForMember(d => d.B2, o => o.MapFrom(s => s.B2))
                .ForMember(d => d.E2, o => o.MapFrom(s => s.E2))
                .ForMember(d => d.B3, o => o.MapFrom(s => s.B3))
                .ForMember(d => d.E3, o => o.MapFrom(s => s.E3))
                .ForMember(d => d.B4, o => o.MapFrom(s => s.B4))
                .ForMember(d => d.E4, o => o.MapFrom(s => s.E4))
                .ForMember(d => d.B5, o => o.MapFrom(s => s.B5))
                .ForMember(d => d.E5, o => o.MapFrom(s => s.E5))
                .ForMember(d => d.B6, o => o.MapFrom(s => s.B6))
                .ForMember(d => d.E6, o => o.MapFrom(s => s.E6))
                .ForMember(d => d.B7, o => o.MapFrom(s => s.B7))
                .ForMember(d => d.E7, o => o.MapFrom(s => s.E7))
                .ForMember(d => d.B8, o => o.MapFrom(s => s.B8))
                .ForMember(d => d.E8, o => o.MapFrom(s => s.E8))
                .ForMember(d => d.B9, o => o.MapFrom(s => s.B9))
                .ForMember(d => d.E9, o => o.MapFrom(s => s.E9))
                .ForMember(d => d.B10, o => o.MapFrom(s => s.B10))
                .ForMember(d => d.E10, o => o.MapFrom(s => s.E10))
                .ForMember(d => d.B11, o => o.MapFrom(s => s.B11))
                .ForMember(d => d.E11, o => o.MapFrom(s => s.E11))
                .ForMember(d => d.B12, o => o.MapFrom(s => s.B12))
                .ForMember(d => d.E12, o => o.MapFrom(s => s.E12))
                .ForMember(d => d.B13, o => o.MapFrom(s => s.B13))
                .ForMember(d => d.E13, o => o.MapFrom(s => s.E13))
                .ForMember(d => d.B14, o => o.MapFrom(s => s.B14))
                .ForMember(d => d.E14, o => o.MapFrom(s => s.E14))
                .ForMember(d => d.B15, o => o.MapFrom(s => s.B15))
                .ForMember(d => d.E15, o => o.MapFrom(s => s.E15))
                .ForMember(d => d.B16, o => o.MapFrom(s => s.B16))
                .ForMember(d => d.E16, o => o.MapFrom(s => s.E16))
                .ForMember(d => d.B17, o => o.MapFrom(s => s.B17))
                .ForMember(d => d.E17, o => o.MapFrom(s => s.E17))
                .ForMember(d => d.B18, o => o.MapFrom(s => s.B18))
                .ForMember(d => d.E18, o => o.MapFrom(s => s.E18))
                .ForMember(d => d.B19, o => o.MapFrom(s => s.B19))
                .ForMember(d => d.E19, o => o.MapFrom(s => s.E19))
                .ForMember(d => d.B20, o => o.MapFrom(s => s.B20))
                .ForMember(d => d.E20, o => o.MapFrom(s => s.E20))
                .ForMember(d => d.B21, o => o.MapFrom(s => s.B21))
                .ForMember(d => d.E21, o => o.MapFrom(s => s.E21))
                .ForMember(d => d.B22, o => o.MapFrom(s => s.B22))
                .ForMember(d => d.E22, o => o.MapFrom(s => s.E22))
                .ForMember(d => d.B23, o => o.MapFrom(s => s.B23))
                .ForMember(d => d.E23, o => o.MapFrom(s => s.E23))
                .ForMember(d => d.B24, o => o.MapFrom(s => s.B24))
                .ForMember(d => d.E24, o => o.MapFrom(s => s.E24))
                .ForMember(d => d.B25, o => o.MapFrom(s => s.B25))
                .ForMember(d => d.E25, o => o.MapFrom(s => s.E25))
                .ForMember(d => d.B26, o => o.MapFrom(s => s.B26))
                .ForMember(d => d.E26, o => o.MapFrom(s => s.E26))
                .ForMember(d => d.B27, o => o.MapFrom(s => s.B27))
                .ForMember(d => d.E27, o => o.MapFrom(s => s.E27))
                .ForMember(d => d.B28, o => o.MapFrom(s => s.B28))
                .ForMember(d => d.E28, o => o.MapFrom(s => s.E28))
                .ForMember(d => d.B29, o => o.MapFrom(s => s.B29))
                .ForMember(d => d.E29, o => o.MapFrom(s => s.E29))
                .ForMember(d => d.B30, o => o.MapFrom(s => s.B30))
                .ForMember(d => d.E30, o => o.MapFrom(s => s.E30))
                .ForMember(d => d.B31, o => o.MapFrom(s => s.B31))
                .ForMember(d => d.E31, o => o.MapFrom(s => s.E31))
                .ForMember(d => d.B32, o => o.MapFrom(s => s.B32))
                .ForMember(d => d.E32, o => o.MapFrom(s => s.E32))
                .ForMember(d => d.B33, o => o.MapFrom(s => s.B33))
                .ForMember(d => d.E33, o => o.MapFrom(s => s.E33))
                .ForMember(d => d.B34, o => o.MapFrom(s => s.B34))
                .ForMember(d => d.E34, o => o.MapFrom(s => s.E34))
                .ForMember(d => d.B35, o => o.MapFrom(s => s.B35))
                .ForMember(d => d.E35, o => o.MapFrom(s => s.E35))
                .ForMember(d => d.B36, o => o.MapFrom(s => s.B36))
                .ForMember(d => d.E36, o => o.MapFrom(s => s.E36))
                .ForMember(d => d.B37, o => o.MapFrom(s => s.B37))
                .ForMember(d => d.E37, o => o.MapFrom(s => s.E37))
                .ForMember(d => d.B38, o => o.MapFrom(s => s.B38))
                .ForMember(d => d.E38, o => o.MapFrom(s => s.E38))
                .ForMember(d => d.B39, o => o.MapFrom(s => s.B39))
                .ForMember(d => d.E39, o => o.MapFrom(s => s.E39))
                .ForMember(d => d.B40, o => o.MapFrom(s => s.B40))
                .ForMember(d => d.E40, o => o.MapFrom(s => s.E40))
                .ForAllOtherMembers(d => d.Ignore());
            });
            var lstValidateNewBaiscAmount = Mapper.Map<List<Model.ValidateNewBasicAmount>>(validateNewBasicAmountDetails);
            return lstValidateNewBaiscAmount.OrderBy(x => x.EmployeeCode).ToList();
        }

        public List<ValidateNewBasicAmount> GetUpdateValidateNewBasicAmountDetails(int? employeeId, int? incrementMonth)
        {
            var validateNewBasicAmountDetails = incrementRepo.GetUpdateValidateNewBasicAmountDetails(employeeId, incrementMonth);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.GetUpdateValidateNewBasicAmountDetails_Result, Model.ValidateNewBasicAmount>()
                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.e_Basic, o => o.MapFrom(s => s.e_Basic))
                .ForMember(d => d.LastBasic, o => o.MapFrom(s => s.LastBasic))
                .ForMember(d => d.LastIncrement, o => o.MapFrom(s => s.LastIncrement))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                .ForMember(d => d.payscale, o => o.MapFrom(s => s.payscale))
                .ForMember(d => d.Level, o => o.MapFrom(s => s.Level))
                .ForMember(d => d.B1, o => o.MapFrom(s => s.B1))
                .ForMember(d => d.E1, o => o.MapFrom(s => s.E1))
                .ForMember(d => d.B2, o => o.MapFrom(s => s.B2))
                .ForMember(d => d.E2, o => o.MapFrom(s => s.E2))
                .ForMember(d => d.B3, o => o.MapFrom(s => s.B3))
                .ForMember(d => d.E3, o => o.MapFrom(s => s.E3))
                .ForMember(d => d.B4, o => o.MapFrom(s => s.B4))
                .ForMember(d => d.E4, o => o.MapFrom(s => s.E4))
                .ForMember(d => d.B5, o => o.MapFrom(s => s.B5))
                .ForMember(d => d.E5, o => o.MapFrom(s => s.E5))
                .ForMember(d => d.B6, o => o.MapFrom(s => s.B6))
                .ForMember(d => d.E6, o => o.MapFrom(s => s.E6))
                .ForMember(d => d.B7, o => o.MapFrom(s => s.B7))
                .ForMember(d => d.E7, o => o.MapFrom(s => s.E7))
                .ForMember(d => d.B8, o => o.MapFrom(s => s.B8))
                .ForMember(d => d.E8, o => o.MapFrom(s => s.E8))
                .ForMember(d => d.B9, o => o.MapFrom(s => s.B9))
                .ForMember(d => d.E9, o => o.MapFrom(s => s.E9))
                .ForMember(d => d.B10, o => o.MapFrom(s => s.B10))
                .ForMember(d => d.E10, o => o.MapFrom(s => s.E10))
                .ForMember(d => d.B11, o => o.MapFrom(s => s.B11))
                .ForMember(d => d.E11, o => o.MapFrom(s => s.E11))
                .ForMember(d => d.B12, o => o.MapFrom(s => s.B12))
                .ForMember(d => d.E12, o => o.MapFrom(s => s.E12))
                .ForMember(d => d.B13, o => o.MapFrom(s => s.B13))
                .ForMember(d => d.E13, o => o.MapFrom(s => s.E13))
                .ForMember(d => d.B14, o => o.MapFrom(s => s.B14))
                .ForMember(d => d.E14, o => o.MapFrom(s => s.E14))
                .ForMember(d => d.B15, o => o.MapFrom(s => s.B15))
                .ForMember(d => d.E15, o => o.MapFrom(s => s.E15))
                .ForMember(d => d.B16, o => o.MapFrom(s => s.B16))
                .ForMember(d => d.E16, o => o.MapFrom(s => s.E16))
                .ForMember(d => d.B17, o => o.MapFrom(s => s.B17))
                .ForMember(d => d.E17, o => o.MapFrom(s => s.E17))
                .ForMember(d => d.B18, o => o.MapFrom(s => s.B18))
                .ForMember(d => d.E18, o => o.MapFrom(s => s.E18))
                .ForMember(d => d.B19, o => o.MapFrom(s => s.B19))
                .ForMember(d => d.E19, o => o.MapFrom(s => s.E19))
                .ForMember(d => d.B20, o => o.MapFrom(s => s.B20))
                .ForMember(d => d.E20, o => o.MapFrom(s => s.E20))
                .ForMember(d => d.B21, o => o.MapFrom(s => s.B21))
                .ForMember(d => d.E21, o => o.MapFrom(s => s.E21))
                .ForMember(d => d.B22, o => o.MapFrom(s => s.B22))
                .ForMember(d => d.E22, o => o.MapFrom(s => s.E22))
                .ForMember(d => d.B23, o => o.MapFrom(s => s.B23))
                .ForMember(d => d.E23, o => o.MapFrom(s => s.E23))
                .ForMember(d => d.B24, o => o.MapFrom(s => s.B24))
                .ForMember(d => d.E24, o => o.MapFrom(s => s.E24))
                .ForMember(d => d.B25, o => o.MapFrom(s => s.B25))
                .ForMember(d => d.E25, o => o.MapFrom(s => s.E25))
                .ForMember(d => d.B26, o => o.MapFrom(s => s.B26))
                .ForMember(d => d.E26, o => o.MapFrom(s => s.E26))
                .ForMember(d => d.B27, o => o.MapFrom(s => s.B27))
                .ForMember(d => d.E27, o => o.MapFrom(s => s.E27))
                .ForMember(d => d.B28, o => o.MapFrom(s => s.B28))
                .ForMember(d => d.E28, o => o.MapFrom(s => s.E28))
                .ForMember(d => d.B29, o => o.MapFrom(s => s.B29))
                .ForMember(d => d.E29, o => o.MapFrom(s => s.E29))
                .ForMember(d => d.B30, o => o.MapFrom(s => s.B30))
                .ForMember(d => d.E30, o => o.MapFrom(s => s.E30))
                .ForMember(d => d.B31, o => o.MapFrom(s => s.B31))
                .ForMember(d => d.E31, o => o.MapFrom(s => s.E31))
                .ForMember(d => d.B32, o => o.MapFrom(s => s.B32))
                .ForMember(d => d.E32, o => o.MapFrom(s => s.E32))
                .ForMember(d => d.B33, o => o.MapFrom(s => s.B33))
                .ForMember(d => d.E33, o => o.MapFrom(s => s.E33))
                .ForMember(d => d.B34, o => o.MapFrom(s => s.B34))
                .ForMember(d => d.E34, o => o.MapFrom(s => s.E34))
                .ForMember(d => d.B35, o => o.MapFrom(s => s.B35))
                .ForMember(d => d.E35, o => o.MapFrom(s => s.E35))
                .ForMember(d => d.B36, o => o.MapFrom(s => s.B36))
                .ForMember(d => d.E36, o => o.MapFrom(s => s.E36))
                .ForMember(d => d.B37, o => o.MapFrom(s => s.B37))
                .ForMember(d => d.E37, o => o.MapFrom(s => s.E37))
                .ForMember(d => d.B38, o => o.MapFrom(s => s.B38))
                .ForMember(d => d.E38, o => o.MapFrom(s => s.E38))
                .ForMember(d => d.B39, o => o.MapFrom(s => s.B39))
                .ForMember(d => d.E39, o => o.MapFrom(s => s.E39))
                .ForMember(d => d.B40, o => o.MapFrom(s => s.B40))
                .ForMember(d => d.E40, o => o.MapFrom(s => s.E40))
                .ForAllOtherMembers(d => d.Ignore());
            });
            var lstValidateNewBaiscAmount = Mapper.Map<List<Model.ValidateNewBasicAmount>>(validateNewBasicAmountDetails);
            return lstValidateNewBaiscAmount.OrderBy(x => x.EmployeeCode).ToList();
        }

        public List<Model.StopIncrement> GetStopIncrementDetails(int?[] incrmentMonth, bool validateincrement)
        {
            var stopIncrementDetails = genericRepo.Get<DTOModel.tblMstEmployee>(x => ((incrmentMonth.Count() > 0) ? (incrmentMonth.Contains(x.IncrementMonth)) : 1 > 0) && x.ValidateIncrement == validateincrement && !x.IsDeleted && x.DOLeaveOrg == null);
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.tblMstEmployee, Model.StopIncrement>()
                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                .ForMember(d => d.ValidateIncrement, o => o.MapFrom(s => s.ValidateIncrement))
                .ForMember(d => d.Reason, o => o.MapFrom(s => s.Reason))
                .ForMember(d => d.StopIncrementEffectiveDate, o => o.MapFrom(s => s.StopIncrementEffectiveDate))
                .ForMember(d => d.StopIncrementEffectiveToDate, o => o.MapFrom(s => s.ToDate))
                .ForAllOtherMembers(d => d.Ignore());
            });
            var lstStopIncrement = Mapper.Map<List<Model.StopIncrement>>(stopIncrementDetails);
            return lstStopIncrement.OrderBy(x => x.EmployeeCode).ToList();
        }

        public bool UpdateStopIncrementDetails(List<Model.StopIncrement> stopIncrementDetails)
        {
            List<DTOModel.tblMstEmployee> lstStopIncrementEmployeeDetails = new List<DTOModel.tblMstEmployee>();
            bool flag = false;
            foreach (var item in stopIncrementDetails)
            {
                lstStopIncrementEmployeeDetails.Add(new DTOModel.tblMstEmployee { EmployeeId = item.EmployeeId, EmployeeCode = item.EmployeeCode, ValidateIncrement = item.ValidateIncrement, Reason = item.Reason, StopIncrementEffectiveDate = item.StopIncrementEffectiveDate, ToDate = item.StopIncrementEffectiveToDate });
            }
            flag = incrementRepo.UpdateStopIncrementDetails(lstStopIncrementEmployeeDetails);
            return flag;
        }

        public string GetLastIncrementMonthDetails(int? incrementMonthID)
        {
            string lastUpdatedDate = "0";
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            if (currentMonth == 12)
                currentYear = currentYear + 1;

            var lastIncrementDetails = genericRepo.Get<DTOModel.tblLastIncrementMonth>(x => x.IncrementMonth == incrementMonthID && x.IncrementYear == currentYear).OrderByDescending(x => x.LastUpdateDate).FirstOrDefault();
            if (lastIncrementDetails != null)
            {
                if (lastIncrementDetails.IncrementMonth == incrementMonthID && lastIncrementDetails.IncrementYear == currentYear)
                    lastUpdatedDate = lastIncrementDetails.LastUpdateDate.ToString();
            }
            return lastUpdatedDate;
        }

        public bool InsertUpdateLastIncrementDetails(Model.LastIncrementMonth lastincMonth, int userID)
        {
            bool flag = false;

            var lastIncrementDetails = genericRepo.Get<DTOModel.tblLastIncrementMonth>(x => x.IncrementMonth == lastincMonth.IncrementMonth && x.IncrementYear == lastincMonth.IncrementYear).FirstOrDefault();
            if (lastIncrementDetails == null)
            {
                lastincMonth.CreatedOn = DateTime.Now;
                lastincMonth.LastUpdateDate = DateTime.Now;
                lastincMonth.CreatedBy = userID;
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.LastIncrementMonth, DTOModel.tblLastIncrementMonth>()
                    .ForMember(c => c.IncrementMonth, c => c.MapFrom(s => s.IncrementMonth))
                    .ForMember(c => c.IncrementYear, c => c.MapFrom(s => s.IncrementYear))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.LastUpdateDate, c => c.MapFrom(s => s.LastUpdateDate))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoLastIncrement = Mapper.Map<DTOModel.tblLastIncrementMonth>(lastincMonth);
                genericRepo.Insert<DTOModel.tblLastIncrementMonth>(dtoLastIncrement);
                flag = true;
            }
            else
            {
                lastIncrementDetails.LastUpdateDate = DateTime.Now;
                lastIncrementDetails.UpdatedOn = DateTime.Now;
                lastIncrementDetails.UpdatedBy = userID;
                genericRepo.Update<DTOModel.tblLastIncrementMonth>(lastIncrementDetails);
                flag = true;
            }
            return flag;
        }

        public int GetMaxRankDesignation()
        {
            var maxRank = genericRepo.Get<DTOModel.Designation>(x => !x.IsDeleted && x.Rank != null).Max(x => x.Rank.Value);
            return maxRank;
        }

        public string ExportIncrement(int? branchId, int? incrementMonth, string employeeName, string employeeCode, string fileName, string fullPath, string type)
        {
            log.Info("IncrementService/ExportIncrement");
            try
            {
                string result = string.Empty;
                DataSet dsExportApplicableIncrement = new DataSet();
                if (Directory.Exists(fullPath))
                {
                    fullPath = $"{fullPath}{fileName}";
                    var applicableIncrDT = incrementRepo.GetExportApplicableIncrement(branchId, incrementMonth, employeeName, employeeCode, fileName, fullPath, type);
                    dsExportApplicableIncrement.Tables.Add(applicableIncrDT);
                    dsExportApplicableIncrement.Tables[0].Columns["Branch Name"].SetOrdinal(11);
                    dsExportApplicableIncrement.Tables[0].Columns["Branch Code"].SetOrdinal(12);
                    bool res = false;
                    if (type == "1")
                        res = export.ExportToExcelIncrement(dsExportApplicableIncrement, fullPath, "ApplicableIncrement");
                    else
                        res = export.ExportToExcelIncrement(dsExportApplicableIncrement, fullPath, "UpdateIncrement");
                }
                return "";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
