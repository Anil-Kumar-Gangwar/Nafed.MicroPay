using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using DTOModel = Nafed.MicroPay.Data.Models;
using System.Data;

namespace Nafed.MicroPay.Services
{
    public class Form12BBService : BaseService, IForm12BBService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IForm12BBRepository _Form12BBRepository;
        private readonly IForm12BBDocumentRepository _Form12BBDocumentRepository;
        public Form12BBService(IGenericRepository genericRepo, IForm12BBRepository Form12BBRepo, IForm12BBDocumentRepository Form12BBDocumentRepository)
        {
            this.genericRepo = genericRepo;
            this._Form12BBRepository = Form12BBRepo;
            this._Form12BBDocumentRepository = Form12BBDocumentRepository;
        }

        #region ===== Tax Deduction Section / SubSection / Description Master ===========

        public int CreateSection(DeductionSection dSection)
        {
            log.Info($"Form12BBService/CreateSection");
            int sectionID = 0;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DeductionSection, DTOModel.DeductionSection>();
                });
                var dtoDeductionSection = Mapper.Map<DTOModel.DeductionSection>(dSection);
                int res = genericRepo.Insert<DTOModel.DeductionSection>(dtoDeductionSection);
                if (res > 0)
                    return dtoDeductionSection.SectionID;
                return sectionID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public int CreateSubSection(DeductionSubSection dSubSection)
        {
            log.Info($"Form12BBService/CreateSubSection");
            int subsectionID = 0;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DeductionSubSection, DTOModel.DeductionSubSection>();
                });
                var dtoDeductionSubSection = Mapper.Map<DTOModel.DeductionSubSection>(dSubSection);
                int res = genericRepo.Insert<DTOModel.DeductionSubSection>(dtoDeductionSubSection);
                if (res > 0)
                    return dtoDeductionSubSection.SubSectionID;
                return subsectionID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public int CreateSubSectionDescription(DeductionSubSectionDescription dSubSecDescription)
        {
            log.Info($"Form12BBService/CreateSubSectionDescription");
            int subsectionID = 0;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DeductionSubSectionDescription, DTOModel.DeductionSubSectionDescription>();
                });
                var dtoDeductionSubSectionDesc = Mapper.Map<DTOModel.DeductionSubSectionDescription>(dSubSecDescription);
                int res = genericRepo.Insert<DTOModel.DeductionSubSectionDescription>(dtoDeductionSubSectionDesc);
                if (res > 0)
                    return dtoDeductionSubSectionDesc.SubSectionID;
                return subsectionID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<DeductionSection> GetSectionList()
        {
            log.Info($"Form12BBService/GetSectionList");
            try
            {
                var dtoSections = genericRepo.Get<DTOModel.DeductionSection>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSection, DeductionSection>();
                });
                return Mapper.Map<List<DeductionSection>>(dtoSections);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<DeductionSubSection> GetSubSectionList()
        {
            log.Info($"Form12BBService/GetSubSectionList");
            try
            {
                var dtoSubSections = genericRepo.Get<DTOModel.DeductionSubSection>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSubSection, DeductionSubSection>()
                    .ForMember(d => d.SectionName, o => o.MapFrom(s => s.DeductionSection.SectionName))
                    ;
                });
                return Mapper.Map<List<DeductionSubSection>>(dtoSubSections);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public List<DeductionSubSectionDescription> GetSubSectionDescriptions()
        {
            log.Info($"Form12BBService/GetSubSectionDescriptions");

            try
            {
                var dtoSubSectionDesc = genericRepo.Get<DTOModel.DeductionSubSectionDescription>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSubSectionDescription, DeductionSubSectionDescription>()
                    .ForMember(d => d.SectionName, o => o.MapFrom(s => s.DeductionSection.SectionName))
                    ;
                });
                return Mapper.Map<List<DeductionSubSectionDescription>>(dtoSubSectionDesc);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public bool DeleteSubSection(int sectionID, int subSectionID)
        {
            log.Info($"Form12BBService/DeleteSubSection/{sectionID}/{subSectionID}");
            bool flag = false;
            try
            {
                var dtoSubSection = genericRepo.Get<DTOModel.DeductionSubSection>(x => x.SectionID == sectionID && x.SubSectionID == subSectionID).FirstOrDefault();
                dtoSubSection.IsDeleted = true;
                genericRepo.Update<DTOModel.DeductionSubSection>(dtoSubSection);
                flag = true;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool DeleteSubSectionDescription(int sectionID, int subSectionID, int descriptionID)
        {
            log.Info($"Form12BBService/DeleteSubSection/{sectionID}/{subSectionID}/{descriptionID}");
            bool flag = false;
            try
            {
                var dtoSubSectionDescription = genericRepo.Get<DTOModel.DeductionSubSectionDescription>(x => x.SectionID == sectionID
                && x.SubSectionID == subSectionID && x.DescriptionID == descriptionID).FirstOrDefault();
                dtoSubSectionDescription.IsDeleted = true;
                genericRepo.Update<DTOModel.DeductionSubSectionDescription>(dtoSubSectionDescription);
                flag = true;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool DeleteSection(int sectionID)
        {
            log.Info($"Form12BBService/DeleteSection/{sectionID}");
            bool flag = false;
            try
            {
                var dtoSection = genericRepo.GetByID<DTOModel.DeductionSection>(sectionID);
                dtoSection.IsDeleted = true;
                genericRepo.Update<DTOModel.DeductionSection>(dtoSection);
                flag = true;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateSection(Model.DeductionSection dSection)
        {
            log.Info($"Form12BBService/UpdateSection");
            var flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DeductionSection, DTOModel.DeductionSection>();
                });
                var dtoDSection = Mapper.Map<DTOModel.DeductionSection>(dSection);
                genericRepo.Update<DTOModel.DeductionSection>(dtoDSection);
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UpdateSubSection(Model.DeductionSubSection dSubSection)
        {
            log.Info($"Form12BBService/UpdateSubSection");
            var flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DeductionSubSection, DTOModel.DeductionSubSection>();
                });
                var dtoDSubSection = Mapper.Map<DTOModel.DeductionSubSection>(dSubSection);
                genericRepo.Update<DTOModel.DeductionSubSection>(dtoDSubSection);
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UpdateSubSectionDescription(Model.DeductionSubSectionDescription dSubSectionDesc)
        {
            log.Info($"Form12BBService/UpdateSubSectionDescription");
            var flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DeductionSubSectionDescription, DTOModel.DeductionSubSectionDescription>();
                });
                var dtoDSubSectionDesc = Mapper.Map<DTOModel.DeductionSubSectionDescription>(dSubSectionDesc);
                genericRepo.Update<DTOModel.DeductionSubSectionDescription>(dtoDSubSectionDesc);
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;

        }
        public List<SelectListModel> GetSectionList(string fYr)
        {
            log.Info($"Form12BBService/GetSectionList/{fYr}");
            try
            {
                var dtoSection = genericRepo.Get<DTOModel.DeductionSection>(x => !x.IsDeleted && x.FYear == fYr);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSection, SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.SectionID))
                     .ForMember(d => d.value, o => o.MapFrom(s => s.SectionName))
                     .ForAllOtherMembers(d => d.Ignore())
                    ;
                });
                return Mapper.Map<List<SelectListModel>>(dtoSection);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListModel> GetSubSections(int sectionID)
        {

            log.Info($"Form12BBService/GetSubSections/{sectionID}");
            try
            {
                var dtoSubSection = genericRepo.Get<DTOModel.DeductionSubSection>(x => !x.IsDeleted && x.SectionID == sectionID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSubSection, SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.SubSectionID))
                     .ForMember(d => d.value, o => o.MapFrom(s => s.SubSectionName))
                     .ForAllOtherMembers(d => d.Ignore())
                    ;
                });
                return Mapper.Map<List<SelectListModel>>(dtoSubSection);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListModel> GetSubSectionDescriptions(int sectionID, int subSectionID)
        {
            log.Info($"Form12BBService/GetSubSectionDescriptions/{sectionID}/{subSectionID}");
            try
            {
                var dtoDescriptions = genericRepo.Get<DTOModel.DeductionSubSectionDescription>(x => 
                x.SectionID == sectionID && x.SubSectionID == subSectionID);

                Mapper.Initialize(cfg =>
                {
                      cfg.CreateMap<DTOModel.DeductionSubSectionDescription, SelectListModel>()
                     .ForMember(d => d.id, o => o.MapFrom(s => s.DescriptionID))
                     .ForMember(d => d.value, o => o.MapFrom(s => s.Description))
                     .ForAllOtherMembers(d => d.Ignore());

                });
                return Mapper.Map<List<SelectListModel>>(dtoDescriptions);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Model.DeductionSection GetFormSection(int sectionID)
        {
            log.Info($"Form12BBService/GetFormSection/{sectionID}");
            try
            {
                var dtoFormSection = genericRepo.GetByID<DTOModel.DeductionSection>(sectionID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSection, Model.DeductionSection>();
                });
                return Mapper.Map<Model.DeductionSection>(dtoFormSection);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Model.DeductionSubSection GetFormSubSection(int section, int subSectionID)
        {
            log.Info($"Form12BBService/GetFormSubSection/{section}/{subSectionID}");
            try
            {
                var dtoFormSubSection = genericRepo.Get<DTOModel.DeductionSubSection>(x => x.SectionID == section && x.SubSectionID == subSectionID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSubSection, Model.DeductionSubSection>();
                });
                return Mapper.Map<Model.DeductionSubSection>(dtoFormSubSection);
            }
            catch (Exception)
            {

                throw;
            }

        }


        public Model.DeductionSubSectionDescription GetFormSubSecDesc(int sectionID, int subSectionID, int descriptionID)
        {
            log.Info($"Form12BBService/GetFormSubSecDesc/{sectionID}/{subSectionID}/{descriptionID}");
            try
            {
                var dtoSubSectionDesc = genericRepo.Get<DTOModel.DeductionSubSectionDescription>(x => !x.IsDeleted
                && x.SectionID == sectionID && x.SubSectionID == subSectionID && x.DescriptionID == descriptionID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSubSectionDescription, DeductionSubSectionDescription>();
                });
                return Mapper.Map<DeductionSubSectionDescription>(dtoSubSectionDesc);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region ==== Tax Deduction Form 12 BB ============

        public TaxDeductions GetDeductionSection(string fYear)
        {
            log.Info($"Form12BBService/GetDeductionSection/{fYear}");
            try
            {
                TaxDeductions deductionSections = new TaxDeductions();
                deductionSections.Fyear = fYear;

                var dtoSections = genericRepo.Get<DTOModel.DeductionSection>(x => !x.IsDeleted && x.FYear == fYear);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.DeductionSection, DeductionSection>();
                });
                deductionSections.sections = Mapper.Map<List<DeductionSection>>(dtoSections);

                if (dtoSections.Count() > 0)
                {
                    var sectionIDs = dtoSections.Select(x => x.SectionID).ToArray<int>();

                    var dtoSubSections = genericRepo.Get<DTOModel.DeductionSubSection>(x => !x.IsDeleted &&
                     sectionIDs.Any(y => y == x.SectionID));

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.DeductionSubSection, DeductionSubSection>();
                    });
                    deductionSections.subSections = Mapper.Map<List<DeductionSubSection>>(dtoSubSections);
                }

                if (deductionSections.subSections.Count > 0)
                {
                    var sectionIDs = dtoSections.Select(x => x.SectionID).ToArray<int>();
                    var subSectionIDs = deductionSections.subSections.Select(x => x.SubSectionID).ToArray<int>();

                    var dtoSubSectionDesc = genericRepo.Get<DTOModel.DeductionSubSectionDescription>(x => !x.IsDeleted
                     && sectionIDs.Any(y => y == x.SectionID) && subSectionIDs.Any(sub => sub == x.SubSectionID));

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.DeductionSubSectionDescription, DeductionSubSectionDescription>();
                    });
                    deductionSections.subSectionDescriptions = Mapper.Map<List<DeductionSubSectionDescription>>(dtoSubSectionDesc);
                }
                return deductionSections;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Form12BBInfo GetEmployeeForm12BB(int employeeID, string fYear)
        {
            log.Info($"Form12BBService/GetEmployeeForm12BB/{employeeID}/{fYear}");
            try
            {
                Form12BBInfo frm12BB = new Form12BBInfo();

                if (genericRepo.Exists<DTOModel.EmployeeForm12BB>(x => x.EmployeeID == employeeID && x.FYear == fYear))
                {
                    var dtoForm12BBInfo = genericRepo.Get<DTOModel.EmployeeForm12BB>(x => x.EmployeeID == employeeID
                    && x.FYear == fYear).FirstOrDefault();

                    if (dtoForm12BBInfo?.EmpFormID > 0)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<DTOModel.EmployeeForm12BB, Form12BBInfo>()
                            .ForMember(d => d.RentPaidToLandLord, o => o.MapFrom(s => s.RendPaidToLandLord))
                            .ForMember(d => d.PANNo, o => o.MapFrom(s => s.tblMstEmployee.PANNo))
                            .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                            .ForMember(d => d.FullName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                            .ForMember(d => d.FatherName, o => o.MapFrom(s => s.tblMstEmployee.HBName))
                            .ForMember(d => d.Designation, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName));
                        });

                        frm12BB = Mapper.Map<Form12BBInfo>(dtoForm12BBInfo);

                        if (dtoForm12BBInfo.EmpDeductionUnderChapterVI_A.Any(y => y.EmpFormID == frm12BB.EmpFormID))
                        {
                            #region    ===  Get Section Details ==== 

                            var sectionDtls = dtoForm12BBInfo.EmpDeductionUnderChapterVI_A.Where(x => x.EmpFormID == frm12BB.EmpFormID
                                && x.SubSectionID == null && !x.IsDeleted
                            );
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<DTOModel.EmpDeductionUnderChapterVI_A, DeductionSection>()
                                .ForMember(d => d.SectionName, o => o.MapFrom(s => s.DeductionSection.SectionName));
                            });
                            frm12BB.taxDeductions.sections = Mapper.Map<List<DeductionSection>>(sectionDtls);

                            #endregion

                            #region ====  Get Sub Section Details ==========

                            var subSectionDtls = dtoForm12BBInfo.EmpDeductionUnderChapterVI_A.Where(x =>
                            x.EmpFormID == frm12BB.EmpFormID && x.SubSectionID > 0
                               && x.SubSectionDescriptionID == null && !x.IsDeleted);

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<DTOModel.EmpDeductionUnderChapterVI_A, DeductionSubSection>()
                               .ForMember(d => d.SubSectionName, o => o.MapFrom(s => s.DeductionSection.DeductionSubSections
                                    .FirstOrDefault(y => y.SubSectionID == s.SubSectionID).SubSectionName));
                            });

                            frm12BB.taxDeductions.subSections = Mapper.Map<List<DeductionSubSection>>(subSectionDtls);

                            #endregion

                            #region ===== Get Sub Section Description Details =======

                            var subSectionDescDtls = dtoForm12BBInfo.EmpDeductionUnderChapterVI_A.Where(x =>
                             x.EmpFormID == frm12BB.EmpFormID && x.SubSectionID > 0
                              && x.SubSectionDescriptionID > 0 && !x.IsDeleted);

                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<DTOModel.EmpDeductionUnderChapterVI_A, DeductionSubSectionDescription>()
                                .ForMember(d => d.DescriptionID, o => o.MapFrom(s => s.SubSectionDescriptionID))
                                ;
                            });
                            frm12BB.taxDeductions.subSectionDescriptions = Mapper.Map<List<DeductionSubSectionDescription>>(subSectionDescDtls);

                            var subSecDes = genericRepo.GetIQueryable<DTOModel.DeductionSubSectionDescription>(x => !x.IsDeleted);

                            frm12BB.taxDeductions.subSectionDescriptions =
                                (from x in frm12BB.taxDeductions.subSectionDescriptions
                                 join y in subSecDes on
                                 x.DescriptionID equals y.DescriptionID
                                 select new DeductionSubSectionDescription()
                                 {
                                     Amount = x.Amount,
                                     SectionID = x.SectionID,
                                     SubSectionID = x.SubSectionID,
                                     DescriptionID = x.DescriptionID,
                                     Description = y.Description,
                                     CreatedBy = x.CreatedBy,
                                     CreatedOn = x.CreatedOn,
                                     IsDeleted = x.IsDeleted
                                 }).ToList();

                            #endregion

                        }
                        var dtoEmpForm12BBDocs = genericRepo.Get<DTOModel.EmployeeForm12BBDocument>(x => 
                           x.EmpFormID == frm12BB.EmpFormID);

                        Mapper.Initialize(cfg => {
                            cfg.CreateMap<DTOModel.EmployeeForm12BBDocument, EmployeeForm12BBDocument>();
                        });
                        var empForm12BBDocs = Mapper.Map<List<EmployeeForm12BBDocument>>(dtoEmpForm12BBDocs);

                        if(empForm12BBDocs?.Count>0)
                        {
                            frm12BB.HasHRADocument = empForm12BBDocs.Any(x => x.NatureOfClaim == "HRA");
                            frm12BB.HasDeductionOfInterestDoc = empForm12BBDocs.Any(x => x.NatureOfClaim == "DOI");
                            frm12BB.HasLTCDocument = empForm12BBDocs.Any(x => x.NatureOfClaim == "LTC");
                            frm12BB.HasChapter_VI_I_Document = empForm12BBDocs.Any(x => x.SectionID > 0);
                        }
                    }
                }
                else
                {
                    var empInfo = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.tblMstEmployee, Form12BBInfo>()

                        .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.PANNo, o => o.MapFrom(s => s.PANNo))
                        .ForMember(d => d.EmployeeAddress, o => o.MapFrom(s => s.PmtAdd))
                        .ForMember(d => d.FatherName, o => o.MapFrom(s => s.HBName))
                        .ForMember(d => d.Designation, o => o.MapFrom(s => s.Designation.DesignationName))
                        .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name))
                        .ForMember(d => d.FYear, o => o.UseValue(fYear))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    frm12BB = Mapper.Map<Form12BBInfo>(empInfo);
                }
                return frm12BB;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool PostForm12BB(Form12BBInfo frmForm12BB)
        {
            log.Info($"Form12BBService/PostForm12BB/");
            bool flag = false;
            try
            {
                if (frmForm12BB.FormState == 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Form12BBInfo, DTOModel.EmployeeForm12BB>()
                        .ForMember(d => d.RendPaidToLandLord, o => o.MapFrom(s => s.RentPaidToLandLord))
                        .ForMember(d => d.FormState, o => o.UseValue(1))
                        ;
                    });
                    var dtoForm12BB = Mapper.Map<DTOModel.EmployeeForm12BB>(frmForm12BB);

                    genericRepo.Insert<DTOModel.EmployeeForm12BB>(dtoForm12BB);

                    if (dtoForm12BB.EmpFormID > 0)
                    {
                        var empFormID = dtoForm12BB.EmpFormID;
                        if (frmForm12BB.taxDeductions != null)
                        {
                            List<DTOModel.EmpDeductionUnderChapterVI_A> empDeductions = new List<DTOModel.EmpDeductionUnderChapterVI_A>();

                            if (frmForm12BB.taxDeductions.sections.Count > 0)
                            {
                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<DeductionSection, DTOModel.EmpDeductionUnderChapterVI_A>()
                                    .ForMember(d => d.SectionID, o => o.MapFrom(s => s.SectionID))
                                    .ForMember(d => d.EmpFormID, o => o.UseValue(empFormID))
                                    .ForMember(d => d.FYear, o => o.UseValue(frmForm12BB.FYear))
                                    .ForMember(d => d.EmployeeID, o => o.UseValue(frmForm12BB.EmployeeID))
                                    .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                                     .ForMember(d => d.CreatedOn, o => o.UseValue(frmForm12BB.CreatedOn))
                                    .ForMember(d => d.CreatedBy, o => o.UseValue(frmForm12BB.CreatedBy))
                                    .ForAllOtherMembers(d => d.Ignore());
                                });
                                var dtoDeductionSection = Mapper.Map<List<DTOModel.EmpDeductionUnderChapterVI_A>>(frmForm12BB.taxDeductions.sections);

                                if (dtoDeductionSection?.Count > 0)
                                    empDeductions.AddRange(dtoDeductionSection);
                            }
                            if (frmForm12BB.taxDeductions.subSections.Count > 0)
                            {
                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<DeductionSubSection, DTOModel.EmpDeductionUnderChapterVI_A>()
                                    .ForMember(d => d.SectionID, o => o.MapFrom(s => s.SectionID))
                                    .ForMember(d => d.SubSectionID, o => o.MapFrom(s => s.SubSectionID))
                                    .ForMember(d => d.EmpFormID, o => o.UseValue(empFormID))
                                    .ForMember(d => d.FYear, o => o.UseValue(frmForm12BB.FYear))
                                    .ForMember(d => d.EmployeeID, o => o.UseValue(frmForm12BB.EmployeeID))
                                    .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                                    .ForMember(d => d.CreatedOn, o => o.UseValue(frmForm12BB.CreatedOn))
                                    .ForMember(d => d.CreatedBy, o => o.UseValue(frmForm12BB.CreatedBy))
                                    .ForAllOtherMembers(d => d.Ignore());
                                });
                                var dtoDeductionSubSection = Mapper.Map<List<DTOModel.EmpDeductionUnderChapterVI_A>>(frmForm12BB.taxDeductions.subSections);

                                if (dtoDeductionSubSection.Count > 0)
                                    empDeductions.AddRange(dtoDeductionSubSection);
                            }

                            if (frmForm12BB.taxDeductions.subSectionDescriptions.Count > 0)
                            {
                                Mapper.Initialize(cfg =>
                                {
                                    cfg.CreateMap<DeductionSubSectionDescription, DTOModel.EmpDeductionUnderChapterVI_A>()
                                   .ForMember(d => d.SectionID, o => o.MapFrom(s => s.SectionID))
                                   .ForMember(d => d.SubSectionID, o => o.MapFrom(s => s.SubSectionID))
                                   .ForMember(d => d.SubSectionDescriptionID, o => o.MapFrom(s => s.DescriptionID))
                                   .ForMember(d => d.EmpFormID, o => o.UseValue(empFormID))
                                   .ForMember(d => d.FYear, o => o.UseValue(frmForm12BB.FYear))
                                   .ForMember(d => d.EmployeeID, o => o.UseValue(frmForm12BB.EmployeeID))
                                   .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                                    .ForMember(d => d.CreatedOn, o => o.UseValue(frmForm12BB.CreatedOn))
                                   .ForMember(d => d.CreatedBy, o => o.UseValue(frmForm12BB.CreatedBy))
                                   .ForAllOtherMembers(d => d.Ignore());
                                });
                                var dtoDeductionSubSectionDesc = Mapper.Map<List<DTOModel.EmpDeductionUnderChapterVI_A>>(frmForm12BB.taxDeductions.subSectionDescriptions);

                                if (dtoDeductionSubSectionDesc.Count > 0)
                                    empDeductions.AddRange(dtoDeductionSubSectionDesc);
                            }
                            genericRepo.AddMultipleEntity<DTOModel.EmpDeductionUnderChapterVI_A>(empDeductions);
                            flag = true;
                        }
                    }
                }
                else
                {
                    ///===== Case of Form Data update ===
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Form12BBInfo, DTOModel.EmployeeForm12BB>()
                        .ForMember(d => d.RendPaidToLandLord, o => o.MapFrom(s => s.RentPaidToLandLord))
                        ;
                    });
                    var dtoForm12BB = Mapper.Map<DTOModel.EmployeeForm12BB>(frmForm12BB);
                    genericRepo.Update<DTOModel.EmployeeForm12BB>(dtoForm12BB);


                    #region  =====  Update Deduction sub child tables =========

                    if (genericRepo.Exists<DTOModel.EmpDeductionUnderChapterVI_A>(x =>
                        !x.IsDeleted && x.EmpFormID == frmForm12BB.EmpFormID))
                    {
                        var prevRows = genericRepo.Get<DTOModel.EmpDeductionUnderChapterVI_A>(x =>
                            !x.IsDeleted && x.EmpFormID == frmForm12BB.EmpFormID);

                        genericRepo.DeleteAll<DTOModel.EmpDeductionUnderChapterVI_A>(prevRows);
                    }
                    #endregion

                    #region  =====  Adding new definition of deduction sections=====

                    if (frmForm12BB.taxDeductions != null)
                    {
                        List<DTOModel.EmpDeductionUnderChapterVI_A> empDeductions = new List<DTOModel.EmpDeductionUnderChapterVI_A>();

                        if (frmForm12BB.taxDeductions.sections.Count > 0)
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<DeductionSection, DTOModel.EmpDeductionUnderChapterVI_A>()
                                .ForMember(d => d.SectionID, o => o.MapFrom(s => s.SectionID))
                                .ForMember(d => d.EmpFormID, o => o.UseValue(frmForm12BB.EmpFormID))
                                .ForMember(d => d.FYear, o => o.UseValue(frmForm12BB.FYear))
                                .ForMember(d => d.EmployeeID, o => o.UseValue(frmForm12BB.EmployeeID))
                                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                                 .ForMember(d => d.CreatedOn, o => o.UseValue(frmForm12BB.CreatedOn))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(frmForm12BB.CreatedBy))
                                .ForAllOtherMembers(d => d.Ignore());
                            });
                            var dtoDeductionSection = Mapper.Map<List<DTOModel.EmpDeductionUnderChapterVI_A>>(frmForm12BB.taxDeductions.sections);

                            if (dtoDeductionSection?.Count > 0)
                                empDeductions.AddRange(dtoDeductionSection);
                        }
                        if (frmForm12BB.taxDeductions.subSections.Count > 0)
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<DeductionSubSection, DTOModel.EmpDeductionUnderChapterVI_A>()
                                .ForMember(d => d.SectionID, o => o.MapFrom(s => s.SectionID))
                                .ForMember(d => d.SubSectionID, o => o.MapFrom(s => s.SubSectionID))
                                .ForMember(d => d.EmpFormID, o => o.UseValue(frmForm12BB.EmpFormID))
                                .ForMember(d => d.FYear, o => o.UseValue(frmForm12BB.FYear))
                                .ForMember(d => d.EmployeeID, o => o.UseValue(frmForm12BB.EmployeeID))
                                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(frmForm12BB.CreatedOn))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(frmForm12BB.CreatedBy))
                                .ForAllOtherMembers(d => d.Ignore());
                            });
                            var dtoDeductionSubSection = Mapper.Map<List<DTOModel.EmpDeductionUnderChapterVI_A>>(frmForm12BB.taxDeductions.subSections);

                            if (dtoDeductionSubSection.Count > 0)
                                empDeductions.AddRange(dtoDeductionSubSection);
                        }

                        if (frmForm12BB.taxDeductions.subSectionDescriptions.Count > 0)
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<DeductionSubSectionDescription, DTOModel.EmpDeductionUnderChapterVI_A>()
                               .ForMember(d => d.SectionID, o => o.MapFrom(s => s.SectionID))
                               .ForMember(d => d.SubSectionID, o => o.MapFrom(s => s.SubSectionID))
                               .ForMember(d => d.SubSectionDescriptionID, o => o.MapFrom(s => s.DescriptionID))
                               .ForMember(d => d.EmpFormID, o => o.UseValue(frmForm12BB.EmpFormID))
                               .ForMember(d => d.FYear, o => o.UseValue(frmForm12BB.FYear))
                               .ForMember(d => d.EmployeeID, o => o.UseValue(frmForm12BB.EmployeeID))
                               .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(frmForm12BB.CreatedOn))
                               .ForMember(d => d.CreatedBy, o => o.UseValue(frmForm12BB.CreatedBy))
                               .ForAllOtherMembers(d => d.Ignore());
                            });
                            var dtoDeductionSubSectionDesc = Mapper.Map<List<DTOModel.EmpDeductionUnderChapterVI_A>>(frmForm12BB.taxDeductions.subSectionDescriptions);

                            if (dtoDeductionSubSectionDesc.Count > 0)
                                empDeductions.AddRange(dtoDeductionSubSectionDesc);
                        }
                        genericRepo.AddMultipleEntity<DTOModel.EmpDeductionUnderChapterVI_A>(empDeductions);
                        flag = true;
                    }
                    //==== endd==============
                    #endregion

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

        public List<Form12BBInfo> GetForm12BBList(string fYear)
        {
            log.Info($"Form12BBService/GetForm12BBList/fYear:{fYear}");
            try
            {
                IEnumerable<DTOModel.EmployeeForm12BB> dtoForm12BBs = Enumerable.Empty<DTOModel.EmployeeForm12BB>();

                if (!string.IsNullOrEmpty(fYear))
                    dtoForm12BBs = genericRepo.Get<DTOModel.EmployeeForm12BB>(x =>!x.IsDeleted && x.FYear == fYear);
                else
                    dtoForm12BBs = genericRepo.Get<DTOModel.EmployeeForm12BB>(x => !x.IsDeleted);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeForm12BB, Model.Form12BBInfo>()
                    .ForMember(d => d.RentPaidToLandLord, o => o.MapFrom(s => s.RendPaidToLandLord));
                });
                return Mapper.Map<List<Model.Form12BBInfo>>(dtoForm12BBs);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Form12BBInfo> GetForm12BBList(int employeeID, string fYear)
        {
            log.Info($"Form12BBService/GetForm12BBList/employeeID:{employeeID}&fYear:{fYear}");

            try
            {
                IEnumerable<DTOModel.EmployeeForm12BB> dtoForm12BBs = Enumerable.Empty<DTOModel.EmployeeForm12BB>();

                if (!string.IsNullOrEmpty(fYear))
                    dtoForm12BBs = genericRepo.Get<DTOModel.EmployeeForm12BB>(x => x.EmployeeID == employeeID
                    && x.FYear == fYear);
                else
                    dtoForm12BBs = genericRepo.Get<DTOModel.EmployeeForm12BB>(x => x.EmployeeID == employeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeForm12BB, Model.Form12BBInfo>()
                    .ForMember(d => d.RentPaidToLandLord, o => o.MapFrom(s => s.RendPaidToLandLord))
                    ;
                });
                return Mapper.Map<List<Model.Form12BBInfo>>(dtoForm12BBs);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UploadForm12BBDocument(EmployeeForm12BBDocument empFrmDocument)
        {
            log.Info($"Form12BBService/UploadForm12BBDocument/EmpFormID:{empFrmDocument.EmpFormID}");
            bool flag = false;

            try
            {

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<EmployeeForm12BBDocument, DTOModel.EmployeeForm12BBDocument>()
                    .ForMember(d => d.NatureOfClaim, o => o.MapFrom(s => s.NatureOfClaim));
                });
                var dtoEmployeeFormDoc = Mapper.Map<DTOModel.EmployeeForm12BBDocument>(empFrmDocument);

                if(empFrmDocument.NatureOfClaim=="HRA")
                {
                    var lastSavedRecord = genericRepo.Get<DTOModel.EmployeeForm12BBDocument>(
                         x => x.EmpFormID == empFrmDocument.EmpFormID
                         && x.NatureOfClaim == "HRA");
                          genericRepo.DeleteAll<DTOModel.EmployeeForm12BBDocument>(lastSavedRecord);
                }
                  genericRepo.Insert<DTOModel.EmployeeForm12BBDocument>(dtoEmployeeFormDoc);

                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }

        public DataTable GetForm12BBDetails(string fYear)
        {
            log.Info($"Form12BBService/GetForm12BBList/fYear:{fYear}");
            try
            {               
                return _Form12BBRepository.GetForm12BBDetails(fYear);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
        }

        public IEnumerable<Form12BBDocumentList> GetForm12BBDocumentList(int EmpFormID)
        {
            log.Info($"Form12BBService/GetForm12BBDocumentList/EmpFormID:{EmpFormID}");
            try
            {
                var documentlist = _Form12BBDocumentRepository.GetForm12BBDocumentList(EmpFormID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetForm12BBDocumentList_Result, Form12BBDocumentList>()
                    .ForMember(d => d.Form12BBDocumentID, o => o.MapFrom(s => s.Form12BBDocumentID))
                    .ForMember(d => d.EmpFormID, o => o.MapFrom(s => s.EmpFormID))
                    .ForMember(d => d.NatureOfClaim, o => o.MapFrom(s => s.NatureOfClaim))
                    .ForMember(d => d.FileName, o => o.MapFrom(s => s.FileName))
                    .ForMember(d => d.FileDescription, o => o.MapFrom(s => s.FileDescription))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.SectionName, o => o.MapFrom(s => s.SectionName))
                    .ForMember(d => d.SubSectionName, o => o.MapFrom(s => s.SubSectionName))
                    .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                    ;

                });
                return Mapper.Map<List<Form12BBDocumentList>>(documentlist);


            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;

            }
        }

        public bool DeleteDocument(int form12BBDocumentID)
        {
            try
            {
                genericRepo.Delete<DTOModel.EmployeeForm12BBDocument>(form12BBDocumentID);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }
        }

        //public List<Form12BBInfo> GetForm12BBList(string fYear, int ? employeeID)
        //{
        //    log.Info($"Form12BBService/GetForm12BBList/fYear:{fYear}&employeeID:{employeeID}");

        //    try
        //    {
        //        var dtoEmpForm12BB = genericRepo.Get<DTOModel.EmployeeForm12BB>(
        //             x=>!x.IsDeleted && x.FYear==fYear
        //             && (employeeID.HasValue ? (x.EmployeeID== employeeID.Value) : (1>0))
        //            );

        //        Mapper.Initialize(cfg =>
        //        {
        //            cfg.CreateMap<DTOModel.EmployeeForm12BB, Model.Form12BBInfo>()
        //             .ForMember(d => d.RentPaidToLandLord, o => o.MapFrom(s => s.RendPaidToLandLord))
        //            ;
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }

        //}
        #endregion
    }
}
