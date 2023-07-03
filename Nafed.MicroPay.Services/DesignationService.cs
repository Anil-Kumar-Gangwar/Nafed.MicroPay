using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class DesignationService : BaseService, IDesignationService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IEmployeeRepository empRepo;
        public DesignationService(IGenericRepository genericRepo, IEmployeeRepository empRepo)
        {
            this.genericRepo = genericRepo;
            this.empRepo = empRepo;
        }

        public bool Delete(int designationID)
        {
            log.Info($"DesignationService/Delete{designationID}");

            bool flag = false;
            try
            {
                Data.Models.Designation dtoDesignation = new Data.Models.Designation();
                dtoDesignation = genericRepo.GetByID<Data.Models.Designation>(designationID);
                dtoDesignation.IsDeleted = true;
                genericRepo.Update<Data.Models.Designation>(dtoDesignation);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool DesignationNameExists(int? designationID, string designationName)
        {
            log.Info($"DesignationService/DesignationNameExists{designationID}/{designationName}");
            return genericRepo.Exists<DTOModel.Designation>(x => x.DesignationID != designationID
                   && x.DesignationName == designationName && !x.IsDeleted);
        }

        public bool DesignationCodeExists(int? designationID, string designationCode)
        {
            log.Info($"DesignationService/DesignationCodeExists{designationID}/{designationCode}");
            return genericRepo.Exists<DTOModel.Designation>(x => x.DesignationID != designationID && x.DesignationCode == designationCode && !x.IsDeleted);
        }

        public Designation GetDesignationByID(int designationID)
        {
            log.Info($"DesignationService/GetDesignationByID/{designationID}");
            try
            {
                var designationObj = genericRepo.GetByID<DTOModel.Designation>(designationID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Designation, Model.Designation>()
                    .ForMember(d => d.User, o => o.MapFrom(s => s.User))
                    .ForMember(d => d.CateogryID, o => o.MapFrom(s => s.CategoryID))
                    .ForMember(d => d.User1, o => o.MapFrom(s => s.User1));

                    //.ForMember(d => d.Cadre, o => o.MapFrom(s => s.))
                    //   .ForMember(d => d.Category, o => o.MapFrom(s => s.Category));

                    cfg.CreateMap<DTOModel.User, Model.User>();
                    cfg.CreateMap<DTOModel.EmployeeCategory, Model.EmployeeCategory>();
                    cfg.CreateMap<DTOModel.Cadre, Model.Cadre>();
                });
                var obj = Mapper.Map<DTOModel.Designation, Model.Designation>(designationObj);
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Designation> GetDesignationList(int? designationID, int? cadreID)
        {
            log.Info($"DesignationService/GetDesignationList");
            try
            {
                var result = genericRepo.Get<DTOModel.Designation>(x => !x.IsDeleted && (designationID.HasValue ? x.DesignationID == designationID.Value : 1 > 0)
                && (cadreID.HasValue ? x.CadreID == cadreID.Value : 1 > 0));
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Designation, Model.Designation>()
                    .ForMember(d => d.User, o => o.MapFrom(s => s.User))
                    .ForMember(d => d.User1, o => o.MapFrom(s => s.User1))
                    .ForMember(d => d.CadreName, o => o.MapFrom(s => s.Cadre.CadreName))
                    .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.EmployeeCategory.EmplCatName));
                    cfg.CreateMap<DTOModel.User, Model.User>();
                    cfg.CreateMap<DTOModel.Cadre, Model.Cadre>();
                    //.ForMember(d => d.CadreName, o => o.MapFrom(s => s.CadreName));
                    cfg.CreateMap<DTOModel.Category, Model.Category>();
                    //.ForMember(d => d.CategoryName, o => o.MapFrom(s => s.CategoryName));
                });
                var listDesignation = Mapper.Map<List<Model.Designation>>(result);
                return listDesignation.OrderBy(x => x.DesignationName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertDesignation(Designation createDesignation)
        {
            log.Info($"DesignationService/InsertDesignation");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Designation, DTOModel.Designation>();
                });
                var dtoDesignation = Mapper.Map<DTOModel.Designation>(createDesignation);
                genericRepo.Insert<DTOModel.Designation>(dtoDesignation);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateDesignation(Designation editDesignation)
        {
            log.Info($"DesignationService/UpdateDesignation");
            bool flag = false;
            try
            {
                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<Designation, DTOModel.Designation>();
                //});


                var dtoDesignation = genericRepo.Get<DTOModel.Designation>(x => x.DesignationID == editDesignation.DesignationID).FirstOrDefault();

                dtoDesignation.DesignationName = editDesignation.DesignationName;
                dtoDesignation.Level = editDesignation.Level;
                dtoDesignation.Rank = editDesignation.Rank;
                dtoDesignation.CategoryID = editDesignation.CateogryID == 0 ? null : editDesignation.CateogryID;
                dtoDesignation.CadreID = editDesignation.CadreID;
                dtoDesignation.LCT = editDesignation.LCT;
                dtoDesignation.Promotion = editDesignation.Promotion;
                dtoDesignation.Direct = editDesignation.Direct;
                dtoDesignation.IsOfficer = editDesignation.IsOfficer;
                dtoDesignation.LCTInNo = editDesignation.LCTInNo;
                dtoDesignation.PromotionInNo = editDesignation.PromotionInNo;
                dtoDesignation.DirectInNo = editDesignation.DirectInNo;
                dtoDesignation.FamilyAssured = editDesignation.FamilyAssured;
                genericRepo.Update<DTOModel.Designation>(dtoDesignation);
                flag = true;

                //genericRepo.Update<DTOModel.Designation>(dtoDesignation);
                //flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public bool UpdateDesignationPayScales(Model.EmployeePayScale payScale)
        {
            log.Info($"DesignationService/UpdateDesignationPayScales");
            bool flag = false;
            try
            {
                var dtoDesignation = genericRepo.GetByID<DTOModel.Designation>(payScale.designationID);
                dtoDesignation.B1 = payScale.B1;
                dtoDesignation.E1 = payScale.I1;
                dtoDesignation.B2 = payScale.B2;
                dtoDesignation.E2 = payScale.I2;
                dtoDesignation.B3 = payScale.B3;
                dtoDesignation.E3 = payScale.I3;
                dtoDesignation.B4 = payScale.B4;
                dtoDesignation.E4 = payScale.I4;
                dtoDesignation.B5 = payScale.B5;
                dtoDesignation.E5 = payScale.I5;
                dtoDesignation.B6 = payScale.B6;
                dtoDesignation.E6 = payScale.I6;
                dtoDesignation.B7 = payScale.B7;
                dtoDesignation.E7 = payScale.I7;
                dtoDesignation.B8 = payScale.B8;
                dtoDesignation.E8 = payScale.I8;
                dtoDesignation.B9 = payScale.B9;
                dtoDesignation.E9 = payScale.I9;
                dtoDesignation.B10 = payScale.B10;
                dtoDesignation.E10 = payScale.I10;
                dtoDesignation.B11 = payScale.B11;
                dtoDesignation.E11 = payScale.I11;
                dtoDesignation.B12 = payScale.B12;
                dtoDesignation.E12 = payScale.I12;
                dtoDesignation.B13 = payScale.B13;
                dtoDesignation.E13 = payScale.I13;
                dtoDesignation.B14 = payScale.B14;
                dtoDesignation.E14 = payScale.I14;
                dtoDesignation.B15 = payScale.B15;
                dtoDesignation.E15 = payScale.I15;
                dtoDesignation.B16 = payScale.B16;
                dtoDesignation.E16 = payScale.I16;
                dtoDesignation.B17 = payScale.B17;
                dtoDesignation.E17 = payScale.I17;
                dtoDesignation.B18 = payScale.B18;
                dtoDesignation.E18 = payScale.I18;
                dtoDesignation.B19 = payScale.B19;
                dtoDesignation.E19 = payScale.I19;
                dtoDesignation.B20 = payScale.B20;
                dtoDesignation.E20 = payScale.I20;
                dtoDesignation.B21 = payScale.B21;
                dtoDesignation.E21 = payScale.I21;
                dtoDesignation.B22 = payScale.B22;
                dtoDesignation.E22 = payScale.I22;
                dtoDesignation.B23 = payScale.B23;
                dtoDesignation.E23 = payScale.I23;
                dtoDesignation.B24 = payScale.B24;
                dtoDesignation.E24 = payScale.I24;
                dtoDesignation.B25 = payScale.B25;
                dtoDesignation.E25 = payScale.I25;
                dtoDesignation.B26 = payScale.B26;
                dtoDesignation.E26 = payScale.I26;
                dtoDesignation.B27 = payScale.B27;
                dtoDesignation.E27 = payScale.I27;
                dtoDesignation.B28 = payScale.B28;
                dtoDesignation.E28 = payScale.I28;
                dtoDesignation.B29 = payScale.B29;
                dtoDesignation.E29 = payScale.I29;
                dtoDesignation.B30 = payScale.B30;
                dtoDesignation.E30 = payScale.I30;
                dtoDesignation.B31 = payScale.B31;
                dtoDesignation.E31 = payScale.I31;
                dtoDesignation.B32 = payScale.B32;
                dtoDesignation.E32 = payScale.I32;
                dtoDesignation.B33 = payScale.B33;
                dtoDesignation.E33 = payScale.I33;
                dtoDesignation.B34 = payScale.B34;
                dtoDesignation.E34 = payScale.I34;
                dtoDesignation.B35 = payScale.B35;
                dtoDesignation.E35 = payScale.I35;
                dtoDesignation.B36 = payScale.B36;
                dtoDesignation.E36 = payScale.I36;
                dtoDesignation.B37 = payScale.B37;
                dtoDesignation.E37 = payScale.I37;
                dtoDesignation.B38 = payScale.B38;
                dtoDesignation.E38 = payScale.I38;
                dtoDesignation.B39 = payScale.B39;
                dtoDesignation.E39 = payScale.I39;
                dtoDesignation.B40 = payScale.B40;
                dtoDesignation.E40 = payScale.I40;
                dtoDesignation.UpdatedBy = payScale.UpdatedBy;
                dtoDesignation.UpdatedOn = payScale.UpdatedOn;
                genericRepo.Update<DTOModel.Designation>(dtoDesignation);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public EmployeePayScale CalculateBasicAndIncrement(EmployeePayScale payScale)
        {
            if (Convert.ToInt32(payScale.level.ToString().Substring(0, 2)) < 17)
            {
                payScale.B2 = payScale.B1 + NearestHundred(payScale.B1);
                payScale.I1 = NearestHundred(payScale.B1);

                payScale.B3 = payScale.B2 + NearestHundred(payScale.B2);
                payScale.I2 = NearestHundred(payScale.B2);

                payScale.B4 = payScale.B3 + NearestHundred(payScale.B3);
                payScale.I3 = NearestHundred(payScale.B3);

                if (Convert.ToInt32(payScale.level.ToString().Substring(0, 2)) < 16)
                {
                    payScale.B5 = payScale.B4 + NearestHundred(payScale.B4);
                    payScale.I4 = NearestHundred(payScale.B4);

                    payScale.B6 = payScale.B5 + NearestHundred(payScale.B5);
                    payScale.I5 = NearestHundred(payScale.B5);

                    payScale.B7 = payScale.B6 + NearestHundred(payScale.B6);
                    payScale.I6 = NearestHundred(payScale.B6);

                    payScale.B8 = payScale.B7 + NearestHundred(payScale.B7);
                    payScale.I7 = NearestHundred(payScale.B7);

                    if (Convert.ToInt32(payScale.level.ToString().Substring(0, 2)) < 15)
                    {
                        payScale.B9 = payScale.B8 + NearestHundred(payScale.B8);
                        payScale.I8 = NearestHundred(payScale.B8);

                        payScale.B10 = payScale.B9 + NearestHundred(payScale.B9);
                        payScale.I9 = NearestHundred(payScale.B9);

                        payScale.B11 = payScale.B10 + NearestHundred(payScale.B10);
                        payScale.I10 = NearestHundred(payScale.B10);

                        payScale.B12 = payScale.B11 + NearestHundred(payScale.B11);
                        payScale.I11 = NearestHundred(payScale.B11);

                        payScale.B13 = payScale.B12 + NearestHundred(payScale.B12);
                        payScale.I12 = NearestHundred(payScale.B12);

                        payScale.B14 = payScale.B13 + NearestHundred(payScale.B13);
                        payScale.I13 = NearestHundred(payScale.B13);

                        payScale.B15 = payScale.B14 + NearestHundred(payScale.B14);
                        payScale.I14 = NearestHundred(payScale.B14);

                        if (Convert.ToInt32(payScale.level.ToString().Substring(0, 2)) < 14 && Convert.ToInt32(payScale.level.ToString().Length) <= 3)
                        {
                            payScale.B16 = payScale.B15 + NearestHundred(payScale.B15);
                            payScale.I15 = NearestHundred(payScale.B15);

                            payScale.B17 = payScale.B16 + NearestHundred(payScale.B16);
                            payScale.I16 = NearestHundred(payScale.B16);

                            payScale.B18 = payScale.B17 + NearestHundred(payScale.B17);
                            payScale.I17 = NearestHundred(payScale.B17);

                            if (Convert.ToInt32(payScale.level.ToString().Substring(0, 2)) < 14 && Convert.ToInt32(payScale.level.ToString().Length) == 2)
                            {
                                payScale.B19 = payScale.B18 + NearestHundred(payScale.B18);
                                payScale.I18 = NearestHundred(payScale.B18);

                                payScale.B20 = payScale.B19 + NearestHundred(payScale.B19);
                                payScale.I19 = NearestHundred(payScale.B19);

                                payScale.B21 = payScale.B20 + NearestHundred(payScale.B20);
                                payScale.I20 = NearestHundred(payScale.B20);

                                if (Convert.ToInt32(payScale.level.ToString().Substring(0, 2)) < 13)
                                {
                                    payScale.B22 = payScale.B21 + NearestHundred(payScale.B21);
                                    payScale.I21 = NearestHundred(payScale.B21);

                                    payScale.B23 = payScale.B22 + NearestHundred(payScale.B22);
                                    payScale.I22 = NearestHundred(payScale.B22);

                                    payScale.B24 = payScale.B23 + NearestHundred(payScale.B23);
                                    payScale.I23 = NearestHundred(payScale.B23);

                                    payScale.B25 = payScale.B24 + NearestHundred(payScale.B24);
                                    payScale.I24 = NearestHundred(payScale.B24);

                                    payScale.B26 = payScale.B25 + NearestHundred(payScale.B25);
                                    payScale.I25 = NearestHundred(payScale.B25);

                                    payScale.B27 = payScale.B26 + NearestHundred(payScale.B26);
                                    payScale.I26 = NearestHundred(payScale.B26);

                                    payScale.B28 = payScale.B27 + NearestHundred(payScale.B27);
                                    payScale.I27 = NearestHundred(payScale.B27);

                                    payScale.B29 = payScale.B28 + NearestHundred(payScale.B28);
                                    payScale.I28 = NearestHundred(payScale.B28);

                                    payScale.B30 = payScale.B29 + NearestHundred(payScale.B29);
                                    payScale.I29 = NearestHundred(payScale.B29);

                                    payScale.B31 = payScale.B30 + NearestHundred(payScale.B30);
                                    payScale.I30 = NearestHundred(payScale.B30);

                                    payScale.B32 = payScale.B31 + NearestHundred(payScale.B31);
                                    payScale.I31 = NearestHundred(payScale.B31);

                                    payScale.B33 = payScale.B32 + NearestHundred(payScale.B32);
                                    payScale.I32 = NearestHundred(payScale.B32);

                                    payScale.B34 = payScale.B33 + NearestHundred(payScale.B33);
                                    payScale.I33 = NearestHundred(payScale.B33);

                                    if (Convert.ToInt32(payScale.level.ToString().Substring(0, 2)) < 12)
                                    {
                                        payScale.B35 = payScale.B34 + NearestHundred(payScale.B34);
                                        payScale.I34 = NearestHundred(payScale.B34);

                                        payScale.B36 = payScale.B35 + NearestHundred(payScale.B35);
                                        payScale.I35 = NearestHundred(payScale.B35);

                                        payScale.B37 = payScale.B36 + NearestHundred(payScale.B36);
                                        payScale.I36 = NearestHundred(payScale.B36);

                                        payScale.B38 = payScale.B37 + NearestHundred(payScale.B37);
                                        payScale.I37 = NearestHundred(payScale.B37);

                                        payScale.B39 = payScale.B38 + NearestHundred(payScale.B38);
                                        payScale.I38 = NearestHundred(payScale.B38);

                                        if (Convert.ToInt32(payScale.level.ToString().Substring(0, 2)) < 11)
                                        {
                                            payScale.B40 = payScale.B39 + NearestHundred(payScale.B39);
                                            payScale.I39 = NearestHundred(payScale.B39);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return payScale;
        }

        private int NearestHundred(decimal? num)
        {
            double nextbasic;
            nextbasic = Convert.ToDouble(num) * 3 / 100;
            if (nextbasic.ToString().Substring(nextbasic.ToString().Length - 2, 2) == "50")
            {
                nextbasic = nextbasic + 1;
            }
            var roundVal = Math.Round((nextbasic / 100), 0) * 100;
            return Convert.ToInt32(roundVal);
        }

        public EmployeePayScale GetDesignationPayScale(int designationID)
        {
            log.Info($"DesignationService/GetDesignationPayScale/{designationID}");
            try
            {
                var designationObj = genericRepo.GetByID<DTOModel.Designation>(designationID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Designation, Model.EmployeePayScale>()
                    .ForMember(d => d.designationID, o => o.MapFrom(s => s.DesignationID))
                    .ForMember(d => d.level, o => o.MapFrom(s => s.Level))
                    .ForMember(d => d.B1, o => o.MapFrom(s => s.B1))
                    .ForMember(d => d.I1, o => o.MapFrom(s => s.E1))
                    .ForMember(d => d.B2, o => o.MapFrom(s => s.B2))
                    .ForMember(d => d.I2, o => o.MapFrom(s => s.E2))
                    .ForMember(d => d.B3, o => o.MapFrom(s => s.B3))
                    .ForMember(d => d.I3, o => o.MapFrom(s => s.E3))
                    .ForMember(d => d.B4, o => o.MapFrom(s => s.B4))
                    .ForMember(d => d.I4, o => o.MapFrom(s => s.E4))
                    .ForMember(d => d.B5, o => o.MapFrom(s => s.B5))
                    .ForMember(d => d.I5, o => o.MapFrom(s => s.E5))
                    .ForMember(d => d.B6, o => o.MapFrom(s => s.B6))
                    .ForMember(d => d.I6, o => o.MapFrom(s => s.E6))
                    .ForMember(d => d.B7, o => o.MapFrom(s => s.B7))
                    .ForMember(d => d.I7, o => o.MapFrom(s => s.E7))
                    .ForMember(d => d.B8, o => o.MapFrom(s => s.B8))
                    .ForMember(d => d.I8, o => o.MapFrom(s => s.E8))
                    .ForMember(d => d.B9, o => o.MapFrom(s => s.B9))
                    .ForMember(d => d.I9, o => o.MapFrom(s => s.E9))
                    .ForMember(d => d.B10, o => o.MapFrom(s => s.B10))
                    .ForMember(d => d.I10, o => o.MapFrom(s => s.E10))
                    .ForMember(d => d.B11, o => o.MapFrom(s => s.B11))
                    .ForMember(d => d.I11, o => o.MapFrom(s => s.E11))
                    .ForMember(d => d.B12, o => o.MapFrom(s => s.B12))
                    .ForMember(d => d.I12, o => o.MapFrom(s => s.E12))
                    .ForMember(d => d.B13, o => o.MapFrom(s => s.B13))
                    .ForMember(d => d.I13, o => o.MapFrom(s => s.E13))
                    .ForMember(d => d.B14, o => o.MapFrom(s => s.B14))
                    .ForMember(d => d.I14, o => o.MapFrom(s => s.E14))
                    .ForMember(d => d.B15, o => o.MapFrom(s => s.B15))
                    .ForMember(d => d.I15, o => o.MapFrom(s => s.E15))
                    .ForMember(d => d.B16, o => o.MapFrom(s => s.B16))
                    .ForMember(d => d.I16, o => o.MapFrom(s => s.E16))
                    .ForMember(d => d.B17, o => o.MapFrom(s => s.B17))
                    .ForMember(d => d.I17, o => o.MapFrom(s => s.E17))
                    .ForMember(d => d.B18, o => o.MapFrom(s => s.B18))
                    .ForMember(d => d.I18, o => o.MapFrom(s => s.E18))
                    .ForMember(d => d.B19, o => o.MapFrom(s => s.B19))
                    .ForMember(d => d.I19, o => o.MapFrom(s => s.E19))
                    .ForMember(d => d.B20, o => o.MapFrom(s => s.B20))
                    .ForMember(d => d.I20, o => o.MapFrom(s => s.E20))
                    .ForMember(d => d.B21, o => o.MapFrom(s => s.B21))
                    .ForMember(d => d.I21, o => o.MapFrom(s => s.E21))
                    .ForMember(d => d.B22, o => o.MapFrom(s => s.B22))
                    .ForMember(d => d.I22, o => o.MapFrom(s => s.E22))
                    .ForMember(d => d.B23, o => o.MapFrom(s => s.B23))
                    .ForMember(d => d.I23, o => o.MapFrom(s => s.E23))
                    .ForMember(d => d.B24, o => o.MapFrom(s => s.B24))
                    .ForMember(d => d.I24, o => o.MapFrom(s => s.E24))
                    .ForMember(d => d.B25, o => o.MapFrom(s => s.B25))
                    .ForMember(d => d.I25, o => o.MapFrom(s => s.E25))
                    .ForMember(d => d.B26, o => o.MapFrom(s => s.B26))
                    .ForMember(d => d.I26, o => o.MapFrom(s => s.E26))
                    .ForMember(d => d.B27, o => o.MapFrom(s => s.B27))
                    .ForMember(d => d.I27, o => o.MapFrom(s => s.E27))
                    .ForMember(d => d.B28, o => o.MapFrom(s => s.B28))
                    .ForMember(d => d.I28, o => o.MapFrom(s => s.E28))
                    .ForMember(d => d.B29, o => o.MapFrom(s => s.B29))
                    .ForMember(d => d.I29, o => o.MapFrom(s => s.E29))
                    .ForMember(d => d.B30, o => o.MapFrom(s => s.B30))
                    .ForMember(d => d.I30, o => o.MapFrom(s => s.E30))
                    .ForMember(d => d.B31, o => o.MapFrom(s => s.B31))
                    .ForMember(d => d.I31, o => o.MapFrom(s => s.E31))
                    .ForMember(d => d.B32, o => o.MapFrom(s => s.B32))
                    .ForMember(d => d.I32, o => o.MapFrom(s => s.E32))
                    .ForMember(d => d.B33, o => o.MapFrom(s => s.B33))
                    .ForMember(d => d.I33, o => o.MapFrom(s => s.E33))
                    .ForMember(d => d.B34, o => o.MapFrom(s => s.B34))
                    .ForMember(d => d.I34, o => o.MapFrom(s => s.E34))
                    .ForMember(d => d.B35, o => o.MapFrom(s => s.B35))
                    .ForMember(d => d.I35, o => o.MapFrom(s => s.E35))
                    .ForMember(d => d.B36, o => o.MapFrom(s => s.B36))
                    .ForMember(d => d.I36, o => o.MapFrom(s => s.E36))
                    .ForMember(d => d.B37, o => o.MapFrom(s => s.B37))
                    .ForMember(d => d.I37, o => o.MapFrom(s => s.E37))
                    .ForMember(d => d.B38, o => o.MapFrom(s => s.B38))
                    .ForMember(d => d.I38, o => o.MapFrom(s => s.E38))
                    .ForMember(d => d.B39, o => o.MapFrom(s => s.B39))
                    .ForMember(d => d.I39, o => o.MapFrom(s => s.E39))
                    .ForMember(d => d.B40, o => o.MapFrom(s => s.B40))
                    .ForMember(d => d.I40, o => o.MapFrom(s => s.E40))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Designation, Model.EmployeePayScale>(designationObj);
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetDesignationByCadre(int cadreID)
        {
            log.Info($"DesignationService/GetDesignationByCadre/{cadreID}");

            try
            {
                var designations = genericRepo.Get<DTOModel.Designation>(x => x.CadreID == cadreID && x.IsDeleted == false).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Designation, Model.SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.DesignationID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DesignationName));
                });
                return Mapper.Map<List<DTOModel.Designation>, List<Model.SelectListModel>>(designations);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #region Designation Assignment /Promotion 

        /// <summary>
        /// Assign New Designation / Employee Promotion ===
        /// </summary>
        /// <param name="promotion"> </param>
        /// <returns></returns>
        public Promotion ChangeDesignation(Promotion promotion)
        {
            log.Info($"DesignationService/ChangeDesignation");
            try
            {
                //if (promotion.FormActionType.Equals("Create", StringComparison.OrdinalIgnoreCase))
                //{
                if (promotion.FormActionType == "Create" && ValidatePromotionFormInputs(promotion.EmployeeCode))
                {
                    promotion.IsValidInputs = false;
                    promotion.ValidationMessage = "You can not process new transaction until the details of previous transaction is completed";
                }
                else
                {
                    var seniorityNo = 11111111; DateTime? fromDate = null, toDate = null;
                    var promtionTransEntities = genericRepo.GetIQueryable<DTOModel.tblpromotion>(x => x.EmployeeCode == promotion.EmployeeCode
                    && x.DesignationID == promotion.DesignationID && !x.IsDeleted).ToList();

                    promtionTransEntities.ForEach(x =>
                    {
                        seniorityNo = x.SeniorityCode ?? seniorityNo;
                        fromDate = x.FromDate;
                        toDate = x.ToDate;
                    });
                    if (promotion.FormActionType == "Create" && seniorityNo != 11111111)
                    {
                        promotion.IsValidInputs = false;
                        promotion.ValidationMessage = $"This employee already worked on this designation From {fromDate.Value.ToString("dd/MM/yyyy")} & Todate {toDate.Value.ToString("dd/MM/yyyy")} with Seniority Code {seniorityNo}";
                    }
                    else if (promotion.Confirmed && !promotion.ConfirmationDate.HasValue)
                    {
                        promotion.IsValidInputs = false;
                        promotion.ValidationMessage = $"To Confirm an Employee Confirmation date can not be empty";
                    }
                }

                if (promotion.IsValidInputs)
                {
                    if (promotion.FormActionType == "Create")
                    {
                        //var currentDesignationID = genericRepo.GetByID<DTOModel.tblMstEmployee>((int)promotion.EmployeeID).DesignationID;
                        var currentDesignationID = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == (int)promotion.EmployeeID).FirstOrDefault().DesignationID;
                        promotion.currentDesignation = GetDesignationByID(currentDesignationID);
                        promotion.newDesignation = GetDesignationByID((int)promotion.DesignationID);

                        decimal? nE_Basic = null; // null;nE_Basic = 28400
                        bool salaryUpdated = UpdateSalaryOnDesignationChange(promotion, out nE_Basic);

                        if (salaryUpdated)
                        {
                            var current_Emp = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeCode == promotion.EmployeeCode && !x.IsDeleted).FirstOrDefault();
                            current_Emp.DesignationID = promotion.DesignationID ?? current_Emp.DesignationID;

                            #region Get Max Seniority Code of Designation
                            var maxSen = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.DesignationID == current_Emp.DesignationID && x.DOLeaveOrg == null).Select(s => s.Sen_Code).Max();
                            #endregion
                            var employeeSenList = empRepo.GetSeniorityList((int)promotion.EmployeeID, currentDesignationID);

                            current_Emp.CadreID = promotion.CadreID;
                            current_Emp.DOJ = promotion.FromDate.Value;
                            current_Emp.Sen_Code = (maxSen ?? 0) + 1;
                            current_Emp.ConfirmationDate = promotion.ConfirmationDate;
                            current_Emp.PayScale = empRepo.GetDesignationPayScaleList(promotion.DesignationID).FirstOrDefault().PayScale;
                            current_Emp.UpdatedBy = promotion.FormActionType == "Create" ? promotion.CreatedBy : promotion.UpdatedBy;
                            current_Emp.UpdatedOn = promotion.FormActionType == "Create" ? promotion.CreatedOn : promotion.UpdatedOn;
                            genericRepo.Update<DTOModel.tblMstEmployee>(current_Emp);


                            #region Update sen code for all below seniority by -1 in previous designation
                            
                            for (int i = 0; i < employeeSenList.Count; i++)
                            {
                                var getdata = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeSenList[i].EmployeeId);
                                if (getdata != null)
                                {
                                    getdata.Sen_Code = getdata.Sen_Code - 1;
                                    genericRepo.Update<DTOModel.tblMstEmployee>(getdata);
                                }
                            }
                            #endregion


                            if (promotion.FormActionType == "Create")
                            {
                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<Promotion, DTOModel.tblpromotion>()
                                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                                    .ForMember(d => d.CadreID, o => o.MapFrom(s => s.CadreID))
                                    .ForMember(d => d.WayOfPostingID, o => o.MapFrom(s => s.WayOfPostingID))
                                    .ForMember(d => d.FromDate, o => o.MapFrom(s => s.FromDate))
                                    .ForMember(d => d.ToDate, o => o.MapFrom(s => s.ToDate))
                                    .ForMember(d => d.Confirmed, o => o.MapFrom(s => s.Confirmed))
                                    .ForMember(d => d.ConfirmationDate, o => o.MapFrom(s => s.ConfirmationDate))
                                    .ForMember(d => d.SeniorityCode, o => o.MapFrom(s => s.SeniorityCode))
                                    .ForMember(d => d.E_Basic, o => o.UseValue(nE_Basic))
                                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                    .ForMember(d => d.NewTS, o => o.MapFrom(s => s.NewTS))
                                    .ForMember(d => d.OrderOfPromotion, o => o.MapFrom(s => s.OrderOfPromotion))
                                    .ForAllOtherMembers(d => d.Ignore());
                                });
                                var dtoPromotion = Mapper.Map<DTOModel.tblpromotion>(promotion);
                                int newtransID = genericRepo.Insert<DTOModel.tblpromotion>(dtoPromotion);
                              //  UpdateSeniorityCode(currentDesignationID, promotion.DesignationID, promotion.EmployeeID);

                                #region Promotional Confirmation Form-- if order of promotion date is provide then initiate confirmation process or proceed with direct promotion
                                if (promotion.OrderOfPromotion.HasValue)
                                {
                                    int formABHdrID = 0;
                                    int formTypeID = 0;

                                    int formHdrID = InsertEmployeeConfirmationDetails(promotion.EmployeeID, promotion.DesignationID, promotion.CreatedBy, promotion.OrderOfPromotion, out formABHdrID, out formTypeID);


                                    int? formAHdrid = null;
                                    int? formBHdrid = null;
                                    if (formTypeID == 1)
                                        formAHdrid = formABHdrID;
                                    else
                                        formBHdrid = formABHdrID;

                                    var dtoObjStatus = genericRepo.Get<DTOModel.ConfirmationStatu>(x => x.EmployeeID == promotion.EmployeeID && x.ProcessId == 7 && x.FormHdrID == null).ToList();
                                    if (dtoObjStatus != null && dtoObjStatus.Count > 0)
                                    {
                                        foreach (var item in dtoObjStatus)
                                        {
                                            item.FormHdrID = formHdrID;
                                            item.FormAHeaderID = formAHdrid;
                                            item.FormBHeaderID = formBHdrid;
                                            genericRepo.Update<DTOModel.ConfirmationStatu>(item);
                                        }
                                    }
                                }
                                #endregion
                                promotion.Saved = true;
                            }
                        }
                    }
                    else
                    {
                        var prevTransRow = genericRepo.Get<DTOModel.tblpromotion>(x => x.TransID == promotion.TransID && x.EmployeeCode == promotion.EmployeeCode).FirstOrDefault();
                        prevTransRow.ToDate = promotion.ToDate;
                        prevTransRow.ConfirmationDate = promotion.ConfirmationDate;
                        prevTransRow.FromDate = promotion.FromDate;
                        prevTransRow.DesignationID = promotion.DesignationID;
                        prevTransRow.CadreID = promotion.CadreID;
                        prevTransRow.WayOfPostingID = promotion.WayOfPostingID;
                        prevTransRow.Confirmed = promotion.Confirmed;
                        prevTransRow.NewTS = promotion.NewTS;
                        prevTransRow.UpdatedBy = promotion.UpdatedBy;
                        prevTransRow.UpdatedOn = promotion.UpdatedOn;

                        // prevTransRow.
                        genericRepo.Update<DTOModel.tblpromotion>(prevTransRow);
                        promotion.Saved = true;
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return promotion;
        }
        private bool ValidatePromotionFormInputs(string employeeCode)
        {
            log.Info($"ValidatePromotionFormInputs/ChangeDesignation");

            var lastTrans = genericRepo.GetIQueryable<DTOModel.tblpromotion>
                (x => x.EmployeeCode == employeeCode && !x.IsDeleted).OrderByDescending(y => y.TransID).Take(1);
            return lastTrans.Any(z => z.ToDate == null);
        }

        public bool DeletePromotionTransEntry(int transID)
        {
            log.Info($"DesignationService/DeletePromotionTransEntry/{transID}");
            bool flag = false;
            try
            {
                var transRow = genericRepo.GetByID<DTOModel.tblpromotion>(transID);
                transRow.IsDeleted = true;
                genericRepo.Update<DTOModel.tblpromotion>(transRow);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
            #endregion
        }

        /// <summary>
        /// Update salary on Designation change / Promotion==== 
        /// </summary>
        /// <param name="employeeCode"> EmployeeCode </param>
        /// <param name="designationID">Designation ID </param>
        /// <returns></returns>
        private bool UpdateSalaryOnDesignationChange(Promotion promotion, out decimal? nE_Basic)
        {
            log.Info($"DesignationService/UpdateSalaryOnDesignationChange/");
            bool updated = false;
            try
            {
                var empPayScale = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeCode == promotion.EmployeeCode).FirstOrDefault().PayScale;

                var empCurrentSalary = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == promotion.EmployeeCode).FirstOrDefault().E_Basic;
                var currentBasicWithIncrement = empCurrentSalary; nE_Basic = null;
                if (!string.IsNullOrEmpty(empPayScale))
                {
                    var currentPayScaleLimit = DesignationPayScaleLimit(promotion.currentDesignation.Level);
                    var payScale = empPayScale.Split(new char[] { '-' }).Select(x => int.Parse(x)).AsEnumerable();
                    int[] basics = payScale.Where((x, i) => i % 2 == 0).ToArray<int>();
                    int[] increments = payScale.Where((x, i) => i % 2 != 0).ToArray<int>();

                    ///=== Check current basic salary increment based on current payScale limit ====
                    for (int i = 0; i < currentPayScaleLimit; i++)
                    {
                        if ((int)empCurrentSalary.Value == basics[i])  ///==== 
                        {
                            currentBasicWithIncrement += increments[i];
                            break;
                        }
                    }
                }
                var newDesignationPayScale = empRepo.GetDesignationPayScaleList(promotion.DesignationID).FirstOrDefault().PayScale;
                if (!string.IsNullOrEmpty(newDesignationPayScale) && promotion.DesignationID.HasValue)
                {
                    var newPayScaleLimit = DesignationPayScaleLimit(promotion.newDesignation.Level);

                    var npayScale = newDesignationPayScale.Split(new char[] { '-' }).Select(x => int.Parse(x)).AsEnumerable();
                    int[] nbasics = npayScale.Where((x, i) => i % 2 == 0).ToArray<int>();
                    int[] nincrements = npayScale.Where((x, i) => i % 2 != 0).ToArray<int>();

                    ///=== Check current basic salary increment based on current payScale limit ====
                    for (int i = 0; i < newPayScaleLimit; i++)
                    {
                        if ((int)currentBasicWithIncrement < nbasics[i])  ///==== 
                        {
                            nE_Basic = nbasics[i];
                            break;
                        }
                    }
                }
                if (nE_Basic.HasValue)
                {
                    var empSalaryRow = genericRepo.Get<DTOModel.TblMstEmployeeSalary>(x => x.EmployeeCode == promotion.EmployeeCode).FirstOrDefault();
                    if (empSalaryRow != null)
                    {
                        empSalaryRow.E_Basic = nE_Basic;
                        empSalaryRow.LastBasic = empCurrentSalary;
                        genericRepo.Update<DTOModel.TblMstEmployeeSalary>(empSalaryRow);
                        updated = true;
                    }
                }
                return updated;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            // return true;
        }

        /// <summary>
        ///  Get Designation  PayScale limit===
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private int DesignationPayScaleLimit(string level)
        {
            log.Info($"DesignationService/DesignationPayScaleLimit/{level}");
            int limit = 0;
            if (level == "1" || level == "2" || level == "3" || level == "4" || level == "5" || level == "6" || level == "7" || level == "8" || level == "9" || level == "10")
                limit = 40;
            else if (level == "11")
                limit = 39;
            else if (level == "12")
                limit = 34;
            else if (level == "13")
                limit = 20;
            else if (level == "13A")
                limit = 18;
            else if (level == "14")
                limit = 15;
            else if (level == "15")
                limit = 8;
            else if (level == "16")
                limit = 4;
            else if (level == "17")
                limit = 1;
            else if (level == "18")
                limit = 1;
            return limit;
        }

        private bool UpdateSeniorityCode(int oldDesignationId, int? currentDesignationId, int? employeeId)
        {
            log.Info($"DesignationService/UpdateSeniorityCode");
            try
            {
                bool flag = false;
                var result = empRepo.UpdateSeniorityCode(oldDesignationId, currentDesignationId, employeeId);
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int InsertEmployeeConfirmationDetails(int? employeeID, int? designationID, int createdBy, DateTime? FromDate, out int formABHdrID, out int formTypeID)
        {
            log.Info($"DesignationService/InsertEmployeeConfirmationDetails/{employeeID}/{designationID}");
            // bool flag = false;
            var formHdrID = 0;
            formABHdrID = 0;
            formTypeID = 0;
            try
            {
                var level = genericRepo.Get<DTOModel.Designation>(x => x.tblMstEmployees.Any(y => y.EmployeeId == employeeID)).FirstOrDefault().Level;
                var EmpDetails = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == employeeID).FirstOrDefault();
                if (level == "1" || level == "2" || level == "3" || level == "4" || level == "5" || level == "6" || level == "7")
                {
                    Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                    confirmationFormHdr.FormTypeID = 2;
                    confirmationFormHdr.ProcessID = 7;
                    confirmationFormHdr.StatusID = 0;
                    confirmationFormHdr.EmployeeID = employeeID.Value;
                    confirmationFormHdr.CreatedBy = createdBy;
                    confirmationFormHdr.CreatedOn = DateTime.Now;
                    formTypeID = (int)confirmationFormHdr.FormTypeID;
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
                        .ForMember(d => d.FormTypeID, o => o.MapFrom(s => s.FormTypeID))
                        .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                        .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForAllOtherMembers(d => d.Ignore());
                    });

                    var dtoConfirmationFormHdr = Mapper.Map<DTOModel.ConfirmationFormHdr>(confirmationFormHdr);
                    genericRepo.Insert<DTOModel.ConfirmationFormHdr>(dtoConfirmationFormHdr);

                    formHdrID = dtoConfirmationFormHdr.FormHdrID;

                    Model.ConfirmationFormAHdr confirmationForm = new ConfirmationFormAHdr();
                    confirmationForm.EmployeeId = employeeID.Value;
                    confirmationForm.ProcessId = 7;
                    confirmationForm.DesignationId = designationID.Value;
                    confirmationForm.BranchId = EmpDetails.BranchID;
                    confirmationForm.CertificatesReceived = true;
                    confirmationForm.PoliceVerification = true;
                    confirmationForm.CreatedBy = createdBy;
                    confirmationForm.CreatedOn = DateTime.Now;
                    confirmationForm.DueConfirmationDate = FromDate.Value.AddYears(1);      // here FromDate is order of promotion date            
                    confirmationForm.FormHdrID = formHdrID;

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormBHeader>()
                        .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                        .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                         .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                         .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                         .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                         .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                         .ForMember(d => d.FormState, o => o.UseValue(0))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                        .ForAllOtherMembers(d => d.Ignore());
                    });

                    var dtoConfirmationFormBHdr = Mapper.Map<DTOModel.ConfirmationFormBHeader>(confirmationForm);
                    genericRepo.Insert<DTOModel.ConfirmationFormBHeader>(dtoConfirmationFormBHdr);
                    formABHdrID = dtoConfirmationFormBHdr.FormBHeaderId;
                }
                else if (level == "8" || level == "9" || level == "10" || level == "11" || level == "12" || level == "13A")
                {
                    Model.ConfirmationFormHdr confirmationFormHdr = new ConfirmationFormHdr();
                    confirmationFormHdr.FormTypeID = 1;
                    confirmationFormHdr.ProcessID = 7;
                    confirmationFormHdr.StatusID = 0;
                    confirmationFormHdr.EmployeeID = employeeID.Value;
                    confirmationFormHdr.CreatedBy = createdBy;
                    confirmationFormHdr.CreatedOn = DateTime.Now;
                    formTypeID = (int)confirmationFormHdr.FormTypeID;
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConfirmationFormHdr, DTOModel.ConfirmationFormHdr>()
                        .ForMember(d => d.FormTypeID, o => o.MapFrom(s => s.FormTypeID))
                        .ForMember(d => d.ProcessID, o => o.MapFrom(s => s.ProcessID))
                        .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForAllOtherMembers(d => d.Ignore());
                    });

                    var dtoConfirmationFormHdr = Mapper.Map<DTOModel.ConfirmationFormHdr>(confirmationFormHdr);
                    genericRepo.Insert<DTOModel.ConfirmationFormHdr>(dtoConfirmationFormHdr);

                    formHdrID = dtoConfirmationFormHdr.FormHdrID;

                    Model.ConfirmationFormAHdr confirmationForm = new ConfirmationFormAHdr();
                    confirmationForm.EmployeeId = employeeID.Value;
                    confirmationForm.ProcessId = 7;
                    confirmationForm.DesignationId = designationID.Value;
                    confirmationForm.BranchId = EmpDetails.BranchID;
                    confirmationForm.CertificatesReceived = true;
                    confirmationForm.PoliceVerification = true;
                    confirmationForm.CreatedBy = createdBy;
                    confirmationForm.CreatedOn = DateTime.Now;
                    confirmationForm.DueConfirmationDate = FromDate.Value.AddYears(1);    // here FromDate is order of promotion date          
                    confirmationForm.FormHdrID = formHdrID;

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ConfirmationFormAHdr, DTOModel.ConfirmationFormAHeader>()
                        .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.ProcessId, o => o.MapFrom(s => s.ProcessId))
                        .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                         .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                         .ForMember(d => d.CertificatesReceived, o => o.MapFrom(s => s.CertificatesReceived))
                         .ForMember(d => d.PoliceVerification, o => o.MapFrom(s => s.PoliceVerification))
                         .ForMember(d => d.DueConfirmationDate, o => o.MapFrom(s => s.DueConfirmationDate))
                         .ForMember(d => d.FormState, o => o.UseValue(0))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForMember(d => d.FormHdrID, o => o.MapFrom(s => s.FormHdrID))
                        .ForAllOtherMembers(d => d.Ignore());
                    });

                    var dtoConfirmationFormAHdr = Mapper.Map<DTOModel.ConfirmationFormAHeader>(confirmationForm);
                    genericRepo.Insert<DTOModel.ConfirmationFormAHeader>(dtoConfirmationFormAHdr);
                    formABHdrID = dtoConfirmationFormAHdr.FormAHeaderId;
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now + "InnerException" + ex.InnerException.Message);
            }
            return formHdrID;
        }

        public bool CheckConfrmChildHasRecord(int empID, int processID)
        {
            return genericRepo.Exists<DTOModel.ConfirmationStatu>(x => x.EmployeeID == empID && x.ProcessId == processID);
        }
    }
}
