using Nafed.MicroPay.Data.Repositories.IRepositories;
using System;
using System.Linq;
using Nafed.MicroPay.Model;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using System.Collections.Generic;
using static Nafed.MicroPay.Model.EnumHelper;
using System.Text;
using Nafed.MicroPay.Common;
using System.Threading.Tasks;
using System.Data;
using static Nafed.MicroPay.ImportExport.RecruitmentReportExport;
using System.IO;

namespace Nafed.MicroPay.Services.Recruitment
{
    public class RecruitmentService : BaseService, IRecruitmentService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IRecruitmentRepository recruitmentRepository;
        public RecruitmentService(IGenericRepository genericRepo, IRecruitmentRepository recruitmentRepository)
        {
            this.genericRepo = genericRepo;
            this.recruitmentRepository = recruitmentRepository;
        }

        public string CandidateRegistration(CandidateRegistration candidateForm)
        {
            log.Info($"RecruitmentService/CandidateRegistration");

            try
            {
                var previousTransctionDtls = genericRepo.Get<DTOModel.CandidateRegistration>()
                    .OrderByDescending(y => y.CreatedOn).Take(1).FirstOrDefault();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CandidateRegistration, DTOModel.CandidateRegistration>()
                    .ForMember(d => d.FormStatus, o => o.UseValue(1));
                });
                var dtoCandidateModel = Mapper.Map<DTOModel.CandidateRegistration>(candidateForm);
                //candidateForm.DesignationID = 40; candidateForm.RequirementID = 4;
                dtoCandidateModel.RegistrationNo = GenerateRegistrationNo(previousTransctionDtls?.RegistrationID ?? 0, candidateForm.DesignationID);

                genericRepo.Insert<DTOModel.CandidateRegistration>(dtoCandidateModel);

                if (dtoCandidateModel.RegistrationID > 0)
                {
                    candidateForm.RegistrationNo = dtoCandidateModel.RegistrationNo;
                    Task t1 = Task.Run(() => SendRegistrationConfirmationEmail(candidateForm));
                    Task sendSMS = Task.Run(() => SendRegistrationConfirmationSMS(candidateForm));

                    var JQualificationList = new List<CandidateEducationSummary>();
                    #region Add Education Summary Data 

                    if (candidateForm.JobRequirementQualification?.Count > 0)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.JobRequirementQualification, Model.CandidateEducationSummary>()
                            .ForMember(d => d.RegistrationID, o => o.UseValue(dtoCandidateModel.RegistrationID))
                            .ForMember(d => d.JSelectedQualificationID, o => o.MapFrom(s => s.SelectedQualificationID))
                            .ForMember(d => d.FromMonth, o => o.MapFrom(s => (int)s.enumFromMonth))
                            .ForMember(d => d.FromYear, o => o.MapFrom(s => (int)s.enumFromYear))
                            .ForMember(d => d.ToMonth, o => o.MapFrom(s => (int)s.enumToMonth))
                            .ForMember(d => d.ToYear, o => o.MapFrom(s => (int)s.enumToYear))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                            ;
                        });
                        JQualificationList = Mapper.Map<List<CandidateEducationSummary>>(candidateForm.JobRequirementQualification);

                    }
                    if (candidateForm.CandidateEducationSummary?.Count > 0 || JQualificationList?.Count > 0)
                    {

                        if (genericRepo.Exists<DTOModel.CandidateEducationSummary>(x => x.RegistrationID == dtoCandidateModel.RegistrationID))
                        {

                        }
                        else
                        {
                            var JEducationSummaryList = new List<CandidateEducationSummary>();
                            //  candidateForm.CandidateEducationSummary = JQualificationList;
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<CandidateEducationSummary, CandidateEducationSummary>()
                                .ForMember(d => d.RegistrationID, o => o.UseValue(dtoCandidateModel.RegistrationID))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                                .ForMember(d => d.JSelectedQualificationID, o => o.MapFrom(s => (int)s.OtherQualificationType))
                                .ForMember(d => d.FromMonth, o => o.MapFrom(s => (int)s.enumFromMonth))
                                .ForMember(d => d.FromYear, o => o.MapFrom(s => (int)s.enumFromYear))
                                .ForMember(d => d.ToMonth, o => o.MapFrom(s => (int)s.enumToMonth))
                                .ForMember(d => d.ToYear, o => o.MapFrom(s => (int)s.enumToYear))
                                ;
                            });
                            JEducationSummaryList = Mapper.Map<List<CandidateEducationSummary>>(candidateForm.CandidateEducationSummary);

                            if (JEducationSummaryList?.Count > 0)
                                JQualificationList.AddRange(JEducationSummaryList);


                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<CandidateEducationSummary, DTOModel.CandidateEducationSummary>()
                                .ForMember(d => d.RegistrationID, o => o.UseValue(dtoCandidateModel.RegistrationID))
                                .ForMember(d => d.FromMonth, o => o.MapFrom(s => s.FromMonth == 0 ? null : (int?)s.enumFromMonth))
                                .ForMember(d => d.FromYear, o => o.MapFrom(s => s.FromYear == 0 ? null : (int?)s.enumFromYear))
                                .ForMember(d => d.ToMonth, o => o.MapFrom(s => s.ToMonth == 0 ? null : (int?)s.enumToMonth))
                                .ForMember(d => d.ToYear, o => o.MapFrom(s => s.ToYear == 0 ? null : (int?)s.enumToYear))
                                ;
                            });
                            var dtoEducationSummary = Mapper.Map<List<DTOModel.CandidateEducationSummary>>(JQualificationList);


                            genericRepo.AddMultipleEntity<DTOModel.CandidateEducationSummary>(dtoEducationSummary);

                        }
                    }
                    #endregion

                    #region Add Work Experience for Private Sector

                    if (candidateForm.candidateWorkExperience?.Count > 0)
                    {
                        if (genericRepo.Exists<DTOModel.CandidateWorkExperience>(x => x.RegistrationID == dtoCandidateModel.RegistrationID))
                        {

                        }
                        else
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<CandidateWorkExperince, DTOModel.CandidateWorkExperience>()
                                .ForMember(d => d.RegistrationID, o => o.UseValue(dtoCandidateModel.RegistrationID))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                                .ForMember(d => d.CraetedOn, o => o.UseValue(DateTime.Now))
                                 .ForMember(d => d.OrganisationType, o => o.MapFrom(s => (int)s.OrganizationType));
                            });
                            var dtoWorkExperienceDtls = Mapper.Map<List<DTOModel.CandidateWorkExperience>>(candidateForm.candidateWorkExperience);
                            genericRepo.AddMultipleEntity<DTOModel.CandidateWorkExperience>(dtoWorkExperienceDtls);

                        }
                    }


                    #endregion

                    #region Add Work Experience for Govt Sector

                    if (candidateForm.candidateWorkExperience?.Count > 0)
                    {
                        candidateForm.candidateGovtWrkExp.ForEach(x =>
                        {
                            x.Natureofappointment = (int)x.enumNatureOfAppointment;
                        });


                        if (genericRepo.Exists<DTOModel.CandidateGovtWorkExperience>(x => x.RegistrationID == dtoCandidateModel.RegistrationID))
                        {

                        }
                        else
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<CandidateGovtWorkExperience, DTOModel.CandidateGovtWorkExperience>()
                                .ForMember(d => d.RegistrationID, o => o.UseValue(dtoCandidateModel.RegistrationID))
                                .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                                ;
                            });
                            var dtoGovtWorkExperience = Mapper.Map<List<DTOModel.CandidateGovtWorkExperience>>(candidateForm.candidateGovtWrkExp);
                            genericRepo.AddMultipleEntity<DTOModel.CandidateGovtWorkExperience>(dtoGovtWorkExperience);

                        }
                    }

                    #endregion

                    //candidateForm.RegistrationNo = dtoCandidateModel.RegistrationNo;

                    //Task t1 = Task.Run(() => SendRegistrationConfirmationEmail(candidateForm));

                    //Task sendSMS = Task.Run(() => SendRegistrationConfirmationSMS(candidateForm));

                    return dtoCandidateModel.RegistrationNo;
                }

                else
                    return null;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private string GenerateRegistrationNo(int previousRegID, int designationID)
        {
            try
            {
                return $"NAFED/{DateTime.Now.Year}/{designationID}/{previousRegID + 1}";
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.CandidateDetails> GetCandidateDetails(DateTime PublishDateFrom, DateTime PublishDateTo, int EligibleForWrittenExam, int? DesignationID, int? IssueAdmitCard)
        {
            log.Info($"RecruitmentService/GetCandidadteDetails");
            try
            {
                IEnumerable<Nafed.MicroPay.Data.Models.CandidateRegistration> result = Enumerable.Empty<Nafed.MicroPay.Data.Models.CandidateRegistration>();

                result = genericRepo.Get<Data.Models.CandidateRegistration>(x => x.FormStatus == 2 &&
                x.Requirement.PublishDate >= PublishDateFrom && x.Requirement.PublishDate <= PublishDateTo
                && (DesignationID.HasValue ? x.Requirement.DesinationID == DesignationID : 1 > 0));

                //result = genericRepo.Get<Data.Models.CandidateRegistration>(x => x.FormStatus == 2 && x.RequirementID==);

                if (IssueAdmitCard != 0)
                {
                    if (IssueAdmitCard == 1)
                    {
                        result = result.Where(x => x.EligibleForWrittenExam == true && x.IssueAdmitCard == true);
                    }
                    else if (IssueAdmitCard == 2)
                    {
                        result = result.Where(x => x.EligibleForWrittenExam == true && x.IssueAdmitCard == false);
                    }
                }

                if (EligibleForWrittenExam != 0)
                {
                    if (EligibleForWrittenExam == 1)
                    {
                        result = result.Where(x => x.EligibleForWrittenExam == true);
                    }
                    else if (EligibleForWrittenExam == 2)
                    {
                        result = result.Where(x => x.EligibleForWrittenExam == false || x.EligibleForWrittenExam == null);
                    }
                }


                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.CandidateRegistration, Model.CandidateDetails>()
                    .ForMember(d => d.RegistrationID, o => o.MapFrom(s => s.RegistrationID))
                    .ForMember(d => d.Post, o => o.MapFrom(s => s.Requirement.Post))
                    .ForMember(d => d.CandidateFullName, o => o.MapFrom(s => s.CandidateFullName))
                    .ForMember(d => d.GenderID, o => o.MapFrom(s => s.GenderID))
                    .ForMember(d => d.PersonalEmailID, o => o.MapFrom(s => s.PersonalEmailID))
                    .ForMember(d => d.EligibleForWrittenExam, o => o.MapFrom(s => s.EligibleForWrittenExam))
                    .ForMember(d => d.Age, o => o.MapFrom(s => s.Age))
                    .ForMember(d => d.IssueAdmitCard, o => o.MapFrom(s => s.IssueAdmitCard))
                    .ForMember(d => d.RegistrationNo, o => o.MapFrom(s => s.RegistrationNo))
                    .ForMember(d => d.CandidatePicture, o => o.MapFrom(s => s.CandidatePicture))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var lstcandidate = Mapper.Map<List<Model.CandidateDetails>>(result);
                return lstcandidate.OrderBy(x => x.RegistrationNo).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateCandidatedetails(List<Model.CandidateDetails> UpdateCandidate)
        {
            log.Info($"RecruitmentService/UpdateCandidatedetails");
            bool flag = false;
            try
            {
                for (int i = 0; i < UpdateCandidate.Count; i++)
                {
                    var dtoObj = genericRepo.GetByID<DTOModel.CandidateRegistration>(UpdateCandidate[i].RegistrationID);
                    dtoObj.EligibleForWrittenExam = UpdateCandidate[i].EligibleForWrittenExam;
                    dtoObj.EligiblityRemark = UpdateCandidate[i].EligiblityRemark;
                    dtoObj.UpdatedBy = UpdateCandidate[i].UpdatedBy;

                    dtoObj.UpdatedOn = DateTime.Now;
                    genericRepo.Update<DTOModel.CandidateRegistration>(dtoObj);
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

        public CandidateRegistration GetApplicationForm(int registrationID)
        {
            log.Info($"RecruitmentService/CandidateRegistration");

            try
            {
                var jobLocationName = string.Empty;
                var dtoCandidateForm = genericRepo.GetByID<DTOModel.CandidateRegistration>(registrationID);

                if (dtoCandidateForm.Requirement.JLocTypeId == (int)RequirementOptionType.Anywhereinindia)
                    jobLocationName =   /*"Anywhere in India"*/  RequirementOptionType.Anywhereinindia.GetDisplayName();

                else if (dtoCandidateForm.Requirement.JLocTypeId == (int)RequirementOptionType.SpecificZone)

                    jobLocationName = dtoCandidateForm.ZoneAppliedFor != null ? ((EnumZoneAppliedFor)dtoCandidateForm.ZoneAppliedFor).GetDisplayName() : string.Empty;

                else if (dtoCandidateForm.Requirement.JLocTypeId == (int)RequirementOptionType.SpecificBranch)
                    jobLocationName = dtoCandidateForm.Branch != null ? dtoCandidateForm.Branch.BranchName : string.Empty;

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.CandidateRegistration, CandidateRegistration>()
                    .ForMember(d => d.AppliedJobLocation, o => o.UseValue(jobLocationName))
                    .ForMember(d => d.PostName, o => o.MapFrom(s => s.Requirement.Post))
                    .ForMember(d => d.ReligionName, o => o.MapFrom(s => s.Religion.ReligionName))
                    .ForMember(d => d.MaritalStatus, o => o.MapFrom(s => s.MaritalStatu.MaritalStatusName))
                    .ForMember(d => d.GenderName, o => o.MapFrom(s => s.Gender.Name))
                    .ForMember(d => d.StateName, o => o.MapFrom(s => s.State.StateName))
                    .ForMember(d => d.CandidateEducationSummary, o => o.MapFrom(s => s.CandidateEducationSummaries))
                    .ForMember(d => d.candidateGovtWrkExp, o => o.MapFrom(s => s.CandidateGovtWorkExperiences))
                    .ForMember(d => d.candidateWorkExperience, o => o.MapFrom(s => s.CandidateWorkExperiences))
                    .ForMember(d => d.JobLocTypeID, o => o.MapFrom(s => s.Requirement.JLocTypeId))
                    .ForMember(d => d.IfYesStateNameAndRelationship, o => o.MapFrom(s => s.IfYesStateNameAndRelationship))
                    .ForMember(d => d.HaveYouEverAppliedinNafedBefore, o => o.MapFrom(s => s.HaveYouEverAppliedinNafedBefore))
                    .ForMember(d => d.FriendsAndRelativesInNafed, o => o.MapFrom(s => s.FriendsAndRelativesInNafed))
                    .ForMember(d => d.AppliedDate, o => o.MapFrom(s => s.AppliedDate))
                    .ForMember(d => d.ZoneAppliedFor, o => o.MapFrom(s => s.ZoneAppliedFor))
                    .ForMember(d => d.BranchID, o => o.MapFrom(s => s.BranchID))
                    .ForMember(d => d.PaymentType, o => o.MapFrom(s => s.PaymentType))
                     .ForMember(d => d.FeesApplicable, o => o.MapFrom(s => s.Requirement.FeesApplicable))
                     .ForMember(d => d.MimimumExpenrienceNo, o => o.MapFrom(s => s.Requirement.MimimumExpenrienceNo))
                    ;

                    cfg.CreateMap<DTOModel.CandidateGovtWorkExperience, CandidateGovtWorkExperience>();

                    cfg.CreateMap<DTOModel.CandidateEducationSummary, CandidateEducationSummary>()

                        .ForMember(d => d.Percentage_GradeSystem, o => o.MapFrom(s => s.Percentage_GradeSystem))
                        .ForMember(d => d.QualificationName, o => o.MapFrom(s => (s.JSelectedQualificationID < 10
                             ? ((Model.EnumReqQualification)s.JSelectedQualificationID).GetDisplayName() :
                             ((Model.OtherQualificationType)s.JSelectedQualificationID).GetDisplayName()
                             )))
                         .ForMember(d => d.Per_GradeTextBoxCss, o => o.MapFrom(s => s.Percentage_GradeSystem ? null : "hide"))
                         .ForMember(d => d.FromMonth, o => o.MapFrom(s => s.FromMonth == null ? 0 : s.FromMonth))
                         .ForMember(d => d.ToMonth, o => o.MapFrom(s => s.ToMonth == null ? 0 : s.ToMonth))
                         .ForMember(d => d.FromYear, o => o.MapFrom(s => s.FromYear == null ? 0 : s.FromYear))
                         .ForMember(d => d.ToYear, o => o.MapFrom(s => s.ToYear == null ? 0 : s.ToYear))
                         .ForMember(d => d.FromMonthName, o => o.MapFrom(s => s.FromMonth != null ? ((Model.EnumMonth)s.FromMonth).GetDisplayName() : "-"))
                         .ForMember(d => d.FromYearName, o => o.MapFrom(s => s.FromYear != null ? ((Model.EnumYear)s.FromYear).GetDisplayName() : "-"))
                         .ForMember(d => d.ToMonthName, o => o.MapFrom(s => s.ToMonth != null ? ((Model.EnumMonth)s.ToMonth).GetDisplayName() : "-"))
                         .ForMember(d => d.ToYearName, o => o.MapFrom(s => s.ToYear != null ? ((Model.EnumYear)s.ToYear).GetDisplayName() : "-"))
                                  ;
                    cfg.CreateMap<DTOModel.CandidateWorkExperience, CandidateWorkExperince>()
                    .ForMember(d => d.OrganisationTypeName, o => o.MapFrom(s => ((OrganizationType)s.OrganisationType).GetDisplayName()))
                    .ForMember(d => d.OrganisationType, o => o.MapFrom(s => ((OrganizationType)s.OrganisationType)));

                });
                var registrationFormData = Mapper.Map<CandidateRegistration>(dtoCandidateForm);



                return registrationFormData;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.Requirement> GetRequirementDetails(int? designationID, DateTime? publishDateFrom, DateTime? publishDateTo)
        {
            //var result = genericRepo.Get<DTOModel.Requirement>(x => !x.IsDeleted && (designationID.HasValue ? x.DesinationID == designationID.Value : 1 > 0)
            //&& (branchID.HasValue ? x.BranchID == branchID.Value : 1 > 0) && (PublishDate.HasValue ? x.PublishDate == PublishDate : 1 > 0));

            //publishDateTo = publishDateTo == null ? DateTime.Now : publishDateTo;

            var result = recruitmentRepository.GetRequirementDetails(designationID, publishDateFrom, publishDateTo);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.Requirement, Model.Requirement>()
                .ForMember(c => c.RequirementID, c => c.MapFrom(s => s.RequirementID))
                .ForMember(c => c.DesinationID, c => c.MapFrom(s => s.DesinationID))
                .ForMember(c => c.Post, c => c.MapFrom(s => s.Post))
                .ForMember(c => c.NoofPosition, c => c.MapFrom(s => s.NoofPosition))
                .ForMember(c => c.PublishDate, c => c.MapFrom(s => s.PublishDate))
                .ForMember(c => c.LastDateofApplicationReceived, c => c.MapFrom(s => s.LastDateofApplicationReceived))
                .ForMember(c => c.MimimumExpenrienceNo, c => c.MapFrom(s => s.MimimumExpenrienceNo))
                .ForMember(c => c.MaximumAgeLimit, c => c.MapFrom(s => s.MaximumAgeLimit))
                .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.Designation.DesignationName))
                .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
                .ForMember(c => c.LocationName, c => c.MapFrom(s => (s.JLocTypeId == (int)RequirementOptionType.SpecificBranch ? RequirementOptionType.SpecificBranch.GetDisplayName() : s.JLocTypeId == (int)RequirementOptionType.SpecificZone ? RequirementOptionType.SpecificZone.GetDisplayName() : RequirementOptionType.Anywhereinindia.GetDisplayName())))

               .ForAllOtherMembers(c => c.Ignore());
            });
            var listRequirement = Mapper.Map<List<Model.Requirement>>(result);
            return listRequirement;
        }

        public bool InsertRequirememnt(Model.Requirement requirement)
        {
            log.Info($"RecruitmentService/InsertRequirememnt");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Requirement, DTOModel.Requirement>()
                    .ForMember(c => c.DesinationID, c => c.MapFrom(m => m.DesinationID))
                    .ForMember(c => c.Post, c => c.MapFrom(m => m.Post))
                    .ForMember(c => c.NoofPosition, c => c.MapFrom(m => m.NoofPosition))
                    .ForMember(c => c.FeesApplicable, c => c.MapFrom(m => m.FeesApplicable))
                    .ForMember(c => c.ApplicationFees, c => c.MapFrom(m => m.ApplicationFees))
                    .ForMember(c => c.HowToPay, c => c.MapFrom(m => m.HowToPay))
                    .ForMember(c => c.PublishDate, c => c.MapFrom(m => m.PublishDate))
                    .ForMember(c => c.PayScale, c => c.MapFrom(m => m.PayScale))
                    .ForMember(c => c.LastDateofApplicationReceived, c => c.MapFrom(m => m.LastDateofApplicationReceived))
                    .ForMember(c => c.MinimumQualification, c => c.MapFrom(m => m.MinimumQualification))
                    .ForMember(c => c.MinimumExperinceDetail, c => c.MapFrom(m => m.MinimumExperinceDetail))
                    .ForMember(c => c.MimimumExpenrienceNo, c => c.MapFrom(m => m.MimimumExpenrienceNo))
                    .ForMember(c => c.DesiredKeySkills, c => c.MapFrom(m => m.DesiredKeySkills))
                    .ForMember(c => c.OptionalKeySkills, c => c.MapFrom(m => m.OptionalKeySkills))
                    .ForMember(c => c.Essential_Duties_and_Responsibilities, c => c.MapFrom(m => m.Essential_Duties_and_Responsibilities))
                    .ForMember(c => c.MaximumAgeLimit, c => c.MapFrom(m => Convert.ToInt32(m.MaximumAgeLimit)))
                    .ForMember(c => c.MaximumAgeLimitDesc, c => c.MapFrom(m => m.MaximumAgeLimitDesc))
                    .ForMember(c => c.MethodofRecruitment, c => c.MapFrom(m => m.MethodofRecruitment))
                    .ForMember(c => c.BranchID, c => c.MapFrom(m => m.BranchID))
                    .ForMember(c => c.INSTRUCTIONS, c => c.MapFrom(m => m.INSTRUCTIONS))
                    .ForMember(c => c.WrittenExamDate, c => c.MapFrom(m => m.WrittenExamDate))
                     .ForMember(c => c.GDExamDate, c => c.MapFrom(m => m.GDExamDate))
                     .ForMember(c => c.MinimumAgeLimits, c => c.MapFrom(m => m.MinimumAgeLimits))
                     .ForMember(c => c.Qualification, c => c.MapFrom(m => m.Qualification))
                     .ForMember(c => c.Employementtype, c => c.MapFrom(m => m.Employementtype))
                     .ForMember(c => c.JLocTypeId, c => c.MapFrom(m => m.JLocTypeId))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))

                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoRequirement = Mapper.Map<DTOModel.Requirement>(requirement);
                genericRepo.Insert<DTOModel.Requirement>(dtoRequirement);

                if (dtoRequirement.RequirementID > 0)
                {
                    List<DTOModel.JobRequirementLocation> dtoJobReqLocations = new List<DTOModel.JobRequirementLocation>();
                    List<DTOModel.JobRequirementQualification> dtoJobReqQualification = new List<DTOModel.JobRequirementQualification>();
                    if ((int)requirement.requirementOptionType < 3)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<int, DTOModel.JobRequirementLocation>()
                            .ForMember(d => (d.BranchId), o => o.MapFrom(s => (requirement.requirementOptionType == RequirementOptionType.SpecificBranch ? (int?)s : null)))
                            .ForMember(d => (d.ZoneId), o => o.MapFrom(s => (requirement.requirementOptionType == RequirementOptionType.SpecificZone ? (int?)s : null)))
                             .ForMember(d => (d.RequirementId), o => o.UseValue(dtoRequirement.RequirementID))
                              .ForMember(d => (d.LocTypeId), o => o.UseValue((int)requirement.requirementOptionType));
                        });

                        dtoJobReqLocations = Mapper.Map<List<DTOModel.JobRequirementLocation>>(requirement.jobLocations);
                    }
                    else
                        dtoJobReqLocations.Add(new DTOModel.JobRequirementLocation { RequirementId = dtoRequirement.RequirementID, LocTypeId = 3 });
                    genericRepo.AddMultipleEntity<DTOModel.JobRequirementLocation>(dtoJobReqLocations);


                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<int, DTOModel.JobRequirementQualification>()
                        .ForMember(d => (d.SelectedQualificationID), o => o.MapFrom(s => s))
                        .ForMember(d => (d.RequirementID), o => o.UseValue(dtoRequirement.RequirementID));
                    });
                    dtoJobReqQualification = Mapper.Map<List<DTOModel.JobRequirementQualification>>(requirement.JobQualification);
                    genericRepo.AddMultipleEntity<DTOModel.JobRequirementQualification>(dtoJobReqQualification);


                    if (requirement.JobExamCenterDetails != null)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.JobExamCenterDetails, DTOModel.ExamCenterDetail>()
                            .ForMember(d => d.RequirementID, o => o.UseValue(dtoRequirement.RequirementID))
                            .ForMember(d => d.LocTypeID, o => o.UseValue(requirement.JLocTypeId))
                            .ForMember(d => d.SelectedLocID, o => o.MapFrom(s => s.SelectedLocID))
                             .ForMember(d => d.ExamCentreAddress, o => o.MapFrom(s => s.ExamCentreAddress))
                               .ForMember(d => d.ReportingTime, o => o.MapFrom(s => s.ReportingTime))
                               .ForMember(d => d.EntryCloseTime, o => o.MapFrom(s => s.EntryCloseTime))
                                .ForMember(d => d.ExamTiming, o => o.MapFrom(s => s.ExamTiming))
                            ;
                        });
                        var dtoExamCenterDtls = Mapper.Map<List<DTOModel.ExamCenterDetail>>(requirement.JobExamCenterDetails);
                        genericRepo.AddMultipleEntity<DTOModel.ExamCenterDetail>(dtoExamCenterDtls);
                    }
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
        public Model.Requirement GetRequirementByID(int requirementId)
        {
            log.Info($"RecruitmentService/GetRequirementByID/{requirementId}");
            try
            {
                var requirementObj = genericRepo.GetByID<DTOModel.Requirement>(requirementId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Requirement, Model.Requirement>()
                    .ForMember(c => c.RequirementID, c => c.MapFrom(s => s.RequirementID))
                    .ForMember(c => c.DesinationID, c => c.MapFrom(s => s.DesinationID))
                    .ForMember(c => c.Post, c => c.MapFrom(s => s.Post))
                    .ForMember(c => c.NoofPosition, c => c.MapFrom(s => s.NoofPosition))
                    .ForMember(c => c.FeesApplicable, c => c.MapFrom(s => s.FeesApplicable))
                    .ForMember(c => c.ApplicationFees, c => c.MapFrom(s => s.ApplicationFees))
                    .ForMember(c => c.HowToPay, c => c.MapFrom(s => s.HowToPay))
                    .ForMember(c => c.PublishDate, c => c.MapFrom(s => s.PublishDate))
                    .ForMember(c => c.PayScale, c => c.MapFrom(s => s.PayScale))
                    .ForMember(c => c.LastDateofApplicationReceived, c => c.MapFrom(s => s.LastDateofApplicationReceived))
                    .ForMember(c => c.MinimumQualification, c => c.MapFrom(s => s.MinimumQualification))
                    .ForMember(c => c.MinimumExperinceDetail, c => c.MapFrom(s => s.MinimumExperinceDetail))
                    .ForMember(c => c.MimimumExpenrienceNo, c => c.MapFrom(s => s.MimimumExpenrienceNo))
                    .ForMember(c => c.DesiredKeySkills, c => c.MapFrom(s => s.DesiredKeySkills))
                    .ForMember(c => c.OptionalKeySkills, c => c.MapFrom(s => s.OptionalKeySkills))
                    .ForMember(c => c.Essential_Duties_and_Responsibilities, c => c.MapFrom(s => s.Essential_Duties_and_Responsibilities))
                    .ForMember(c => c.MaximumAgeLimit, c => c.MapFrom(s => s.MaximumAgeLimit))
                    .ForMember(c => c.MaximumAgeLimitDesc, c => c.MapFrom(s => s.MaximumAgeLimitDesc))
                    .ForMember(c => c.MethodofRecruitment, c => c.MapFrom(s => s.MethodofRecruitment))
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                    .ForMember(c => c.INSTRUCTIONS, c => c.MapFrom(s => s.INSTRUCTIONS))
                    .ForMember(c => c.WrittenExamDate, c => c.MapFrom(m => m.WrittenExamDate))
                     .ForMember(c => c.GDExamDate, c => c.MapFrom(m => m.GDExamDate))
                    .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.Designation.DesignationName))
                    .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
                    .ForMember(c => c.MinimumAgeLimits, c => c.MapFrom(s => s.MinimumAgeLimits))
                    .ForMember(c => c.Qualification, c => c.MapFrom(s => s.Qualification))
                    .ForMember(c => c.Employementtype, c => c.MapFrom(s => s.Employementtype))
                    .ForMember(c => c.employementName, c => c.MapFrom(s => (EmployementType)s.Employementtype))
                    .ForMember(c => c.JLocTypeId, c => c.MapFrom(s => s.JLocTypeId))
                     .ForMember(c => c.jobRequirementLocs, c => c.MapFrom(s => s.JobRequirementLocations))
                     .ForMember(c => c.JobRequirementQualification, c => c.MapFrom(s => s.JobRequirementQualifications))
                     .ForMember(c => c.JobExamCenterDetails, c => c.MapFrom(s => s.ExamCenterDetails))
                   .ForAllOtherMembers(c => c.Ignore());

                    cfg.CreateMap<DTOModel.JobRequirementLocation, Model.JobRequirementLocation>()
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => (s.Branch == null ? string.Empty : s.Branch.BranchName)))
                    .ForMember(d => d.ZoneName, o => o.MapFrom(s => (s.ZoneId == null ? string.Empty : ((EnumZoneAppliedFor)s.ZoneId).GetDisplayName())))
                    ;

                    cfg.CreateMap<DTOModel.JobRequirementQualification, Model.JobRequirementQualification>()
                   .ForMember(d => d.RequirementID, o => o.MapFrom(s => (s.RequirementID)))
                   .ForMember(d => d.SelectedQualificationID, o => o.MapFrom(s => (s.SelectedQualificationID)))
                   .ForMember(d => d.Qualification, o => o.MapFrom(s => ((EnumReqQualification)(s.SelectedQualificationID)).GetDisplayName()))
                   ;

                    cfg.CreateMap<DTOModel.ExamCenterDetail, Model.JobExamCenterDetails>()
                   .ForMember(d => d.RequirementID, o => o.MapFrom(s => (s.RequirementID)))
                   .ForMember(d => d.LocTypeID, o => o.MapFrom(s => s.LocTypeID))
                   .ForMember(d => d.SelectedLocID, o => o.MapFrom(s => s.SelectedLocID))
                   .ForMember(d => d.ExamCentreAddress, o => o.MapFrom(s => s.ExamCentreAddress))
                   .ForMember(d => d.ReportingTime, o => o.MapFrom(s => s.ReportingTime))
                   .ForMember(d => d.EntryCloseTime, o => o.MapFrom(s => s.EntryCloseTime))
                   .ForMember(d => d.ExamTiming, o => o.MapFrom(s => s.ExamTiming));


                });
                var obj = Mapper.Map<DTOModel.Requirement, Model.Requirement>(requirementObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateRequirememnt(Model.Requirement requirement)
        {
            log.Info($"RecruitmentService/UpdateRequirememnt");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.Get<DTOModel.Requirement>(x => x.RequirementID == requirement.RequirementID).FirstOrDefault();
                if (dtoObj != null)
                {
                    dtoObj.DesinationID = requirement.DesinationID;
                    dtoObj.Post = requirement.Post;
                    dtoObj.NoofPosition = requirement.NoofPosition;
                    dtoObj.FeesApplicable = requirement.FeesApplicable;
                    dtoObj.ApplicationFees = requirement.ApplicationFees;
                    dtoObj.HowToPay = requirement.HowToPay;
                    dtoObj.PublishDate = requirement.PublishDate.Value;
                    dtoObj.PayScale = requirement.PayScale;
                    dtoObj.LastDateofApplicationReceived = requirement.LastDateofApplicationReceived.Value;
                    dtoObj.MinimumQualification = requirement.MinimumQualification;
                    dtoObj.MinimumExperinceDetail = requirement.MinimumExperinceDetail;
                    dtoObj.MimimumExpenrienceNo = requirement.MimimumExpenrienceNo;
                    dtoObj.DesiredKeySkills = requirement.DesiredKeySkills;
                    dtoObj.OptionalKeySkills = requirement.OptionalKeySkills;
                    dtoObj.Essential_Duties_and_Responsibilities = requirement.Essential_Duties_and_Responsibilities;
                    dtoObj.MaximumAgeLimit = Convert.ToInt32(requirement.MaximumAgeLimit.Value);
                    dtoObj.MaximumAgeLimitDesc = requirement.MaximumAgeLimitDesc;
                    dtoObj.MethodofRecruitment = requirement.MethodofRecruitment;
                    dtoObj.BranchID = requirement.BranchID;
                    dtoObj.INSTRUCTIONS = requirement.INSTRUCTIONS;
                    dtoObj.WrittenExamDate = requirement.WrittenExamDate;
                    dtoObj.GDExamDate = requirement.GDExamDate;
                    dtoObj.Employementtype = requirement.Employementtype;
                    dtoObj.MinimumAgeLimits = Convert.ToInt32(requirement.MinimumAgeLimits);
                    dtoObj.Qualification = requirement.Qualification;
                    dtoObj.JLocTypeId = requirement.JLocTypeId;
                    dtoObj.UpdatedBy = requirement.UpdatedBy;
                    dtoObj.UpdatedOn = requirement.UpdatedOn;
                    genericRepo.Update<DTOModel.Requirement>(dtoObj);


                    recruitmentRepository.RemoveJobRequirementLocation(requirement.RequirementID);

                    List<DTOModel.JobRequirementLocation> dtoJobReqLocations = new List<DTOModel.JobRequirementLocation>();
                    if ((int)requirement.requirementOptionType < 3)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<int, DTOModel.JobRequirementLocation>()
                            .ForMember(d => (d.BranchId), o => o.MapFrom(s => (requirement.requirementOptionType == RequirementOptionType.SpecificBranch ? (int?)s : null)))
                            .ForMember(d => (d.ZoneId), o => o.MapFrom(s => (requirement.requirementOptionType == RequirementOptionType.SpecificZone ? (int?)s : null)))
                             .ForMember(d => (d.RequirementId), o => o.UseValue(requirement.RequirementID))
                              .ForMember(d => (d.LocTypeId), o => o.UseValue((int)requirement.requirementOptionType));
                        });
                        dtoJobReqLocations = Mapper.Map<List<DTOModel.JobRequirementLocation>>(requirement.jobLocations);
                    }
                    else
                        dtoJobReqLocations.Add(new DTOModel.JobRequirementLocation { RequirementId = requirement.RequirementID, LocTypeId = 3 });
                    genericRepo.AddMultipleEntity<DTOModel.JobRequirementLocation>(dtoJobReqLocations);

                    recruitmentRepository.RemoveJobRequirementQualification(requirement.RequirementID);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<int, DTOModel.JobRequirementQualification>()
                        .ForMember(d => (d.SelectedQualificationID), o => o.MapFrom(s => s))
                        .ForMember(d => (d.RequirementID), o => o.UseValue(requirement.RequirementID));
                    });
                    var addJobQualificationLocations = Mapper.Map<List<DTOModel.JobRequirementQualification>>(requirement.JobQualification);
                    genericRepo.AddMultipleEntity<DTOModel.JobRequirementQualification>(addJobQualificationLocations);

                    recruitmentRepository.RemoveJobExamDetailCenter(requirement.RequirementID);

                    if (requirement.JobExamCenterDetails != null)
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.JobExamCenterDetails, DTOModel.ExamCenterDetail>()
                            .ForMember(d => d.RequirementID, o => o.UseValue(requirement.RequirementID))
                            .ForMember(d => d.LocTypeID, o => o.UseValue(requirement.JLocTypeId))
                            .ForMember(d => d.SelectedLocID, o => o.MapFrom(s => s.SelectedLocID))
                             .ForMember(d => d.ExamCentreAddress, o => o.MapFrom(s => s.ExamCentreAddress))
                               .ForMember(d => d.ReportingTime, o => o.MapFrom(s => s.ReportingTime))
                               .ForMember(d => d.EntryCloseTime, o => o.MapFrom(s => s.EntryCloseTime))
                                .ForMember(d => d.ExamTiming, o => o.MapFrom(s => s.ExamTiming))
                            ;
                        });
                        var dtoExamCenterDtls = Mapper.Map<List<DTOModel.ExamCenterDetail>>(requirement.JobExamCenterDetails);
                        genericRepo.AddMultipleEntity<DTOModel.ExamCenterDetail>(dtoExamCenterDtls);
                    }
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

        public bool Delete(int requirementID)
        {
            log.Info($"RecruitmentService/UpdateRequirememnt");
            bool flag = false;
            try
            {
                var dto = genericRepo.GetByID<DTOModel.Requirement>(requirementID);
                if (dto != null)
                {
                    dto.IsDeleted = true;
                    genericRepo.Update<DTOModel.Requirement>(dto);
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

        public bool RequirementAlreadyExists(int requirementId, int designationId, DateTime? publishDate)
        {
            try
            {
                log.Info($"RecruitmentService/RequirementAlreadyExists/");
                return recruitmentRepository.RequirementAlreadyExists(requirementId, designationId, publishDate);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.Requirement> GetVacanciesList()
        {
            var result = recruitmentRepository.GetVacanciesList();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.Requirement, Model.Requirement>()
                .ForMember(c => c.RequirementID, c => c.MapFrom(s => s.RequirementID))
                .ForMember(c => c.DesinationID, c => c.MapFrom(s => s.DesinationID))
                .ForMember(c => c.Post, c => c.MapFrom(s => s.Post))
                .ForMember(c => c.NoofPosition, c => c.MapFrom(s => s.NoofPosition))
                .ForMember(c => c.PublishDate, c => c.MapFrom(s => s.PublishDate))
                .ForMember(c => c.LastDateofApplicationReceived, c => c.MapFrom(s => s.LastDateofApplicationReceived))
                .ForMember(c => c.MimimumExpenrienceNo, c => c.MapFrom(s => s.MimimumExpenrienceNo))
                .ForMember(c => c.MaximumAgeLimit, c => c.MapFrom(s => s.MaximumAgeLimit))
                .ForMember(c => c.DesignationName, c => c.MapFrom(s => s.Designation.DesignationName))
                .ForMember(c => c.BranchName, c => c.MapFrom(s => s.Branch.BranchName))
               .ForAllOtherMembers(c => c.Ignore());
            });
            var listRequirement = Mapper.Map<List<Model.Requirement>>(result);
            return listRequirement;
        }

        public IEnumerable<SelectListModel> GetFilterFields(int filter)
        {
            log.Info($"Recruitment/GetFilterFields");

            IEnumerable<SelectListModel> fields = Enumerable.Empty<SelectListModel>();

            try
            {
                if (filter == 1)
                {
                    fields = genericRepo.Get<DTOModel.Branch>(x => !x.IsDeleted)
                        .Select(x => new SelectListModel { id = x.BranchID, value = x.BranchName }).ToList().OrderBy(x => x.value);
                }
                else if (filter == 2)
                {
                    fields = from EnumZoneAppliedFor e in Enum.GetValues(typeof(EnumZoneAppliedFor))
                             select new SelectListModel
                             {
                                 id = (int)e,
                                 value = e.GetDisplayName()
                             };
                }
                return fields;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListModel> GetLocRequirement(int jLocTypeId, int requirementId)
        {
            log.Info($"Recruitment/GetFilterFields");
            try
            {
                var list = genericRepo.Get<DTOModel.JobRequirementLocation>(x => x.RequirementId == requirementId && x.LocTypeId == jLocTypeId);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.JobRequirementLocation, Model.SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => jLocTypeId == 1 ? s.BranchId : s.ZoneId))
                    .ForMember(d => d.value, o => o.MapFrom(s => jLocTypeId == 1 ? s.Branch.BranchName :
                    ((EnumZoneAppliedFor)s.ZoneId).ToString()));

                });
                return Mapper.Map<List<SelectListModel>>(list);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<SelectListModel> GetJobLocation(int requirementId)
        {
            log.Info($"Recruitment/GetJobLocation");
            try
            {
                var list = genericRepo.Get<DTOModel.JobRequirementLocation>(x => x.RequirementId == requirementId);

                if (list != null)
                {
                    var jLocTypeId = list.FirstOrDefault().LocTypeId;


                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.JobRequirementLocation, Model.SelectListModel>()
                        .ForMember(d => d.id, o => o.MapFrom(s => jLocTypeId == 1 ? s.BranchId : s.ZoneId))
                        .ForMember(d => d.value, o => o.MapFrom(s => jLocTypeId == 1 ? s.Branch.BranchName :
                        jLocTypeId == 2 ? ((EnumZoneAppliedFor)s.ZoneId).GetDisplayName() : "Anywhere In India"
                        ));

                    });
                    return Mapper.Map<List<SelectListModel>>(list);
                }
                else
                {
                    return new List<SelectListModel>();
                }
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        //public IEnumerable<SelectListModel> GetQualificationList()
        //{
        //    log.Info($"Recruitment/GetQualificationList");
        //    IEnumerable<SelectListModel> fields = Enumerable.Empty<SelectListModel>();
        //    try
        //    {
        //        fields = from EnumQualification e in Enum.GetValues(typeof(EnumQualification))
        //                 select new SelectListModel
        //                 {
        //                     id = (int)e,
        //                     value = e.ToString()
        //                 };
        //        return fields;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
        //        throw ex;
        //    }
        //}

        public List<SelectListModel> GetJobQualification(int requirementId)
        {
            log.Info($"Recruitment/GetJobQualification");
            try
            {
                List<DTOModel.JobRequirementQualification> list = new List<DTOModel.JobRequirementQualification>();
                list = genericRepo.Get<DTOModel.JobRequirementQualification>(x => x.RequirementID == requirementId).ToList();
                if (list.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.JobRequirementQualification, Model.SelectListModel>()
                        .ForMember(d => d.id, o => o.MapFrom(s => s.SelectedQualificationID));

                    });
                    return Mapper.Map<List<SelectListModel>>(list);
                }
                else
                    return new List<SelectListModel>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool ValidateUser(CandidateLogin userCredential, out string sAuthenticationMessage, out CandidateRegistration candidateForm)
        {
            log.Info("CandidateloginService/ValidateUser");

            sAuthenticationMessage = string.Empty;
            string sUserName = userCredential.RegistrationNo;
            DateTime sPassword = Convert.ToDateTime(userCredential.DOB);
            string dbPassword = string.Empty;
            bool isAuthenticated = false;
            bool isExists = false;
            candidateForm = new Model.CandidateRegistration();

            try
            {
                isExists = recruitmentRepository.VerifyUser(sUserName, sPassword);

                if (isExists)
                {

                    var getRegistration = genericRepo.Get<DTOModel.CandidateRegistration>(x => x.RegistrationNo == sUserName).FirstOrDefault();

                    if (candidateForm != null)
                    {
                        candidateForm.RegistrationID = getRegistration.RegistrationID;
                        candidateForm.FormStatus = getRegistration.FormStatus;
                    }

                    sAuthenticationMessage = "User Validated";
                    isAuthenticated = true;

                }
                else
                {
                    isAuthenticated = false;
                    sAuthenticationMessage = "Invalid Credentials";
                }
            }
            catch (System.Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return isAuthenticated;
        }

        public void SendRegistrationConfirmationEmail(CandidateRegistration registrationForm)
        {
            log.Info($"Recruitment/SendRegistrationConfirmationEmail/");

            try
            {
                EmailMessage emailMessage = new EmailMessage();
                StringBuilder mailBody = new StringBuilder();
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();

                Mapper.Initialize(cfg =>
                {

                    cfg.CreateMap<DTOModel.EmailConfiguration, EmailMessage>()
                    .ForMember(d => d.From, o => o.MapFrom(s => $"NAFED HRMS <{s.ToEmail}>"))
                    .ForMember(d => d.To, o => o.UseValue(registrationForm.PersonalEmailID))
                    .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                    .ForMember(d => d.Password, o => o.MapFrom(s => s.Password))
                    .ForMember(d => d.SmtpClientHost, o => o.MapFrom(s => s.Server))
                    .ForMember(d => d.SmtpPort, o => o.MapFrom(s => s.Port))
                    .ForMember(d => d.enableSSL, o => o.MapFrom(s => s.SSLStatus))
                    .ForMember(d => d.HTMLView, o => o.UseValue(true))
                    .ForMember(d => d.FriendlyName, o => o.UseValue("NAFED"));
                });

                emailMessage = Mapper.Map<EmailMessage>(emailsetting);

                if (registrationForm.FormStatus == 2)
                {

                    emailMessage.Subject = $"Thank you for applying to the {registrationForm.PostName} position at NAFED";
                    mailBody.Clear();
                    mailBody.AppendFormat("<div>Dear Candidate,</div> <br> <br>");
                    mailBody.AppendFormat($"<div>We would like to inform you that we received your application for the {registrationForm.PostName}. Our hiring team is currently reviewing all the applications and If you are among qualified candidate then you will receive an email for written exam with your admit card. In any case, we will keep you posted on the status of your application.</div> <br> <br>");
                    mailBody.AppendFormat($"<div>For further communication please use below Registration No.</div> <br>");
                    mailBody.AppendFormat($"<div>Registration No.- <b>{registrationForm.RegistrationNo}</b> </div> <br> <br>");
                    mailBody.AppendFormat($"<div>Regards, </div> <br>");
                    mailBody.AppendFormat($"<div>Recruitment Team,  </div> <br>");
                    mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                }
                else
                {
                    emailMessage.Subject = "Candidate Registration Confirmation.";
                    mailBody.AppendFormat("<div>Dear Candidate,</div> <br> <br>");
                    mailBody.AppendFormat($"<div>This is to inform you that your Application Form for the post <b>{registrationForm.PostName}</b> has been submitted successfully and your Registration No. is <b>{registrationForm.RegistrationNo}</b></div> <br> <br>");
                    mailBody.AppendFormat($"<div>You will get the intimation shortly if your candidature will be found as per the desired criteria. </div> <br> <br>");
                    mailBody.AppendFormat($"<div>Please visit Nafed website for more details regarding the Organization. </div> <br> <br>");
                    mailBody.AppendFormat($"<div>Regards, </div> <br>");
                    mailBody.AppendFormat($"<div>Recruitment Team,  </div> <br>");
                    mailBody.AppendFormat($"<div>Nafed  </div> <br> <br>");
                }
                ///===== Read Email Configuration Setting and set its property value to send email ===




                emailMessage.Body = mailBody.ToString();
                EmailHelper.SendEmail(emailMessage);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public void SendRegistrationConfirmationSMS(CandidateRegistration registrationForm)
        {
            try
            {
                SMSConfiguration smssetting = new SMSConfiguration();
                var smssettings = genericRepo.Get<DTOModel.SMSConfiguration>().FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SMSConfiguration, Model.SMSConfiguration>();

                });
                smssetting = Mapper.Map<Model.SMSConfiguration>(smssettings);

                string msgRecepient = registrationForm.MobileNo.Length == 10 ? "91" + registrationForm.MobileNo : registrationForm.MobileNo;
                var message = $"Dear Candidate, Application Form for the post {registrationForm.PostName} has been submitted successfully,Your Registration No. is {registrationForm.RegistrationNo}";
                SMSParameter sms = new SMSParameter();
                sms.MobileNo = msgRecepient;
                sms.Message = message;
                sms.URL = smssetting.SMSUrl;
                sms.User = smssetting.UserName;
                sms.Password = smssetting.Password;
                SMSHelper.SendSMS(sms);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
            }
        }

        #region Send Mail
        public bool CandidateSendMail(CandidateDetails Candidate, string mailType, string AdmitCard)
        {
            log.Info($"RecruitmentService/CandidateSendMail");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                if (!String.IsNullOrEmpty(Candidate.PersonalEmailID))
                {
                    emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:10pt;'> <p>Dear Candidate,</p>"
                     + "<p>Your candidature has been shortlisted for written test. </p>"
                      + "<p>Please find attached your Admit card for the same. </p>"
                   + "<p>Please read the instructions mentioned on the Admit card carefully. </p>"
                    + "<p>Recruitment Team,</p>"
                  + "<p>Nafed</p></div>");
                }
                if (!String.IsNullOrEmpty(Candidate.PersonalEmailID))
                {
                    EmailConfiguration emailsetting = new EmailConfiguration();
                    var emailsettings = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.EmailConfiguration, Model.EmailConfiguration>();

                    });
                    emailsetting = Mapper.Map<Model.EmailConfiguration>(emailsettings);

                    EmailMessage message = new EmailMessage();
                    message.To = Candidate.PersonalEmailID;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED - Candidate Registration";
                    Task t1 = Task.Run(() => GetEmailConfiguration(message.To, message.Body, message.Subject, AdmitCard));
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

        private void GetEmailConfiguration(string ToEmailID, string mailbody, string subject, string AdmitCard)
        {
            try
            {

                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                EmailMessage message = new EmailMessage();
                message.To = ToEmailID;
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                message.Subject = subject;
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                message.Body = mailbody;
                message.HTMLView = true;


                message.Attachments = new List<MailAttachment>() { new MailAttachment { FileName = AdmitCard, Content = null } };
                message.FriendlyName = "NAFED";
                EmailHelper.SendEmail(message);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public CandidateDetails GetCandidateDetails(int registrationID)
        {
            log.Info($"RecruitmentService/CandidateDetails");
            try
            {
                var getcandidatedetails = genericRepo.Get<DTOModel.CandidateRegistration>(em => em.RegistrationID == registrationID).FirstOrDefault();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.CandidateRegistration, Model.CandidateDetails>()
                         .ForMember(d => d.RegistrationNo, o => o.MapFrom(s => s.RegistrationNo))
                     .ForMember(d => d.CandidateFullName, o => o.MapFrom(s => s.CandidateFullName))
                      .ForMember(d => d.PersonalEmailID, o => o.MapFrom(s => s.PersonalEmailID))
                     .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<Model.CandidateDetails>(getcandidatedetails);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        public List<JobExamCenterDetails> GetExamCenterDetails(int requirementId)
        {
            var result = genericRepo.Get<DTOModel.ExamCenterDetail>(x => x.RequirementID == requirementId);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DTOModel.ExamCenterDetail, Model.JobExamCenterDetails>()
                .ForMember(c => c.RequirementID, c => c.MapFrom(s => s.RequirementID))
                .ForMember(c => c.ExamCentreAddress, c => c.MapFrom(s => s.ExamCentreAddress))
                .ForMember(c => c.SelectedLocID, c => c.MapFrom(s => s.SelectedLocID))
                .ForMember(c => c.LocTypeID, c => c.MapFrom(s => s.LocTypeID))
                .ForMember(c => c.ReportingTime, c => c.MapFrom(s => s.ReportingTime))
                .ForMember(c => c.EntryCloseTime, c => c.MapFrom(s => s.EntryCloseTime))
                .ForMember(c => c.ExamTiming, c => c.MapFrom(s => s.ExamTiming))
               .ForAllOtherMembers(c => c.Ignore());
            });
            var listExamCenter = Mapper.Map<List<Model.JobExamCenterDetails>>(result);
            return listExamCenter;
        }

        public bool UpdateCandidateRegistration(CandidateRegistration candidateForm)
        {
            log.Info($"RecruitmentService/UpdateCandidateRegistration");
            bool flag = false;

            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.CandidateRegistration, DTOModel.CandidateRegistration>();
                });

                var dtoCandidateHdr = Mapper.Map<DTOModel.CandidateRegistration>(candidateForm);
                genericRepo.Update<DTOModel.CandidateRegistration>(dtoCandidateHdr);

                if (candidateForm.FormStatus == 2)
                {

                    Task t1 = Task.Run(() => SendRegistrationConfirmationEmail(candidateForm));

                }
                var dtoCandidateEduSummary = genericRepo.Get<DTOModel.CandidateEducationSummary>(x => x.RegistrationID == candidateForm.RegistrationID).ToList();

                genericRepo.DeleteAll<DTOModel.CandidateEducationSummary>(dtoCandidateEduSummary);

                var JQualificationList = new List<CandidateEducationSummary>();


                #region Add Education Summary Data 

                if (candidateForm.JobRequirementQualification?.Count > 0)
                {




                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.JobRequirementQualification, Model.CandidateEducationSummary>()
                        .ForMember(d => d.RegistrationID, o => o.UseValue(candidateForm.RegistrationID))
                        .ForMember(d => d.JSelectedQualificationID, o => o.MapFrom(s => s.SelectedQualificationID))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForMember(d => d.FromMonth, o => o.MapFrom(s => (int?)s.enumFromMonth))
                        .ForMember(d => d.FromYear, o => o.MapFrom(s => (int?)s.enumFromYear))
                        .ForMember(d => d.ToMonth, o => o.MapFrom(s => (int?)s.enumToMonth))
                        .ForMember(d => d.ToYear, o => o.MapFrom(s => (int?)s.enumToYear))
                        ;
                    });
                    JQualificationList = Mapper.Map<List<CandidateEducationSummary>>(candidateForm.JobRequirementQualification);
                }

                if (candidateForm.CandidateEducationSummary?.Count > 0 || JQualificationList?.Count > 0)
                {
                    if (genericRepo.Exists<DTOModel.CandidateEducationSummary>(x => x.RegistrationID == candidateForm.RegistrationID))
                    {

                    }
                    else
                    {
                        var JEducationSummaryList = new List<CandidateEducationSummary>();
                        //  candidateForm.CandidateEducationSummary = JQualificationList;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CandidateEducationSummary, CandidateEducationSummary>()
                            .ForMember(d => d.RegistrationID, o => o.UseValue(candidateForm.RegistrationID))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                            .ForMember(d => d.JSelectedQualificationID, o => o.MapFrom(s => (int)s.OtherQualificationType))
                            .ForMember(d => d.FromMonth, o => o.MapFrom(s => (int)s.enumFromMonth))
                            .ForMember(d => d.FromYear, o => o.MapFrom(s => (int)s.enumFromYear))
                            .ForMember(d => d.ToMonth, o => o.MapFrom(s => (int)s.enumToMonth))
                            .ForMember(d => d.ToYear, o => o.MapFrom(s => (int)s.enumToYear))
                            ;
                        });
                        JEducationSummaryList = Mapper.Map<List<CandidateEducationSummary>>(candidateForm.CandidateEducationSummary);

                        if (JEducationSummaryList?.Count > 0)
                            JQualificationList.AddRange(JEducationSummaryList);


                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CandidateEducationSummary, DTOModel.CandidateEducationSummary>()
                            .ForMember(d => d.RegistrationID, o => o.UseValue(candidateForm.RegistrationID))
                            .ForMember(d => d.Percentage, o => o.MapFrom(s => (s.Percentage.HasValue ? Convert.ToInt32(s.Percentage) : (decimal?)null)))
                            //.ForMember(d => d.FromMonth, o => o.MapFrom(s => s.FromMonth == 0 ? null : (int?)s.enumFromMonth))
                            //.ForMember(d => d.FromYear, o => o.MapFrom(s => s.FromYear == 0 ? null : (int?)s.enumFromYear))
                            //.ForMember(d => d.ToMonth, o => o.MapFrom(s => s.ToMonth == 0 ? null : (int?)s.enumToMonth))
                            //.ForMember(d => d.ToYear, o => o.MapFrom(s => s.ToMonth == 0 ? null : (int?)s.enumToMonth))

                                       .ForMember(d => d.FromMonth, o => o.MapFrom(s => s.FromMonth == 0 ? null : (int?)s.enumFromMonth))
                                .ForMember(d => d.FromYear, o => o.MapFrom(s => s.FromYear == 0 ? null : (int?)s.enumFromYear))
                                .ForMember(d => d.ToMonth, o => o.MapFrom(s => s.ToMonth == 0 ? null : (int?)s.enumToMonth))
                                .ForMember(d => d.ToYear, o => o.MapFrom(s => s.ToYear == 0 ? null : (int?)s.enumToYear))


                            ;
                        });
                        var dtoEducationSummary = Mapper.Map<List<DTOModel.CandidateEducationSummary>>(JQualificationList);
                        genericRepo.AddMultipleEntity<DTOModel.CandidateEducationSummary>(dtoEducationSummary);

                    }
                }
                #endregion


                var dtoCandidateGovtWorkExperience = genericRepo.Get<DTOModel.CandidateGovtWorkExperience>(x => x.RegistrationID == candidateForm.RegistrationID).ToList();
                genericRepo.DeleteAll<DTOModel.CandidateGovtWorkExperience>(dtoCandidateGovtWorkExperience);

                #region Add Work Experience for Govt Sector

                if (candidateForm.candidateWorkExperience?.Count > 0)
                {
                    candidateForm.candidateGovtWrkExp.ForEach(x =>
                    {
                        x.Natureofappointment = (int)x.enumNatureOfAppointment;
                    });
                    if (genericRepo.Exists<DTOModel.CandidateGovtWorkExperience>(x => x.RegistrationID == candidateForm.RegistrationID))
                    { }
                    else
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CandidateGovtWorkExperience, DTOModel.CandidateGovtWorkExperience>()
                            .ForMember(d => d.RegistrationID, o => o.UseValue(candidateForm.RegistrationID))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                            .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                            ;
                        });
                        var dtoGovtWorkExperience = Mapper.Map<List<DTOModel.CandidateGovtWorkExperience>>(candidateForm.candidateGovtWrkExp);
                        genericRepo.AddMultipleEntity<DTOModel.CandidateGovtWorkExperience>(dtoGovtWorkExperience);

                    }
                }

                #endregion

                var dtoCandidateWorkExperience = genericRepo.Get<DTOModel.CandidateWorkExperience>(x => x.RegistrationID == candidateForm.RegistrationID).ToList();
                genericRepo.DeleteAll<DTOModel.CandidateWorkExperience>(dtoCandidateWorkExperience);

                #region Add Work Experience for Private Sector

                if (candidateForm.candidateWorkExperience?.Count > 0)
                {
                    if (genericRepo.Exists<DTOModel.CandidateWorkExperience>(x => x.RegistrationID == candidateForm.RegistrationID))
                    {

                    }
                    else
                    {
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<CandidateWorkExperince, DTOModel.CandidateWorkExperience>()
                            .ForMember(d => d.RegistrationID, o => o.UseValue(candidateForm.RegistrationID))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(1))
                            .ForMember(d => d.CraetedOn, o => o.UseValue(DateTime.Now))
                            .ForMember(d => d.OrganisationType, o => o.MapFrom(s => (int)s.OrganizationType));
                        });
                        var dtoWorkExperienceDtls = Mapper.Map<List<DTOModel.CandidateWorkExperience>>(candidateForm.candidateWorkExperience);
                        genericRepo.AddMultipleEntity<DTOModel.CandidateWorkExperience>(dtoWorkExperienceDtls);

                    }
                }

                #endregion


                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool UpdateCandidateAdmitCarddetails(Model.CandidateDetails UpdateCandidate)
        {
            log.Info($"RecruitmentService/UpdateCandidateAdmitCarddetails");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.Get<DTOModel.CandidateRegistration>(x => x.RegistrationID == UpdateCandidate.RegistrationID).FirstOrDefault();
                dtoObj.IssueAdmitCard = UpdateCandidate.IssueAdmitCard;
                dtoObj.UpdatedBy = UpdateCandidate.UpdatedBy;
                dtoObj.UpdatedOn = DateTime.Now;
                genericRepo.Update<DTOModel.CandidateRegistration>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool VerifyCandidationRegisterInfo(int requirementID, string inputText, int type = 1)
        {
            log.Info($"RecruitmentService/VerifyCandidationRegisterInfo/{inputText}/{type}");

            if (type == 1)
                return genericRepo.Exists<DTOModel.CandidateRegistration>(x => x.RequirementID == requirementID && x.PersonalEmailID.ToLower() == inputText);
            else if (type == 2)
                return genericRepo.Exists<DTOModel.CandidateRegistration>(x => x.RequirementID == requirementID && x.MobileNo.ToLower() == inputText);

            return false;
        }

        public int? CheckAlreadyRegistered(int requirementID, string mobileNo, string emailID)
        {
            log.Info($"RecruitmentService/CheckAlreadyRegistered/{requirementID}/{mobileNo}/{emailID}");

            var registrationID = genericRepo.Get<DTOModel.CandidateRegistration>(x => x.RequirementID == requirementID
             && x.MobileNo == mobileNo && x.PersonalEmailID.ToLower() == emailID).FirstOrDefault();

            return registrationID?.RegistrationID;
        }

        public List<CandidateDetails> GetTotalAppliedCandidate(int? designationID, DateTime startDate, DateTime endDate)
        {
            log.Info($"RecruitmentService/GetTotalAppliedCandidate");
            try
            {
                var getData = recruitmentRepository.GetTotalAppliedCandidate(designationID, startDate, endDate);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetCandidateAppliedCount_Result, Model.CandidateDetails>()
                    .ForMember(c => c.RequirementID, c => c.MapFrom(s => s.RequirementID))
                    .ForMember(c => c.Post, c => c.MapFrom(s => s.Post))
                    .ForMember(c => c.PublishDateFrom, c => c.MapFrom(s => s.PublishDate))
                    .ForMember(c => c.TotalApplied, c => c.MapFrom(s => s.TotalCandidateApplied))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var AppliedCandidateList = Mapper.Map<List<Model.CandidateDetails>>(getData);
                return AppliedCandidateList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.CandidateDetails> GetCandidateDetailsByReqID(int requirementID, int EligibleForWrittenExam)
        {
            log.Info($"RecruitmentService/GetCandidateDetailsByReqID");
            try
            {
                IEnumerable<Nafed.MicroPay.Data.Models.CandidateRegistration> result = Enumerable.Empty<Nafed.MicroPay.Data.Models.CandidateRegistration>();


                result = genericRepo.Get<Data.Models.CandidateRegistration>(x => x.FormStatus == 2 && x.RequirementID == requirementID);

                if (EligibleForWrittenExam != 0)
                {
                    if (EligibleForWrittenExam == 1)
                    {
                        result = result.Where(x => x.EligibleForWrittenExam == true);
                    }
                    else if (EligibleForWrittenExam == 2)
                    {
                        result = result.Where(x => x.EligibleForWrittenExam == false || x.EligibleForWrittenExam == null);
                    }
                }


                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.CandidateRegistration, Model.CandidateDetails>()
                    .ForMember(d => d.RegistrationID, o => o.MapFrom(s => s.RegistrationID))
                    .ForMember(d => d.Post, o => o.MapFrom(s => s.Requirement.Post))
                    .ForMember(d => d.CandidateFullName, o => o.MapFrom(s => s.CandidateFullName))
                    .ForMember(d => d.GenderID, o => o.MapFrom(s => s.GenderID))
                    .ForMember(d => d.PersonalEmailID, o => o.MapFrom(s => s.PersonalEmailID))
                    .ForMember(d => d.EligibleForWrittenExam, o => o.MapFrom(s => s.EligibleForWrittenExam))
                    .ForMember(d => d.Age, o => o.MapFrom(s => s.Age))
                    .ForMember(d => d.IssueAdmitCard, o => o.MapFrom(s => s.IssueAdmitCard))
                    .ForMember(d => d.RegistrationNo, o => o.MapFrom(s => s.RegistrationNo))
                    .ForMember(d => d.CandidatePicture, o => o.MapFrom(s => s.CandidatePicture))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var lstcandidate = Mapper.Map<List<Model.CandidateDetails>>(result);
                return lstcandidate.OrderBy(x => x.RegistrationNo).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<AppliedCandidateDetail> AppliedCandidatesList(int requirementID)
        {
            log.Info($"RecruitmentService/AppliedCandidatesList");
            try
            {
                var getData = recruitmentRepository.AppliedCandidatesList(requirementID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetAppliedCandidateDetail_Result, Model.AppliedCandidateDetail>();
                });
                var AppliedCandidateList = Mapper.Map<List<Model.AppliedCandidateDetail>>(getData);
                return AppliedCandidateList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #region Export

        public bool ExportAppCandidateList(DataTable dtTable, string sFullPath, string fileName)
        {
            log.Info($"RecruitmentService/ExportAppCandidateList/{sFullPath}/{fileName}");
            try
            {
                var flag = false;
                if (dtTable != null)
                {
                    if (Directory.Exists(sFullPath))
                    {
                        IEnumerable<string> exportHdr = Enumerable.Empty<string>();
                        exportHdr = dtTable.Columns.Cast<DataColumn>()
                            .Select(x => x.ColumnName).AsEnumerable<string>();
                        sFullPath = $"{sFullPath}{fileName}";
                        ExportToExcel(exportHdr, dtTable, fileName, sFullPath);
                    }
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
    }
}
