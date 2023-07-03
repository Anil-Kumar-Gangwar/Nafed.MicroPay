using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using AutoMapper;
using System.Data;
using Nafed.MicroPay.ImportExport.Interfaces;
using System.IO;
using static Nafed.MicroPay.ImportExport.ExGratiaManualEntryForm;

namespace Nafed.MicroPay.Services
{
    public class ExgratiaService : BaseService, IExgratiaService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IExgratiaRepository exgrRepo;
        private readonly IExport export;
        public ExgratiaService(IGenericRepository genericRepo, IExgratiaRepository exgrRepo, IExport export)
        {
            this.genericRepo = genericRepo;
            this.exgrRepo = exgrRepo;
            this.export = export;
        }
        public List<ExgratiaList> checkexistExgratiaList(string financialyear)
        {
            try
            {
                var dto = genericRepo.Get<DTOModel.TblExGratia_Cal>(x => x.FinancialYear == financialyear);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.TblExGratia_Cal, Model.ExgratiaList>()
                   .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmpCode))
                     .ForMember(c => c.Amt_Tot, c => c.MapFrom(s => s.Amt_Tot))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var listExgratia = Mapper.Map<List<Model.ExgratiaList>>(dto);
                return listExgratia.ToList();
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int GenerateList(string fYear, int year)
        {

            Model.ExgratiaList exgratiaList = new Model.ExgratiaList();
            int GenerateList = 0;
            try
            {
                var dtoGRATIADUMMY = genericRepo.Get<DTOModel.TBLGRATIADUMMY>().ToList().Where(x => x.HEAD == "BONUS SALARY");
                var dtbonusAmt = genericRepo.Get<DTOModel.TblBonusAmt>().ToList().Where(x => x.Year == year);
                var fieldLists = (from left in dtbonusAmt
                                  join right in dtoGRATIADUMMY on left.Empcode equals right.EMPCODE into joinedList
                                  from sub in joinedList.DefaultIfEmpty()
                                  select new ExgratiaList()
                                  {
                                      EmpCode = left.Empcode,
                                      EmployeeID = left.EmployeeID,
                                      TOT1 = sub == null ? 0 : (decimal)sub.TOT1
                                  }).ToList();
                DataTable dt = Common.ExtensionMethods.ToDataTable(fieldLists);
                if (dt.Rows.Count > 0)
                {
                    for (int count = 0; count < dt.Rows.Count; count++)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.ExgratiaList, DTOModel.TblExGratia_Cal>()
                            .ForMember(c => c.EmpCode, c => c.MapFrom(m => dt.Rows[count]["EmpCode"].ToString()))
                            .ForMember(c => c.EmployeeID, c => c.MapFrom(m => dt.Rows[count]["EmployeeID"].ToString()))
                            .ForMember(c => c.Amt_Tot, c => c.MapFrom(m => Convert.ToDecimal(dt.Rows[count]["TOT1"].ToString() == "0" ? 0 : dt.Rows[count]["TOT1"])))
                            .ForMember(c => c.FinancialYear, c => c.MapFrom(m => fYear))
                            .ForMember(c => c.salyear, c => c.MapFrom(m => year))
                            .ForMember(c => c.salmonth, c => c.MapFrom(m => DateTime.Now.Month))
                            .ForMember(c => c.ExGratiaOneday, c => c.MapFrom(m => Convert.ToDecimal(Convert.ToInt32(dt.Rows[count]["TOT1"].ToString() == "0" ? 0 : dt.Rows[count]["TOT1"]) / 360)))
                            .ForMember(c => c.CreatedBy, c => c.MapFrom(m => 1))
                            .ForMember(c => c.CreatedOn, c => c.MapFrom(m => DateTime.Now))
                            .ForAllOtherMembers(c => c.Ignore());
                        });
                        var dtoexgratiaList = Mapper.Map<DTOModel.TblExGratia_Cal>(exgratiaList);
                        genericRepo.Insert<DTOModel.TblExGratia_Cal>(dtoexgratiaList);
                    }
                    GenerateList = 1;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return GenerateList;
        }

        public List<Model.ExgratiaList> GetExgratiaList(string fyear, int? BranchID, int empTypeID)
        {
            log.Info($"ExgratiaService/GetExgratiaList");
            try
            {
                var result = (dynamic)null;
                if (fyear != null)
                {
                    result = genericRepo.Get<DTOModel.TblExGratia_Cal>(x => !x.IsDeleted && x.FinancialYear == fyear && (BranchID > 0 ? x.BranchID == BranchID : 1 > 0) && (empTypeID > 0 ? x.EmployeeTypeID == empTypeID : 1 > 0));
                }
                else
                {
                    result = genericRepo.Get<DTOModel.TblExGratia_Cal>(x => !x.IsDeleted);
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.TblExGratia_Cal, Model.ExgratiaList>()

                      .ForMember(c => c.ExgratiaID, c => c.MapFrom(s => s.ExgratiaID))
                    .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmpCode))
                    .ForMember(c => c.Name, c => c.MapFrom(s => s.tblMstEmployee.Name))
                      .ForMember(c => c.branchName, c => c.MapFrom(s => s.Branch.BranchName))
                    .ForMember(c => c.Amt_Tot, c => c.MapFrom(s => s.Amt_Tot))
                    .ForMember(c => c.net_amount, c => c.MapFrom(s => s.net_amount))
                     .ForMember(c => c.Incometax, c => c.MapFrom(s => s.Incometax))
                       .ForMember(c => c.Other, c => c.MapFrom(s => s.other))
                    .ForMember(c => c.FinancialYear, c => c.MapFrom(s => s.FinancialYear))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listExgratia = Mapper.Map<List<Model.ExgratiaList>>(result);
                return listExgratia;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //public int calculateExgratia(string fyear,string empcode,int noofdays)
        //{
        //    int calExgratia = 0;
        //    log.Info($"ExgratiaService/calculateExgratia");
        //    try
        //    {
        //        var result = genericRepo.Get<DTOModel.TblExGratia_Cal>(x => !x.IsDeleted && x.FinancialYear == fyear);
        //        Mapper.Initialize(cfg =>
        //        {
        //            cfg.CreateMap<DTOModel.TblExGratia_Cal, Model.ExgratiaList>()
        //            .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
        //            .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmpCode))
        //            .ForMember(c => c.ExGratiaOneday, c => c.MapFrom(s => s.ExGratiaOneday))
        //            .ForMember(c => c.Incometax, c => c.MapFrom(s => s.Incometax))
        //            .ForMember(c => c.Other, c => c.MapFrom(s => s.other))
        //           .ForAllOtherMembers(c => c.Ignore());
        //        });
        //        var listExgratia = Mapper.Map<List<Model.ExgratiaList>>(result);
        //        DataTable dt = Common.ExtensionMethods.ToDataTable(listExgratia);
        //         if (dt.Rows.Count > 0)
        //         {
        //                for (int count = 0; count < dt.Rows.Count; count++)
        //                {
        //                empcode = dt.Rows[count]["EmpCode"].ToString();
        //                    var prevTransRow = genericRepo.Get<DTOModel.TblExGratia_Cal>(x =>  x.EmpCode == empcode && x.FinancialYear== fyear).FirstOrDefault();

        //                prevTransRow.ExGratiaAmt =Convert.ToDecimal( dt.Rows[count]["ExGratiaOneday"].ToString()=="0"?"0" : dt.Rows[count]["ExGratiaOneday"].ToString())* noofdays;
        //                    prevTransRow.net_amount = (Convert.ToDecimal(dt.Rows[count]["ExGratiaOneday"].ToString() == "0" ? "0" : dt.Rows[count]["ExGratiaOneday"].ToString()) * noofdays)- Convert.ToDecimal(dt.Rows[count]["Incometax"].ToString() == "0" ? "0" : dt.Rows[count]["Incometax"].ToString())- Convert.ToDecimal(dt.Rows[count]["Other"].ToString() == "0" ? "0" : dt.Rows[count]["Other"].ToString());
        //                    prevTransRow.DaysCount = noofdays;
        //                    prevTransRow.UpdatedBy = 1;
        //                    prevTransRow.UpdatedOn = DateTime.Now;

        //                genericRepo.Update<DTOModel.TblExGratia_Cal>(prevTransRow);
        //            }
        //               calExgratia = 1;
        //        }
        //   }
        //   catch (Exception ex)
        //   {
        //            log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //            throw ex;
        //   }
        //   return calExgratia;

        //}

        public Model.ExgratiaList GetExgratiaByID(int? ExgratiaID)
        {
            log.Info($"ExgratiaService/GetExgratiaByID/ {ExgratiaID}");
            try
            {
                var exgratiaObj = genericRepo.GetByID<DTOModel.TblExGratia_Cal>(ExgratiaID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.TblExGratia_Cal, Model.ExgratiaList>()
                     .ForMember(c => c.EmployeeID, c => c.MapFrom(s => s.EmployeeID))
                    .ForMember(c => c.EmpCode, c => c.MapFrom(s => s.EmpCode))
                    .ForMember(c => c.Name, c => c.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(c => c.Amt_Tot, c => c.MapFrom(s => s.Amt_Tot))
                    .ForMember(c => c.FinancialYear, c => c.MapFrom(s => s.FinancialYear))
                     .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.TblExGratia_Cal, Model.ExgratiaList>(exgratiaObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool Delete(int ExgratiaID, int userID)
        {
            log.Info($"ExgratiaService/Delete/{ExgratiaID}");
            bool flag = false;
            try
            {
                DTOModel.TblExGratia_Cal dtoExgratia = new DTOModel.TblExGratia_Cal();
                dtoExgratia = genericRepo.GetByID<DTOModel.TblExGratia_Cal>(ExgratiaID);
                dtoExgratia.IsDeleted = true;
                dtoExgratia.UpdatedBy = userID;
                dtoExgratia.UpdatedOn = DateTime.Now;
                genericRepo.Update<DTOModel.TblExGratia_Cal>(dtoExgratia);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
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

        public int updateExgratiaTDS(Model.ExgratiaList exgratia)
        {
            int calExgratia = 0;
            log.Info($"ExgratiaService/calculateExgratia");
            try
            {
                var prevTransRow = genericRepo.Get<DTOModel.TblExGratia_Cal>(x => x.EmployeeID == exgratia.EmployeeID && x.FinancialYear == exgratia.FinancialYear).FirstOrDefault();

                prevTransRow.other = exgratia.Other;
                prevTransRow.Incometax = exgratia.Incometax;
                prevTransRow.UpdatedBy = 1;
                prevTransRow.UpdatedOn = DateTime.Now;
                genericRepo.Update<DTOModel.TblExGratia_Cal>(prevTransRow);
                calExgratia = 1;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return calExgratia;

        }


        public int calculateExgratia(int noofdays, string fromYear, string toYear, int branchID, string fYear, bool isPercentage, int emptype)
        {
            int flag = 0;
            log.Info($"ExgratiaService/calculateExgratiaBasicDA");
            try
            {
                flag = exgrRepo.CalculateExgratia(noofdays, fromYear, toYear, branchID, fYear, isPercentage, emptype);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;

        }

        public DataTable GetExgratiaExport(string fromYear, string toYear, string fyear, int branchID, int emptype)
        {
            DataTable dt = new DataTable();
            log.Info($"ExgratiaService/GetExgratiaExport");
            try
            {
                dt = exgrRepo.GetExgratiaExport(fromYear, toYear, fyear, branchID, emptype);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return dt;

        }

        public string GetExgratiaExportTemplate(string fromYear, string toYear, string fyear, int branchID, int emptype, string fileName, string filePath)
        {
            log.Info($"ExgratiaService/GetExgratiaExportTemplate");
            try
            {
                string result = string.Empty;
                if (Directory.Exists(filePath))
                {
                    filePath = $"{filePath}{fileName}";

                    DataTable dtData = new DataTable();
                    dtData = exgrRepo.GetExportExgratiaTemplate(fromYear, toYear, fyear, branchID, emptype);

                    if (dtData != null && dtData.Rows.Count > 0)
                    {
                        DataSet ds = new DataSet();
                        ds.Tables.Add(dtData);
                        bool res = export.ExportFormatedExcel(ds, filePath, "Ex-gratia - Template");
                    }
                    else
                        result = "norec";
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public string GetExGratiaEntryForm(int branchID, int salMonth, int salYear, string fileName, string filePath)
        {
            log.Info($"ImportExGratiaService/GetExGratiaEntryForm/branchID:{branchID}&salMonth:{salMonth} & salYear:{salYear} ");

            try
            {
                string result = string.Empty;
                if (Directory.Exists(filePath))
                {
                    filePath = $"{filePath}{fileName}";

                    DataTable dtData = new DataTable();

                    dtData = exgrRepo.GetExgratiaEntryFormData(branchID, salMonth, salYear);

                    if (dtData != null && dtData.Rows.Count > 0)  //====== export ex-Gratia form if there is data ========= 
                    {
                        //  dtData.Columns[0].ColumnName = "#";

                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).AsEnumerable<string>();

                        result = ExportToExcel(exportHdr, dtData, "Ex-Gratia Manual Entry Form", filePath);
                    }
                    else
                        result = "norec";
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public void PublishExGratia(string financialyear, int emptype, int branchID)
        {
            log.Info($"ExgratiaService/PublishExGratia");
            try
            {
                exgrRepo.PublishExGratia(financialyear, emptype, branchID);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
