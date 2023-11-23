using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using static Nafed.MicroPay.ImportExport.AttendanceForm;
using static Nafed.MicroPay.Common.DataValidation;
using Nafed.MicroPay.ImportExport;
using Nafed.MicroPay.Services.IServices;
using System.Data.SqlClient;

namespace Nafed.MicroPay.Services.Arrear
{
    public class ArrearService : BaseService, IArrearService
    {
        private readonly IExcelService excelService;
        private readonly IGenericRepository genericRepo;
        private readonly IArrearRepository arrearRepo;
        public ArrearService(IExcelService excelService, IGenericRepository genericRepo, IArrearRepository arrearRepo)
        {
            this.excelService = excelService;
            this.genericRepo = genericRepo;
            this.arrearRepo = arrearRepo;
        }

        #region ArrearManualData

        public List<ArrearManualData> GetArrearManualdataList()
        {
            log.Info($"ArrearService/GetArrearManualdataList");
            try

            {
                var result = genericRepo.Get<Data.Models.TblArrearMonthlyInput>(em => !em.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblArrearMonthlyInput, Model.ArrearManualData>()
                     .ForMember(c => c.arrearmonthlyinputID, c => c.MapFrom(s => s.arrearmonthlyinputID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Empcode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Empname, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                    .ForMember(c => c.HeadName, c => c.MapFrom(s => s.HeadName))
                    .ForMember(c => c.HeadValue, c => c.MapFrom(s => s.HeadValue))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBranch = Mapper.Map<List<Model.ArrearManualData>>(result);
                return listBranch.OrderBy(x => x.arrearmonthlyinputID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public List<ArrearManualData> GetArrearManualdata(int branchID, int employeeID, int month, int year)
        {
            log.Info($"ArrearService/GetArrearManualdata");
            try
            {
                var result = genericRepo.Get<Data.Models.TblArrearMonthlyInput>(em => !em.IsDeleted && (employeeID > 0 ? em.EmployeeId == employeeID : 1 > 0) && (month > 0 ? em.Month == month : 1 > 0)
                && (year > 0 ? em.Year == year : 1 > 0) && (branchID > 0 ? em.tblMstEmployee.BranchID == branchID : 1 > 0));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblArrearMonthlyInput, Model.ArrearManualData>()
                     .ForMember(c => c.arrearmonthlyinputID, c => c.MapFrom(s => s.arrearmonthlyinputID))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                      .ForMember(c => c.BranchID, c => c.MapFrom(s => s.tblMstEmployee.BranchID))
                    .ForMember(c => c.Empcode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Empname, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                    .ForMember(c => c.HeadName, c => c.MapFrom(s => s.HeadName))
                    .ForMember(c => c.HeadValue, c => c.MapFrom(s => s.HeadValue))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBranch = Mapper.Map<List<Model.ArrearManualData>>(result);
                return listBranch.OrderBy(x => x.arrearmonthlyinputID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
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
        public List<SelectListModel> GetSalHeads()
        {
            try
            {
                var result = genericRepo.Get<DTOModel.SalaryHead>();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SalaryHead, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.SeqNo))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.FieldName));
                });
                var employeedetail = Mapper.Map<List<Model.SelectListModel>>(result);
                return employeedetail.OrderBy(x => x.value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ArrearDataExists(int EmployeeID, int month, int year, string Headname)
        {
            return genericRepo.Exists<Data.Models.TblArrearMonthlyInput>(x => x.EmployeeId == EmployeeID && x.Month == month && x.Year == year && x.HeadName == Headname);
        }

        public Model.ArrearManualData GetArreardataByID(int arrearmonthlyinputID)
        {
            log.Info($"GetPFBalancesByID {arrearmonthlyinputID}");
            try
            {
                var ArreardataObj = genericRepo.GetByID<DTOModel.TblArrearMonthlyInput>(arrearmonthlyinputID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.TblArrearMonthlyInput, Model.ArrearManualData>()
                   .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Empcode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Empname, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                     .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                    .ForMember(c => c.HeadName, c => c.MapFrom(s => s.HeadName))
                    .ForMember(c => c.HeadValue, c => c.MapFrom(s => s.HeadValue))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.TblArrearMonthlyInput, Model.ArrearManualData>(ArreardataObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public int Insertarrearmanualdata(Model.ArrearManualData createArrearmanualdata)
        {
            log.Info($"Insertarrearmanualdata");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.ArrearManualData, DTOModel.TblArrearMonthlyInput>()
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                     .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                    .ForMember(c => c.HeadName, c => c.MapFrom(s => s.HeadName))
                    .ForMember(c => c.HeadValue, c => c.MapFrom(s => s.HeadValue))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => 0))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoarrear = Mapper.Map<DTOModel.TblArrearMonthlyInput>(createArrearmanualdata);
                genericRepo.Insert<DTOModel.TblArrearMonthlyInput>(dtoarrear);
                return dtoarrear.arrearmonthlyinputID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public int Updatearrearmanualdata(Model.ArrearManualData editArrearmanualdata)
        {
            log.Info($"Updatearrearmanualdata");
            int res = 0;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.TblArrearMonthlyInput>(editArrearmanualdata.arrearmonthlyinputID);
                dtoObj.HeadValue = editArrearmanualdata.HeadValue;
                dtoObj.HeadName = editArrearmanualdata.HeadName;
                dtoObj.UpdatedBy = editArrearmanualdata.UpdatedBy;
                dtoObj.UpdatedOn = editArrearmanualdata.UpdatedOn;

                genericRepo.Update<DTOModel.TblArrearMonthlyInput>(dtoObj);
                res = 1;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return res;
        }

        public bool Delete(int arrearmonthlyinputID)
        {
            log.Info($"ArrearService/Delete/{arrearmonthlyinputID}");
            bool flag = false;
            try
            {
                DTOModel.TblArrearMonthlyInput dtoArrearmanualdata = new DTOModel.TblArrearMonthlyInput();
                dtoArrearmanualdata = genericRepo.GetByID<DTOModel.TblArrearMonthlyInput>(arrearmonthlyinputID);
                dtoArrearmanualdata.IsDeleted = true;
                genericRepo.Update<DTOModel.TblArrearMonthlyInput>(dtoArrearmanualdata);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }


        public string GetManualdataForm(int branchID, int month, int year, string fileName, string sFullPath)
        {
            log.Info($"MarkAttendanceService/GetAttendanceForm/{branchID},{month},{year}");

            string result = string.Empty;
            if (Directory.Exists(sFullPath))
            {
                sFullPath = $"{sFullPath}{fileName}";
                var empDTO = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.BranchID == branchID && !x.IsDeleted);

                DataTable dt = new DataTable();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.ArrearManualData>()
                     .ForMember(d => d.Empcode, o => o.MapFrom(s => s.EmployeeCode))
                     .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(d => d.Empname, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.Month, o => o.MapFrom(s => month))
                    .ForMember(d => d.Year, o => o.MapFrom(s => year))
                     .ForMember(d => d.AdditionalOverTime, o => o.UseValue(0))
                    .ForMember(d => d.ChildrenEducationAllowance, o => o.UseValue(0))
                    .ForMember(d => d.DriverAllowance, o => o.UseValue(0))
                    .ForMember(d => d.IncomeTaxDeduction, o => o.UseValue(0))
                    .ForMember(d => d.LeaveWithoutPay, o => o.UseValue(0))
                    .ForMember(d => d.LifeInsurance, o => o.UseValue(0))
                    .ForMember(d => d.MedicalReimbursement, o => o.UseValue(0))
                    .ForMember(d => d.MiscellaneousDeduction1, o => o.UseValue(0))
                    .ForMember(d => d.MiscellaneousDeduction2, o => o.UseValue(0))
                    .ForMember(d => d.MiscellaneousDeduction3, o => o.UseValue(0))
                    .ForMember(d => d.MiscellaneousIncome1, o => o.UseValue(0))
                    .ForMember(d => d.MiscellaneousIncome2, o => o.UseValue(0))
                    .ForMember(d => d.MiscellaneousIncome3, o => o.UseValue(0))
                    .ForMember(d => d.NafedBazaarDeduction, o => o.UseValue(0))
                    .ForMember(d => d.NewsPaper, o => o.UseValue(0))
                    .ForMember(d => d.OverTime, o => o.UseValue(0))
                    .ForMember(d => d.PersonalPay, o => o.UseValue(0))
                    .ForMember(d => d.PetrolCharges, o => o.UseValue(0))
                    .ForMember(d => d.ProfessionalTax, o => o.UseValue(0))
                    .ForMember(d => d.SpecialAllowance, o => o.UseValue(0))
                    .ForMember(d => d.SpecialPay, o => o.UseValue(0))
                   .ForMember(d => d.SundryAdvance, o => o.UseValue(0))
                    .ForMember(d => d.Telephone, o => o.UseValue(0))
                    .ForMember(d => d.TransportAllowance, o => o.UseValue(0))
                    .ForMember(d => d.WashingAllowance, o => o.UseValue(0))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var empList = Mapper.Map<List<Model.ArrearManualData>>(empDTO);

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
                        x.Empcode,
                        x.Empname,
                        x.BranchName,
                        x.DesignationName,
                        x.Month,
                        x.Year,
                        x.AdditionalOverTime,
                        x.ChildrenEducationAllowance,
                        x.DriverAllowance,
                        x.IncomeTaxDeduction,
                        x.LeaveWithoutPay,
                        x.LifeInsurance,
                        x.MedicalReimbursement,
                        x.MiscellaneousDeduction1,
                        x.MiscellaneousDeduction2,
                        x.MiscellaneousDeduction3,
                        x.MiscellaneousIncome1,
                        x.MiscellaneousIncome2,
                        x.MiscellaneousIncome3,
                        x.NafedBazaarDeduction,
                        x.NewsPaper,
                        x.OverTime,
                        x.PersonalPay,
                        x.PetrolCharges,
                        x.ProfessionalTax,
                        x.SpecialAllowance,
                        x.SpecialPay,
                        x.SundryAdvance,
                        x.Telephone,
                        x.TransportAllowance,
                        x.WashingAllowance
                    }).ToList();

                    dt = Common.ExtensionMethods.ToDataTable(formData);

                }

                if (dt != null && dt.Rows.Count > 0)  //====== export attendance form if there is data ========= 
                {
                    dt.Columns[0].ColumnName = "#";

                    IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                    exportHdr = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                    result = ExportToExcelManualData(exportHdr, dt, "ManualDataForm", sFullPath);
                }
                else
                    result = "norec";
            }
            return result;
        }



        public int ImportManualDataDetails(int userID, int userType, List<ArrearManualData> dataDetails)
        {
            log.Info($"ImportAttendanceService/ImportAttendanceDetails");
            try
            {

                var dtoList = dataDetails.Select(x => new { EmployeeId = x.EmployeeId, Month = x.Month, Year = x.Year, AdditionalOverTime = x.AdditionalOverTime, ChildrenEducationAllowance = x.ChildrenEducationAllowance, DriverAllowance = x.DriverAllowance, IncomeTaxDeduction = x.IncomeTaxDeduction, LeaveWithoutPay = x.LeaveWithoutPay, LifeInsurance = x.LifeInsurance, MedicalReimbursement = x.MedicalReimbursement, MiscellaneousDeduction1 = x.MiscellaneousDeduction1, MiscellaneousDeduction2 = x.MiscellaneousDeduction2, MiscellaneousDeduction3 = x.MiscellaneousDeduction3, MiscellaneousIncome1 = x.MiscellaneousIncome1, MiscellaneousIncome2 = x.MiscellaneousIncome1, MiscellaneousIncome3 = x.MiscellaneousIncome3, NafedBazaarDeduction = x.NafedBazaarDeduction, NewsPaper = x.NewsPaper, OverTime = x.OverTime, PersonalPay = x.PersonalPay, PetrolCharges = x.PetrolCharges, ProfessionalTax = x.ProfessionalTax, SpecialAllowance = x.SpecialAllowance, SpecialPay = x.SpecialPay, SundryAdvance = x.SundryAdvance, Telephone = x.Telephone, TransportAllowance = x.TransportAllowance, WashingAllowance = x.WashingAllowance }).ToList();
                DataTable DT = new DataTable();
                DT = Common.ExtensionMethods.ToDataTable(dtoList);
                int res = arrearRepo.ImportManualData(userID, DT);
                return res;
            }
            catch
            {
                // log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        #endregion

        #region ActualDArates

        public List<ArrearManualData> GetDAratesList(int month, int year)
        {
            log.Info($"ArrearService/GetDAratesList");
            try

            {

                var result = genericRepo.Get<DTOModel.TblActualArrear>(em => !em.IsDeleted && (month > 0 ? em.Month == month : 1 > 0)
                && (year > 0 ? em.Year == year : 1 > 0));


                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblActualArrear, Model.ArrearManualData>()
                     .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                    .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                    .ForMember(c => c.E_01, c => c.MapFrom(s => s.E_01))
                    .ForMember(c => c.Saldate, c => c.MapFrom(s => s.Saldate))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBranch = Mapper.Map<List<Model.ArrearManualData>>(result);
                return listBranch.OrderBy(x => x.arrearmonthlyinputID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DArateexists(int month, int year, double E_01)
        {
            return genericRepo.Exists<Data.Models.TblActualArrear>(x => x.Month == month && x.Year == year && x.E_01 == E_01);
        }

        public Model.ArrearManualData GetDAratesByID(int ID)
        {
            log.Info($"GetDAratesByID {ID}");
            try
            {
                var DARatesdataObj = genericRepo.GetByID<DTOModel.TblActualArrear>(ID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.TblActualArrear, Model.ArrearManualData>()
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                     .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                    .ForMember(c => c.E_01, c => c.MapFrom(s => s.E_01))
                    .ForMember(c => c.Saldate, c => c.MapFrom(s => s.Saldate))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.TblActualArrear, Model.ArrearManualData>(DARatesdataObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public int InsertDAratesdata(Model.ArrearManualData createDAratesdatadata)
        {
            log.Info($"Insertarrearmanualdata");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.ArrearManualData, DTOModel.TblActualArrear>()
                    .ForMember(c => c.Year, c => c.MapFrom(s => s.Year))
                     .ForMember(c => c.Month, c => c.MapFrom(s => s.Month))
                    .ForMember(c => c.E_01, c => c.MapFrom(s => s.E_01))
                    .ForMember(c => c.Saldate, c => c.MapFrom(s => s.Saldate))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => 0))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoDArates = Mapper.Map<DTOModel.TblActualArrear>(createDAratesdatadata);
                genericRepo.Insert<DTOModel.TblActualArrear>(dtoDArates);
                return dtoDArates.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public int UpdateDAratesdata(Model.ArrearManualData editDAratesdatadata)
        {
            log.Info($"UpdateDAratesdata");
            int res = 0;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.TblActualArrear>(editDAratesdatadata.ID);
                dtoObj.E_01 = editDAratesdatadata.E_01;
                dtoObj.UpdatedBy = editDAratesdatadata.UpdatedBy;
                dtoObj.UpdatedOn = editDAratesdatadata.UpdatedOn;

                genericRepo.Update<DTOModel.TblActualArrear>(dtoObj);
                res = 1;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return res;
        }

        public bool DeleteDArates(int ID)
        {
            log.Info($"ArrearService/DeleteDArates/{ID}");
            bool flag = false;
            try
            {
                DTOModel.TblActualArrear dtoDAratesdata = new DTOModel.TblActualArrear();
                dtoDAratesdata = genericRepo.GetByID<DTOModel.TblActualArrear>(ID);
                dtoDAratesdata.IsDeleted = true;
                genericRepo.Update<DTOModel.TblActualArrear>(dtoDAratesdata);
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

        #region SalaryManualData

        public List<SalaryManualData> GetSalarymanualData(int branchID, int employeeID, int DesignationID, int month, int year)
        {
            log.Info($"ArrearService/GetSalarymanualData");
            try

            {

                var result = genericRepo.Get<Data.Models.tblarrearmanualdata>(em => !em.IsDeleted && (branchID > 0 ? em.BranchID == branchID : 1 > 0) && (employeeID > 0 ? em.EmployeeId == employeeID : 1 > 0) && (month > 0 ? em.SalMonth == month : 1 > 0)
                && (year > 0 ? em.SalYear == year : 1 > 0) && (DesignationID > 0 ? em.DesignationID == DesignationID : 1 > 0));


                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblarrearmanualdata, Model.SalaryManualData>()
                     .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                    .ForMember(c => c.SalMonth, c => c.MapFrom(s => s.SalMonth))
                    .ForMember(c => c.SalYear, c => c.MapFrom(s => s.SalYear))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                     .ForMember(c => c.DesignationID, c => c.MapFrom(s => s.DesignationID))
                     .ForMember(c => c.Empcode, c => c.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(c => c.Empname, c => c.MapFrom(s => s.tblMstEmployee.Name))
                     .ForMember(c => c.RecordType, c => c.MapFrom(s => s.RecordType))
                    .ForMember(c => c.SeqNo, c => c.MapFrom(s => s.SeqNo))
                    .ForMember(c => c.E_Basic, c => c.MapFrom(s => s.E_Basic))
                    .ForMember(c => c.E_SP, c => c.MapFrom(s => s.E_SP))
                    .ForMember(c => c.E_FDA, c => c.MapFrom(s => s.E_FDA))
                    .ForMember(c => c.E_01, c => c.MapFrom(s => s.E_01))
                    .ForMember(c => c.E_02, c => c.MapFrom(s => s.E_02))
                    .ForMember(c => c.E_03, c => c.MapFrom(s => s.E_03))
                    .ForMember(c => c.E_04, c => c.MapFrom(s => s.E_04))
                    .ForMember(c => c.E_05, c => c.MapFrom(s => s.E_05))
                    .ForMember(c => c.E_06, c => c.MapFrom(s => s.E_06))
                    .ForMember(c => c.E_07, c => c.MapFrom(s => s.E_07))
                    .ForMember(c => c.E_08, c => c.MapFrom(s => s.E_08))
                    .ForMember(c => c.E_09, c => c.MapFrom(s => s.E_09))
                    .ForMember(c => c.E_10, c => c.MapFrom(s => s.E_10))
                    .ForMember(c => c.E_11, c => c.MapFrom(s => s.E_11))
                    .ForMember(c => c.E_12, c => c.MapFrom(s => s.E_12))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listsalmandata = Mapper.Map<List<Model.SalaryManualData>>(result);
                return listsalmandata.OrderBy(x => x.ID).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool salaryDataExists(int BranchID, int EmployeeID, int month, int year, int arrearType)
        {
            return genericRepo.Exists<Data.Models.tblarrearmanualdata>(x => x.EmployeeId == EmployeeID && x.SalMonth == month && x.SalYear == year && x.ArrearType == arrearType);
        }

        public int InsertSalarymanualdata(Model.SalaryManualData createsalarymanualdata)
        {
            log.Info($"InsertSalarymanualdata");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.SalaryManualData, DTOModel.tblarrearmanualdata>()
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                     .ForMember(c => c.DesignationID, c => c.MapFrom(s => s.DesignationID))
                    .ForMember(c => c.SalMonth, c => c.MapFrom(s => s.SalMonth))
                    .ForMember(c => c.SalYear, c => c.MapFrom(s => s.SalYear))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                     .ForMember(c => c.RecordType, c => c.MapFrom(s => s.RecordType))
                    .ForMember(c => c.SeqNo, c => c.MapFrom(s => s.SeqNo))
                     .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.E_Basic, c => c.MapFrom(s => s.E_Basic_A))
                    .ForMember(c => c.E_SP, c => c.MapFrom(s => s.E_SP_A))
                    .ForMember(c => c.E_FDA, c => c.MapFrom(s => s.E_FDA_A))
                    .ForMember(c => c.E_01, c => c.MapFrom(s => s.E_01_A))
                    .ForMember(c => c.E_02, c => c.MapFrom(s => s.E_02_A))
                    .ForMember(c => c.E_03, c => c.MapFrom(s => s.E_03_A))
                    .ForMember(c => c.E_04, c => c.MapFrom(s => s.E_04_A))
                    .ForMember(c => c.E_05, c => c.MapFrom(s => s.E_05_A))
                    .ForMember(c => c.E_06, c => c.MapFrom(s => s.E_06_A))
                    .ForMember(c => c.E_07, c => c.MapFrom(s => s.E_07_A))
                    .ForMember(c => c.E_08, c => c.MapFrom(s => s.E_08_A))
                    .ForMember(c => c.E_09, c => c.MapFrom(s => s.E_09_A))
                    .ForMember(c => c.E_10, c => c.MapFrom(s => s.E_10_A))
                     .ForMember(c => c.E_22, c => c.MapFrom(s => s.E_22_A))

                    .ForMember(c => c.E_11, c => c.MapFrom(s => s.E_11_A))
                    .ForMember(c => c.E_12, c => c.MapFrom(s => s.E_12_A))
                    .ForMember(c => c.E_13, c => c.MapFrom(s => s.E_13_A))
                    .ForMember(c => c.E_30, c => c.MapFrom(s => s.E_30_A))
                    .ForMember(c => c.E_Basic_A, c => c.MapFrom(s => s.E_Basic_A))
                    .ForMember(c => c.E_SP_A, c => c.MapFrom(s => s.E_SP_A))
                    .ForMember(c => c.E_FDA_A, c => c.MapFrom(s => s.E_FDA_A))
                    .ForMember(c => c.E_01_A, c => c.MapFrom(s => s.E_01_A))
                    .ForMember(c => c.E_02_A, c => c.MapFrom(s => s.E_02_A))
                    .ForMember(c => c.E_03_A, c => c.MapFrom(s => s.E_03_A))
                    .ForMember(c => c.E_04_A, c => c.MapFrom(s => s.E_04_A))
                    .ForMember(c => c.E_05_A, c => c.MapFrom(s => s.E_05_A))
                    .ForMember(c => c.E_06_A, c => c.MapFrom(s => s.E_06_A))
                    .ForMember(c => c.E_07_A, c => c.MapFrom(s => s.E_07_A))
                    .ForMember(c => c.E_08_A, c => c.MapFrom(s => s.E_08_A))
                    .ForMember(c => c.E_09_A, c => c.MapFrom(s => s.E_09_A))
                    .ForMember(c => c.E_10_A, c => c.MapFrom(s => s.E_10_A))
                     .ForMember(c => c.E_22_A, c => c.MapFrom(s => s.E_22_A))

                    .ForMember(c => c.E_11_A, c => c.MapFrom(s => s.E_11_A))
                    .ForMember(c => c.E_12_A, c => c.MapFrom(s => s.E_12_A))
                    .ForMember(c => c.E_13_A, c => c.MapFrom(s => s.E_13_A))
                    .ForMember(c => c.E_30_A, c => c.MapFrom(s => s.E_30_A))
                    .ForMember(c => c.D_PF, c => c.MapFrom(s => s.D_PF_A))
                    .ForMember(c => c.D_VPF, c => c.MapFrom(s => s.D_VPF_A))
                    .ForMember(c => c.D_01, c => c.MapFrom(s => s.D_01_A))
                    .ForMember(c => c.D_02, c => c.MapFrom(s => s.D_02_A))
                    .ForMember(c => c.D_03, c => c.MapFrom(s => s.D_03_A))
                    .ForMember(c => c.D_04, c => c.MapFrom(s => s.D_04_A))
                    .ForMember(c => c.D_05, c => c.MapFrom(s => s.D_05_A))
                    .ForMember(c => c.D_06, c => c.MapFrom(s => s.D_06_A))
                    .ForMember(c => c.D_07, c => c.MapFrom(s => s.D_07_A))
                    .ForMember(c => c.D_08, c => c.MapFrom(s => s.D_08_A))
                    .ForMember(c => c.D_09, c => c.MapFrom(s => s.D_09_A))
                    .ForMember(c => c.D_10, c => c.MapFrom(s => s.D_10_A))
                    .ForMember(c => c.D_11, c => c.MapFrom(s => s.D_11_A))
                    .ForMember(c => c.D_12, c => c.MapFrom(s => s.D_12_A))
                    .ForMember(c => c.D_13, c => c.MapFrom(s => s.D_13_A))
                    .ForMember(c => c.D_14, c => c.MapFrom(s => s.D_14_A))
                    .ForMember(c => c.D_15, c => c.MapFrom(s => s.D_15_A))
                    .ForMember(c => c.D_16, c => c.MapFrom(s => s.D_16_A))
                    .ForMember(c => c.D_17, c => c.MapFrom(s => s.D_17_A))
                    .ForMember(c => c.D_18, c => c.MapFrom(s => s.D_18_A))
                    .ForMember(c => c.D_19, c => c.MapFrom(s => s.D_19_A))
                    .ForMember(c => c.D_20, c => c.MapFrom(s => s.D_20_A))
                    .ForMember(c => c.D_30, c => c.MapFrom(s => s.D_30_A))
                     .ForMember(c => c.D_PF_A, c => c.MapFrom(s => s.D_PF_A))
                    .ForMember(c => c.D_VPF_A, c => c.MapFrom(s => s.D_VPF_A))
                    .ForMember(c => c.D_01_A, c => c.MapFrom(s => s.D_01_A))
                    .ForMember(c => c.D_02_A, c => c.MapFrom(s => s.D_02_A))
                    .ForMember(c => c.D_03_A, c => c.MapFrom(s => s.D_03_A))
                    .ForMember(c => c.D_04_A, c => c.MapFrom(s => s.D_04_A))
                    .ForMember(c => c.D_05_A, c => c.MapFrom(s => s.D_05_A))
                    .ForMember(c => c.D_06_A, c => c.MapFrom(s => s.D_06_A))
                    .ForMember(c => c.D_07_A, c => c.MapFrom(s => s.D_07_A))
                    .ForMember(c => c.D_08_A, c => c.MapFrom(s => s.D_08_A))
                    .ForMember(c => c.D_09_A, c => c.MapFrom(s => s.D_09_A))
                    .ForMember(c => c.D_10_A, c => c.MapFrom(s => s.D_10_A))
                    .ForMember(c => c.D_11_A, c => c.MapFrom(s => s.D_11_A))
                    .ForMember(c => c.D_12_A, c => c.MapFrom(s => s.D_12_A))
                    .ForMember(c => c.D_13_A, c => c.MapFrom(s => s.D_13_A))
                    .ForMember(c => c.D_14_A, c => c.MapFrom(s => s.D_14_A))
                    .ForMember(c => c.D_15_A, c => c.MapFrom(s => s.D_15_A))
                    .ForMember(c => c.D_16_A, c => c.MapFrom(s => s.D_16_A))
                    .ForMember(c => c.D_17_A, c => c.MapFrom(s => s.D_17_A))
                    .ForMember(c => c.D_18_A, c => c.MapFrom(s => s.D_18_A))
                    .ForMember(c => c.D_19_A, c => c.MapFrom(s => s.D_19_A))
                    .ForMember(c => c.D_20_A, c => c.MapFrom(s => s.D_20_A))
                    .ForMember(c => c.D_30_A, c => c.MapFrom(s => s.D_30_A))
                    .ForMember(c => c.C_TotEarn, c => c.MapFrom(s => s.C_TotEarn))
                    .ForMember(c => c.C_TotDedu, c => c.MapFrom(s => s.C_TotDedu))
                    .ForMember(c => c.C_NetSal, c => c.MapFrom(s => s.C_NetSal))
                    .ForMember(c => c.LWP, c => c.MapFrom(s => s.LWP))
                    .ForMember(c => c.Attendance, c => c.MapFrom(s => s.Attendance))
                     .ForMember(c => c.WorkingDays, c => c.MapFrom(s => s.WorkingDays))
                     .ForMember(c => c.RateVPFA, c => c.MapFrom(s => s.RateVPFA))
                     .ForMember(c => c.DateofGenerateSalary, c => c.MapFrom(s => s.DateofGenerateSalary))
                    .ForMember(c => c.ArrearType, c => c.MapFrom(s => s.ArrearType))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dto = Mapper.Map<DTOModel.tblarrearmanualdata>(createsalarymanualdata);
                genericRepo.Insert<DTOModel.tblarrearmanualdata>(dto);
                return dto.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.SalaryManualData GetSalaryManualbyID(int ID)
        {
            log.Info($"SalaryManualData {ID}");
            try
            {
                var salarymanualdataObj = genericRepo.GetByID<DTOModel.tblarrearmanualdata>(ID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.tblarrearmanualdata, Model.SalaryManualData>()
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                      .ForMember(c => c.DesignationID, c => c.MapFrom(s => s.DesignationID))
                    .ForMember(c => c.SalMonth, c => c.MapFrom(s => s.SalMonth))
                    .ForMember(c => c.SalYear, c => c.MapFrom(s => s.SalYear))
                    .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                     .ForMember(c => c.RecordType, c => c.MapFrom(s => s.RecordType))
                    .ForMember(c => c.SeqNo, c => c.MapFrom(s => s.SeqNo))
                     .ForMember(c => c.EmployeeId, c => c.MapFrom(s => s.EmployeeId))
                    .ForMember(c => c.E_Basic, c => c.MapFrom(s => s.E_Basic_A))
                    .ForMember(c => c.E_SP, c => c.MapFrom(s => s.E_SP_A))
                    .ForMember(c => c.E_FDA, c => c.MapFrom(s => s.E_FDA_A))
                    .ForMember(c => c.E_01, c => c.MapFrom(s => s.E_01_A))
                    .ForMember(c => c.E_02, c => c.MapFrom(s => s.E_02_A))
                    .ForMember(c => c.E_03, c => c.MapFrom(s => s.E_03_A))
                    .ForMember(c => c.E_04, c => c.MapFrom(s => s.E_04_A))
                    .ForMember(c => c.E_05, c => c.MapFrom(s => s.E_05_A))
                    .ForMember(c => c.E_06, c => c.MapFrom(s => s.E_06_A))
                    .ForMember(c => c.E_07, c => c.MapFrom(s => s.E_07_A))
                    .ForMember(c => c.E_08, c => c.MapFrom(s => s.E_08_A))
                    .ForMember(c => c.E_09, c => c.MapFrom(s => s.E_09_A))
                    .ForMember(c => c.E_10, c => c.MapFrom(s => s.E_10_A))
                    .ForMember(c => c.E_11, c => c.MapFrom(s => s.E_11_A))
                    .ForMember(c => c.E_12, c => c.MapFrom(s => s.E_12_A))
                    .ForMember(c => c.E_13, c => c.MapFrom(s => s.E_13_A))
                    .ForMember(c => c.E_30, c => c.MapFrom(s => s.E_30_A))
                    .ForMember(c => c.E_Basic_A, c => c.MapFrom(s => s.E_Basic_A))
                    .ForMember(c => c.E_SP_A, c => c.MapFrom(s => s.E_SP_A))
                    .ForMember(c => c.E_FDA_A, c => c.MapFrom(s => s.E_FDA_A))
                    .ForMember(c => c.E_01_A, c => c.MapFrom(s => s.E_01_A))
                    .ForMember(c => c.E_02_A, c => c.MapFrom(s => s.E_02_A))
                    .ForMember(c => c.E_03_A, c => c.MapFrom(s => s.E_03_A))
                    .ForMember(c => c.E_04_A, c => c.MapFrom(s => s.E_04_A))
                    .ForMember(c => c.E_05_A, c => c.MapFrom(s => s.E_05_A))
                    .ForMember(c => c.E_06_A, c => c.MapFrom(s => s.E_06_A))
                    .ForMember(c => c.E_07_A, c => c.MapFrom(s => s.E_07_A))
                    .ForMember(c => c.E_08_A, c => c.MapFrom(s => s.E_08_A))
                    .ForMember(c => c.E_09_A, c => c.MapFrom(s => s.E_09_A))
                    .ForMember(c => c.E_10_A, c => c.MapFrom(s => s.E_10_A))
                    .ForMember(c => c.E_11_A, c => c.MapFrom(s => s.E_11_A))
                    .ForMember(c => c.E_12_A, c => c.MapFrom(s => s.E_12_A))
                    .ForMember(c => c.E_13_A, c => c.MapFrom(s => s.E_13_A))
                    .ForMember(c => c.E_30_A, c => c.MapFrom(s => s.E_30_A))
                    .ForMember(c => c.D_PF, c => c.MapFrom(s => s.D_PF_A))
                    .ForMember(c => c.D_VPF, c => c.MapFrom(s => s.D_VPF_A))
                    .ForMember(c => c.D_01, c => c.MapFrom(s => s.D_01_A))
                    .ForMember(c => c.D_02, c => c.MapFrom(s => s.D_02_A))
                    .ForMember(c => c.D_03, c => c.MapFrom(s => s.D_03_A))
                    .ForMember(c => c.D_04, c => c.MapFrom(s => s.D_04_A))
                    .ForMember(c => c.D_05, c => c.MapFrom(s => s.D_05_A))
                    .ForMember(c => c.D_06, c => c.MapFrom(s => s.D_06_A))
                    .ForMember(c => c.D_07, c => c.MapFrom(s => s.D_07_A))
                    .ForMember(c => c.D_08, c => c.MapFrom(s => s.D_08_A))
                    .ForMember(c => c.D_09, c => c.MapFrom(s => s.D_09_A))
                    .ForMember(c => c.D_10, c => c.MapFrom(s => s.D_10_A))
                    .ForMember(c => c.D_11, c => c.MapFrom(s => s.D_11_A))
                    .ForMember(c => c.D_12, c => c.MapFrom(s => s.D_12_A))
                    .ForMember(c => c.D_13, c => c.MapFrom(s => s.D_13_A))
                    .ForMember(c => c.D_14, c => c.MapFrom(s => s.D_14_A))
                    .ForMember(c => c.D_15, c => c.MapFrom(s => s.D_15_A))
                    .ForMember(c => c.D_16, c => c.MapFrom(s => s.D_16_A))
                    .ForMember(c => c.D_17, c => c.MapFrom(s => s.D_17_A))
                    .ForMember(c => c.D_18, c => c.MapFrom(s => s.D_18_A))
                    .ForMember(c => c.D_19, c => c.MapFrom(s => s.D_19_A))
                    .ForMember(c => c.D_20, c => c.MapFrom(s => s.D_20_A))
                    .ForMember(c => c.D_30, c => c.MapFrom(s => s.D_30_A))
                     .ForMember(c => c.D_PF_A, c => c.MapFrom(s => s.D_PF_A))
                    .ForMember(c => c.D_VPF_A, c => c.MapFrom(s => s.D_VPF_A))
                    .ForMember(c => c.D_01_A, c => c.MapFrom(s => s.D_01_A))
                    .ForMember(c => c.D_02_A, c => c.MapFrom(s => s.D_02_A))
                    .ForMember(c => c.D_03_A, c => c.MapFrom(s => s.D_03_A))
                    .ForMember(c => c.D_04_A, c => c.MapFrom(s => s.D_04_A))
                    .ForMember(c => c.D_05_A, c => c.MapFrom(s => s.D_05_A))
                    .ForMember(c => c.D_06_A, c => c.MapFrom(s => s.D_06_A))
                    .ForMember(c => c.D_07_A, c => c.MapFrom(s => s.D_07_A))
                    .ForMember(c => c.D_08_A, c => c.MapFrom(s => s.D_08_A))
                    .ForMember(c => c.D_09_A, c => c.MapFrom(s => s.D_09_A))
                    .ForMember(c => c.D_10_A, c => c.MapFrom(s => s.D_10_A))
                    .ForMember(c => c.D_11_A, c => c.MapFrom(s => s.D_11_A))
                    .ForMember(c => c.D_12_A, c => c.MapFrom(s => s.D_12_A))
                    .ForMember(c => c.D_13_A, c => c.MapFrom(s => s.D_13_A))
                    .ForMember(c => c.D_14_A, c => c.MapFrom(s => s.D_14_A))
                    .ForMember(c => c.D_15_A, c => c.MapFrom(s => s.D_15_A))
                    .ForMember(c => c.D_16_A, c => c.MapFrom(s => s.D_16_A))
                    .ForMember(c => c.D_17_A, c => c.MapFrom(s => s.D_17_A))
                    .ForMember(c => c.D_18_A, c => c.MapFrom(s => s.D_18_A))
                    .ForMember(c => c.D_19_A, c => c.MapFrom(s => s.D_19_A))
                    .ForMember(c => c.D_20_A, c => c.MapFrom(s => s.D_20_A))
                    .ForMember(c => c.D_30_A, c => c.MapFrom(s => s.D_30_A))
                    .ForMember(c => c.C_TotEarn, c => c.MapFrom(s => s.C_TotEarn))
                    .ForMember(c => c.C_TotDedu, c => c.MapFrom(s => s.C_TotDedu))
                    .ForMember(c => c.C_NetSal, c => c.MapFrom(s => s.C_NetSal))
                    .ForMember(c => c.LWP, c => c.MapFrom(s => s.LWP))
                    .ForMember(c => c.Attendance, c => c.MapFrom(s => s.Attendance))
                     .ForMember(c => c.WorkingDays, c => c.MapFrom(s => s.WorkingDays))
                     .ForMember(c => c.RateVPFA, c => c.MapFrom(s => s.RateVPFA))
                     .ForMember(c => c.ArrearType, c => c.MapFrom(s => s.ArrearType))
                     .ForMember(c => c.DateofGenerateSalary, c => c.MapFrom(s => s.DateofGenerateSalary))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.tblarrearmanualdata, Model.SalaryManualData>(salarymanualdataObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int Updatesalarymanualdata(Model.SalaryManualData editsalarymanualdata)
        {
            log.Info($"Updatesalarymanualdata");
            int res = 0;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    var dtoObj = genericRepo.GetByID<DTOModel.tblarrearmanualdata>(editsalarymanualdata.ID);
                    dtoObj.E_Basic = editsalarymanualdata.E_Basic;
                    dtoObj.E_SP = editsalarymanualdata.E_SP;
                    dtoObj.E_FDA = editsalarymanualdata.E_FDA;
                    dtoObj.E_01 = editsalarymanualdata.E_01;
                    dtoObj.E_02 = editsalarymanualdata.E_02;
                    dtoObj.E_03 = editsalarymanualdata.E_03;
                    dtoObj.E_04 = editsalarymanualdata.E_04;
                    dtoObj.E_05 = editsalarymanualdata.E_05;
                    dtoObj.E_06 = editsalarymanualdata.E_06;
                    dtoObj.E_07 = editsalarymanualdata.E_07;
                    dtoObj.E_08 = editsalarymanualdata.E_08;
                    dtoObj.E_09 = editsalarymanualdata.E_09;
                    dtoObj.E_10 = editsalarymanualdata.E_10;
                    dtoObj.E_11 = editsalarymanualdata.E_11;
                    dtoObj.E_12 = editsalarymanualdata.E_12;
                    dtoObj.E_13 = editsalarymanualdata.E_13;
                    dtoObj.E_30 = editsalarymanualdata.E_30;

                    dtoObj.E_Basic_A = editsalarymanualdata.E_Basic_A;
                    dtoObj.E_SP_A = editsalarymanualdata.E_SP_A;
                    dtoObj.E_FDA_A = editsalarymanualdata.E_FDA_A;
                    dtoObj.E_01_A = editsalarymanualdata.E_01_A;
                    dtoObj.E_02_A = editsalarymanualdata.E_02_A;
                    dtoObj.E_03_A = editsalarymanualdata.E_03_A;
                    dtoObj.E_04_A = editsalarymanualdata.E_04_A;
                    dtoObj.E_05_A = editsalarymanualdata.E_05_A;
                    dtoObj.E_06_A = editsalarymanualdata.E_06_A;
                    dtoObj.E_07_A = editsalarymanualdata.E_07_A;
                    dtoObj.E_08_A = editsalarymanualdata.E_08_A;
                    dtoObj.E_09_A = editsalarymanualdata.E_09_A;
                    dtoObj.E_10_A = editsalarymanualdata.E_10_A;
                    dtoObj.E_11_A = editsalarymanualdata.E_11_A;
                    dtoObj.E_12_A = editsalarymanualdata.E_12_A;
                    dtoObj.E_13_A = editsalarymanualdata.E_13_A;
                    dtoObj.E_30_A = editsalarymanualdata.E_30_A;

                    dtoObj.D_PF = editsalarymanualdata.D_PF;
                    dtoObj.D_VPF = editsalarymanualdata.D_VPF;
                    dtoObj.D_01 = editsalarymanualdata.D_01;
                    dtoObj.D_02 = editsalarymanualdata.D_02;
                    dtoObj.D_03 = editsalarymanualdata.D_03;
                    dtoObj.D_04 = editsalarymanualdata.D_04;
                    dtoObj.D_05 = editsalarymanualdata.D_05;
                    dtoObj.D_06 = editsalarymanualdata.D_06;
                    dtoObj.D_07 = editsalarymanualdata.D_07;
                    dtoObj.D_08 = editsalarymanualdata.D_08;
                    dtoObj.D_09 = editsalarymanualdata.D_09;
                    dtoObj.D_10 = editsalarymanualdata.D_10;
                    dtoObj.D_11 = editsalarymanualdata.D_11;
                    dtoObj.D_12 = editsalarymanualdata.D_12;
                    dtoObj.D_13 = editsalarymanualdata.D_13;
                    dtoObj.D_14 = editsalarymanualdata.D_14;
                    dtoObj.D_15 = editsalarymanualdata.D_15;
                    dtoObj.D_16 = editsalarymanualdata.D_16;
                    dtoObj.D_17 = editsalarymanualdata.D_17;
                    dtoObj.D_18 = editsalarymanualdata.D_18;
                    dtoObj.D_19 = editsalarymanualdata.D_19;
                    dtoObj.D_20 = editsalarymanualdata.D_20;
                    dtoObj.D_30 = editsalarymanualdata.D_30;

                    dtoObj.D_PF_A = editsalarymanualdata.D_PF_A;
                    dtoObj.D_VPF_A = editsalarymanualdata.D_VPF_A;
                    dtoObj.D_01_A = editsalarymanualdata.D_01_A;
                    dtoObj.D_02_A = editsalarymanualdata.D_02_A;
                    dtoObj.D_03_A = editsalarymanualdata.D_03_A;
                    dtoObj.D_04_A = editsalarymanualdata.D_04_A;
                    dtoObj.D_05_A = editsalarymanualdata.D_05_A;
                    dtoObj.D_06_A = editsalarymanualdata.D_06_A;
                    dtoObj.D_07_A = editsalarymanualdata.D_07_A;
                    dtoObj.D_08_A = editsalarymanualdata.D_08_A;
                    dtoObj.D_09_A = editsalarymanualdata.D_09_A;
                    dtoObj.D_10_A = editsalarymanualdata.D_10_A;
                    dtoObj.D_11_A = editsalarymanualdata.D_11_A;
                    dtoObj.D_12_A = editsalarymanualdata.D_12_A;
                    dtoObj.D_13_A = editsalarymanualdata.D_13_A;
                    dtoObj.D_14_A = editsalarymanualdata.D_14_A;
                    dtoObj.D_15_A = editsalarymanualdata.D_15_A;
                    dtoObj.D_16_A = editsalarymanualdata.D_16_A;
                    dtoObj.D_17_A = editsalarymanualdata.D_17_A;
                    dtoObj.D_18_A = editsalarymanualdata.D_18_A;
                    dtoObj.D_19_A = editsalarymanualdata.D_19_A;
                    dtoObj.D_20_A = editsalarymanualdata.D_20_A;
                    dtoObj.D_30_A = editsalarymanualdata.D_30_A;

                    dtoObj.C_TotEarn = editsalarymanualdata.C_TotEarn;
                    dtoObj.C_TotDedu = editsalarymanualdata.C_TotDedu;
                    dtoObj.C_NetSal = editsalarymanualdata.C_NetSal;
                    dtoObj.LWP = editsalarymanualdata.LWP;
                    dtoObj.Attendance = editsalarymanualdata.Attendance;
                    dtoObj.WorkingDays = editsalarymanualdata.WorkingDays;
                    dtoObj.RateVPFA = editsalarymanualdata.RateVPFA;

                    dtoObj.UpdatedBy = editsalarymanualdata.UpdatedBy;
                    dtoObj.UpdatedOn = editsalarymanualdata.UpdatedOn;

                    genericRepo.Update<DTOModel.tblarrearmanualdata>(dtoObj);
                    res = 1;
                });

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return res;
        }


        public bool DeleteArrearmanualdata(int ID)
        {
            log.Info($"ArrearService/DeleteDArates/{ID}");
            bool flag = false;
            try
            {
                DTOModel.tblarrearmanualdata dto = new DTOModel.tblarrearmanualdata();
                dto = genericRepo.GetByID<DTOModel.tblarrearmanualdata>(ID);
                dto.IsDeleted = true;
                genericRepo.Update<DTOModel.tblarrearmanualdata>(dto);
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

        #region Calculate DA Arrear

        public DataSet GetMultipleTableDataResult(string fromYear, string toYear)
        {
            log.Info($"ArrearService/GetMultipleTableDataResult");
            try
            {
                var dataModal = new CalculateArrear();
                DataTable dtFieldIds = new DataTable();
                DataTable DTSALARY = new DataTable();
                DataTable objDt1 = new DataTable("Table 1");
                DataTable objDt2 = new DataTable("Table 2");
                DataTable objDt3 = new DataTable("Table 3");
                DataTable objDt4 = new DataTable("Table 4");
                DataTable objDt5 = new DataTable("Table 5");
                DataTable objDt6 = new DataTable("Table 6");
                DataTable objDt7 = new DataTable("Table 7");
                DataTable objDt8 = new DataTable("Table 8");
                DataTable objDt9 = new DataTable("Table 9");
                DataTable objDt10 = new DataTable("Table 10");
                #region Get tblFinalMonthlySalaryData
                objDt1 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "1");
                #endregion

                #region Get tblFinalMonthlySalaryData for PAYArrear
                objDt2 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "2");
                #endregion

                #region Get TBLMSTEMPLOYEESALARY 
                objDt3 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "3");
                #endregion

                #region Get TBLARREARDUMMY
                objDt4 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "4");
                #endregion

                #region Get MONTHLYINPUT
                objDt5 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "5");
                #endregion

                #region Get SALARYHEADSNew
                objDt6 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "6");
                #endregion

                #region Get HEADLOOKUPHISTORY
                objDt7 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "7");
                #endregion

                #region Get ARREARDETAIL
                objDt8 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "8");
                #endregion

                #region Get HEADLOOKUPHISTORY Head value
                objDt9 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "9");
                #endregion

                #region blank table
                objDt10 = arrearRepo.GetMultipleTableDataResult(fromYear, toYear, "10");
                #endregion


                dataModal.SearchedResultDT = new DataSet();
                dataModal.SearchedResultDT.Tables.Add(objDt1);
                dataModal.SearchedResultDT.Tables.Add(objDt2);
                dataModal.SearchedResultDT.Tables.Add(objDt3);
                dataModal.SearchedResultDT.Tables.Add(objDt4);
                dataModal.SearchedResultDT.Tables.Add(objDt5);
                dataModal.SearchedResultDT.Tables.Add(objDt6);
                dataModal.SearchedResultDT.Tables.Add(objDt7);
                dataModal.SearchedResultDT.Tables.Add(objDt8);
                dataModal.SearchedResultDT.Tables.Add(objDt9);
                dataModal.SearchedResultDT.Tables.Add(objDt10);
                return dataModal.SearchedResultDT;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int CalculateDAArrear(int listcount, int[] empIDs, string ArrearType, int arrPeriods, DataSet searchResult, int BranchID, DateTime fromdate, DateTime todate, int vpfRate, string formula, string orderNumber, DateTime? orderDate)
        {
            bool ContinueProcess = false;
            int OrgBranchID = BranchID;
            /*colv = false, RND = false, COND = false;*/
            int period = 0, SEQNO = 0, /*RNDTO = 0,*/ count = 0, empID = 0;
            int calc = 0, COLVAL = 0, IntSH = 0;
            double LWP, CalculatedValue, ColValue = 0;
            DataRowView DRSal = null, DRMI = null, DRMI1;
            string ColName;
            string fromYear = fromdate.Year.ToString() + (fromdate.Month.ToString().Length == 1 ? "0" + fromdate.Month.ToString() : fromdate.Month.ToString());
            string toYear = todate.Year.ToString() + (todate.Month.ToString().Length == 1 ? "0" + todate.Month.ToString() : todate.Month.ToString());

            try
            {
                string prd = "";

                #region listcountLoop

                for (count = 0; count < listcount; count++)
                {
                    if (OrgBranchID == 0)
                        BranchID = 0;


                    empID = empIDs[count];

                    #region Period Loop
                    for (period = 0; period < arrPeriods; period++)
                    {
                        if (BranchID == 0)
                        {
                            var getbranchID = getBranchID((fromdate.Month + period), fromdate.Year, empID, BranchID);
                            DataTable dtgetbranchID = Common.ExtensionMethods.ToDataTable(getbranchID);
                            if (dtgetbranchID.Rows.Count > 0)
                            {
                                BranchID = Convert.ToInt32(dtgetbranchID.Rows[0]["BranchID"]);

                            }
                        }
                        if (BranchID > 0)
                        {
                            prd = fromdate.Year.ToString() + ((fromdate.Month + period).ToString().Length == 1 ? "0" + (fromdate.Month + period) : (fromdate.Month + period).ToString());
                            searchResult.Tables[0].DefaultView.RowFilter = "EmployeeID =" + empID + " AND SalYear = '" + fromdate.Year + "' AND  SalMonth = '" + (fromdate.Month + period) + "' ";
                            searchResult.Tables[1].DefaultView.RowFilter = " EmployeeID= " + empID + "";

                            #region CHECK THAT EMPLOYEE IS EXIST IN A PARTICULAR BRANCH IN A PARTICULAR PERIOD

                            //searchResult.Tables[0].DefaultView.RowFilter = "EmployeeID ='" + empID + "' AND PERIOD = '" + prd + "'  AND BranchID =" + BranchID;
                            searchResult.Tables[0].DefaultView.RowFilter = "EmployeeID ='" + empID + "' AND PERIOD = '" + prd + "'";
                            if (searchResult.Tables[0].DefaultView.Count > 0)
                            {
                                ContinueProcess = true;
                                DRSal = searchResult.Tables[0].DefaultView[0];
                            }
                            else
                            {

                                ContinueProcess = false;
                            }


                            #endregion

                            if (ContinueProcess == true)
                            {
                                searchResult.Tables[2].DefaultView.RowFilter = "EmployeeID =" + empID;
                                if (searchResult.Tables[2].DefaultView.Count > 0)
                                {
                                    DRMI = searchResult.Tables[2].DefaultView[0];
                                }

                                searchResult.Tables[4].DefaultView.RowFilter = "EmployeeID =" + empID + " AND PERIOD = '" + prd + "'  AND BranchID =" + BranchID;
                                if (searchResult.Tables[4].DefaultView.Count > 0)
                                {
                                    DRMI1 = searchResult.Tables[4].DefaultView[0];
                                    LWP = Convert.ToDouble(searchResult.Tables[4].DefaultView[0]["LWP"].ToString());
                                }
                                else
                                {
                                    LWP = 0;
                                }

                                var getmanualdata = manualdata(empID, fromdate.Year, (fromdate.Month + period), "LWP");
                                DataTable dtgetmanualdata = Common.ExtensionMethods.ToDataTable(getmanualdata);
                                if (dtgetmanualdata.Rows.Count > 0)
                                {
                                    COLVAL = Convert.ToInt32(dtgetmanualdata.Rows[0]["HeadValue"]);

                                }

                                if (COLVAL > 0)
                                {
                                    LWP = COLVAL;
                                }
                                //searchResult.Tables[5].DefaultView.RowFilter = "PERIOD = '" + prd + "' and   FORMULACOLUMN = 1  AND ACTIVEFIELD = 1 AND FIELDNAME NOT LIKE 'C%' ";

                                searchResult.Tables[5].DefaultView.RowFilter = "PERIOD = '" + prd + "' and  ACTIVEFIELD = 1 AND FIELDNAME NOT LIKE 'C%' ";
                                searchResult.Tables[5].DefaultView.Sort = "SEQNO";

                                searchResult.Tables[3].DefaultView.RowFilter = "EmployeeID =" + empID + " AND SalYear = '" + fromdate.Year + "' AND  SalMonth = '" + (fromdate.Month + period) + "' AND BranchID = " + BranchID;
                                SEQNO = searchResult.Tables[3].DefaultView.Count + 1;
                                DataRow DRArrearDummy;
                                DRArrearDummy = searchResult.Tables[3].NewRow();
                                DRArrearDummy["BranchCode"] = BranchID;
                                DRArrearDummy["SalMonth"] = fromdate.Month + period;
                                DRArrearDummy["SalYear"] = fromdate.Year;
                                DRArrearDummy["SEQNO"] = SEQNO;
                                DRArrearDummy["EMPLOYEECODE"] = empID;
                                DRArrearDummy["ArrearType"] = "D";
                                DRArrearDummy["CreatedOn"] = DateTime.Now;
                                DRArrearDummy["CreatedBy"] = 1;

                                #region CALCULATION OF HEADS WHICH ARE FORMULA DEPENDENT

                                for (IntSH = 0; IntSH < searchResult.Tables[5].DefaultView.Count; IntSH++)
                                {
                                    searchResult.Tables[6].DefaultView.RowFilter = "PERIOD ='" + prd + "' AND FIELDNAME = '" + searchResult.Tables[5].DefaultView[IntSH]["FieldName"].ToString() + "'";
                                    ColName = searchResult.Tables[5].DefaultView[IntSH]["FieldName"].ToString();
                                    if (searchResult.Tables[5].DefaultView[IntSH]["FORMULACOLUMN"].ToString() == "True")
                                    {
                                        #region Formula dependent Column
                                        if (searchResult.Tables[5].DefaultView[IntSH]["Conditional"].ToString() == "True")
                                        {
                                            if (ColName == "E_01") //for DA
                                            {
                                                CalculatedValue = CalculateFromulaValueArrear(searchResult.Tables[6].DefaultView[0]["LookUpHead"].ToString(), DRSal, null);
                                            }
                                            else
                                            {
                                                if (searchResult.Tables[6].DefaultView.Count > 0)
                                                {
                                                    CalculatedValue = CalculateFromulaValueArrear(searchResult.Tables[6].DefaultView[0]["LookUpHeadName"].ToString(), DRSal, null);
                                                }
                                                else
                                                {
                                                    CalculatedValue = 0;
                                                }
                                            }
                                            ColValue = ConditionalHeadArrear(empID, ColName, CalculatedValue, fromdate.Year, (fromdate.Month + period), DRMI, vpfRate);
                                        }
                                        else
                                        {
                                            //string formula = "17 % of (|E_Basic| + |E_SP|)";
                                            if (ColName == "E_01") //for DA
                                            {
                                                ColValue = CalculateFromulaValueArrear(formula, DRSal, null);
                                            }
                                            else
                                            {
                                                if (searchResult.Tables[6].DefaultView.Count > 0)
                                                {
                                                    ColValue = CalculateFromulaValueArrear(searchResult.Tables[6].DefaultView[0]["LookUpHeadName"].ToString(), DRSal, null);
                                                    if (ColName == "D_PF")
                                                    {
                                                        ColValue = Math.Round(ColValue, 0);

                                                    }
                                                }
                                                else
                                                {
                                                    ColValue = 0;

                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Formula independent Column
                                        if (ColName != "E_Basic" && ColName != "E_FDA" && ColName != "E_SP")
                                        {
                                            searchResult.Tables[0].DefaultView.RowFilter = "EmployeeID =" + empID + " AND SalYear = '" + fromdate.Year + "' AND  SalMonth = '" + (fromdate.Month + period) + "' ";
                                            ColValue = checkingTransportdetails(empID, Convert.ToInt32(fromdate.Month + period), Convert.ToInt32(fromdate.Year), fromYear, toYear, searchResult, formula);
                                        }
                                        #endregion
                                    }
                                    if (Convert.ToDouble(DRSal.Row[ColName].ToString()) > ColValue && ColValue < 0)
                                    {
                                        ColValue = Convert.ToDouble(DRSal.Row[ColName].ToString());
                                    }
                                    COLVAL = 0;

                                    if (LWP > 0)
                                    {
                                        if (ColName != "E_22")
                                        {
                                            //ColValue = ColValue - ((ColValue * LWP) / Convert.ToInt32(DRSal.Row["WorkingDays"].ToString()));
                                            ColValue = ColValue - ((ColValue / Convert.ToInt32(DRSal.Row["WorkingDays"].ToString())) * LWP);
                                        }
                                    }

                                    if (ColName != "E_Basic" && ColName != "E_FDA" && ColName != "E_SP")
                                    {
                                        DRArrearDummy[ColName] = ColValue;
                                        DRSal[ColName] = ColValue;
                                    }
                                }
                                searchResult.Tables[3].Rows.Add(DRArrearDummy.ItemArray);

                                #endregion

                            }
                        }
                        if (OrgBranchID == 0)
                        {
                            BranchID = 0;
                        }

                    }
                    #endregion


                    #region Deleting Duplicate data from tblarreardetail and tblfinalmonthlysalary

                    DataTable dtdelarr = new DataTable();
                    DataTable dtdelsal = new DataTable();
                    int PrvBranchID = 0;
                    if (OrgBranchID == 0)
                    {
                        for (period = 0; period < arrPeriods; period++)
                        {
                            if (BranchID == 0)
                            {
                                var getbranchID = getBranchID((fromdate.Month + period), fromdate.Year, empID, BranchID);
                                DataTable dtgetbranchID = Common.ExtensionMethods.ToDataTable(getbranchID);
                                if (dtgetbranchID.Rows.Count > 0)
                                {
                                    BranchID = Convert.ToInt32(dtgetbranchID.Rows[0]["BranchID"]);
                                    if (PrvBranchID != BranchID)
                                    {
                                        dtdelarr = arrearRepo.DeleteDuplicateData(empID, fromYear, toYear, BranchID, 1, "D");
                                        dtdelsal = arrearRepo.DeleteDuplicateData(empID, fromYear, toYear, BranchID, 2, "D");
                                    }
                                    PrvBranchID = BranchID;
                                }
                            }
                            BranchID = 0;
                        }
                    }
                    else
                    {
                        dtdelarr = arrearRepo.DeleteDuplicateData(empID, fromYear, toYear, BranchID, 1, "D");
                        dtdelsal = arrearRepo.DeleteDuplicateData(empID, fromYear, toYear, BranchID, 2, "D");
                    }

                    #endregion
                }

                int res = arrearRepo.UpdateTblArreardummy(searchResult.Tables[3]);



                #region Transfer Data

                int transferdata = TransferData(fromYear, toYear, searchResult, formula, orderNumber, orderDate);

                #endregion

                #region Update WorkingDays and lwp

                arrearRepo.UpdateWorkingDayslwp();

                #endregion

                #region Calculate period
                arrearRepo.Calculateperiod();
                #endregion


                #region CREATE SUM FROM TBLARREARDETAIL And TRANSFER INTO TBLFINALMONTHLYSALARY

                for (count = 0; count < listcount; count++)
                {

                    DataTable dtfinalSalary = new DataTable();
                    dtfinalSalary = arrearRepo.TransferarreardetailinFinalmonthlysalary(empIDs[count], fromYear, toYear, OrgBranchID);

                }
                #endregion

                #endregion
                return calc;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public double ConditionalHeadArrear(int empID, string colName, double HeadValue, int year, int month, DataRowView DRCurrentRow, int vpfRate)
        {
            int VPFRATE = 0;
            double ConditionalHeadArrear = 0;
            DataTable dtVPF = new DataTable();
            try
            {
                #region E_02/D_07
                if (colName == "E_02")
                {
                    if (Convert.ToBoolean(DRCurrentRow.Row["HRA"].ToString() == "" ? false : Convert.ToBoolean(DRCurrentRow.Row["HRA"].ToString()) == false ? false : true) == true)
                    {
                        ConditionalHeadArrear = HeadValue;
                    }
                    else
                    {
                        ConditionalHeadArrear = 0.0;
                    }

                }
                if (colName == "D_07")
                {
                    if (Convert.ToBoolean(DRCurrentRow.Row["IsFlatDeduction"].ToString() == "" ? false : Convert.ToBoolean(DRCurrentRow.Row["IsFlatDeduction"].ToString()) == false ? false : true) == true)
                    {
                        ConditionalHeadArrear = HeadValue;
                    }
                    else
                    {
                        ConditionalHeadArrear = 0.0;
                    }

                }

                #endregion

                #region VPF
                if (colName == "D_VPF")
                {
                    if (empID == 0)
                    { }
                    else
                    {
                        var result = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(em => (empID > 0 ? em.EmployeeID == empID : 1 > 0) && (month > 0 ? em.SalMonth == month : 1 > 0)
                        && (year > 0 ? em.SalYear == year : 1 > 0) && (em.RateVPFA > 0));

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Data.Models.tblFinalMonthlySalary, Model.SalaryManualData>()
                            .ForMember(c => c.SalYear, c => c.MapFrom(s => s.SalYear))
                             .ForMember(c => c.SalYear, c => c.MapFrom(s => s.SalMonth))
                            .ForMember(c => c.RateVPFA, c => c.MapFrom(s => s.RateVPFA))
                            .ForAllOtherMembers(c => c.Ignore());
                        });
                        var list = Mapper.Map<List<Model.SalaryManualData>>(result);

                        DataTable dt = Common.ExtensionMethods.ToDataTable(list);
                        if (dt.Rows.Count > 0)
                        {
                            if (vpfRate == 0)
                            {
                                if (VPFRATE > 24)
                                {
                                    VPFRATE = 24;
                                }
                                else
                                {
                                    VPFRATE = Convert.ToInt32(dt.Rows[0]["RateVPFA"].ToString());
                                }

                            }
                            else if (vpfRate == 1)
                            {
                                VPFRATE = Convert.ToInt32(dt.Rows[0]["RateVPFA"].ToString());

                            }
                            ConditionalHeadArrear = Math.Round((HeadValue * VPFRATE) / 100, 0);
                        }
                        else
                        {
                            ConditionalHeadArrear = 0;
                        }
                    }
                }

                #endregion

                #region D_06/D_10/D_14

                if (colName == "D_06")
                {
                    if (Convert.ToBoolean(DRCurrentRow.Row["SportClub"].ToString() == "" ? false : Convert.ToBoolean(DRCurrentRow.Row["SportClub"].ToString()) == false ? false : true) == true)

                    {
                        ConditionalHeadArrear = 2;
                    }
                    else
                    {
                        ConditionalHeadArrear = 0;
                    }

                }
                if (colName == "D_10")
                {
                    ConditionalHeadArrear = 0;
                }

                if (colName == "D_14")
                {
                    if (Convert.ToBoolean(DRCurrentRow.Row["UnionFee"].ToString() == "" ? false : Convert.ToBoolean(DRCurrentRow.Row["UnionFee"].ToString()) == false ? false : true) == true)

                    {
                        ConditionalHeadArrear = HeadValue;
                    }
                    else
                    {
                        ConditionalHeadArrear = 0;
                    }
                }
                #endregion

                return ConditionalHeadArrear;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public double CalculateFromulaValueArrear(string HeadValue, DataRowView drfinal, DataRow dtNew)
        {
            double EvaluateFormulaArrear = 0, TempFormula = 0;
            DataTable dt = new DataTable();
            string TempOperator, num = "";

            string HeadName = "";



            try
            {
                if (HeadValue.Length == 0)
                {
                    EvaluateFormulaArrear = 0;

                }
                if (IsNumeric(HeadValue) == true)
                {
                    EvaluateFormulaArrear = Convert.ToDouble(HeadValue);
                    goto a;
                }

                dt = arrearRepo.GetDatatableResult(HeadValue);
                #region EvaluateFormulaArrear
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["value"].ToString().Trim() == "+" || dt.Rows[i]["value"].ToString().Trim() == "-")
                        {
                            TempOperator = dt.Rows[i]["value"].ToString();
                        }
                        if (dt.Rows[i]["value"].ToString().Trim() != "+" && dt.Rows[i]["value"].ToString().Trim() != "-")
                        {
                            if (dt.Rows[i]["value"].ToString().Trim() == "-")
                            {
                                HeadName = dt.Rows[i]["value"].ToString();
                                if (dtNew != null)
                                {
                                    TempFormula = TempFormula - Convert.ToDouble(dtNew[HeadName]);
                                }
                                else
                                {
                                    TempFormula = TempFormula - Convert.ToDouble(drfinal.Row[HeadName]);
                                }

                            }
                            else
                            {
                                HeadName = dt.Rows[i]["value"].ToString();
                                if (dtNew != null)
                                {
                                    if (dtNew[HeadName].ToString() == "")
                                        TempFormula = TempFormula + 0;
                                    else
                                        TempFormula = TempFormula + Convert.ToDouble(dtNew[HeadName]);
                                }
                                else
                                {
                                    TempFormula = TempFormula + Convert.ToDouble(drfinal.Row[HeadName]);
                                }
                            }
                        }
                        else
                        {

                            TempOperator = (dt.Rows[i]["value"].ToString().Trim() == "-" ? "-" : "+");
                        }
                    }
                    num = HeadValue.ToString().Substring(0, HeadValue.ToString().IndexOf('%') - 1);
                    EvaluateFormulaArrear = Convert.ToDouble(num == "" ? "0" : num) * 0.01 * TempFormula;
                }
                #endregion
                a:
                return EvaluateFormulaArrear;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public int TransferData(string fromYear, string toYear, DataSet searchResult, string formula, string orderNumber, DateTime? orderDate)
        {
            int flag = 1;
            double ColValue = 0;
            string DOG;
            DataTable dtVPF = new DataTable();
            try
            {
                DataRow DRArrearDetail;
                DOG = DateTime.Now.Year + (DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + (DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                for (int i = 0; i < searchResult.Tables[3].Rows.Count; i++)
                {
                    DRArrearDetail = searchResult.Tables[9].NewRow();
                    searchResult.Tables[0].DefaultView.RowFilter = "EmployeeID =" + searchResult.Tables[3].Rows[i]["Employeecode"].ToString() + " AND SALYEAR = " + searchResult.Tables[3].Rows[i]["SalYear"].ToString() + " and Salmonth= " + searchResult.Tables[3].Rows[i]["SalMonth"].ToString() + "";

                    if (searchResult.Tables[1].Rows.Count > 0)
                    {
                        searchResult.Tables[1].DefaultView.RowFilter = "EmployeeID =" + searchResult.Tables[3].Rows[i]["Employeecode"].ToString() + " AND SALYEAR = " + searchResult.Tables[3].Rows[i]["SalYear"].ToString() + " and Salmonth= " + searchResult.Tables[3].Rows[i]["SalMonth"].ToString() + "";
                    }

                    DRArrearDetail["BranchCode"] = searchResult.Tables[3].Rows[i]["BranchCode"].ToString();
                    DRArrearDetail["SalMonth"] = searchResult.Tables[3].Rows[i]["SalMonth"].ToString();
                    DRArrearDetail["SalYear"] = searchResult.Tables[3].Rows[i]["SalYear"].ToString();
                    DRArrearDetail["SEQNO"] = searchResult.Tables[3].Rows[i]["SEQNO"].ToString();
                    DRArrearDetail["EMPLOYEECODE"] = searchResult.Tables[3].Rows[i]["EmployeeCode"].ToString();
                    DRArrearDetail["ArrearType"] = searchResult.Tables[3].Rows[i]["ArrearType"].ToString();
                    DRArrearDetail["DateOfGenerateArrear"] = DOG;
                    DRArrearDetail["PeriodFrom"] = fromYear;
                    DRArrearDetail["PeriodTo"] = toYear;
                    DRArrearDetail["Formula"] = formula;
                    DRArrearDetail["CreatedOn"] = DateTime.Now;
                    DRArrearDetail["CreatedBy"] = 1;

                    int flag1 = 0;
                    #region CALCULATE DIFFERENCE BETWEEN TBLARREARDUMMY AND TBLFINALMONTHLYSALARY 
                    int j = 0;
                    for (j = 6; j < 70; j++)
                    {
                        if (searchResult.Tables[3].Rows[i][j].ToString() == "")
                        {
                            ColValue = 0;
                        }
                        else
                        {
                            if (searchResult.Tables[3].Columns[j].ColumnName.ToString() == "E_01")
                            {
                                if (Convert.ToBoolean(searchResult.Tables[3].Rows[i]["ArrearType"].ToString() == "B"))
                                {
                                    ColValue = Convert.ToDouble(searchResult.Tables[3].Rows[i][j].ToString());
                                }
                                else
                                {
                                    if (searchResult.Tables[1].DefaultView.Count == 0)
                                    {
                                        ColValue = Convert.ToDouble(searchResult.Tables[3].Rows[i][j].ToString()) - (Convert.ToInt32(searchResult.Tables[8].DefaultView[0]["value"].ToString()) * (Convert.ToDouble(searchResult.Tables[0].DefaultView[0]["E_Basic_A"].ToString()) + Convert.ToDouble(searchResult.Tables[0].DefaultView[0]["E_SP_A"].ToString())) * 0.01);

                                    }
                                    else if (Convert.ToDouble(searchResult.Tables[1].DefaultView[0]["E_Basic"].ToString()) <= (Convert.ToDouble(searchResult.Tables[0].DefaultView[0]["E_Basic"].ToString())))
                                    {
                                        ColValue = Convert.ToDouble(searchResult.Tables[3].Rows[i][j].ToString()) - (Convert.ToInt32(searchResult.Tables[8].DefaultView[0]["value"].ToString()) * (Convert.ToDouble(searchResult.Tables[0].DefaultView[0]["E_Basic_A"].ToString()) + Convert.ToDouble(searchResult.Tables[0].DefaultView[0]["E_SP_A"].ToString())) * 0.01);


                                        //ColValue = Convert.ToDouble(searchResult.Tables[3].Rows[i][j].ToString()) - ((Convert.ToInt32(searchResult.Tables[8].DefaultView[0]["value"].ToString())) * (Convert.ToDouble(searchResult.Tables[1].DefaultView[0]["E_Basic"].ToString()) + Convert.ToDouble(searchResult.Tables[0].DefaultView[0]["E_SP_A"].ToString()) + (Convert.ToDouble(searchResult.Tables[0].DefaultView[0]["E_Basic_A"].ToString()))) * 0.01);
                                    }
                                    DRArrearDetail["sum_e_01"] = searchResult.Tables[3].Rows[i][j].ToString();
                                    if (ColValue == 0)
                                    {
                                        flag1 = 1;
                                    }
                                }
                            }
                            else
                            {
                                if (Convert.ToBoolean(searchResult.Tables[3].Rows[i]["ArrearType"].ToString() == "B"))
                                {
                                    ColValue = Convert.ToDouble(searchResult.Tables[3].Rows[i][j].ToString());
                                }
                                else
                                {
                                    if (flag1 == 1)
                                    {
                                        if (searchResult.Tables[3].Columns[j].ColumnName.ToString() == "E_22")
                                        {
                                            ColValue = Convert.ToDouble(searchResult.Tables[3].Rows[i][j].ToString());
                                        }
                                        else
                                        {
                                            ColValue = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (searchResult.Tables[3].Columns[j].ColumnName.ToString() == "E_22")
                                        {
                                            ColValue = Convert.ToDouble(searchResult.Tables[3].Rows[i][j].ToString());
                                        }
                                        else
                                        {
                                            ColValue = Convert.ToDouble(searchResult.Tables[3].Rows[i][j].ToString()) - ((Convert.ToDouble(searchResult.Tables[0].DefaultView[0][searchResult.Tables[3].Columns[j].ColumnName + "_A"].ToString())));
                                        }

                                        if (ColValue < 1)
                                        { ColValue = 0; }
                                    }
                                }
                            }

                        }

                        DRArrearDetail[searchResult.Tables[3].Columns[j].ColumnName] = ColValue;
                    }

                    searchResult.Tables[9].Rows.Add(DRArrearDetail.ItemArray);

                    #endregion
                }
                int res = arrearRepo.UpdateTblArrearDetail(searchResult.Tables[9]);
                if(res>1)
                {
                    arrearRepo.UPdateOrderNumberDate(DOG, orderNumber, orderDate);
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public double checkingTransportdetails(int empID, int month, int year, string fromYear, string toYear, DataSet searchResult, string formula)
        {
            DataTable dtVPF = new DataTable();
            int Newdapercent = 0, olddapercent = 0;
            var fArray = formula.Split('%');
            if (fArray.Length > 0)
                Newdapercent = Convert.ToString(fArray[0]).Trim() == "" ? 0 : Convert.ToInt32(fArray[0]);

            double oldta = 0, newta = 0;
            if (Newdapercent == 0)
                return newta;

            try
            {
                searchResult.Tables[9].DefaultView.RowFilter = "EmployeeID =" + empID;
                if (searchResult.Tables[8].Rows.Count > 0)
                {
                    olddapercent = Convert.ToInt32(searchResult.Tables[8].DefaultView[0]["value"].ToString());


                }
                if (searchResult.Tables[0].DefaultView.Count > 0)
                {
                    if (Convert.ToInt32(searchResult.Tables[0].DefaultView[0]["E_22_A"]) > 1)
                    {
                        oldta = (Convert.ToInt32(searchResult.Tables[0].DefaultView[0]["E_22_A"]) * 100) / (olddapercent + 100);
                        newta = oldta + (oldta * Newdapercent / 100);
                        newta = newta - Convert.ToInt32(searchResult.Tables[0].DefaultView[0]["E_22_A"]);
                    }
                    else
                    {
                        newta = 0;
                    }
                }
                else { newta = 0; }
                //int res = arrearRepo.UpdateTransportDetails( empID, newta,month,year, fromYear, toYear);
                return newta;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public List<ArrearManualData> manualdata(int empID, int year, int month, string columnName)
        {
            log.Info($"ArrearService/manualdata");
            try
            {
                var result = genericRepo.Get<DTOModel.TblArrearMonthlyInput>(em => (empID > 0 ? em.EmployeeId == empID : 1 > 0) && (month > 0 ? em.Month == month : 1 > 0)
               && (year > 0 ? em.Year == year : 1 > 0) && (em.HeadValue > 0 ? em.HeadValue == 80 : 1 > 0));

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblArrearMonthlyInput, Model.ArrearManualData>()
                    .ForMember(c => c.HeadValue, c => c.MapFrom(s => s.HeadValue))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBranch = Mapper.Map<List<Model.ArrearManualData>>(result);
                return listBranch.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public SalaryHead GetSalaryHeadFormulaRule(string fieldName)
        {
            log.Info($"SalaryHeadRuleService/GetSalaryHeadFormulaRule/{fieldName}");
            SalaryHead salaryHead = new SalaryHead();
            try
            {
                var dtoSalaryHead = genericRepo.Get<DTOModel.TblHeadLookup>(x => x.FieldName == fieldName).FirstOrDefault();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblHeadLookup, SalaryHead>()
                     .ForMember(d => d.LookUpHead, o => o.MapFrom(s => s.HeadValueName))

                    .ForAllOtherMembers(d => d.Ignore());
                });
                salaryHead = Mapper.Map<DTOModel.TblHeadLookup, Model.SalaryHead>(dtoSalaryHead);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return salaryHead;
        }


        public List<SelectListModel> getemployeeByBranchID(int branchID, string fromDate, string toDate, string empID)
        {
            try
            {
                var result = arrearRepo.getemployeeByBranchID(branchID, fromDate, toDate, empID);
                List<Model.SelectListModel> employeeList = new List<Model.SelectListModel>();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.getemployeeByBranchID_Result, Model.SelectListModel>()
                    .ForMember(c => c.id, c => c.MapFrom(m => m.EmployeeID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.EMP));
                });
                return Mapper.Map<List<DTOModel.getemployeeByBranchID_Result>, List<Model.SelectListModel>>(result); ;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<ArrearManualData> getBranchID(int salmonth, int salyear, int empID, int branchID)
        {

            try
            {
                if (branchID == 0)
                {
                    var dto = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.EmployeeID == empID && x.SalMonth == salmonth && x.SalYear == salyear && x.BranchID != 44 && x.RecordType == "S");
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Data.Models.tblFinalMonthlySalary, Model.ArrearManualData>()
                       .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    var listBranch = Mapper.Map<List<Model.ArrearManualData>>(dto);
                    return listBranch.ToList();
                }
                else
                {
                    var dto = genericRepo.Get<DTOModel.tblFinalMonthlySalary>(x => x.EmployeeID == empID && x.SalMonth == salmonth && x.SalYear == salyear && x.BranchID == branchID && x.RecordType == "S");
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Data.Models.tblFinalMonthlySalary, Model.ArrearManualData>()
                       .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                        .ForAllOtherMembers(c => c.Ignore());
                    });
                    var listBranch = Mapper.Map<List<Model.ArrearManualData>>(dto);
                    return listBranch.ToList();
                }

            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Calculate PAY Arrear
        public int CalculatePayArrear(int listcount, DataTable dtPayArrear, string ArrearType, int arrPeriods, DataSet searchResult, int BranchID, DateTime fromdate, DateTime todate, int vpfRate, string orderNumber, DateTime? orderDate)
        {
            bool AllEmployees = false;
            bool ContinueProcess = false, /*colv = false,*/ RND = false, COND = false;
            int period = 0, SEQNO = 0, RNDTO = 0, count = 0, empID = 0;
            int calc = 0, /*COLVAL = 0,*/ IntSH = 0;
            double LWP, CalculatedValue, ColValue = 0, DaysInMonth = 0;
            DataRowView DRSal = null, DRMI = null, DRES = null, DRES1 = null;
            bool AD = false;   //Attendence Dependent
            bool MI = false;   //Monthly Input
            bool FC = false;   //Formula Column
            string ColName;
            string fromYear = fromdate.Year.ToString() + (fromdate.Month.ToString().Length == 1 ? "0" + fromdate.Month.ToString() : fromdate.Month.ToString());
            string toYear = todate.Year.ToString() + (todate.Month.ToString().Length == 1 ? "0" + todate.Month.ToString() : todate.Month.ToString());

            try

            {
                string prd = "";

                #region listcountLoop

                for (count = 0; count < dtPayArrear.Rows.Count; count++)
                {
                    if (Convert.ToInt32(dtPayArrear.Rows[count]["BasicSalary"].ToString()) > 0 || Convert.ToInt32(dtPayArrear.Rows[count]["IncomeTaxdeduction"].ToString()) > 0)
                    {

                        empID = Convert.ToInt32(dtPayArrear.Rows[count][0]);

                        #region Period Loop
                        for (period = 0; period < arrPeriods; period++)
                        {

                            prd = fromdate.Year.ToString() + ((fromdate.Month + period).ToString().Length == 1 ? "0" + (fromdate.Month + period) : fromdate.Month.ToString());


                            if (BranchID == 0)
                            {
                                var getbranchID = getBranchID((fromdate.Month + period), fromdate.Year, empID, BranchID);
                                DataTable dtgetbranchID = Common.ExtensionMethods.ToDataTable(getbranchID);
                                if (dtgetbranchID.Rows.Count > 0)
                                {
                                    BranchID = Convert.ToInt32(dtgetbranchID.Rows[0]["BranchID"]);

                                }
                            }

                            #region CHECK THAT EMPLOYEE IS EXIST IN A PARTICULAR BRANCH IN A PARTICULAR PERIOD

                            searchResult.Tables[0].DefaultView.RowFilter = "EmployeeID ='" + empID + "' AND PERIOD = '" + prd + "'";
                            if (searchResult.Tables[0].DefaultView.Count > 0)
                            {
                                ContinueProcess = true;
                                DRSal = searchResult.Tables[0].DefaultView[0];
                            }
                            else
                            {
                                ContinueProcess = false;
                            }
                            #endregion

                            if (ContinueProcess == true)
                            {
                                searchResult.Tables[2].DefaultView.RowFilter = "EmployeeID =" + empID;
                                if (searchResult.Tables[2].DefaultView.Count > 0)
                                {
                                    DRES = searchResult.Tables[2].DefaultView[0];
                                }

                                searchResult.Tables[4].DefaultView.RowFilter = "EmployeeID =" + empID + " AND PERIOD = '" + prd + "' ";
                                if (searchResult.Tables[4].DefaultView.Count > 0)
                                {
                                    DRMI = searchResult.Tables[4].DefaultView[0];
                                    LWP = Convert.ToDouble(searchResult.Tables[4].DefaultView[0]["LWP"].ToString());
                                }
                                else
                                {
                                    LWP = 0;
                                }


                                searchResult.Tables[6].DefaultView.RowFilter = "ACTIVEFIELD =" + 1 + " AND LOANHEAD =" + 0 + " AND PERIOD = '" + prd + "' and  fieldname not in('E_31','E_32','C_TotEarn','C_TotDedu','C_NetSal','C_Pension','C_GrossSalary','OTHrs','AOTHrs','LWP') ";
                                searchResult.Tables[6].DefaultView.Sort = "SEQNO";
                                if (searchResult.Tables[6].DefaultView.Count > 0)
                                {
                                    DRES1 = searchResult.Tables[6].DefaultView[0];
                                }

                                searchResult.Tables[3].DefaultView.RowFilter = "EmployeeID =" + empID + " AND SalYear = '" + fromdate.Year + "' AND  SalMonth = '" + (fromdate.Month + period) + "'";
                                SEQNO = searchResult.Tables[3].DefaultView.Count + 1;

                                DataRow DRArrearDummy;
                                DRArrearDummy = searchResult.Tables[3].NewRow();
                                DRArrearDummy["BranchCode"] = BranchID;
                                DRArrearDummy["SalMonth"] = fromdate.Month + period;
                                DRArrearDummy["SalYear"] = fromdate.Year;
                                DRArrearDummy["SEQNO"] = SEQNO;
                                DRArrearDummy["EMPLOYEECODE"] = empID;
                                DRArrearDummy["ArrearType"] = "B";
                                DRArrearDummy["CreatedOn"] = DateTime.Now;
                                DRArrearDummy["CreatedBy"] = 1;

                                if (fromdate.Day != 1 && period == 0)
                                {
                                    DateTime last_Date = new DateTime(fromdate.Year, fromdate.Month, 1)
                                        .AddMonths(1).AddDays(-1);
                                    DRArrearDummy["Formula"] = (last_Date - fromdate).TotalDays + 1;
                                }

                                DaysInMonth = System.DateTime.DaysInMonth(fromdate.Year, (fromdate.Month + period));

                                #region CALCULATION OF HEADS WHICH ARE FORMULA DEPENDENT

                                for (IntSH = 0; IntSH < searchResult.Tables[6].DefaultView.Count; IntSH++)
                                {
                                    //searchResult.Tables[6].DefaultView.RowFilter = "PERIOD ='" + prd + "' AND FIELDNAME = '" + searchResult.Tables[5].DefaultView[IntSH]["FieldName"].ToString() + "'";
                                    ColName = searchResult.Tables[6].DefaultView[IntSH]["FieldName"].ToString();
                                    RND = Convert.ToBoolean(searchResult.Tables[6].DefaultView[IntSH]["RoundToHigher"].ToString());
                                    RNDTO = Convert.ToInt32(searchResult.Tables[6].DefaultView[IntSH]["RoundingUpto"].ToString());
                                    COND = Convert.ToBoolean(searchResult.Tables[6].DefaultView[IntSH]["Conditional"].ToString());
                                    MI = Convert.ToBoolean(searchResult.Tables[6].DefaultView[IntSH]["MONTHLYINPUT"].ToString());
                                    FC = Convert.ToBoolean(searchResult.Tables[6].DefaultView[IntSH]["FORMULACOLUMN"].ToString());
                                    AD = Convert.ToBoolean(searchResult.Tables[6].DefaultView[IntSH]["ATTENDANCEDEP"].ToString());

                                    #region for Manual LWP for Pay Arrear

                                    var mandata = genericRepo.Get<DTOModel.tblarrearmanualdata>(em => em.EmployeeId == empID && em.SalYear == fromdate.Year && em.SalMonth == (fromdate.Month + period) && em.BranchID == BranchID && em.ArrearType == 'B');
                                    var LWP1 = mandata.Where(p => p.LWP == p.LWP).FirstOrDefault();
                                    var E_Sp = mandata.Where(p => p.E_SP == p.E_SP).FirstOrDefault();

                                    #endregion

                                    #region  calculate PAy Arrear

                                    if (COND == true && FC == true && MI == false)
                                    {
                                        if (searchResult.Tables[6].DefaultView.Count > 0)
                                        {
                                            CalculatedValue = CalculateFromulaValueArrear(searchResult.Tables[6].DefaultView[IntSH]["LookUpHeadName"].ToString(), null, DRArrearDummy);

                                        }
                                        else
                                        { CalculatedValue = 0; }

                                        ColValue = Math.Round(ConditionalHeadArrear(empID, ColName, CalculatedValue, fromdate.Year, (fromdate.Month + period), DRES, vpfRate), RNDTO);                                        

                                    }
                                    else if (COND == false && FC == true && MI == false)
                                    {
                                        if (searchResult.Tables[6].DefaultView.Count > 0)
                                        {
                                            ColValue = CalculateFromulaValueArrear(searchResult.Tables[6].DefaultView[IntSH]["LookUpHeadName"].ToString(), null, DRArrearDummy);

                                        }
                                        else
                                        { ColValue = 0; }
                                    }
                                    else if (COND == false && FC == false && MI == true)
                                    {
                                        if (ColName.ToUpper() == "D_01")
                                        {
                                            ColValue = Convert.ToInt32(dtPayArrear.Rows[count][2].ToString() == "" ? "0" : dtPayArrear.Rows[count][2].ToString());
                                        }
                                        else
                                        {
                                            ColValue = Convert.ToDouble(DRSal.Row[ColName + "_A"].ToString());
                                        }

                                    }
                                    else if (COND == false && FC == false && MI == false)
                                    {
                                        #region Basic
                                        if (ColName.ToUpper() == "E_BASIC")
                                        {
                                            if (AllEmployees == true)
                                            { }
                                            else
                                            {
                                                ColValue = Convert.ToInt32(dtPayArrear.Rows[count][1].ToString() == "" ? "0" : dtPayArrear.Rows[count][1].ToString());

                                                if (fromdate.Day != 1 && period == 0)
                                                {
                                                    DateTime last_Date = new DateTime(fromdate.Year, fromdate.Month, 1)
                                                        .AddMonths(1).AddDays(-1);
                                                    var days = (last_Date - fromdate).TotalDays + 1;
                                                    Double lastDay = DateTime.DaysInMonth(fromdate.Year, fromdate.Month);
                                                    ColValue = Math.Round((ColValue / lastDay) * days, RNDTO);
                                                }

                                                if (AD == true)
                                                {
                                                    if (LWP1 != null)
                                                    {
                                                        if (LWP1.LWP > 0)
                                                        {
                                                            ColValue = ColValue - ((ColValue / DaysInMonth) * Convert.ToDouble(LWP1.LWP));
                                                            if (ColName != "E_Basic")
                                                            {
                                                                ColValue = (RND == true ? Math.Round(ColValue, RNDTO) : ColValue);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ColValue = (RND == true ? Math.Round(ColValue, RNDTO) : ColValue);

                                                        }
                                                    }
                                                    else
                                                    {
                                                        ColValue = (RND == true ? Math.Round(ColValue, RNDTO) : ColValue);
                                                    }
                                                }

                                            }

                                        }
                                        else
                                        {
                                            ColValue = Convert.ToDouble(DRSal.Row[ColName + "_A"].ToString());
                                        }
                                        #endregion
                                    }
                                    else if (COND = true && FC == false && MI == true)
                                    {
                                        ColValue = 0;
                                    }

                                    #region FDA

                                    if (ColName.ToUpper() == "E_FDA")
                                    {
                                        int Basic = Convert.ToInt32(dtPayArrear.Rows[count][1].ToString());
                                        var result = genericRepo.Get<DTOModel.tblmstFDA>(em => Basic > 0 ? em.upperlimit > Basic : 1 > 0);
                                        var record = result.Where(p => p.val == result.Min(r => r.val)).FirstOrDefault();

                                        if (record.val > 0)
                                        {
                                            ColValue = Convert.ToInt32(Convert.ToInt32(dtPayArrear.Rows[count][1].ToString()) * Convert.ToDouble(record.val) / 100);

                                        }
                                    }
                                    #endregion                                  


                                    #region Special Pay

                                    if (ColName.ToUpper() == "E_SP")
                                    {
                                        if (E_Sp != null)
                                        {
                                            if (E_Sp.E_SP > 0)
                                            {
                                                ColValue = (RND == true ? Math.Round(Convert.ToDouble(E_Sp.E_SP), RNDTO) : Convert.ToDouble(E_Sp.E_SP));

                                            }
                                        }

                                    }
                                    #endregion

                                    #region CCA

                                    if (ColName.ToUpper() == "E_05")
                                    {
                                        if (COND = true && FC == false && MI == false)
                                        {

                                            int brcode = Convert.ToInt32(DRArrearDummy["BranchCode"]);
                                            string BranchName = genericRepo.GetByID<Nafed.MicroPay.Data.Models.Branch>(brcode).BranchName;
                                            var dtoBranch = genericRepo.Get<DTOModel.Branch>(x => x.BranchID == brcode).ToList();
                                            var dtcity = genericRepo.Get<DTOModel.tblMstCity>(x => x.CityName == BranchName).ToList();

                                            var fieldLists = (from left in dtcity
                                                              join right in dtoBranch on left.CityName equals right.BranchName into joinedList
                                                              from sub in joinedList.DefaultIfEmpty()
                                                              select new ArrearManualData
                                                              {
                                                                  Grade = left.Grade
                                                              }).ToList();
                                            DataTable dt = Common.ExtensionMethods.ToDataTable(fieldLists);
                                            if (dt.Rows.Count > 0)
                                            {
                                                string GRADE;
                                                GRADE = "AmtCityGrade" + dt.Rows[0]["Grade"];
                                                int Basic1 = Convert.ToInt32(dtPayArrear.Rows[count][1].ToString());
                                                var result = genericRepo.Get<DTOModel.tblCCAMaster>(em => Basic1 > 0 ? em.UpperLimit >= Basic1 : 1 > 0).OrderBy(em => em.UpperLimit).ToList();
                                                DataTable dtCCA = Common.ExtensionMethods.ToDataTable(result);
                                                if (dtCCA.Rows.Count > 0)
                                                {
                                                    ColValue = Convert.ToDouble(dtCCA.Rows[0][GRADE].ToString());
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    if (AD = true && ColName.ToUpper() != "E_01" && ColName.ToUpper() != "E_02" && ColName.ToUpper() != "D_PF" && ColName.ToUpper() != "D_VPF")
                                    {
                                        if (LWP1 != null)
                                        {
                                            if (LWP1.LWP > 0)
                                            {
                                                ColValue = ColValue - ((ColValue / DaysInMonth) * Convert.ToDouble(LWP1.LWP));
                                                if (ColName.ToUpper() != "D_PF" && ColName.ToUpper() != "D_VPF")
                                                {

                                                }
                                                else
                                                {
                                                    ColValue = (RND == true ? Math.Round(ColValue, RNDTO) : ColValue);

                                                }
                                            }
                                            else
                                            {

                                                ColValue = (RND == true ? Math.Round(ColValue, RNDTO) : ColValue);
                                            }
                                        }
                                        else
                                        {

                                            ColValue = (RND == true ? Math.Round(ColValue, RNDTO) : ColValue);
                                        }
                                    }


                                    #endregion

                                    ColValue = (RND == true ? Math.Round(ColValue, RNDTO) : ColValue);
                                    if (fromdate.Day != 1 && period == 0)
                                    {
                                        ColValue = Math.Round(ColValue, 0);
                                    }

                                    if (ColName == "E_02")
                                    {
                                        var dataRow = searchResult.Tables[0].Select("EmployeeID = " + empID + " AND SalYear = '" + fromdate.Year + "' AND  SalMonth = '" + (fromdate.Month + period) + "'");


                                        foreach (DataRow o in searchResult.Tables[0].Select("EmployeeID = " + empID + " AND SalYear = '" + fromdate.Year + "' AND  SalMonth = '" + (fromdate.Month + period) + "'"))

                                        {                                            
                                            if (Convert.ToString(o["E_02"]) == "" || Convert.ToString(o["E_02"]) == "0.0000")
                                                ColValue = 0;
                                        }
                                    }

                                    DRArrearDummy[ColName] = ColValue;
                                }

                                searchResult.Tables[3].Rows.Add(DRArrearDummy.ItemArray);

                                #endregion

                            }

                        }
                        #endregion


                        #region Deleting Duplicate data from tblarreardetail and tblfinalmonthlysalary

                        DataTable dtdelarr = new DataTable();
                        DataTable dtdelsal = new DataTable();

                        dtdelarr = arrearRepo.DeleteDuplicateData(empID, fromYear, toYear, BranchID, 1, "B");
                        dtdelsal = arrearRepo.DeleteDuplicateData(empID, fromYear, toYear, BranchID, 2, "B");
                        #endregion
                    }
                }

                int res = arrearRepo.UpdateTblArreardummy(searchResult.Tables[3]);



                #region Transfer Data

                if (fromdate.Day != 1)
                {
                    DateTime last_Date = new DateTime(fromdate.Year, fromdate.Month, 1)
                        .AddMonths(1).AddDays(-1);
                    var days = (last_Date - fromdate).TotalDays + 1;
                    int transferdata = TransferData(fromYear, toYear, searchResult, Convert.ToString(days), orderNumber, orderDate);
                }
                else
                {
                    int transferdata = TransferData(fromYear, toYear, searchResult, "", orderNumber,  orderDate);
                }

                #endregion

                #region CREATE SUM FROM TBLARREARDETAIL And TRANSFER INTO TBLFINALMONTHLYSALARY

                for (count = 0; count < dtPayArrear.Rows.Count; count++)
                {
                    if (Convert.ToInt32(dtPayArrear.Rows[count]["BasicSalary"].ToString()) > 0 || Convert.ToInt32(dtPayArrear.Rows[count]["IncomeTaxdeduction"].ToString()) > 0)
                    {
                        empID = Convert.ToInt32(dtPayArrear.Rows[count][0]);
                        string DOG = DateTime.Now.Year + (DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + (DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                        res = arrearRepo.updatePayArrearDifference(empID, fromYear, toYear, BranchID, DOG);
                    }
                }

                #endregion

                #endregion
                return calc;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }



        #endregion

        #region Export/Import Pay Arrear Details

        public string GetPayArrearentryform(int branchID, string fromYear, string toYear, string fileName, string sFullPath, string empID)
        {
            log.Info($"ArrearService/GetPayArrearentryform/{branchID},{fromYear},{toYear},{empID}");
            string result = string.Empty;
            if (Directory.Exists(sFullPath))
            {
                sFullPath = $"{sFullPath}{fileName}";
                var payarreardata = arrearRepo.getemployeeByBranchID(branchID, fromYear, toYear, empID);

                DataTable dt = new DataTable();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.getemployeeByBranchID_Result, Model.ArrearManualData>()
                     .ForMember(d => d.Empcode, o => o.MapFrom(s => s.EMPLOYEECODE))
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.Empname, o => o.MapFrom(s => s.Name))
                     .ForMember(d => d.BasicSalary, o => o.UseValue(0))
                    .ForMember(d => d.IncomeTaxDeduction, o => o.UseValue(0))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var empList = Mapper.Map<List<DTOModel.getemployeeByBranchID_Result>, List<Model.ArrearManualData>>(payarreardata); ;
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
                        x.Empcode,
                        x.Empname,
                        x.BranchName,
                        x.DesignationName,
                        x.BasicSalary,
                        x.IncomeTaxDeduction
                    }).ToList();

                    dt = Common.ExtensionMethods.ToDataTable(formData);

                }

                if (dt != null && dt.Rows.Count > 0)  //====== export data form if there is data ========= 
                {
                    dt.Columns[0].ColumnName = "#";

                    IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                    exportHdr = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                    result = ExportToExcelManualData(exportHdr, dt, "PayArrearForm", sFullPath);
                }
                else
                    result = "norec";
            }
            return result;
        }


        public List<Model.ArrearManualData> ReadArrearImportExcelData(string fileFullPath, bool isHeader, out string message, out int error, out int warning)
        {
            log.Info($"ArrearService/ReadArrearImportExcelData/{0}");
            message = string.Empty;
            error = 0; warning = 0;
            bool isValidHeaderList;
            List<string> missingHeaders;

            List<Model.ArrearManualData> objModel = new List<ArrearManualData>();

            var headerList = new List<string> { "EmpCode", "BasicSalary", "IncomeTaxDeduction" };
            DataTable dtExcel = ReadAndValidateHeader(fileFullPath, headerList, isHeader, out isValidHeaderList, out missingHeaders);

            if (isValidHeaderList)
            {
                DeleteTempFile(fileFullPath);


                DataTable dataWithHeader = excelService.GetDataWithHeader(dtExcel);
                if (dataWithHeader.Rows.Count > 0)
                {
                    DataTable dtEmpCodes = new DataTable();
                    DataTable dtEmpCodeFromDB = new DataTable();
                    ManualDataImportColValues colWithDistinctValues = GetDistinctValuesOfImportData(dataWithHeader);

                    if (colWithDistinctValues.empCode != null && colWithDistinctValues.empCode.Count() > 0)
                    {
                        var empCodes = (from x in colWithDistinctValues.empCode select new { ItemCode = x }).ToList();
                        dtEmpCodes = Common.ExtensionMethods.ToDataTable(empCodes);
                    }
                    dtEmpCodeFromDB = arrearRepo.GetImportFieldValues(dtEmpCodes);
                    MergeArrearSheetDataWithDBValues(dataWithHeader, dtEmpCodeFromDB);
                    ValidateArrearImportData(dataWithHeader, out error, out warning);
                    ///====  Merge Imported Data with Database data value

                    ///
                    ///====  Validate Imported data with database values 


                    objModel = (from DataRow row in dataWithHeader.Rows
                                select new ArrearManualData
                                {
                                    Empcode = row["EmpCode"].ToString(),
                                    Empname = row["Empname"].ToString(),
                                    BranchName = row["BranchName"].ToString(),
                                    DesignationName = row["DesignationName"].ToString(),
                                    BasicSalary = Convert.ToDecimal(row["BasicSalary"].ToString() == "" ? 0 : row["BasicSalary"]),
                                    IncomeTaxDeduction = Convert.ToDecimal(row["IncomeTaxDeduction"].ToString() == "" ? 0 : row["IncomeTaxDeduction"]),
                                    EmployeeId = int.Parse(row["employeeID"].ToString()),
                                    error = row["error"].ToString(),
                                    warning = row["warning"].ToString()
                                }).ToList();    //.OrderBy(x => x.storeNumber).ThenBy(x => x.itemCode)

                    message = "success";
                    return objModel;
                }
            }
            return objModel;
        }

        private DataTable ReadAndValidateHeader(string sFilePath, List<string> headerList, bool IsHeader, out bool isValid, out List<string> sMissingHeader)
        {

            log.Info($"ArrearService/ReadAndValidateHeader/{0}");

            isValid = false; sMissingHeader = new List<string>();
            DataTable dtExcel = new DataTable();
            List<string> inValidColumn = new List<string>();
            try
            {
                AttendanceImport ob = new AttendanceImport();
                using (dtExcel = ob.ReadExcelWithNoHeader(sFilePath))
                {
                    RemoveBlankRowAndColumns(ref dtExcel, true);
                    int iExcelRowCount = dtExcel.Rows.Count;
                    if (iExcelRowCount > 0)
                    {
                        List<string> strList = dtExcel.Rows[0].ItemArray.Select(o => o == null ? String.Empty : o.ToString()).ToList();
                        isValid = ValidateHeader(strList, headerList, out sMissingHeader, out inValidColumn);
                        return dtExcel;
                    }
                    else
                        return dtExcel;

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private ManualDataImportColValues GetDistinctValuesOfImportData(DataTable dtImportData)
        {
            log.Info($"ArrearService/AttendanceImportColValues");
            try
            {
                ManualDataImportColValues colWithDistinctValues = new ManualDataImportColValues();
                colWithDistinctValues.empCode = Common.ExtensionMethods.GetDistinctCellValues<string>(dtImportData, "EmpCode").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                return colWithDistinctValues;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        void MergeArrearSheetDataWithDBValues(DataTable excelData, DataTable dtEmpCodes)
        {
            try
            {
                excelData.Columns.Add("employeeID", typeof(int));
                excelData.Columns.Add("error", typeof(string));
                excelData.Columns.Add("warning", typeof(string));
                excelData.Columns.Add("rowCount", typeof(int));

                foreach (DataRow row in excelData.Rows)
                {
                    string sEmpCode = row["EmpCode"].ToString();
                    DataRow[] arrIC = dtEmpCodes.AsEnumerable().Where(x => x.Field<string>("EmpCode").Equals(sEmpCode, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (arrIC.Length > 0)
                    {
                        row["employeeID"] = arrIC[0]["EmployeeID"].ToString().Equals("") ? -1 : int.Parse(arrIC[0]["EmployeeID"].ToString());

                    }
                    else
                        row["employeeID"] = -1;

                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        void ValidateArrearImportData(DataTable dtSource, out int iError, out int iWarning)
        {
            iError = 0; iWarning = 0;

            log.Info($"ArrearService/ValidateArrearImportData");
            try
            {
                iError = 0; iWarning = 0;
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    DataRow cDrow = dtSource.Rows[i];
                    string sError = string.Empty;
                    string sWarning = string.Empty;
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        string sColName = dtSource.Columns[j].ColumnName.ToString().Trim().ToLower();
                        if (
                            sColName.ToLower().Equals("employeeID")
                            || sColName.ToLower().Equals("error")
                            || sColName.ToLower().Equals("rowcount") || sColName.ToLower().Equals("warning"))
                        {
                            continue;
                        }
                        string sColVal = dtSource.Rows[i][j].ToString().Trim();

                        if (sColName.Equals("empcode"))
                        {
                            #region Validate employee code
                            if (IsBlank(sColVal))
                            {
                                sError = sError + "B|";
                                iError++;
                            }
                            else
                            {
                                int employeeID = int.Parse(dtSource.Rows[i]["employeeID"].ToString());
                                if (!(employeeID > 0))
                                {
                                    sError = sError + "NF|";
                                    iError++;
                                }
                                else
                                    sError = sError + "|";
                            }
                            sWarning = sWarning + "|";
                            #endregion
                        }
                        else
                        {
                            sError = sError + "|";
                            sWarning = sWarning + "|";
                        }
                    }
                    dtSource.Rows[i]["error"] = sError;
                    dtSource.Rows[i]["warning"] = sWarning;
                    dtSource.Rows[i]["rowCount"] = i + 1;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public DataTable ImportPayArrearDataDetails(List<ArrearManualData> dataDetails)
        {
            log.Info($"ImportAttendanceService/ImportPayArrearDataDetails");
            try
            {

                var dtoList = dataDetails.Select(x => new { EmployeeId = x.EmployeeId, BasicSalary = x.BasicSalary, IncomeTaxDeduction = x.IncomeTaxDeduction }).ToList();
                DataTable DT = new DataTable();
                DT = Common.ExtensionMethods.ToDataTable(dtoList);
                return DT;
            }
            catch
            {
                // log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        #endregion


        #region Pay Arrear Reports
        public string getSelectedEmployeecode(int listcount, int[] empIDs)
        {
            int count = 0, empID = 0;
            string Empcode = string.Empty;
            try
            {
                for (count = 0; count < listcount; count++)
                {

                    empID = empIDs[count];
                    Empcode = Empcode + "," + empID;
                }
                Empcode = Empcode.Substring(1, Empcode.Length - 1);
                return Empcode;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }
        public int getPayArrearEmployee(string branchCode, int fromYear, int toYear, string DOG)
        {
            try
            {
                int flag = 0;
                var listBranch = (dynamic)null;
                if (branchCode != "")
                {
                    var result = genericRepo.Get<DTOModel.TblArrearDetail>(em => em.ArrearType == "B" && em.PeriodFrom >= fromYear && em.PeriodTo <= toYear && em.DateOfGenerateArrear == DOG && em.BranchCode == branchCode);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.TblArrearDetail, Model.ArrearManualData>()
                        .ForMember(c => c.Empcode, c => c.MapFrom(s => s.EmployeeCode))
                       .ForAllOtherMembers(c => c.Ignore());
                    });
                    listBranch = Mapper.Map<List<Model.ArrearManualData>>(result);
                }
                else
                {
                    var result = genericRepo.Get<DTOModel.TblArrearDetail>(em => em.ArrearType == "B" && em.PeriodFrom >= fromYear && em.PeriodTo <= toYear && em.DateOfGenerateArrear == DOG && em.BranchCode != "99");

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.TblArrearDetail, Model.ArrearManualData>()
                        .ForMember(c => c.Empcode, c => c.MapFrom(s => s.EmployeeCode))
                       .ForAllOtherMembers(c => c.Ignore());
                    });
                    listBranch = Mapper.Map<List<Model.ArrearManualData>>(result);
                }
                DataTable dtgetID = Nafed.MicroPay.Common.ExtensionMethods.ToDataTable(listBranch);
                if (dtgetID.Rows.Count > 0)
                {
                    for (int count = 0; count < dtgetID.Rows.Count; count++)
                    {
                        var result = arrearRepo.CreatePayArrearDetail(fromYear.ToString(), toYear.ToString(), "B", dtgetID.Rows[count]["Empcode"].ToString(), branchCode, DOG);
                    }
                }

                return flag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ArrerPeriodDetailsForPAYDA> GetArrearPeriodsDetailsforPay(string arrerType, int? branchId = null)
        {
            log.Info($"AdjustOldLoanService/GetArrearPeriodsDetails");
            try
            {
                var result = arrearRepo.GetArrearPeriodsDetailsforPay(arrerType, branchId);
                var arrerperiodDetails = Common.ExtensionMethods.ConvertToList<ArrerPeriodDetailsForPAYDA>(result);
                return arrerperiodDetails;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        public bool getemployeebranchDetails(int branchID, out ArrearFilters BRdetail)
        {
            bool flag = false;
            try
            {
                BRdetail = new ArrearFilters();

                BRdetail.BranchName = genericRepo.GetByID<Nafed.MicroPay.Data.Models.Branch>(branchID).BranchName;
                BRdetail.BranchCode = genericRepo.GetByID<Nafed.MicroPay.Data.Models.Branch>(branchID).BranchCode;

                flag = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }
    }
}
