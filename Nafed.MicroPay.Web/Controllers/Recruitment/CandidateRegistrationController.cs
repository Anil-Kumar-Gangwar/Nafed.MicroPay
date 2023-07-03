using Nafed.MicroPay.Common;
using Nafed.MicroPay.Services;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Services.Recruitment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model = Nafed.MicroPay.Model;
using static Nafed.MicroPay.Common.ExtensionMethods;
using static Nafed.MicroPay.Common.FileHelper;
using CrystalDecisions.CrystalReports.Engine;
using MicroPay.Web.Models;
using AutoMapper;
using static Nafed.MicroPay.Model.EnumHelper;
using System.Text;
using MicroPay.Web.Attributes;

namespace MicroPay.Web.Controllers.Recruitment
{
    [EncryptedActionParameter]
    [CandidateSessionTimeout]
    public class CandidateRegistrationController : Controller
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDropdownBindService ddlService;
        private readonly IRecruitmentService recruitmentService;
        // GET: CandidateRegistration
        public CandidateRegistrationController(IDropdownBindService ddlService, IRecruitmentService recruitmentService)
        {
            this.ddlService = ddlService;
            this.recruitmentService = recruitmentService;
        }

       
        public ActionResult Index()
        {
            int? registrationID = Session["RegistrationID"] == null ? (int?)null : Convert.ToInt32(Session["RegistrationID"]);
            log.Info($"CandidateRegistration/Index/{registrationID}");

            BindDropdowns();
            ModelState.Clear();

            Model.CandidateRegistration registrationForm = new Model.CandidateRegistration();
            if (!registrationID.HasValue)
            {
                if (TempData["CandidateRegistration"] != null /*|| 1 > 0*/)
                {
                    var candidateinfo = (Model.CandidateRegistration)TempData["CandidateRegistration"] ?? new Model.CandidateRegistration();


                    registrationForm.JobLocation = recruitmentService.GetJobLocation(candidateinfo.RequirementID);
                    registrationForm.JobLocTypeID = candidateinfo.JobLocTypeID;
                    registrationForm.RequirementID = candidateinfo.RequirementID;
                    registrationForm.DesignationID = candidateinfo.DesignationID;
                    registrationForm.CandidateFullName = candidateinfo.CandidateFullName;
                    registrationForm.PersonalEmailID = candidateinfo.PersonalEmailID;
                    registrationForm.MobileNo = candidateinfo.MobileNo;
                    registrationForm.PostName = candidateinfo.PostName;
                    registrationForm.DeclarationName = candidateinfo.CandidateFullName;
                    registrationForm.MimimumExpenrienceNo = candidateinfo.MimimumExpenrienceNo;
                    var getJobQualDtls = recruitmentService.GetRequirementByID(registrationForm.RequirementID);
                    registrationForm.JobRequirementQualification = getJobQualDtls.JobRequirementQualification;


                    registrationForm.JobMinimumAgeValue = getJobQualDtls.MinimumAgeLimits;
                    registrationForm.JobMaximumAgeValue = getJobQualDtls.MaximumAgeLimit;


                }
                else
                {
                    return RedirectToAction("Index", "JobList");
                }


                registrationForm.CPhotoUNCPath = Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");

                registrationForm.CSignatureUNCPath = Path.Combine(DocumentUploadFilePath.CandidateSignature + "/CandidateSignature.png");
            }
            else
            {
                registrationForm = recruitmentService.GetApplicationForm(registrationID.Value);

                registrationForm.enumZoneAppliedFor = (Model.EnumZoneAppliedFor)(registrationForm.ZoneAppliedFor ?? 0);
                registrationForm.SelectedLocationID = registrationForm.JobLocTypeID == 2 ? registrationForm.ZoneAppliedFor.HasValue ? registrationForm.ZoneAppliedFor.Value : 0 :
                    registrationForm.JobLocTypeID == 3 ? 0 :
                    registrationForm.BranchID.Value;
                registrationForm.JobLocation = recruitmentService.GetJobLocation(registrationForm.RequirementID);
                registrationForm.enumPaymentType = (Model.EnumPaymentType)registrationForm.PaymentType;
                var getJobQualDtls = recruitmentService.GetRequirementByID(registrationForm.RequirementID);
                registrationForm.MimimumExpenrienceNo = getJobQualDtls.MimimumExpenrienceNo;
                //registrationForm.JobRequirementQualification = getJobQualDtls.JobRequirementQualification;

                if (registrationForm.CandidateEducationSummary?.Count > 0)
                {
                    if (registrationForm.CandidateEducationSummary.Count(x => x.JSelectedQualificationID < 10) > 0)
                    {
                        var JQualificationList = registrationForm.CandidateEducationSummary.Where(x => x.JSelectedQualificationID <= 10).ToList();

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.CandidateEducationSummary, Model.JobRequirementQualification>()
                               .ForMember(d => d.RequirementID, o => o.UseValue(registrationForm.RequirementID))
                               .ForMember(d => d.Percentage_GradeSystem, o => o.MapFrom(s => s.Percentage_GradeSystem))
                               .ForMember(d => d.SelectedQualificationID, o => o.MapFrom(s => (s.JSelectedQualificationID)))
                               .ForMember(d => d.Qualification, o => o.MapFrom(s => ((Model.EnumReqQualification)(s.JSelectedQualificationID))
                               .GetDisplayName()))
                               .ForMember(d => d.Per_GradeTextBoxCss, o => o.MapFrom(s => s.Percentage_GradeSystem ? null : "hide"))
                                .ForMember(d => d.FromMonth, o => o.MapFrom(s => (s.FromMonth)))
                              .ForMember(d => d.FromYear, o => o.MapFrom(s => (s.FromYear)))
                              .ForMember(d => d.ToMonth, o => o.MapFrom(s => (s.ToMonth)))
                              .ForMember(d => d.ToYear, o => o.MapFrom(s => (s.ToYear)))

                            ;
                        });
                        registrationForm.JobRequirementQualification = Mapper.Map<List<Model.JobRequirementQualification>>(JQualificationList);
                    }

                    registrationForm.CandidateEducationSummary = registrationForm.CandidateEducationSummary.Where(x => x.JSelectedQualificationID > 10).ToList();
                    registrationForm.CandidateEducationSummary.ForEach(x =>
                    {
                        x.OtherQualificationType = (Model.OtherQualificationType)x.JSelectedQualificationID;
                        x.Per_GradeTextBoxCss = x.Percentage_GradeSystem ? null : "hide";
                        //x.ddlFromMonth = monthDDL;
                        //x.ddlFromYear = yearDDl;
                        //x.ddlToMonth = monthDDL;
                        //x.ddlToYear = yearDDl;
                        x.enumFromMonth = (Model.EnumMonth)x.FromMonth;
                        x.enumFromYear = (Model.EnumYear)x.FromYear;
                        x.enumToMonth = (Model.EnumMonth)x.ToMonth;
                        x.enumToYear = (Model.EnumYear)x.ToYear;
                    });

                    registrationForm.JobRequirementQualification.ForEach(x =>
                    {
                        //x.ddlFromMonth = monthDDL;
                        //x.ddlFromYear = yearDDl;
                        //x.ddlToMonth = monthDDL;
                        //x.ddlToYear = yearDDl;
                        x.enumFromMonth = (Model.EnumMonth)x.FromMonth;
                        x.enumFromYear = (Model.EnumYear)x.FromYear;
                        x.enumToMonth = (Model.EnumMonth)x.ToMonth;
                        x.enumToYear = (Model.EnumYear)x.ToYear;
                    });

                    if (registrationForm.candidateGovtWrkExp?.Count > 0)
                    {
                        registrationForm.candidateGovtWrkExp.ForEach(x =>
                        {
                            x.enumNatureOfAppointment = (Model.EnumNatureOfAppointment)(x.Natureofappointment);
                        });
                    }

                }
                else
                {
                    registrationForm.JobRequirementQualification = getJobQualDtls.JobRequirementQualification;
                    registrationForm.JobRequirementQualification.ForEach(x =>
                    {
                        //x.ddlFromMonth = monthDDL;
                        //x.ddlFromYear = yearDDl;
                        //x.ddlToMonth = monthDDL;
                        //x.ddlToYear = yearDDl;
                        x.enumFromMonth = (Model.EnumMonth)x.FromMonth;
                        x.enumFromYear = (Model.EnumYear)x.FromYear;
                        x.enumToMonth = (Model.EnumMonth)x.ToMonth;
                        x.enumToYear = (Model.EnumYear)x.ToYear;
                    });

                }


                if (registrationForm.candidateWorkExperience?.Count > 0)
                {

                    registrationForm.candidateWorkExperience.ForEach(x =>
                    {

                        x.OrganizationType = (Model.OrganizationType)(x.OrganisationType);
                    });
                }
                registrationForm.JobMinimumAgeValue = getJobQualDtls.MinimumAgeLimits;
                registrationForm.JobMaximumAgeValue = getJobQualDtls.MaximumAgeLimit;
                registrationForm.enumfriendsAndRelativesInNafed = registrationForm.FriendsAndRelativesInNafed ? Model.EnumFriendsAndRelativesInNafed.Yes :

                    Model.EnumFriendsAndRelativesInNafed.No;

                registrationForm.enumhaveYouEverAppliedInNafedBefore = registrationForm.HaveYouEverAppliedinNafedBefore ? Model.EnumHaveYouEverAppliedinNafedBefore.Yes : Model.EnumHaveYouEverAppliedinNafedBefore.No;

                if (!string.IsNullOrEmpty(registrationForm.CandidatePicture))
                {
                    registrationForm.CPhotoUNCPath =
                           System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                            DocumentUploadFilePath.CandidatePhoto + "/" + registrationForm.CandidatePicture.Trim())) ?

                            Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/" + registrationForm.CandidatePicture.Trim()) : Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");
                }
                else
                    registrationForm.CPhotoUNCPath = Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");

                if (!string.IsNullOrEmpty(registrationForm.CandidateSignature))
                {
                    registrationForm.CSignatureUNCPath =
                         System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                          DocumentUploadFilePath.CandidateSignature + "/" + registrationForm.CandidateSignature.Trim())) ?

                          Path.Combine(DocumentUploadFilePath.CandidateSignature + "/" + registrationForm.CandidateSignature.Trim()) :
                          null;
                }

                TempData["CandidateEducation"] = registrationForm;
            }
            return View(registrationForm);
        }

        [HttpPost]
        public ActionResult SubmitRegistrationForm(Model.CandidateRegistration candidateForm, string ButtonType)
        {
            log.Info($"CandidateRegistration/SubmitRegistrationForm");

            if (ButtonType == "Add Work Experience")
            {
                if (candidateForm.candidateWorkExperience == null)
                    candidateForm.candidateWorkExperience = new List<Model.CandidateWorkExperince>() {
                           new Model.CandidateWorkExperince() { sno = 1, } };
                else
                {
                    if (candidateForm.candidateWorkExperience.Count == 1)
                        candidateForm.candidateWorkExperience.FirstOrDefault().sno = 1;
                    else
                    {
                        var s_no = 1;
                        candidateForm.candidateWorkExperience.ForEach(x =>
                        {
                            x.sno = s_no++;
                        });

                    }
                    if (candidateForm.candidateWorkExperience.Count < 5)
                        candidateForm.candidateWorkExperience.Add(new Model.CandidateWorkExperince()
                        {
                            sno = candidateForm.candidateWorkExperience.Count + 1,
                            RegistrationID = candidateForm.RegistrationID
                        });
                }
                TempData["CandidateWorkExperience"] = candidateForm;
                return Json(new { part = 1, htmlData = ConvertViewToString("_CandidateWorkExperience", candidateForm) }, JsonRequestBehavior.AllowGet);
            }

            else if (ButtonType == "Add Work Experience (Govt Sector)")
            {

                if (candidateForm.candidateGovtWrkExp == null)
                    candidateForm.candidateGovtWrkExp = new List<Model.CandidateGovtWorkExperience>() {
                           new Model.CandidateGovtWorkExperience() { sno = 1, } };
                else
                {
                    if (candidateForm.candidateGovtWrkExp.Count == 1)
                        candidateForm.candidateGovtWrkExp.FirstOrDefault().sno = 1;
                    else
                    {
                        var s_no = 1;
                        candidateForm.candidateGovtWrkExp.ForEach(x =>
                        {
                            x.sno = s_no++;
                        });

                    }
                    if (candidateForm.candidateGovtWrkExp.Count < 5)
                        candidateForm.candidateGovtWrkExp.Add(new Model.CandidateGovtWorkExperience()
                        {
                            sno = candidateForm.candidateGovtWrkExp.Count + 1,
                            RegistrationID = candidateForm.RegistrationID
                        });
                }
                TempData["CandidateGovtWorkExperience"] = candidateForm;
                return Json(new { part = 2, htmlData = ConvertViewToString("_CandidateGovtWorkExperience", candidateForm) }, JsonRequestBehavior.AllowGet);
            }
            else if (ButtonType == "Add Education Summary")
            {
                //var yearDDl = Enumerable.Range(2000, DateTime.Now.Year - 2000 + 1).
                //   Select(i => new Model.SelectListModel { id = i, value = i.ToString() }).OrderByDescending(x => x.value).ToList();
                //var monthDDL = Enumerable.Range(1, 12).
                //                        Select(i => new Model.SelectListModel
                //                        {
                //                            id = i,
                //                            value = i.ToString()
                //                        }).ToList();


                if (candidateForm.CandidateEducationSummary == null)
                    candidateForm.CandidateEducationSummary = new List<Model.CandidateEducationSummary>() {
                          new Model.CandidateEducationSummary() { sno = 1
                          //,ddlFromMonth=monthDDL,
                          //    ddlFromYear =yearDDl,
                          //    ddlToMonth =monthDDL,
                          //    ddlToYear =yearDDl
                          } };
                else
                {
                    if (candidateForm.CandidateEducationSummary.Count == 1)
                        candidateForm.CandidateEducationSummary.FirstOrDefault().sno = 1;
                    else
                    {
                        var s_no = 1;
                        candidateForm.CandidateEducationSummary.ForEach(x =>
                        {
                            x.sno = s_no++;
                            //x.ddlFromMonth = monthDDL;
                            //x.ddlFromYear = yearDDl;
                            //x.ddlToMonth = monthDDL;
                            //x.ddlToYear = yearDDl;
                        });

                    }
                    if (candidateForm.CandidateEducationSummary.Count < 4)

                        candidateForm.CandidateEducationSummary.Add(new Model.CandidateEducationSummary()
                        {
                            sno = candidateForm.CandidateEducationSummary.Count + 1,
                            RegistrationID = candidateForm.RegistrationID,
                            //ddlFromMonth = monthDDL,
                            //ddlFromYear = yearDDl,
                            //ddlToMonth = monthDDL,
                            //ddlToYear = yearDDl
                        });
                    //candidateForm.JobRequirementQualification.ForEach(x =>
                    //{
                    //    x.ddlFromMonth = monthDDL;
                    //    x.ddlFromYear = yearDDl;
                    //    x.ddlToMonth = monthDDL;
                    //    x.ddlToYear = yearDDl;
                    //});
                }
                TempData["CandidateEducation"] = candidateForm;
                return Json(new { part = 3, htmlData = ConvertViewToString("_AcademicProfessionalEducSummary", candidateForm) }, JsonRequestBehavior.AllowGet);
            }
            else if (ButtonType == "Save")
            {
                BindDropdowns();
                candidateForm.JobLocation = recruitmentService.GetJobLocation(candidateForm.RequirementID);

                // var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();

                if (candidateForm.JobLocTypeID == 3)
                    ModelState.Remove("SelectedLocationID");
                if (candidateForm.CandidateEducationSummary == null)
                {

                    for (int i = 0; i < candidateForm.JobRequirementQualification.Count; i++)
                    {
                        ModelState.Remove($"JobRequirementQualification[{i}].OtherQualificationType");
                    }


                }
                if (candidateForm.JobRequirementQualification != null && candidateForm.JobRequirementQualification.Count > 0)
                {

                    for (int i = 0; i < candidateForm.JobRequirementQualification.Count; i++)
                    {
                        ModelState.Remove($"JobRequirementQualification[{i}].OtherQualificationType");
                    }

                }
                var ff = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();


                if (ModelState.IsValid)
                {
                    candidateForm.CreatedBy = 1;
                    candidateForm.CreatedOn = System.DateTime.Now;
                    candidateForm.NationalityID = candidateForm.NationalityID == 0 ? null : candidateForm.NationalityID;
                    candidateForm.MaritalStsID = candidateForm.MaritalStsID == 0 ? null : candidateForm.MaritalStsID;
                    candidateForm.ReligionID = candidateForm.ReligionID == 0 ? null : candidateForm.ReligionID;
                    candidateForm.PmtStateID = candidateForm.PmtStateID == 0 ? null : candidateForm.PmtStateID;
                    candidateForm.GenderID = candidateForm.GenderID == 0 ? null : (int?)candidateForm.GenderID;
                    candidateForm.BranchID = candidateForm.JobLocTypeID == (int)Model.RequirementOptionType.SpecificBranch ? (int?)candidateForm.SelectedLocationID : null;
                    candidateForm.ZoneAppliedFor = candidateForm.JobLocTypeID == (int)Model.RequirementOptionType.SpecificZone ? (int?)candidateForm.SelectedLocationID : null;

                    // candidateForm.DesignationID = 40;
                    // candidateForm.RequirementID = 4;

                    candidateForm.HaveYouEverAppliedinNafedBefore = candidateForm.enumhaveYouEverAppliedInNafedBefore == Model.EnumHaveYouEverAppliedinNafedBefore.Yes ? true : false;
                    candidateForm.AppliedDate = !candidateForm.HaveYouEverAppliedinNafedBefore ? null : candidateForm.AppliedDate;
                    candidateForm.EarilerAppliedPostInNAFED = !candidateForm.HaveYouEverAppliedinNafedBefore ? null : candidateForm.EarilerAppliedPostInNAFED;

                    candidateForm.FriendsAndRelativesInNafed = candidateForm.enumfriendsAndRelativesInNafed == Model.EnumFriendsAndRelativesInNafed.Yes ? true : false;
                    candidateForm.IfYesStateNameAndRelationship = !candidateForm.FriendsAndRelativesInNafed ? null : candidateForm.IfYesStateNameAndRelationship;
                    candidateForm.Age = candidateForm.DOB.Value.CalculateAge();

                    var isValidAgeLimit = (candidateForm.Age >= (decimal)candidateForm.JobMinimumAgeValue) && (candidateForm.Age <= (decimal)candidateForm.JobMaximumAgeValue) ? true : false;
                    if (!isValidAgeLimit)
                    {
                        ModelState.AddModelError("JobAgeLimitValidation",
                            $"Age must be between {(int)candidateForm.JobMinimumAgeValue} and {(int)candidateForm.JobMaximumAgeValue} ");
                        //candidateForm.CPhotoUNCPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        // DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");
                        candidateForm.CPhotoUNCPath = Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");
                        return PartialView("_CandidateRegistration", candidateForm);
                    }

                    var candidatedRegisteredID = recruitmentService.CheckAlreadyRegistered(candidateForm.RequirementID, candidateForm.MobileNo, candidateForm.PersonalEmailID);

                    if (candidatedRegisteredID.HasValue)
                    {
                        return Json(new
                        {
                            action = "update",
                            msgType = "success",
                            registrationID = candidatedRegisteredID.Value,
                            //msg = $"Application Form Updated Successfully."
                        }, JsonRequestBehavior.AllowGet);
                    }


                    var registrationNumber = recruitmentService.CandidateRegistration(candidateForm);

                    if (!string.IsNullOrEmpty(registrationNumber))
                    {

                        TempData["message"] = $"Candidate Registerd Successfully. Your Registration No is {registrationNumber}";

                        var registrationArr = registrationNumber.Split(new char[] { '/' }).ToArray();
                        //  return JavaScript("window.location = '" + Url.Action("Index", "CandidateRegistration", new { @registrationID = candidateForm.RegistrationID }) + "'");
                        var registrationID = int.Parse(registrationArr[registrationArr.Length - 1]);
                        return Json(new
                        {
                            //registrationNo = registrationNumber,
                            action = "update",
                            msgType = "success",
                            registrationID = registrationID,
                            //msg = $"Application Form Updated Successfully."
                        }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return PartialView("_CandidateRegistration", candidateForm);
                }
                else
                {
                    candidateForm.CPhotoUNCPath = Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");

                    //    var yearDDl = Enumerable.Range(2000, DateTime.Now.Year - 2000 + 1).
                    //Select(i => new Model.SelectListModel { id = i, value = i.ToString() }).OrderByDescending(x => x.value).ToList();
                    //    var monthDDL = Enumerable.Range(1, 12).
                    //                            Select(i => new Model.SelectListModel
                    //                            {
                    //                                id = i,
                    //                                value = i.ToString()
                    //                            }).ToList();

                    //candidateForm.CandidateEducationSummary.Add(new Model.CandidateEducationSummary()
                    //{
                    //    sno = candidateForm.CandidateEducationSummary.Count + 1,
                    //    RegistrationID = candidateForm.RegistrationID,
                    //    //ddlFromMonth = monthDDL,
                    //    //ddlFromYear = yearDDl,
                    //    //ddlToMonth = monthDDL,
                    //    //ddlToYear = yearDDl
                    //});
                    //candidateForm.JobRequirementQualification.ForEach(x =>
                    //{
                    //    x.ddlFromMonth = monthDDL;
                    //    x.ddlFromYear = yearDDl;
                    //    x.ddlToMonth = monthDDL;
                    //    x.ddlToYear = yearDDl;
                    //});

                    return PartialView("_CandidateRegistration", candidateForm);
                }


            }
            else if (ButtonType == "Update" || (ButtonType == "Submit"))
            {
                BindDropdowns();
                if (ButtonType == "Submit")
                {

                    if (!string.IsNullOrEmpty(candidateForm.CandidatePicture))
                    {
                        candidateForm.CPhotoUNCPath =
                               System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                DocumentUploadFilePath.CandidatePhoto + "/" + candidateForm.CandidatePicture.Trim())) ?

                                Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/" + candidateForm.CandidatePicture.Trim()) :
                                Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");
                    }
                    else
                        candidateForm.CPhotoUNCPath = Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");

                    if (!string.IsNullOrEmpty(candidateForm.CandidateSignature))
                    {
                        candidateForm.CSignatureUNCPath =
                             System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                              DocumentUploadFilePath.CandidateSignature + "/" + candidateForm.CandidateSignature.Trim())) ?

                              Path.Combine(DocumentUploadFilePath.CandidateSignature + "/" + candidateForm.CandidateSignature.Trim()) :
                              null;
                    }
                    if (candidateForm.FeesApplicable)
                    {
                        if ((int)candidateForm.enumPaymentType == 0)
                        {
                            var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                            if (gg.Count > 0)
                                candidateForm.JobLocation = recruitmentService.GetJobLocation(candidateForm.RequirementID);

                            ModelState.AddModelError("PaymentTypeValidation", "Please select Payment Mode.");
                            return PartialView("_CandidateRegistration", candidateForm);
                        }

                        if (!candidateForm.PaymentDate.HasValue)
                        {

                            ModelState.AddModelError("PaymentDateValidation", "Please select Payment date");

                            var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                            if (gg.Count > 0)
                                candidateForm.JobLocation = recruitmentService.GetJobLocation(candidateForm.RequirementID);

                            return PartialView("_CandidateRegistration", candidateForm);
                        }
                        if (string.IsNullOrEmpty(candidateForm.PaymentTransactionID))
                        {
                            ModelState.AddModelError("PaymentTransactionIDValidation", "Please enter Payment TransactionID");
                            var gg = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                            if (gg.Count > 0)
                                candidateForm.JobLocation = recruitmentService.GetJobLocation(candidateForm.RequirementID);
                            return PartialView("_CandidateRegistration", candidateForm);
                        }


                    }

                    var totalExperienceInYear = candidateForm.RelevantExperienceInYear == null ? 0 : candidateForm.RelevantExperienceInYear;
                    var totalExperienceInMonth = candidateForm.RelevantExperienceInMonth == null ? 0 : candidateForm.RelevantExperienceInMonth;
                    var govtReleExpInYear = candidateForm.GovtReleExpInYear == null ? 0 : candidateForm.GovtReleExpInYear;
                    var govtReleExpInMonth = candidateForm.GovtReleExpInMonth == null ? 0 : candidateForm.GovtReleExpInYear;

                    var totalYears = ((totalExperienceInYear + govtReleExpInYear) + ((totalExperienceInMonth + govtReleExpInMonth) / 12));
                    if (candidateForm.MimimumExpenrienceNo <= totalYears)
                    {
                    }
                    else
                    {
                        ModelState.AddModelError("MimimumExpenrienceNoValidation", $"Please provide work experience detail as per eligibility criteria.");
                        candidateForm.JobLocation = recruitmentService.GetJobLocation(candidateForm.RequirementID);
                        return PartialView("_CandidateRegistration", candidateForm);
                    }
                    candidateForm.FormStatus = 2;
                }

                // BindDropdowns();
                candidateForm.JobLocation = recruitmentService.GetJobLocation(candidateForm.RequirementID);


                if (candidateForm.JobLocTypeID == 3)
                    ModelState.Remove("SelectedLocationID");
                if (candidateForm.CandidateEducationSummary == null)
                {
                    for (int i = 0; i < candidateForm.JobRequirementQualification.Count; i++)
                    {
                        ModelState.Remove($"JobRequirementQualification[{i}].OtherQualificationType");
                    }
                }
                if (candidateForm.JobRequirementQualification != null && candidateForm.JobRequirementQualification.Count > 0)
                {
                    for (int i = 0; i < candidateForm.JobRequirementQualification.Count; i++)
                    {
                        ModelState.Remove($"JobRequirementQualification[{i}].OtherQualificationType");
                    }
                }

                if (ModelState.IsValid)
                {
                    candidateForm.UpdatedBy = 1;
                    candidateForm.UpdatedOn = System.DateTime.Now;
                    candidateForm.NationalityID = candidateForm.NationalityID == 0 ? null : candidateForm.NationalityID;
                    candidateForm.MaritalStsID = candidateForm.MaritalStsID == 0 ? null : candidateForm.MaritalStsID;
                    candidateForm.ReligionID = candidateForm.ReligionID == 0 ? null : candidateForm.ReligionID;
                    candidateForm.PmtStateID = candidateForm.PmtStateID == 0 ? null : candidateForm.PmtStateID;
                    candidateForm.GenderID = candidateForm.GenderID == 0 ? null : (int?)candidateForm.GenderID;

                    candidateForm.BranchID = candidateForm.JobLocTypeID == (int)Model.RequirementOptionType.SpecificBranch ? (int?)candidateForm.SelectedLocationID : null;
                    candidateForm.ZoneAppliedFor = candidateForm.JobLocTypeID == (int)Model.RequirementOptionType.SpecificZone ? (int?)candidateForm.SelectedLocationID : null;
                    candidateForm.PaymentType = candidateForm.FormStatus == null ? 0 : (int)candidateForm.enumPaymentType;
                    candidateForm.HaveYouEverAppliedinNafedBefore = candidateForm.enumhaveYouEverAppliedInNafedBefore == Model.EnumHaveYouEverAppliedinNafedBefore.Yes ? true : false;
                    candidateForm.AppliedDate = !candidateForm.HaveYouEverAppliedinNafedBefore ? null : candidateForm.AppliedDate;
                    candidateForm.EarilerAppliedPostInNAFED = !candidateForm.HaveYouEverAppliedinNafedBefore ? null : candidateForm.EarilerAppliedPostInNAFED;

                    candidateForm.FriendsAndRelativesInNafed = candidateForm.enumfriendsAndRelativesInNafed == Model.EnumFriendsAndRelativesInNafed.Yes ? true : false;
                    candidateForm.IfYesStateNameAndRelationship = !candidateForm.FriendsAndRelativesInNafed ? null : candidateForm.IfYesStateNameAndRelationship;

                    candidateForm.Age = candidateForm.DOB.Value.CalculateAge();

                    var isValidAgeLimit = (candidateForm.Age >= (decimal)candidateForm.JobMinimumAgeValue) && (candidateForm.Age <= (decimal)candidateForm.JobMaximumAgeValue) ? true : false;
                    if (!isValidAgeLimit)
                    {
                        ModelState.AddModelError("JobAgeLimitValidation",
                            $"Age must be between {(int)candidateForm.JobMinimumAgeValue} and {(int)candidateForm.JobMaximumAgeValue} ");
                        candidateForm.CPhotoUNCPath = Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");
                        return PartialView("_CandidateRegistration", candidateForm);
                    }

                    bool updated = recruitmentService.UpdateCandidateRegistration(candidateForm);

                    if (updated)
                    {
                        TempData["message"] = "Application Form Updated Successfully.";
                        //  return JavaScript("window.location = '" + Url.Action("Index", "CandidateRegistration", new { @registrationID = candidateForm.RegistrationID }) + "'");

                        return Json(new
                        {
                            //registrationNo = registrationNumber,
                            action = "update",
                            msgType = "success",
                            registrationID = candidateForm.RegistrationID,
                            msg = $"Application Form Updated Successfully."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return PartialView("_CandidateRegistration", candidateForm);

                    ///==== Write Code here to update Application Form ===========
                }
                else
                {
                    candidateForm.CPhotoUNCPath = Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");

                    //      var yearDDl = Enumerable.Range(2000, DateTime.Now.Year - 2000 + 1).
                    //Select(i => new Model.SelectListModel { id = i, value = i.ToString() }).OrderByDescending(x => x.value).ToList();
                    //      var monthDDL = Enumerable.Range(1, 12).
                    //                              Select(i => new Model.SelectListModel
                    //                              {
                    //                                  id = i,
                    //                                  value = i.ToString()
                    //                              }).ToList();

                    //if (candidateForm.CandidateEducationSummary?.Count > 0)
                    //{
                    //    candidateForm.CandidateEducationSummary.Add(new Model.CandidateEducationSummary()
                    //    {
                    //        sno = candidateForm.CandidateEducationSummary.Count + 1,
                    //        RegistrationID = candidateForm.RegistrationID,
                    //        //ddlFromMonth = monthDDL,
                    //        //ddlFromYear = yearDDl,
                    //        //ddlToMonth = monthDDL,
                    //        //ddlToYear = yearDDl
                    //    });
                    //}
                    //candidateForm.JobRequirementQualification.ForEach(x =>
                    //{
                    //    x.ddlFromMonth = monthDDL;
                    //    x.ddlFromYear = yearDDl;
                    //    x.ddlToMonth = monthDDL;
                    //    x.ddlToYear = yearDDl;
                    //});

                    return PartialView("_CandidateRegistration", candidateForm);
                }
            }

            return View("");
        }

        [HttpGet]
        public ActionResult _RemoveTargetRow(int sNo)
        {
            var modelData = (Model.CandidateRegistration)TempData["CandidateWorkExperience"];
            if (modelData != null)
            {
                var deletedRow = modelData.candidateWorkExperience.Where(x => x.sno == sNo).FirstOrDefault();
                modelData.candidateWorkExperience.Remove(deletedRow);
                TempData["CandidateWorkExperience"] = modelData;
                return PartialView("_CandidateWorkExperience", modelData);
            }
            return Content("");

        }

        [HttpGet]
        public ActionResult _RemoveCandidateGvotWrkExp(int sNo)
        {
            var modelData = (Model.CandidateRegistration)TempData["CandidateGovtWorkExperience"];
            if (modelData != null)
            {
                var deletedRow = modelData.candidateGovtWrkExp.Where(x => x.sno == sNo).FirstOrDefault();
                modelData.candidateGovtWrkExp.Remove(deletedRow);
                TempData["CandidateGovtWorkExperience"] = modelData;
                return PartialView("_CandidateGovtWorkExperience", modelData);
            }
            return Content("");
        }

        [HttpGet]
        public ActionResult _RemoveCandidateEducRow(int sNo)
        {
            var modelData = (Model.CandidateRegistration)TempData["CandidateEducation"];
            if (modelData != null)
            {
                var deletedRow = modelData.CandidateEducationSummary.Where(x => x.sno == sNo).FirstOrDefault();
                modelData.CandidateEducationSummary.Remove(deletedRow);

                TempData["CandidateEducation"] = modelData;
                return PartialView("_AcademicProfessionalEducSummary", modelData);
            }
            return Content("");

        }

        private void BindDropdowns()
        {
            var ddlGenderList = ddlService.ddlGenderList();
            ddlGenderList.OrderBy(x => x.value);
            Model.SelectListModel selectGender = new Model.SelectListModel();
            selectGender.id = 0;
            selectGender.value = "Select";
            ddlGenderList.Insert(0, selectGender);
            ViewBag.Gender = new SelectList(ddlGenderList, "id", "value");


            var ddlReligionList = ddlService.ddlReligionList();
            ddlReligionList.OrderBy(x => x.value);
            Model.SelectListModel selectReligion = new Model.SelectListModel();
            selectReligion.id = 0;
            selectReligion.value = "Select";
            ddlReligionList.Insert(0, selectReligion);
            ViewBag.Religion = new SelectList(ddlReligionList, "id", "value");


            var ddlMaritalStsList = ddlService.ddlMaritalStsList();
            ddlMaritalStsList.OrderBy(x => x.value);
            Model.SelectListModel maritalSts = new Model.SelectListModel();
            maritalSts.id = 0;
            maritalSts.value = "Select";
            ddlMaritalStsList.Insert(0, maritalSts);
            ViewBag.MaritalSts = new SelectList(ddlMaritalStsList, "id", "value");

            var ddlBloodGroupList = ddlService.ddlBloodGroupList();
            ddlBloodGroupList.OrderBy(x => x.value);
            Model.SelectListModel bloodGroup = new Model.SelectListModel();
            bloodGroup.id = 0;
            bloodGroup.value = "Select";
            ddlBloodGroupList.Insert(0, bloodGroup);
            ViewBag.BloodGroup = new SelectList(ddlBloodGroupList, "id", "value");


            List<Model.SelectListModel> nationalityList = new List<Model.SelectListModel>();
            nationalityList.Add(new Nafed.MicroPay.Model.SelectListModel { id = 1, value = "Indian" });
            //Model.SelectListModel nationality = new Model.SelectListModel();
            //nationality.id = 0;
            //nationality.value = "Select";
            //nationalityList.Insert(0, nationality);
            ViewBag.nationalityList = new SelectList(nationalityList, "id", "value");


            var ddlState = ddlService.ddlStateList();
            ddlState.OrderBy(x => x.value);
            Model.SelectListModel State = new Model.SelectListModel();
            State.id = 0;
            State.value = "Select";
            ddlState.Insert(0, State);
            ViewBag.ddlState = new SelectList(ddlState, "id", "value");

        }

        public ActionResult ViewApplicationForm()
        {
            int registrationID = Convert.ToInt32(Session["RegistrationID"]);
            var candidateFormData = recruitmentService.GetApplicationForm(registrationID);

            candidateFormData.Nationality = candidateFormData.NationalityID.HasValue ? ((Model.EnumNationality)candidateFormData.NationalityID.Value).ToString() : string.Empty;
            candidateFormData.PaymentTypeName = candidateFormData.PaymentType != 0 ? ((Model.EnumPaymentType)candidateFormData.PaymentType).ToString() : string.Format("-");
            if (!string.IsNullOrEmpty(candidateFormData.CandidatePicture))
            {
                candidateFormData.CPhotoUNCPath =

                       System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        DocumentUploadFilePath.CandidatePhoto + "/" + candidateFormData.CandidatePicture.Trim())) ?

                        Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/" + candidateFormData.CandidatePicture.Trim()) :
                        Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");
            }
            else
                candidateFormData.CPhotoUNCPath = Path.Combine(DocumentUploadFilePath.CandidatePhoto + "/CandidatePhoto.png");


            if (!string.IsNullOrEmpty(candidateFormData.CandidateSignature))
            {
                candidateFormData.CSignatureUNCPath =
                       System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        DocumentUploadFilePath.CandidateSignature + "/" + candidateFormData.CandidateSignature.Trim())) ?
                        Path.Combine(DocumentUploadFilePath.CandidateSignature + "/" + candidateFormData.CandidateSignature.Trim()) :
                        null;
            }

            if (candidateFormData.candidateGovtWrkExp.Count > 0)
            {
                candidateFormData.candidateGovtWrkExp.ForEach(x =>
                x.NatureOfAppointmentName = ((Model.EnumNatureOfAppointment)x.Natureofappointment).ToString());
            }
            return View("CandidateApplicationForm", candidateFormData);
        }

        [NonAction]
        public string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }


        public FileResult GetImage(string imgPath)
        {
            log.Info($"Get Item image path {imgPath}");
            byte[] imageByteData = ImageService.GetImage(imgPath);
            return File(imageByteData, "image/jpg;base64");
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                string fileName = string.Empty; string filePath = string.Empty;
                string fname = "";
                string docType = Request["docType"];
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;

                    #region Check Mime Type
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (!IsValidFileName(file.FileName))
                        {
                            stringBuilder.Append($"# {i + 1} The file name of the {file.FileName} file is incorrect");
                            stringBuilder.Append($"File name must not contain special characters.In File Name,Only following characters are allowed.");
                            stringBuilder.Append($"I. a to z characters.");
                            stringBuilder.Append($"II. numbers(0 to 9).");
                            stringBuilder.Append($"III. - and _ with space.");
                        }
                        var contentType = GetFileContentType(file.InputStream);
                        var dicValue = GetDictionaryValueByKeyName(Path.GetExtension(file.FileName));
                        if (dicValue != contentType)
                        {
                            stringBuilder.Append($"# {i + 1} The file format of the {file.FileName} file is incorrect");
                            stringBuilder.Append("<br>");
                        }
                    }
                    if (stringBuilder.ToString() != "")
                    {
                        return Json(new
                        {
                            FileName = stringBuilder.ToString(),
                            msgType = "error",
                            filetype = docType
                        }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];

                        if (docType == "photo")
                        {
                            fname = ExtensionMethods.SetUniqueFileName("CandidatePic-",
                         Path.GetExtension(file.FileName));
                            fileName = fname;
                            string pFileName = Request["CandidatePicture"];
                            filePath = "~/" + DocumentUploadFilePath.CandidatePhoto;
                            if (pFileName != "")
                            {
                                string fullPath = Request.MapPath("~/Images/CandidatePhoto/" + pFileName);
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }

                            fname = Path.Combine(Server.MapPath("~/Images/CandidatePhoto/"), fname);
                            file.SaveAs(fname);
                        }
                        else if (docType == "signature")
                        {
                            fname = ExtensionMethods.SetUniqueFileName("SignaturePic-",
                        Path.GetExtension(file.FileName));
                            fileName = fname;
                            string pFileName = Request["CandidateSignature"];
                            if (pFileName != "")
                            {
                                string fullPath = Request.MapPath("~/Images/CandidateSignature/" + pFileName);
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }

                            fname = Path.Combine(Server.MapPath("~/Images/CandidateSignature/"), fname);
                            file.SaveAs(fname);
                        }
                    }
                    // Returns message that successfully uploaded  
                    return Json(new
                    {
                        FileName = fileName,
                        msgType = "success",
                        filetype = docType
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                    return Json(new
                    {
                        FileName = fname,
                        msgType = "error",
                        filetype = docType
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public ActionResult Download_AdmitCard(string registrationNo, string candidatePicture, string candidateSignature)
        {
            BaseReportModel reportModel = new BaseReportModel();
            List<ReportParameter> parameterList = new List<ReportParameter>();
            parameterList.Add(new ReportParameter() { name = "registrationNo", value = registrationNo });

            reportModel.reportParameters = parameterList;
            reportModel.rptName = "AdmitCard.rpt";
            candidatePicture = candidatePicture ?? "CandidateAdmitCardPhoto.png";
            candidateSignature = candidateSignature ?? "candidatesignature.png";
            ReportDocument objReport = new ReportDocument();
            var rptModel = reportModel;
            string path = Path.Combine(Server.MapPath(rptModel.ReportFilePath), rptModel.rptName);
            objReport.Load(path);
            objReport.SetDatabaseLogon(ConfigManager.Value("odbcUser"), ConfigManager.Value("odbcPassword"), @"odbcNAFEDHR", ConfigManager.Value("odbcDatabase"));

            objReport.Refresh();
            if (rptModel.reportParameters?.Count > 0)
            {
                foreach (var item in rptModel.reportParameters)
                    objReport.SetParameterValue($"@{item.name}", item.value);
            }

            objReport.SetParameterValue("candidatephoto", Path.Combine(Server.MapPath(rptModel.CandidateImage), candidatePicture));
            objReport.SetParameterValue("candidatesignature", Path.Combine(Server.MapPath(rptModel.CandidateSignature), candidateSignature));
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation;

            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            //return File(stream, "application/pdf", $"Admit Card.pdf");
            //  ConfigManager.Value("AdmitCardUNCPath");
            registrationNo = registrationNo.Replace('/', '-');
            //  objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, ConfigManager.Value("AdmitCardUNCPath") + $"Admit Card_{registrationNo}.pdf");
            return File(stream, "application/pdf", $"Admit Card_{registrationNo}.pdf");

            //return File(stream, "application/pdf", $"Admit Card_{registrationNo}.pdf");
        }

    }


}