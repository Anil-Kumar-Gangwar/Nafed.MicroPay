using System;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using static Nafed.MicroPay.Common.DataValidation;
using Nafed.MicroPay.ImportExport;

namespace Nafed.MicroPay.Services
{
    public class ControlSettingService : BaseService, IControlSettingService
    {
        private readonly IExcelService excelService;
        private readonly IGenericRepository genricRepo;
        public ControlSettingService(IGenericRepository genricRepo, IExcelService excelService)
        {
            this.excelService = excelService;
            this.genricRepo = genricRepo;
        }


        public bool IsMonthlyTCSFileExists(string tcsFilePeriod)
        {
            log.Info($"ControlSetting/IsMonthlyTCSFileExists/{tcsFilePeriod}");
            try
            {
                return genricRepo.Exists<DTOModel.MonthlyTCSFile>(x => x.TCSFilePeriod.ToLower() == tcsFilePeriod.ToLower());
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<MonthlyTCSFile> GetMonthlyTCSFiles()
        {
            log.Info($"ControlSetting/GetMonthlyTCSFiles");

            List<MonthlyTCSFile> tcsFiles = new List<MonthlyTCSFile>();

            try
            {
                var dtoTCSFiles = genricRepo.Get<DTOModel.MonthlyTCSFile>();
                Mapper.Initialize(cfg =>
                {

                    cfg.CreateMap<DTOModel.MonthlyTCSFile, MonthlyTCSFile>();
                });
                tcsFiles = Mapper.Map<List<MonthlyTCSFile>>(dtoTCSFiles);

                return tcsFiles;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public int InsertMonthlyTCSFile(MonthlyTCSFile mTCSFile)
        {
            log.Info($"ControlSetting/InsertMonthlyTCSFile");

            int tcsFileID = 0;
            var dtoMTCSFile = new DTOModel.MonthlyTCSFile();
            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<MonthlyTCSFile, DTOModel.MonthlyTCSFile>();
            });
            dtoMTCSFile = Mapper.Map<DTOModel.MonthlyTCSFile>(mTCSFile);
            genricRepo.Insert<DTOModel.MonthlyTCSFile>(dtoMTCSFile);
            tcsFileID = dtoMTCSFile.TCSFileID;

            return tcsFileID;
        }

        public bool UpdateMonthlyTCSFile(MonthlyTCSFile mTCSFile)
        {
            log.Info($"ControlSetting/UpdateMonthlyTCSFile");
            bool flag = false;
            try
            {
                var mTCSdtoFile = genricRepo.Get<DTOModel.MonthlyTCSFile>(x => x.TCSFilePeriod == mTCSFile.TCSFilePeriod).FirstOrDefault();
                mTCSdtoFile.TCSFileName = mTCSFile.TCSFileName;
                mTCSdtoFile.UpdatedBy = mTCSFile.UpdatedBy;
                mTCSdtoFile.UpdatedOn = mTCSdtoFile.UpdatedOn;
                genricRepo.Update<DTOModel.MonthlyTCSFile>(mTCSdtoFile);
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool ReadTCSFileData(string fileFullPath, bool isHeader, out List<string> missingHeaders, out List<string> inValidColumns,  out string message, out int error, out int warning)
        {
            log.Info($"ControlSettingService/ReadTCSFileData/fileFullPath:{fileFullPath}");
            try
            {
                message = string.Empty;
                error = 0; warning = 0;
                bool isValidHeaderList;
                missingHeaders = new List<string>(); inValidColumns = new List<string>();
                var headerList =
                   //new List<string> { "loctcd", "mmyy", "emplcd" , "emplname", "comp_dep",
                   //"em_loan", "em_ln_int", "gl_loan", "gl_ln_int", "recr_dep","misc_dedn1","misc_dedn2","tcsln" }
                   TCSFileColumns.GetTCSFileColumns();

                DataTable dtExcel = ReadAndValidateHeader(fileFullPath, headerList, isHeader, out isValidHeaderList, out missingHeaders, out inValidColumns);
                DeleteTempFile(fileFullPath);
                return isValidHeaderList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFilePath">File Path </param>
        /// <param name="isHeader"></param>
        /// <param name="headerList"></param>
        /// <param name="isValid"></param>
        /// <param name="sMissingHeader"></param>
        /// <returns></returns>
        /// 
        private DataTable ReadAndValidateHeader(string sFilePath, List<string> headerList, bool IsHeader, out bool isValid, 
            out List<string> sMissingHeader, out List<string> inValidColumns)
        {

            log.Info($"ControlSettingService/ReadAndValidateHeader/{0}");

            isValid = false; sMissingHeader = new List<string>();
            inValidColumns = new List<string>();

            DataTable dtExcel = new DataTable();

            try
            {
                TCSFileImport ob = new TCSFileImport();
                using (dtExcel = ob.ReadExcelWithNoHeader(sFilePath))
                {
                    RemoveBlankRowAndColumns(ref dtExcel, true);
                    int iExcelRowCount = dtExcel.Rows.Count;
                    if (iExcelRowCount > 0)
                    {
                        List<string> strList = dtExcel.Rows[0].ItemArray.Select(o => o == null ? String.Empty : o.ToString()).ToList();
                        isValid = ValidateHeader(strList, headerList, out sMissingHeader,out inValidColumns,true);
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

    }
}
