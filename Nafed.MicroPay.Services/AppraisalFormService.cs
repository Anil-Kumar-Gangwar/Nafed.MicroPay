using System;
using System.Collections.Generic;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using System.Linq;
using static Nafed.MicroPay.Common.ExtensionMethods;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static Nafed.MicroPay.ImportExport.APARReportsExport;
using Nafed.MicroPay.ImportExport.Interfaces;

namespace Nafed.MicroPay.Services
{
    public class AppraisalFormService : BaseService, IAppraisalFormService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IAppraisalRepository appraisalRepo;
        private readonly IExport exportExcel;
        public AppraisalFormService(IGenericRepository genericRepo, IAppraisalRepository appraisalRepo, IExport exportExcel)
        {
            this.genericRepo = genericRepo;
            this.appraisalRepo = appraisalRepo;
            this.exportExcel = exportExcel;
        }
        public IEnumerable<Model.AppraisalForm> GetAppraisalForms()
        {
            log.Info($"AppraisalFormService/GetAppraisalForms");

            try
            {
                var appraisalForms = genericRepo.Get<DTOModel.AppraisalForm>();

                var ReportingYr = appraisalForms.Select(x => x.ReportingYr).LastOrDefault();
                appraisalForms = appraisalForms.Where(x => x.ReportingYr == ReportingYr);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AppraisalForm, AppraisalForm>();
                });
                return Mapper.Map<List<AppraisalForm>>(appraisalForms);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertUpdateDesignationApprasialForm(int formID, List<int> designation, int userID)
        {
            log.Info($"AppraisalFormService/InsertUpdateDesignationApprasialForm/{formID}/{designation}/{userID}");

            bool flag = false;

            try
            {
                List<DTOModel.DesignationAppraisalForm> objForms = new List<Data.Models.DesignationAppraisalForm>();

                var dtoAppraisalForms = genericRepo.Get<DTOModel.DesignationAppraisalForm>(x => x.IsDeleted == false
                 // && x.AppraisalFormID == formID
                 && designation.Contains(x.DesignationID)
                 ).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<int, DTOModel.DesignationAppraisalForm>()
                   .ForMember(d => d.DesignationID, o => o.MapFrom(s => s))
                   .ForMember(d => d.AppraisalFormID, o => o.UseValue(formID))
                   .ForMember(d => d.CreatedBy, o => o.UseValue(userID))
                   .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                   .ForAllOtherMembers(d => d.Ignore());
                });

                objForms = Mapper.Map<List<DTOModel.DesignationAppraisalForm>>(designation);

                if (dtoAppraisalForms.Count > 0)
                    dtoAppraisalForms.ForEach(x => { x.IsDeleted = true; });

                //   genericRepo.RemoveMultipleEntity<DTOModel.DesignationAppraisalForm>(dtoAppraisalForms);
                genericRepo.AddMultipleEntity<DTOModel.DesignationAppraisalForm>(objForms);

                flag = true;
            }
            catch (Exception)
            {
                throw;
            }

            return flag;
        }

        public IEnumerable<int> LinkedDesignations(int formID)
        {
            log.Info($"AppraisalFormService/LinkedDesignations/{formID}");
            try
            {
                IEnumerable<int> designationID = Enumerable.Empty<int>();
                designationID = genericRepo.Get<DTOModel.DesignationAppraisalForm>(x => x.AppraisalFormID == formID && !x.IsDeleted).Select(x => x.DesignationID);
                return designationID;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public IEnumerable<Employee> GetEmployeeAppraisalList(int userEmpID, string empCode, string empName)
        //{
        //    log.Info($"AppraisalFormService/GetEmployeeAppraisalList/{userEmpID}");

        //    try
        //    {
        //        IEnumerable<Employee> empList = Enumerable.Empty<Employee>();
        //        var dtoEmpList = appraisalRepo.EmployeeAppraisalList(userEmpID, empCode, empName);
        //        Mapper.Initialize(cfg =>
        //        {
        //            cfg.CreateMap<DTOModel.tblMstEmployee, Employee>()
        //           .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeId))
        //           .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
        //           .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
        //           .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
        //           .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
        //           .ForMember(d => d.ReportingToID, o => o.MapFrom(s => s.ReportingTo))
        //           .ForMember(d => d.ReviewerTo, o => o.MapFrom(s => s.ReviewerTo))
        //           .ForMember(d => d.AcceptanceAuthority, o => o.MapFrom(s => s.AcceptanceAuthority))
        //           .ForMember(d => d.AppraisalFormID, o => o.MapFrom(s => s.Designation.DesignationAppraisalForms.FirstOrDefault().AppraisalFormID))
        //           .ForMember(d => d.AppraisalFormName, o => o.MapFrom(s => s.Designation.DesignationAppraisalForms.FirstOrDefault().AppraisalForm.FormName))

        //           .ForAllOtherMembers(d => d.Ignore());
        //        });
        //        empList = Mapper.Map<List<Employee>>(dtoEmpList);
        //        return empList.Where(x => x.AppraisalFormID != null);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public IEnumerable<AppraisalFormHdr> GetAppraisalFormHdr(AppraisalFormApprovalFilter filters)
        {
            log.Info($"AppraisalFormService/GetAppraisalFormHdr/");
            try
            {
                List<DTOModel.AppraisalFormHdr> dtoFormHdrs = new List<DTOModel.AppraisalFormHdr>();

                var appformsHdr = genericRepo.GetIQueryable<DTOModel.AppraisalFormHdr>(x => x.tblMstEmployee.EmployeeProcessApprovals
                .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.Appraisal
                && y.ToDate == null
                && (y.ReportingTo == filters.loggedInEmployeeID || y.ReviewingTo == filters.loggedInEmployeeID || y.AcceptanceAuthority == filters.loggedInEmployeeID))
                );

                if (filters != null)
                {
                    var selectedYr = ((filters.selectedReportingYear == "" || filters.selectedReportingYear == null) ? filters.reportingYrs.First().value : filters.selectedReportingYear);
                    dtoFormHdrs = appformsHdr.Where(x => (filters.selectedEmployeeID == 0 ? (1 > 0) : x.EmployeeID == filters.selectedEmployeeID) && (filters.selectedFormID == 0 ? (1 > 0) : x.FormID == filters.selectedFormID) && (x.ReportingYr == selectedYr) && (((int)filters.appraisalFormStatus == 0) ? (1 > 0) : x.StatusID == (int)filters.appraisalFormStatus)).ToList();
                }

                //  var dtoFormHdrs = appraisalRepo.GetAppraisalFormHdr();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AppraisalFormHdr, Model.AppraisalFormHdr>()
                    .ForMember(d => d.AppraisalHdrID, o => o.MapFrom(s => s.AppraisalHdrID))
                    .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.EmpName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                    .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                    .ForMember(d => d.FormName, o => o.MapFrom(s => s.AppraisalForm.FormName))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.Appraisal && y.ToDate == null && y.EmployeeID == s.EmployeeID)))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.tblMstEmployee.DepartmentID))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.tblMstEmployee.DesignationID))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();

                });
                return Mapper.Map<List<Model.AppraisalFormHdr>>(dtoFormHdrs.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IEnumerable<APARSkillSetFormHdr> GetSkillAssessmentFormHdr(SkillAssessmentApprovalFilters filters)
        {
            log.Info($"AppraisalFormService/GetSkillAssessmentFormHdr/");
            try
            {
                IEnumerable<APARSkillSetFormHdr> formHdrs = Enumerable.Empty<APARSkillSetFormHdr>();
                List<DTOModel.APARSkillSetFormHdr> dtoFormHdrs = new List<DTOModel.APARSkillSetFormHdr>();

                var aPARSkillSetHdrs = genericRepo.GetIQueryable<DTOModel.APARSkillSetFormHdr>
                    (x => x.tblMstEmployee.EmployeeProcessApprovals
                       .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.Appraisal
                       && y.ToDate == null
                       && (y.ReportingTo == filters.loggedInEmployeeID
                       || y.ReviewingTo == filters.loggedInEmployeeID
                       || y.AcceptanceAuthority == filters.loggedInEmployeeID))
                );

                if (filters != null)
                    dtoFormHdrs = aPARSkillSetHdrs.Where(x => (filters.selectedEmployeeID == 0 ? (1 > 0) : x.EmployeeID == filters.selectedEmployeeID) && (filters.selectedReportingYr == 0 ? (1 > 0) : x.ReportingYr == filters.reportingYr) && (((int)filters.competencyFormState == 0) ? (1 > 0) : x.StatusID == (int)filters.competencyFormState)).ToList();



                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.APARSkillSetFormHdr, Model.APARSkillSetFormHdr>()
                    .ForMember(d => d.APARHdrID, o => o.MapFrom(s => s.APARHdrID))
                    .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s =>
                     s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y =>
                     y.ProcessID == (int)Common.WorkFlowProcess.Appraisal && y.ToDate == null && y.EmployeeID == s.EmployeeID)))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.DepartmentID))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                    .ForAllOtherMembers(d => d.Ignore());

                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();

                });
                return Mapper.Map<List<Model.APARSkillSetFormHdr>>(dtoFormHdrs.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Model.AppraisalFormHdr> GetEmployeeSelfAppraisalList(int userEmpID, string empCode, string empName)
        {
            log.Info($"AppraisalFormService/GetEmployeeSelfAppraisalList/{userEmpID}");

            try
            {
                // List<AppraisalFormHdr> empAppList = new List<AppraisalFormHdr>();
                var dtoAppraisalForms = appraisalRepo.EmployeeSelfAppraisalList(userEmpID, empCode, empName).ToList();
                if (dtoAppraisalForms != null && dtoAppraisalForms.Count > 0)
                {

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.AppraisalFormHdr, Model.AppraisalFormHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))

                   .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                   .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                   .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.Appraisal && y.ToDate == null && y.EmployeeID == s.EmployeeID)))

                   .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                   .ForMember(d => d.FormName, o => o.MapFrom(s => s.AppraisalForm.FormName))
                   .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                   .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                   ;
                        cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                    });

                    return Mapper.Map<List<Model.AppraisalFormHdr>>(dtoAppraisalForms);

                }
                else
                    return new List<AppraisalFormHdr>();

            }
            catch (Exception)
            {
                throw;
            }
        }
        public FormGroupAHdr GetFormGroupHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            log.Info($"AppraisalFormService/GetFormGroupHdrDetail/{tableName}/{branchID}/{empID}/{reportingYear}/{formID}");
            try
            {
                FormGroupAHdr formGroupHdr = new FormGroupAHdr();
                var formGroupHdrDTO = appraisalRepo.GetFormGroupHdrDetail(tableName, branchID, empID, reportingYear, formID);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetFormGroupHdrDetail_Result, FormGroupAHdr>()
                       .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name));
                  }
                  );
                formGroupHdr = Mapper.Map<List<FormGroupAHdr>>(formGroupHdrDTO).FirstOrDefault();
                return formGroupHdr;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<FormGroupDetail1> GetFormGroupDetail1(string hdrTableName, string childTableName, int empID, int? formGroupID)
        {
            log.Info($"AppraisalFormService/GetFormGroupDetail1/{hdrTableName}/{childTableName}/{empID}");
            try
            {
                IEnumerable<FormGroupDetail1> formGroupDetail1 = Enumerable.Empty<FormGroupDetail1>();
                var formGroupDetail1DTO = appraisalRepo.GetFormGroupDetail1(hdrTableName, childTableName, empID, formGroupID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetFormGroupDetail1_Result, FormGroupDetail1>();
                });
                formGroupDetail1 = Mapper.Map<List<FormGroupDetail1>>(formGroupDetail1DTO);
                return formGroupDetail1;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<FormGroupDetail2> GetFormGroupDetail2(string hdrTableName, string childTableName, int empID, int? formGroupID)
        {
            log.Info($"AppraisalFormService/GetFormGroupDetail2/{hdrTableName}/{childTableName}/{empID}");
            try
            {
                IEnumerable<FormGroupDetail2> formGroupDetail2 = Enumerable.Empty<FormGroupDetail2>();
                var formGroupDetail2DTO = appraisalRepo.GetFormGroupDetail2(hdrTableName, childTableName, empID, formGroupID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetFormGroupDetail2_Result, FormGroupDetail2>()
                    .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activity));

                });
                formGroupDetail2 = Mapper.Map<List<FormGroupDetail2>>(formGroupDetail2DTO);
                return formGroupDetail2;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool AppraisalDataExists(int empID, int formID, string reportingYear)
        {
            log.Info($"AppraisalFormService/AppraisalDataExists");
            try
            {
                return genericRepo.Exists<DTOModel.AppraisalFormHdr>(x => x.ReportingYr == reportingYear && x.EmployeeID == empID && x.FormID == formID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<FormTrainingDtls> GetFormTrainingDetail(string hdrTableName, string childTableName, int empID, int? formGroupID, int? formID, string ReportingYr)
        {
            log.Info($"AppraisalFormService/GetFormTrainingDetail/{hdrTableName}/{childTableName}/{empID}");
            try
            {
                IEnumerable<FormTrainingDtls> formGroupTrainingDtls = Enumerable.Empty<FormTrainingDtls>();
                var formGroupDetail1DTO = appraisalRepo.GetFormTrainingDtls(hdrTableName, childTableName, empID, formGroupID, formID, ReportingYr);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetFormTrainingDtls_Result, FormTrainingDtls>();
                });
                formGroupTrainingDtls = Mapper.Map<List<FormTrainingDtls>>(formGroupDetail1DTO);
                return formGroupTrainingDtls;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateAppraisalFormHdr(Model.AppraisalForm appraisal)
        {
            log.Info($"AppraisalFormService/UpdateAppraisalFormHdr");
            try
            {
                var objHdr = genericRepo.Get<DTOModel.AppraisalFormHdr>(x => x.EmployeeID == appraisal.EmployeeID && x.ReportingYr == appraisal.ReportingYr
                  && x.FormID == appraisal.FormID && x.FormGroupID == appraisal.FormGroupID).FirstOrDefault();
                if (objHdr != null)
                {
                    objHdr.StatusID = appraisal.FormState;
                    genericRepo.Update<DTOModel.AppraisalFormHdr>(objHdr);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Form A
        /// <summary>
        /// Always Created By Employee
        /// </summary>
        /// <param name="appraisalForm"></param>
        /// <returns></returns>
        public bool InsertFormAData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/InsertFormAData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupAHdr>()
                     .ForMember(d => d.FormID, o => o.UseValue(appraisalForm.FormID));

                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupAHdr>(appraisalForm.formGroupAHdr);
                if (dtoappraisalForm != null)
                    genericRepo.Insert<DTOModel.FormGroupAHdr>(dtoappraisalForm);
                int result = dtoappraisalForm.FormGroupID;
                if (result > 0)
                {

                    #region Add Apprasial Form Header details ============

                    if (result > 0)
                        AddAppraisalFormHeader(new AppraisalFormHdr
                        {
                            CreatedBy = appraisalForm.formGroupAHdr.CreatedBy,
                            CreatedOn = appraisalForm.formGroupAHdr.CreatedOn,
                            EmployeeID = appraisalForm.formGroupAHdr.EmployeeID,
                            ReportingYr = appraisalForm.formGroupAHdr.ReportingYr,
                            FormID = appraisalForm.FormID,
                            FormGroupID = result,
                            StatusID = appraisalForm.formGroupAHdr.FormState
                        });
                    #endregion

                    #region Insert FormGroupADetail1
                    var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupADetail1>()
                        .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                        .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                        .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                        .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                        .ForAllOtherMembers(d => d.Ignore())
                        ;
                    });
                    var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupADetail1>>(formGroupADetail1);
                    if (dtoFormGroupADtls != null)
                        genericRepo.AddMultipleEntity<DTOModel.FormGroupADetail1>(dtoFormGroupADtls);
                    #endregion
                    if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                    {
                        appraisalForm._ProcessWorkFlow.ReferenceID = result;
                        AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                        SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                        SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                    }
                }
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UpdateFormAData(AppraisalForm appraisalForm, string reportingType)
        {
            log.Info($"AppraisalFormService/UpdateFormAData");
            bool flag = false;
            try
            {
                if (appraisalForm.submittedBy == Model.SubmittedBy.Employee)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupAHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                        .ForMember(d => d.PART5_1, o => o.MapFrom(s => s.PART5_1))
                        .ForMember(d => d.PART5_2, o => o.MapFrom(s => s.PART5_2))
                        .ForMember(d => d.PART5_3, o => o.MapFrom(s => s.PART5_3))
                        .ForMember(d => d.PART5_4, o => o.MapFrom(s => s.PART5_4))
                        .ForMember(d => d.PART5_5_Weightage, o => o.MapFrom(s => s.PART5_5_Weightage))
                        .ForMember(d => d.PART5_5_Remark, o => o.MapFrom(s => s.PART5_5_Remark))
                        .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                        .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                        .ForMember(d => d.FormState, o => o.MapFrom(s => s.FormState))
                        ;
                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupAHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupAHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);
                        #region Insert FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupADetail1>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupADetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupADetail1>>(formGroupADetail1);
                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupADetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupADetail1>(dtoFormGroupADtls);

                        #endregion


                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                            SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                            SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                else
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupAHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        //.ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        //.ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.formGroupAHdr.FormGroupID))
                        .ForMember(d => d.PART4_1_Grade, o => o.MapFrom(s => (int)s.Part4_1_Gr))
                        .ForMember(d => d.PART4_2_Grade, o => o.MapFrom(s => (int)s.Part4_2_Gr))
                        .ForMember(d => d.PART4_3_Grade, o => o.MapFrom(s => (int)s.Part4_3_Gr))
                        .ForMember(d => d.PART4_4_Grade, o => o.MapFrom(s => (int)s.FormPart4Integrity))
                            //.ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.formGroupAHdr.CreatedOn))
                            //.ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.formGroupAHdr.CreatedBy))
                            //.ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.formGroupAHdr.UpdatedBy))
                            //.ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.formGroupAHdr.UpdatedOn))
                            //.ForMember(d => d.IsDeleted, o => o.MapFrom(m => m.IsDeleted))
                            //.ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.formGroupAHdr.ReportingYr))
                            ;

                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupAHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupAHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);

                        #region Update FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupADetail1>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupADetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdateBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupADetail1>>(formGroupADetail1);
                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupADetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupADetail1>(dtoFormGroupADtls);
                        #endregion

                        #region Update FormGroupADetail2
                        var getFormGroupADetail2 = genericRepo.Get<DTOModel.FormGroupADetail2>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail2 = appraisalForm.formGroupADetail2List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail2, DTOModel.FormGroupADetail2>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdatedBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.ActivityID))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result));

                        });
                        var dtoFormDetail2 = Mapper.Map<List<DTOModel.FormGroupADetail2>>(formGroupADetail2);
                        if (getFormGroupADetail2 != null && getFormGroupADetail2.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupADetail2>(getFormGroupADetail2);
                        if (dtoFormDetail2 != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupADetail2>(dtoFormDetail2);
                        #endregion

                        #region Update FormGroupTrainingDtls
                        var getFormGroupTrainingDtl = genericRepo.Get<DTOModel.FormGroupTrainingDtl>(x => x.ReportingYr == appraisalForm.formGroupAHdr.ReportingYr && x.EmployeeID == dtoappraisalForm.EmployeeID && x.FormID == dtoappraisalForm.FormID && x.FormGroupID == result).ToList();
                        var formGroupTrainingDtls = appraisalForm.formGroupATrainingDtls;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormTrainingDtls, DTOModel.FormGroupTrainingDtl>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.TrainingID, o => o.MapFrom(s => (int)s.FormTraining))
                            .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForMember(d => d.FormID, o => o.UseValue(dtoappraisalForm.FormID))
                            .ForMember(d => d.EmployeeID, o => o.UseValue(dtoappraisalForm.EmployeeID))
                           .ForMember(d => d.ReportingYr, o => o.UseValue(appraisalForm.formGroupAHdr.ReportingYr))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoformGroupTrainingDtls = Mapper.Map<List<DTOModel.FormGroupTrainingDtl>>(formGroupTrainingDtls);
                        if (getFormGroupTrainingDtl != null && getFormGroupTrainingDtl.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupTrainingDtl>(getFormGroupTrainingDtl);
                        if (dtoformGroupTrainingDtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupTrainingDtl>(dtoformGroupTrainingDtls);
                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReporting ||
                            appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReviewer ||
                            appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);

                            if (appraisalForm._AutoPushWorkFlow.Count > 0)
                                AddAutoPushWorkFlow(appraisalForm._AutoPushWorkFlow);

                            if (appraisalForm.formGroupAHdr.FormState != (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                                SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                flag = true;
            }

            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool IsTrainingSubmitted(int empID, string reportingYear)
        {
            log.Info($"AppraisalFormService/IsTrainingSubmitted");
            try
            {
                return genericRepo.Exists<DTOModel.APARSkillSetFormHdr>(x => x.ReportingYr == reportingYear && x.EmployeeID == empID && !x.IsDeleted);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Form B
        public bool InsertFormBData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/InsertFormAData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupBHdr>()
                    .ForMember(d => d.FormID, o => o.UseValue(appraisalForm.FormID))
                    ;

                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupBHdr>(appraisalForm.formGroupAHdr);
                if (dtoappraisalForm != null)
                    genericRepo.Insert<DTOModel.FormGroupBHdr>(dtoappraisalForm);
                int result = dtoappraisalForm.FormGroupID;
                if (result > 0)
                {
                    #region Add Apprasial Form Header details ============

                    if (result > 0)
                        AddAppraisalFormHeader(new AppraisalFormHdr
                        {
                            CreatedBy = appraisalForm.formGroupAHdr.CreatedBy,
                            CreatedOn = appraisalForm.formGroupAHdr.CreatedOn,
                            EmployeeID = appraisalForm.formGroupAHdr.EmployeeID,
                            ReportingYr = appraisalForm.formGroupAHdr.ReportingYr,
                            FormID = appraisalForm.FormID,
                            FormGroupID = result,
                            StatusID = appraisalForm.formGroupAHdr.FormState
                        });
                    #endregion

                    #region Insert FormGroupADetail1
                    var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupBDetail1>()
                        .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                        .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                        .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                        .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                        .ForAllOtherMembers(d => d.Ignore())
                        ;
                    });
                    var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupBDetail1>>(formGroupADetail1);
                    if (dtoFormGroupADtls != null)
                        genericRepo.AddMultipleEntity<DTOModel.FormGroupBDetail1>(dtoFormGroupADtls);
                    #endregion
                    if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                    {
                        appraisalForm._ProcessWorkFlow.ReferenceID = result;
                        AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                        SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                        SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                    }
                }
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UpdateFormBData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/UpdateFormAData");
            bool flag = false;
            try
            {
                if (appraisalForm.submittedBy == Model.SubmittedBy.Employee)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupBHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                        .ForMember(d => d.PART5_1, o => o.MapFrom(s => s.PART5_1))
                        .ForMember(d => d.PART5_2, o => o.MapFrom(s => s.PART5_2))
                        .ForMember(d => d.PART5_3, o => o.MapFrom(s => s.PART5_3))
                        .ForMember(d => d.PART5_4, o => o.MapFrom(s => s.PART5_4))
                        .ForMember(d => d.PART5_5_Weightage, o => o.MapFrom(s => s.PART5_5_Weightage))
                        .ForMember(d => d.PART5_5_Remark, o => o.MapFrom(s => s.PART5_5_Remark))
                        .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                        .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                        .ForMember(d => d.FormState, o => o.MapFrom(s => s.FormState))
                        ;
                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupBHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupBHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);
                        #region Insert FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupBDetail1>(x => x.FormGroupID == result);
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupBDetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupBDetail1>>(formGroupADetail1);
                        genericRepo.DeleteAll<DTOModel.FormGroupBDetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupBDetail1>(dtoFormGroupADtls);

                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                            SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                            SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                else
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupBHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        //.ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        //.ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.formGroupAHdr.FormGroupID))
                        .ForMember(d => d.PART4_1_Grade, o => o.MapFrom(s => (int)s.Part4_1_Gr))
                        .ForMember(d => d.PART4_2_Grade, o => o.MapFrom(s => (int)s.Part4_2_Gr))
                        .ForMember(d => d.PART4_3_Grade, o => o.MapFrom(s => (int)s.Part4_3_Gr))
                        .ForMember(d => d.PART4_4_Grade, o => o.MapFrom(s => (int)s.FormPart4Integrity))
                            //.ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.formGroupAHdr.CreatedOn))
                            //.ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.formGroupAHdr.CreatedBy))
                            //.ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.formGroupAHdr.UpdatedBy))
                            //.ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.formGroupAHdr.UpdatedOn))
                            //.ForMember(d => d.IsDeleted, o => o.MapFrom(m => m.IsDeleted))
                            //.ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.formGroupAHdr.ReportingYr))
                            ;

                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupBHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupBHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);
                        #region Update FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupBDetail1>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupBDetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdateBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupBDetail1>>(formGroupADetail1);
                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupBDetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupBDetail1>(dtoFormGroupADtls);
                        #endregion

                        #region Update FormGroupADetail2
                        var getFormGroupADetail2 = genericRepo.Get<DTOModel.FormGroupBDetail2>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail2 = appraisalForm.formGroupADetail2List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail2, DTOModel.FormGroupBDetail2>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdatedBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.ActivityID))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result));

                        });
                        var dtoFormDetail2 = Mapper.Map<List<DTOModel.FormGroupBDetail2>>(formGroupADetail2);
                        if (getFormGroupADetail2 != null && getFormGroupADetail2.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupBDetail2>(getFormGroupADetail2);
                        if (dtoFormDetail2 != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupBDetail2>(dtoFormDetail2);
                        #endregion

                        #region Update FormGroupTrainingDtls
                        var getFormGroupTrainingDtl = genericRepo.Get<DTOModel.FormGroupTrainingDtl>(x => x.ReportingYr == appraisalForm.formGroupAHdr.ReportingYr &&  x.EmployeeID == dtoappraisalForm.EmployeeID && x.FormID == dtoappraisalForm.FormID && x.FormGroupID == result).ToList();
                        var formGroupTrainingDtls = appraisalForm.formGroupATrainingDtls;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormTrainingDtls, DTOModel.FormGroupTrainingDtl>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.TrainingID, o => o.MapFrom(s => (int)s.FormTraining))
                            .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForMember(d => d.FormID, o => o.UseValue(dtoappraisalForm.FormID))
                            .ForMember(d => d.EmployeeID, o => o.UseValue(dtoappraisalForm.EmployeeID))
                            .ForMember(d => d.ReportingYr, o => o.UseValue(appraisalForm.formGroupAHdr.ReportingYr))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoformGroupTrainingDtls = Mapper.Map<List<DTOModel.FormGroupTrainingDtl>>(formGroupTrainingDtls);
                        if (getFormGroupTrainingDtl != null && getFormGroupTrainingDtl.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupTrainingDtl>(getFormGroupTrainingDtl);
                        if (dtoformGroupTrainingDtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupTrainingDtl>(dtoformGroupTrainingDtls);
                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReporting ||
                            appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReviewer ||
                            appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);

                            ///===== Adding log into Process Work Flow , when form auto push case occured..
                            if (appraisalForm._AutoPushWorkFlow.Count > 0)
                                AddAutoPushWorkFlow(appraisalForm._AutoPushWorkFlow);

                            if (appraisalForm.formGroupAHdr.FormState != (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                                SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                flag = true;
            }

            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        #endregion

        #region Form C

        public bool InsertFormCData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/InsertFormAData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupCHdr>()
                    .ForMember(d => d.FormID, o => o.UseValue(appraisalForm.FormID))
                    ;

                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupCHdr>(appraisalForm.formGroupAHdr);
                if (dtoappraisalForm != null)
                    genericRepo.Insert<DTOModel.FormGroupCHdr>(dtoappraisalForm);
                int result = dtoappraisalForm.FormGroupID;
                if (result > 0)
                {
                    #region Add Apprasial Form Header details ============

                    if (result > 0)
                        AddAppraisalFormHeader(new AppraisalFormHdr
                        {
                            CreatedBy = appraisalForm.formGroupAHdr.CreatedBy,
                            CreatedOn = appraisalForm.formGroupAHdr.CreatedOn,
                            EmployeeID = appraisalForm.formGroupAHdr.EmployeeID,
                            ReportingYr = appraisalForm.formGroupAHdr.ReportingYr,
                            FormID = appraisalForm.FormID,
                            FormGroupID = result,
                            StatusID = appraisalForm.formGroupAHdr.FormState
                        });
                    #endregion

                    #region Insert FormGroupADetail1
                    var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupCDetail1>()
                        .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                        .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                        .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                        .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                        .ForAllOtherMembers(d => d.Ignore())
                        ;
                    });
                    var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupCDetail1>>(formGroupADetail1);
                    if (dtoFormGroupADtls != null)
                        genericRepo.AddMultipleEntity<DTOModel.FormGroupCDetail1>(dtoFormGroupADtls);
                    #endregion
                    if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                    {
                        appraisalForm._ProcessWorkFlow.ReferenceID = result;
                        AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                        SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                        SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                    }
                }
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UpdateFormCData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/UpdateFormAData");
            bool flag = false;
            try
            {
                if (appraisalForm.submittedBy == Model.SubmittedBy.Employee)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupCHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                        .ForMember(d => d.PART5_1, o => o.MapFrom(s => s.PART5_1))
                        .ForMember(d => d.PART5_2, o => o.MapFrom(s => s.PART5_2))
                        .ForMember(d => d.PART5_3, o => o.MapFrom(s => s.PART5_3))
                        .ForMember(d => d.PART5_4, o => o.MapFrom(s => s.PART5_4))
                        .ForMember(d => d.PART5_5_Weightage, o => o.MapFrom(s => s.PART5_5_Weightage))
                        .ForMember(d => d.PART5_5_Remark, o => o.MapFrom(s => s.PART5_5_Remark))
                        .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                        .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                        .ForMember(d => d.FormState, o => o.MapFrom(s => s.FormState))
                        ;
                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupCHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupCHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);

                        #region Insert FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupCDetail1>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupCDetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupCDetail1>>(formGroupADetail1);
                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupCDetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupCDetail1>(dtoFormGroupADtls);

                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                            SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                            SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                else
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupCHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        //.ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        //.ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.formGroupAHdr.FormGroupID))
                        .ForMember(d => d.PART4_1_Grade, o => o.MapFrom(s => (int)s.Part4_1_Gr))
                        .ForMember(d => d.PART4_2_Grade, o => o.MapFrom(s => (int)s.Part4_2_Gr))
                        .ForMember(d => d.PART4_3_Grade, o => o.MapFrom(s => (int)s.Part4_3_Gr))
                        .ForMember(d => d.PART4_4_Grade, o => o.MapFrom(s => (int)s.FormPart4Integrity))
                            //.ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.formGroupAHdr.CreatedOn))
                            //.ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.formGroupAHdr.CreatedBy))
                            //.ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.formGroupAHdr.UpdatedBy))
                            //.ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.formGroupAHdr.UpdatedOn))
                            //.ForMember(d => d.IsDeleted, o => o.MapFrom(m => m.IsDeleted))
                            //.ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.formGroupAHdr.ReportingYr))
                            ;

                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupCHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupCHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);

                        #region Update FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupCDetail1>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupCDetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdateBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupCDetail1>>(formGroupADetail1);
                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupCDetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupCDetail1>(dtoFormGroupADtls);
                        #endregion

                        #region Update FormGroupADetail2
                        var getFormGroupADetail2 = genericRepo.Get<DTOModel.FormGroupCDetail2>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail2 = appraisalForm.formGroupADetail2List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail2, DTOModel.FormGroupCDetail2>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdatedBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.ActivityID))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result));

                        });
                        var dtoFormDetail2 = Mapper.Map<List<DTOModel.FormGroupCDetail2>>(formGroupADetail2);
                        if (getFormGroupADetail2 != null && getFormGroupADetail2.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupCDetail2>(getFormGroupADetail2);
                        if (dtoFormDetail2 != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupCDetail2>(dtoFormDetail2);
                        #endregion

                        #region Update FormGroupTrainingDtls
                        var getFormGroupTrainingDtl = genericRepo.Get<DTOModel.FormGroupTrainingDtl>(x => x.ReportingYr == appraisalForm.formGroupAHdr.ReportingYr && x.EmployeeID == dtoappraisalForm.EmployeeID && x.FormID == dtoappraisalForm.FormID && x.FormGroupID == result).ToList();
                        var formGroupTrainingDtls = appraisalForm.formGroupATrainingDtls;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormTrainingDtls, DTOModel.FormGroupTrainingDtl>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.TrainingID, o => o.MapFrom(s => (int)s.FormTraining))
                            .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForMember(d => d.FormID, o => o.UseValue(dtoappraisalForm.FormID))
                            .ForMember(d => d.EmployeeID, o => o.UseValue(dtoappraisalForm.EmployeeID))
                            .ForMember(d => d.ReportingYr, o => o.UseValue(appraisalForm.formGroupAHdr.ReportingYr))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoformGroupTrainingDtls = Mapper.Map<List<DTOModel.FormGroupTrainingDtl>>(formGroupTrainingDtls);
                        if (getFormGroupTrainingDtl != null && getFormGroupTrainingDtl.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupTrainingDtl>(getFormGroupTrainingDtl);
                        if (dtoformGroupTrainingDtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupTrainingDtl>(dtoformGroupTrainingDtls);
                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReporting ||
                            appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReviewer ||
                            appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);

                            ///===== Adding log into Process Work Flow , when form auto push case occured..
                            if (appraisalForm._AutoPushWorkFlow.Count > 0)
                                AddAutoPushWorkFlow(appraisalForm._AutoPushWorkFlow);

                            if (appraisalForm.formGroupAHdr.FormState != (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                                SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                flag = true;
            }

            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        #endregion

        #region Form D

        public FormGroupDHdr GetFormGroupDHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            log.Info($"AppraisalFormService/GetFormGroupDHdrDetail/{tableName}/{branchID}/{empID}/{reportingYear}/{formID}");
            try
            {
                FormGroupDHdr formGroupDHdr = new FormGroupDHdr();
                var formGroupGHdrDTO = appraisalRepo.GetFormGroupDHdrDetail(tableName, branchID, empID, reportingYear, formID);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetFormGroupDHdrDetail_Result, FormGroupDHdr>()
                       .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name));
                  }
                  );
                formGroupDHdr = Mapper.Map<FormGroupDHdr>(formGroupGHdrDTO.FirstOrDefault());
                return formGroupDHdr;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool InsertFormDData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/InsertFormDData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupDHdr, DTOModel.FormGroupDHdr>();
                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupDHdr>(appraisalForm.formGroupDHdr);

                genericRepo.Insert<DTOModel.FormGroupDHdr>(dtoappraisalForm);
                int result = dtoappraisalForm.FormGroupID;

                if (result > 0)
                {
                    #region Add Apprasial Form Header details ============

                    AddAppraisalFormHeader(new AppraisalFormHdr
                    {
                        CreatedBy = appraisalForm.formGroupDHdr.CreatedBy,
                        CreatedOn = appraisalForm.formGroupDHdr.CreatedOn,
                        EmployeeID = appraisalForm.formGroupDHdr.EmployeeID,
                        ReportingYr = appraisalForm.formGroupDHdr.ReportingYr,
                        FormID = appraisalForm.FormID,
                        FormGroupID = result,
                        StatusID = (int)appraisalForm.formGroupDHdr.FormState
                    });
                    #endregion

                    if (appraisalForm.formGroupDHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                    {
                        appraisalForm._ProcessWorkFlow.ReferenceID = result;
                        AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                        SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                        SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupDHdr.EmployeeCode, appraisalForm.formGroupDHdr.EmployeeName);
                    }
                }

                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }


        public bool UpdateFormDData(AppraisalForm appraisalForm)
        {

            log.Info($"AppraisalFormService/UpdateFormDData");
            bool flag = false;
            try
            {
                if (appraisalForm.submittedBy == Model.SubmittedBy.Employee)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupDHdr, DTOModel.FormGroupDHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                        .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                        .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                        .ForMember(d => d.FormState, o => o.MapFrom(s => s.FormState));
                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupDHdr>(appraisalForm.formGroupDHdr);
                    genericRepo.Update<DTOModel.FormGroupDHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;

                    if (result > 0)
                    {
                        #region   //=======  Updating Appraisal Form Status ===============

                        appraisalForm.FormGroupID = appraisalForm.formGroupDHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupDHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupDHdr.ReportingYr;
                        appraisalForm.FormState = (int)appraisalForm.formGroupDHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);

                        #endregion


                        if (appraisalForm.formGroupDHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                            SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                            SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupDHdr.EmployeeCode, appraisalForm.formGroupDHdr.EmployeeName);
                        }
                    }
                }
                else
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<FormGroupDHdr, DTOModel.FormGroupDHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.PART4_1_Grade, o => o.MapFrom(s => (int)s.Part4_1_Gr))
                        //  .ForMember(d => d.PART4_2_Grade, o => o.MapFrom(s => (int)s.Part4_2_Gr))
                        .ForMember(d => d.PART4_3_Grade, o => o.MapFrom(s => (int)s.Part4_3_Gr))
                        .ForMember(d => d.PART4_4_Grade, o => o.MapFrom(s => (int)s.FormPart4Integrity));

                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupDHdr>(appraisalForm.formGroupDHdr);

                    dtoappraisalForm.PART5_5_Grade = dtoappraisalForm.PART4_6_Grade;
                    genericRepo.Update<DTOModel.FormGroupDHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        #region   //=======  Updating Appraisal Form Status ===============

                        appraisalForm.FormGroupID = appraisalForm.formGroupDHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupDHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupDHdr.ReportingYr;
                        appraisalForm.FormState = (int)appraisalForm.formGroupDHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);

                        #endregion
                        #region Update FormGroupTrainingDtls
                        var getFormGroupTrainingDtl = genericRepo.Get<DTOModel.FormGroupTrainingDtl>(x => x.ReportingYr == appraisalForm.formGroupDHdr.ReportingYr && x.EmployeeID == dtoappraisalForm.EmployeeID && x.FormID == dtoappraisalForm.FormID && x.FormGroupID == result).ToList();
                        var formGroupTrainingDtls = appraisalForm.formGroupATrainingDtls;

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormTrainingDtls, DTOModel.FormGroupTrainingDtl>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.TrainingID, o => o.MapFrom(s => (int)s.FormTraining))
                            .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForMember(d => d.FormID, o => o.UseValue(dtoappraisalForm.FormID))
                            .ForMember(d => d.EmployeeID, o => o.UseValue(dtoappraisalForm.EmployeeID))
                           .ForMember(d => d.ReportingYr, o => o.UseValue(appraisalForm.formGroupDHdr.ReportingYr))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoformGroupTrainingDtls = Mapper.Map<List<DTOModel.FormGroupTrainingDtl>>(formGroupTrainingDtls);
                        if (getFormGroupTrainingDtl != null && getFormGroupTrainingDtl.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupTrainingDtl>(getFormGroupTrainingDtl);
                        if (dtoformGroupTrainingDtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupTrainingDtl>(dtoformGroupTrainingDtls);
                        #endregion

                        if (appraisalForm.formGroupDHdr.FormState == (int)AppraisalFormState.SubmitedByReporting ||
                            appraisalForm.formGroupDHdr.FormState == (int)AppraisalFormState.SubmitedByReviewer ||
                            appraisalForm.formGroupDHdr.FormState == (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);

                            ///===== Adding log into Process Work Flow , when form auto push case occured..
                            if (appraisalForm._AutoPushWorkFlow.Count > 0)
                                AddAutoPushWorkFlow(appraisalForm._AutoPushWorkFlow);

                            if (appraisalForm.formGroupDHdr.FormState != (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                                SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupDHdr.EmployeeCode, appraisalForm.formGroupDHdr.EmployeeName);
                        }
                    }
                }
                flag = true;
            }

            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        #endregion

        #region Form E
        public bool InsertFormEData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/InsertFormEData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupEHdr>()
                    .ForMember(d => d.FormID, o => o.UseValue(appraisalForm.FormID))
                    ;

                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupEHdr>(appraisalForm.formGroupAHdr);
                if (dtoappraisalForm != null)
                    genericRepo.Insert<DTOModel.FormGroupEHdr>(dtoappraisalForm);
                int result = dtoappraisalForm.FormGroupID;
                if (result > 0)
                {
                    #region Add Apprasial Form Header details ============

                    if (result > 0)
                        AddAppraisalFormHeader(new AppraisalFormHdr
                        {
                            CreatedBy = appraisalForm.formGroupAHdr.CreatedBy,
                            CreatedOn = appraisalForm.formGroupAHdr.CreatedOn,
                            EmployeeID = appraisalForm.formGroupAHdr.EmployeeID,
                            ReportingYr = appraisalForm.formGroupAHdr.ReportingYr,
                            FormID = appraisalForm.FormID,
                            FormGroupID = result,
                            StatusID = appraisalForm.formGroupAHdr.FormState
                        });
                    #endregion

                    #region Insert FormGroupADetail1
                    var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupEDetail1>()
                        .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                        .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                        .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                        .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                        .ForAllOtherMembers(d => d.Ignore())
                        ;
                    });
                    var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupEDetail1>>(formGroupADetail1);
                    if (dtoFormGroupADtls != null)
                        genericRepo.AddMultipleEntity<DTOModel.FormGroupEDetail1>(dtoFormGroupADtls);
                    #endregion
                    if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                    {
                        appraisalForm._ProcessWorkFlow.ReferenceID = result;
                        AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                        SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                        SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                    }
                }
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UpdateFormEData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/UpdateFormAData");
            bool flag = false;
            try
            {
                if (appraisalForm.submittedBy == Model.SubmittedBy.Employee)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupEHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                        .ForMember(d => d.PART5_1, o => o.MapFrom(s => s.PART5_1))
                        .ForMember(d => d.PART5_2, o => o.MapFrom(s => s.PART5_2))
                        .ForMember(d => d.PART5_3, o => o.MapFrom(s => s.PART5_3))
                        .ForMember(d => d.PART5_4, o => o.MapFrom(s => s.PART5_4))
                        .ForMember(d => d.PART5_5_Weightage, o => o.MapFrom(s => s.PART5_5_Weightage))
                        .ForMember(d => d.PART5_5_Remark, o => o.MapFrom(s => s.PART5_5_Remark))
                        .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                        .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
                        .ForMember(d => d.FormState, o => o.MapFrom(s => s.FormState))
                        ;
                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupEHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupEHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);

                        #region Insert FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupEDetail1>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupEDetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupEDetail1>>(formGroupADetail1);
                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupEDetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupEDetail1>(dtoFormGroupADtls);

                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                            SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                            SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                else
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupEHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        //.ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        //.ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.formGroupAHdr.FormGroupID))
                        .ForMember(d => d.PART4_1_Grade, o => o.MapFrom(s => (int)s.Part4_1_Gr))
                        .ForMember(d => d.PART4_2_Grade, o => o.MapFrom(s => (int)s.Part4_2_Gr))
                        .ForMember(d => d.PART4_3_Grade, o => o.MapFrom(s => (int)s.Part4_3_Gr))
                        .ForMember(d => d.PART4_4_Grade, o => o.MapFrom(s => (int)s.FormPart4Integrity))
                            //.ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.formGroupAHdr.CreatedOn))
                            //.ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.formGroupAHdr.CreatedBy))
                            //.ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.formGroupAHdr.UpdatedBy))
                            //.ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.formGroupAHdr.UpdatedOn))
                            //.ForMember(d => d.IsDeleted, o => o.MapFrom(m => m.IsDeleted))
                            //.ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.formGroupAHdr.ReportingYr))
                            ;

                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupEHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupEHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);

                        #region Update FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupEDetail1>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupEDetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdateBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupEDetail1>>(formGroupADetail1);
                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupEDetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupEDetail1>(dtoFormGroupADtls);
                        #endregion

                        #region Update FormGroupADetail2
                        var getFormGroupADetail2 = genericRepo.Get<DTOModel.FormGroupEDetail2>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail2 = appraisalForm.formGroupADetail2List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail2, DTOModel.FormGroupEDetail2>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdatedBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.ActivityID))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result));

                        });
                        var dtoFormDetail2 = Mapper.Map<List<DTOModel.FormGroupEDetail2>>(formGroupADetail2);
                        if (getFormGroupADetail2 != null && getFormGroupADetail2.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupEDetail2>(getFormGroupADetail2);
                        if (dtoFormDetail2 != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupEDetail2>(dtoFormDetail2);
                        #endregion

                        #region Update FormGroupTrainingDtls
                        var getFormGroupTrainingDtl = genericRepo.Get<DTOModel.FormGroupTrainingDtl>(x => x.ReportingYr == appraisalForm.formGroupAHdr.ReportingYr && x.EmployeeID == dtoappraisalForm.EmployeeID && x.FormID == dtoappraisalForm.FormID && x.FormGroupID == result).ToList();
                        var formGroupTrainingDtls = appraisalForm.formGroupATrainingDtls;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormTrainingDtls, DTOModel.FormGroupTrainingDtl>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.TrainingID, o => o.MapFrom(s => (int)s.FormTraining))
                            .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForMember(d => d.FormID, o => o.UseValue(dtoappraisalForm.FormID))
                            .ForMember(d => d.EmployeeID, o => o.UseValue(dtoappraisalForm.EmployeeID))
                            .ForMember(d => d.ReportingYr, o => o.UseValue(appraisalForm.formGroupAHdr.ReportingYr))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoformGroupTrainingDtls = Mapper.Map<List<DTOModel.FormGroupTrainingDtl>>(formGroupTrainingDtls);
                        if (getFormGroupTrainingDtl != null && getFormGroupTrainingDtl.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupTrainingDtl>(getFormGroupTrainingDtl);
                        if (dtoformGroupTrainingDtls != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupTrainingDtl>(dtoformGroupTrainingDtls);
                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReporting ||
                            appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReviewer ||
                            appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);

                            ///===== Adding log into Process Work Flow , when form auto push case occured..
                            if (appraisalForm._AutoPushWorkFlow.Count > 0)
                                AddAutoPushWorkFlow(appraisalForm._AutoPushWorkFlow);


                            if (appraisalForm.formGroupAHdr.FormState != (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                                SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                flag = true;
            }

            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        #endregion

        #region Form F

        public FormGroupAHdr GetFormGroupFHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            log.Info($"AppraisalFormService/GetFormGroupFHdrDetail/{tableName}/{branchID}/{empID}/{reportingYear}/{formID}");
            try
            {
                FormGroupAHdr formGraoupHdr = new FormGroupAHdr();
                var formGroupHdrDTO = appraisalRepo.GetFormGroupFHdrDetail(tableName, branchID, empID, reportingYear, formID);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetFormGroupFHdrDetail_Result, FormGroupAHdr>()
                     .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name))
                     .ForMember(d => d.PART5_5, o => o.MapFrom(s => s.PART5_5))
                     .ForMember(d => d.PART5_6_Weightage, o => o.MapFrom(s => s.PART5_6_Weightage))
                     .ForMember(d => d.PART5_6_Remark, o => o.MapFrom(s => s.PART5_6_Remark));
                  });

                formGraoupHdr = Mapper.Map<List<FormGroupAHdr>>(formGroupHdrDTO).FirstOrDefault();
                return formGraoupHdr;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertFormFData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/InsertFormFData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupFHdr>();
                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupFHdr>(appraisalForm.formGroupAHdr);
                genericRepo.Insert<DTOModel.FormGroupFHdr>(dtoappraisalForm);
                int result = dtoappraisalForm.FormGroupID;

                if (result > 0)
                {
                    #region Add Apprasial Form Header details ============
                    AddAppraisalFormHeader(new AppraisalFormHdr
                    {
                        CreatedBy = appraisalForm.formGroupAHdr.CreatedBy,
                        CreatedOn = appraisalForm.formGroupAHdr.CreatedOn,
                        EmployeeID = appraisalForm.formGroupAHdr.EmployeeID,
                        ReportingYr = appraisalForm.formGroupAHdr.ReportingYr,
                        FormID = appraisalForm.FormID,
                        FormGroupID = result,
                        StatusID = appraisalForm.formGroupAHdr.FormState
                    });
                    #endregion

                    #region Insert FormGroupADetail1
                    var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupFDetail1>()
                        .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                        .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                        .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                        .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                        .ForAllOtherMembers(d => d.Ignore());

                    });
                    var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupFDetail1>>(formGroupADetail1);
                    if (dtoFormGroupADtls != null)
                        genericRepo.AddMultipleEntity<DTOModel.FormGroupFDetail1>(dtoFormGroupADtls);

                    #endregion

                    if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                    {
                        appraisalForm._ProcessWorkFlow.ReferenceID = result;
                        AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                        SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                    }
                }
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }


        public bool UpdateFormFData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/UpdateFormFData");
            bool flag = false;
            try
            {
                if (appraisalForm.submittedBy == Model.SubmittedBy.Employee)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupFHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                        .ForMember(d => d.PART5_1, o => o.MapFrom(s => s.PART5_1))
                        .ForMember(d => d.PART5_2, o => o.MapFrom(s => s.PART5_2))
                        .ForMember(d => d.PART5_3, o => o.MapFrom(s => s.PART5_3))
                        .ForMember(d => d.PART5_4, o => o.MapFrom(s => s.PART5_4))
                        //.ForMember(d => d.PART5_5_Weightage, o => o.MapFrom(s => s.PART5_5_Weightage))
                        //.ForMember(d => d.PART5_5_Remark, o => o.MapFrom(s => s.PART5_5_Remark))
                        .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
                        .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn));
                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupFHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupFHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);
                        #region Insert FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupFDetail1>(x => x.FormGroupID == result).ToList();
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupFDetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupFDetail1>>(formGroupADetail1);

                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupFDetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null && dtoFormGroupADtls.Count() > 0)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupFDetail1>(dtoFormGroupADtls);

                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                            SendMailMessageToEmployee((int)appraisalForm._ProcessWorkFlow.SenderID);
                            SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                else
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.FormGroupAHdr, DTOModel.FormGroupFHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.PART4_1_Grade, o => o.MapFrom(s => (int)s.Part4_1_Gr))
                        .ForMember(d => d.PART4_2_Grade, o => o.MapFrom(s => (int)s.Part4_2_Gr))
                        .ForMember(d => d.PART4_3_Grade, o => o.MapFrom(s => (int)s.Part4_3_Gr))
                        .ForMember(d => d.PART4_4_Grade, o => o.MapFrom(s => (int)s.FormPart4Integrity));

                    });
                    var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupFHdr>(appraisalForm.formGroupAHdr);
                    genericRepo.Update<DTOModel.FormGroupFHdr>(dtoappraisalForm);
                    int result = dtoappraisalForm.FormGroupID;
                    if (result > 0)
                    {
                        appraisalForm.FormGroupID = appraisalForm.formGroupAHdr.FormGroupID;
                        appraisalForm.FormID = appraisalForm.formGroupAHdr.FormID;
                        appraisalForm.ReportingYr = appraisalForm.formGroupAHdr.ReportingYr;
                        appraisalForm.FormState = appraisalForm.formGroupAHdr.FormState;
                        UpdateAppraisalFormHdr(appraisalForm);

                        #region Update FormGroupADetail1
                        var getFormGroupADetail1 = genericRepo.Get<DTOModel.FormGroupFDetail1>(x => x.FormGroupID == result);
                        var formGroupADetail1 = appraisalForm.formGroupADetail1List;

                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail1, DTOModel.FormGroupFDetail1>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdateBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Achievements, o => o.MapFrom(s => s.Achievements))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.Activities))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoFormGroupADtls = Mapper.Map<List<DTOModel.FormGroupFDetail1>>(formGroupADetail1);

                        if (getFormGroupADetail1 != null && getFormGroupADetail1.Count() > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupFDetail1>(getFormGroupADetail1);
                        if (dtoFormGroupADtls != null && dtoFormGroupADtls.Count() > 0)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupFDetail1>(dtoFormGroupADtls);
                        #endregion

                        #region Update FormGroupADetail2
                        var getFormGroupADetail2 = genericRepo.Get<DTOModel.FormGroupFDetail2>(x => x.FormGroupID == result);

                        var formGroupADetail2 = appraisalForm.formGroupADetail2List;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormGroupDetail2, DTOModel.FormGroupFDetail2>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.UpdatedBy, o => o.UseValue(dtoappraisalForm.UpdatedBy))
                            .ForMember(d => d.UpdatedOn, o => o.UseValue(dtoappraisalForm.UpdatedOn))
                            .ForMember(d => d.Activities, o => o.MapFrom(s => s.ActivityID))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result));

                        });
                        var dtoFormDetail2 = Mapper.Map<List<DTOModel.FormGroupFDetail2>>(formGroupADetail2);

                        if (getFormGroupADetail2 != null && getFormGroupADetail2.Count() > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupFDetail2>(getFormGroupADetail2);
                        if (dtoFormDetail2 != null)
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupFDetail2>(dtoFormDetail2);
                        #endregion

                        #region Update FormGroupTrainingDtls
                        var getFormGroupTrainingDtl = genericRepo.Get<DTOModel.FormGroupTrainingDtl>(x => x.ReportingYr == appraisalForm.formGroupAHdr.ReportingYr && x.EmployeeID == dtoappraisalForm.EmployeeID && x.FormID == dtoappraisalForm.FormID && x.FormGroupID == result).ToList();
                        var formGroupTrainingDtls = appraisalForm.formGroupATrainingDtls;
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<Model.FormTrainingDtls, DTOModel.FormGroupTrainingDtl>()
                            .ForMember(d => d.CreatedOn, o => o.UseValue(dtoappraisalForm.CreatedOn))
                            .ForMember(d => d.CreatedBy, o => o.UseValue(dtoappraisalForm.CreatedBy))
                            .ForMember(d => d.TrainingID, o => o.MapFrom(s => (int)s.FormTraining))
                            .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                            .ForMember(d => d.FormGroupID, o => o.UseValue(result))
                            .ForMember(d => d.EmployeeID, o => o.UseValue(dtoappraisalForm.EmployeeID))
                            .ForMember(d => d.FormID, o => o.UseValue(dtoappraisalForm.FormID))
                             .ForMember(d => d.ReportingYr, o => o.UseValue(appraisalForm.formGroupAHdr.ReportingYr))
                            .ForAllOtherMembers(d => d.Ignore())
                            ;
                        });
                        var dtoformGroupTrainingDtls = Mapper.Map<List<DTOModel.FormGroupTrainingDtl>>(formGroupTrainingDtls);

                        if (getFormGroupTrainingDtl != null && getFormGroupTrainingDtl.Count > 0)
                            genericRepo.DeleteAll<DTOModel.FormGroupTrainingDtl>(getFormGroupTrainingDtl);

                        if (dtoformGroupTrainingDtls != null)
                        {
                            dtoformGroupTrainingDtls = dtoformGroupTrainingDtls.Where(x => x.FormGroupID == appraisalForm.formGroupAHdr.FormGroupID).ToList();
                            genericRepo.AddMultipleEntity<DTOModel.FormGroupTrainingDtl>(dtoformGroupTrainingDtls);
                        }

                        #endregion

                        if (appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReporting ||
                           appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByReviewer ||
                           appraisalForm.formGroupAHdr.FormState == (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                        {
                            appraisalForm._ProcessWorkFlow.ReferenceID = result;
                            AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);

                            ///===== Adding log into Process Work Flow , when form auto push case occured..
                            if (appraisalForm._AutoPushWorkFlow.Count > 0)
                                AddAutoPushWorkFlow(appraisalForm._AutoPushWorkFlow);

                            if (appraisalForm.formGroupAHdr.FormState != (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                                SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupAHdr.EmployeeCode, appraisalForm.formGroupAHdr.EmployeeName);
                        }
                    }
                }
                flag = true;
            }

            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        #endregion

        #region Form H

        public FormGroupHHdr GetFormGroupHHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            log.Info($"AppraisalFormService/GetFormGroupHHdrDetail/{tableName}/{branchID}/{empID}/{reportingYear}/{formID}");
            try
            {
                FormGroupHHdr formGroupHHdr = new FormGroupHHdr();
                var formGroupHdrDTO = appraisalRepo.GetFormGroupHHdrDetail(tableName, branchID, empID, reportingYear, formID);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetFormGroupHHdrDetail_Result, FormGroupHHdr>()
                       .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name));
                  }
                  );
                formGroupHHdr = Mapper.Map<FormGroupHHdr>(formGroupHdrDTO.FirstOrDefault());
                return formGroupHHdr;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool InsertFormHData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/InsertFormHData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupHHdr, DTOModel.FormGroupHHdr>();
                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupHHdr>(appraisalForm.formGroupHHdr);

                genericRepo.Insert<DTOModel.FormGroupHHdr>(dtoappraisalForm);
                int result = dtoappraisalForm.FormGroupID;


                #region Add Apprasial Form Header details ============

                if (result > 0)
                    AddAppraisalFormHeader(new AppraisalFormHdr
                    {
                        CreatedBy = appraisalForm.formGroupHHdr.CreatedBy,
                        CreatedOn = appraisalForm.formGroupHHdr.CreatedOn,
                        EmployeeID = appraisalForm.formGroupHHdr.EmployeeID,
                        ReportingYr = appraisalForm.formGroupHHdr.ReportingYr,
                        FormID = appraisalForm.FormID,
                        FormGroupID = result,
                        StatusID = appraisalForm.formGroupHHdr.FormState
                    });
                #endregion

                if (appraisalForm.formGroupHHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                {
                    appraisalForm._ProcessWorkFlow.ReferenceID = result;
                    AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                }
                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public bool UpdateFormHData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/UpdateFormHData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupHHdr, DTOModel.FormGroupHHdr>();
                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupHHdr>(appraisalForm.formGroupHHdr);
                genericRepo.Update<DTOModel.FormGroupHHdr>(dtoappraisalForm);
                //  int result = dtoappraisalForm.FormGroupID;

                appraisalForm.FormGroupID = appraisalForm.formGroupHHdr.FormGroupID;
                appraisalForm.FormID = appraisalForm.formGroupHHdr.FormID;
                appraisalForm.ReportingYr = appraisalForm.formGroupHHdr.ReportingYr;
                appraisalForm.FormState = appraisalForm.formGroupHHdr.FormState;
                UpdateAppraisalFormHdr(appraisalForm);


                if (appraisalForm.formGroupHHdr.FormState == (int)AppraisalFormState.SubmitedByReporting ||
                           appraisalForm.formGroupHHdr.FormState == (int)AppraisalFormState.SubmitedByReviewer ||
                           appraisalForm.formGroupHHdr.FormState == (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                {
                    appraisalForm._ProcessWorkFlow.ReferenceID = dtoappraisalForm.FormGroupID;
                    AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);

                    ///===== Adding log into Process Work Flow , when form auto push case occured..
                    if (appraisalForm._AutoPushWorkFlow.Count > 0)
                        AddAutoPushWorkFlow(appraisalForm._AutoPushWorkFlow);

                    if (appraisalForm.formGroupHHdr.FormState != (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                        SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupHHdr.EmployeeCode, appraisalForm.formGroupHHdr.EmployeeName);
                }

                flag = true;
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }
        #endregion

        #region Form G

        public bool InsertFormGData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/InsertFormGData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupGHdr, DTOModel.FormGroupGHdr>();
                });
                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupGHdr>(appraisalForm.formGroupGHdr);

                genericRepo.Insert<DTOModel.FormGroupGHdr>(dtoappraisalForm);
                int result = dtoappraisalForm.FormGroupID;

                #region Add Apprasial Form Header details ============
                if (result > 0)
                    AddAppraisalFormHeader(new AppraisalFormHdr
                    {
                        CreatedBy = appraisalForm.formGroupGHdr.CreatedBy,
                        CreatedOn = appraisalForm.formGroupGHdr.CreatedOn,
                        EmployeeID = appraisalForm.formGroupGHdr.EmployeeID,
                        ReportingYr = appraisalForm.formGroupGHdr.ReportingYr,
                        FormID = appraisalForm.FormID,
                        FormGroupID = result,
                        StatusID = appraisalForm.formGroupGHdr.FormState
                    });
                #endregion
                if (appraisalForm.formGroupGHdr.FormState == (int)AppraisalFormState.SubmitedByEmployee)
                {
                    appraisalForm._ProcessWorkFlow.ReferenceID = result;
                    AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);
                    flag = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }

        public FormGroupGHdr GetFormGroupGHdrDetail(string tableName, int? branchID, int? empID, string reportingYear, int? formID)
        {
            log.Info($"AppraisalFormService/GetFormGroupGHdrDetail/{tableName}/{branchID}/{empID}/{reportingYear}/{formID}");
            try
            {
                FormGroupGHdr formGroupGHdr = new FormGroupGHdr();
                var formGroupGHdrDTO = appraisalRepo.GetFormGroupGHdrDetail(tableName, branchID, empID, reportingYear, formID);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetFormGroupGHdrDetail_Result, FormGroupGHdr>()
                     .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name));
                  });

                formGroupGHdr = Mapper.Map<FormGroupGHdr>(formGroupGHdrDTO.FirstOrDefault());
                return formGroupGHdr;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateFormGData(AppraisalForm appraisalForm)
        {
            log.Info($"AppraisalFormService/UpdateFormGData");
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FormGroupGHdr, DTOModel.FormGroupGHdr>();
                });

                var dtoappraisalForm = Mapper.Map<DTOModel.FormGroupGHdr>(appraisalForm.formGroupGHdr);
                genericRepo.Update<DTOModel.FormGroupGHdr>(dtoappraisalForm);

                appraisalForm.FormGroupID = appraisalForm.formGroupGHdr.FormGroupID;
                appraisalForm.FormID = appraisalForm.formGroupGHdr.FormID;
                appraisalForm.ReportingYr = appraisalForm.formGroupGHdr.ReportingYr;
                appraisalForm.FormState = appraisalForm.formGroupGHdr.FormState;
                UpdateAppraisalFormHdr(appraisalForm);

                if (appraisalForm.formGroupGHdr.FormState == (int)AppraisalFormState.SubmitedByReporting ||
                         appraisalForm.formGroupGHdr.FormState == (int)AppraisalFormState.SubmitedByReviewer ||
                         appraisalForm.formGroupGHdr.FormState == (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                {
                    appraisalForm._ProcessWorkFlow.ReferenceID = dtoappraisalForm.FormGroupID;
                    AddProcessWorkFlow(appraisalForm._ProcessWorkFlow);

                    ///===== Adding log into Process Work Flow , when form auto push case occured..
                    if (appraisalForm._AutoPushWorkFlow.Count > 0)
                        AddAutoPushWorkFlow(appraisalForm._AutoPushWorkFlow);

                    if (appraisalForm.formGroupGHdr.FormState != (int)AppraisalFormState.SubmitedByAcceptanceAuth)
                        SendMailMessageToReceiver((int)appraisalForm._ProcessWorkFlow.ReceiverID, appraisalForm.formGroupGHdr.EmployeeCode, appraisalForm.formGroupGHdr.EmployeeName);
                    flag = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return flag;
        }


        #endregion

        public FormRulesAttributes GetFormRulesAttributes(Common.AppraisalForm formID, int employeeID, string reportingYr)
        {
            log.Info($"AppraisalFormService/GetFormRulesAttributes{formID},{employeeID}");
            try
            {
                FormRulesAttributes frmAttributes = new FormRulesAttributes();
                #region Get /Set Form Submission Due Dates/

                //  var currentYr = DateTime.Now.Year;
                var formSubmissionDates = genericRepo.GetIQueryable<DTOModel.AppraisalForm>(x => x.ReportingYr == reportingYr).FirstOrDefault();
                frmAttributes.AcceptanceSubmissionDate = formSubmissionDates.AcceptanceAuthSubmissionDueDate.Value;
                frmAttributes.ReviewerSubmissionDate = formSubmissionDates.ReviewerSubmissionDueDate.Value;
                frmAttributes.ReportingSubmissionDate = formSubmissionDates.ReportingSubmissionDueDate.Value;
                frmAttributes.EmployeeSubmissionDate = formSubmissionDates.EmployeeSubmissionDueDate.Value;

                #endregion

                if (formID == Common.AppraisalForm.FormA)
                {
                    if (genericRepo.Exists<DTOModel.FormGroupAHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted))
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.FormGroupAHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted).Select(x => new FormRulesAttributes
                        {
                            FormID = x.FormID,
                            FormGroupID = x.FormGroupID,
                            FormState = x.FormState,
                            AcceptanceSubmissionDate = frmAttributes.AcceptanceSubmissionDate,
                            ReviewerSubmissionDate = frmAttributes.ReviewerSubmissionDate,
                            ReportingSubmissionDate = frmAttributes.ReportingSubmissionDate,
                            EmployeeSubmissionDate = frmAttributes.EmployeeSubmissionDate,
                        }).FirstOrDefault();
                }
                else if (formID == Common.AppraisalForm.FormB)
                {
                    if (genericRepo.Exists<DTOModel.FormGroupBHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted))
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.FormGroupBHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted).Select(x => new FormRulesAttributes
                        {
                            FormID = x.FormID,
                            FormGroupID = x.FormGroupID,
                            FormState = x.FormState,
                            AcceptanceSubmissionDate = frmAttributes.AcceptanceSubmissionDate,
                            ReviewerSubmissionDate = frmAttributes.ReviewerSubmissionDate,
                            ReportingSubmissionDate = frmAttributes.ReportingSubmissionDate,
                            EmployeeSubmissionDate = frmAttributes.EmployeeSubmissionDate,
                        }).FirstOrDefault();
                }
                else if (formID == Common.AppraisalForm.FormC)
                {
                    if (genericRepo.Exists<DTOModel.FormGroupCHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted))
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.FormGroupCHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted).Select(x => new FormRulesAttributes
                        {
                            FormID = x.FormID,
                            FormGroupID = x.FormGroupID,
                            FormState = x.FormState,
                            AcceptanceSubmissionDate = frmAttributes.AcceptanceSubmissionDate,
                            ReviewerSubmissionDate = frmAttributes.ReviewerSubmissionDate,
                            ReportingSubmissionDate = frmAttributes.ReportingSubmissionDate,
                            EmployeeSubmissionDate = frmAttributes.EmployeeSubmissionDate,
                        }).FirstOrDefault();
                }
                else if (formID == Common.AppraisalForm.FormD)
                {
                    if (genericRepo.Exists<DTOModel.FormGroupDHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted))
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.FormGroupDHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted).Select(x => new FormRulesAttributes
                        {
                            FormID = x.FormID,
                            FormGroupID = x.FormGroupID,
                            FormState = x.FormState,
                            AcceptanceSubmissionDate = frmAttributes.AcceptanceSubmissionDate,
                            ReviewerSubmissionDate = frmAttributes.ReviewerSubmissionDate,
                            ReportingSubmissionDate = frmAttributes.ReportingSubmissionDate,
                            EmployeeSubmissionDate = frmAttributes.EmployeeSubmissionDate,
                        }).FirstOrDefault();
                }
                else if (formID == Common.AppraisalForm.FormE)
                {
                    if (genericRepo.Exists<DTOModel.FormGroupEHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted))
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.FormGroupEHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted).Select(x => new FormRulesAttributes
                        {
                            FormID = x.FormID,
                            FormGroupID = x.FormGroupID,
                            FormState = x.FormState,
                            AcceptanceSubmissionDate = frmAttributes.AcceptanceSubmissionDate,
                            ReviewerSubmissionDate = frmAttributes.ReviewerSubmissionDate,
                            ReportingSubmissionDate = frmAttributes.ReportingSubmissionDate,
                            EmployeeSubmissionDate = frmAttributes.EmployeeSubmissionDate,
                        }).FirstOrDefault();
                }
                else if (formID == Common.AppraisalForm.FormF)
                {
                    if (genericRepo.Exists<DTOModel.FormGroupFHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID && !x.IsDeleted))
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.FormGroupFHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID).Select(x => new FormRulesAttributes
                        {
                            FormID = x.FormID,
                            FormGroupID = x.FormGroupID,
                            FormState = x.FormState,
                            AcceptanceSubmissionDate = frmAttributes.AcceptanceSubmissionDate,
                            ReviewerSubmissionDate = frmAttributes.ReviewerSubmissionDate,
                            ReportingSubmissionDate = frmAttributes.ReportingSubmissionDate,
                            EmployeeSubmissionDate = frmAttributes.EmployeeSubmissionDate,
                        }).FirstOrDefault();
                }
                else if (formID == Common.AppraisalForm.FormG)
                {
                    if (genericRepo.Exists<DTOModel.FormGroupGHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID))
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.FormGroupGHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID).Select(x => new FormRulesAttributes
                        {
                            FormID = x.FormID,
                            FormGroupID = x.FormGroupID,
                            FormState = x.FormState,
                            AcceptanceSubmissionDate = frmAttributes.AcceptanceSubmissionDate,
                            ReviewerSubmissionDate = frmAttributes.ReviewerSubmissionDate,
                            ReportingSubmissionDate = frmAttributes.ReportingSubmissionDate,
                            EmployeeSubmissionDate = frmAttributes.EmployeeSubmissionDate,
                        }).FirstOrDefault();
                }
                else if (formID == Common.AppraisalForm.FormH)
                {
                    if (genericRepo.Exists<DTOModel.FormGroupHHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID))
                        frmAttributes = genericRepo.GetIQueryable<DTOModel.FormGroupHHdr>(x => x.ReportingYr == reportingYr && x.EmployeeID == employeeID).Select(x => new FormRulesAttributes
                        {
                            FormID = x.FormID,
                            FormGroupID = x.FormGroupID,
                            FormState = x.FormState,
                            AcceptanceSubmissionDate = frmAttributes.AcceptanceSubmissionDate,
                            ReviewerSubmissionDate = frmAttributes.ReviewerSubmissionDate,
                            ReportingSubmissionDate = frmAttributes.ReportingSubmissionDate,
                            EmployeeSubmissionDate = frmAttributes.EmployeeSubmissionDate,
                        }).FirstOrDefault();
                }

                return frmAttributes;


            }
            catch (Exception)
            {

                throw;
            }
        }

        #region A.P.A.R Skill Set
        public List<APARSkillSet> GetAPARSkillList(int? departmentID, int? designationID)
        {
            log.Info($"AppraisalFormService/GetAPARSkillList");
            try
            {
                var aPARSkillList = appraisalRepo.GetAPARSkillList(departmentID, designationID);
                Mapper.Initialize(cfg =>
                cfg.CreateMap<DTOModel.APARSkillSetList_Result, APARSkillSet>()
                .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.DepartmentID))
                 .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                .ForMember(d => d.Department, o => o.MapFrom(s => s.DepartmentName))
                 .ForMember(d => d.Designation, o => o.MapFrom(s => s.DesignationName))
                .ForAllOtherMembers(d => d.Ignore())
                );

                var dtoAPARSkillList = Mapper.Map<List<APARSkillSet>>(aPARSkillList);
                return dtoAPARSkillList;


            }
            catch (Exception)
            {

                throw;
            }
        }

        public int InsertAPARSkill(APARSkills aparSkills)
        {
            log.Info($"AppraisalFormService/InsertAPARSkill");
            try
            {
                Mapper.Initialize(cfg =>
                cfg.CreateMap<APARSkillSet, DTOModel.APARSkillSet>()
                );

                var dtoaparSkills = Mapper.Map<DTOModel.APARSkillSet>(aparSkills.APARSkillSet);
                var res = genericRepo.Insert(dtoaparSkills);
                if (dtoaparSkills.SkillSetID > 0)
                {
                    if (aparSkills.CheckBoxListBehavioral != null)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<int, DTOModel.APARSkillSetDetail>()
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                        .ForMember(d => d.SkillSetID, o => o.UseValue(dtoaparSkills.SkillSetID))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparSkills.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoBehavioral = Mapper.Map<List<DTOModel.APARSkillSetDetail>>(aparSkills.CheckBoxListBehavioral);
                        //var getBehavioral = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == dtoaparSkills.SkillSetID);
                        //genericRepo.RemoveMultipleEntity(getBehavioral);
                        genericRepo.AddMultipleEntity(dtoBehavioral);
                    }
                    if (aparSkills.CheckBoxListFunctional != null)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<int, DTOModel.APARSkillSetDetail>()
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                        .ForMember(d => d.SkillSetID, o => o.UseValue(dtoaparSkills.SkillSetID))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparSkills.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoFunctional = Mapper.Map<List<DTOModel.APARSkillSetDetail>>(aparSkills.CheckBoxListFunctional);
                        //var getFunctional = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == dtoaparSkills.SkillSetID);
                        //genericRepo.RemoveMultipleEntity(getFunctional);
                        genericRepo.AddMultipleEntity(dtoFunctional);
                    }

                }
                return dtoaparSkills.SkillSetID;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<APARSkillSetDetails> GetAPARSkillDetail(int skillSetID, int? skillTypeID)
        {
            log.Info($"AppraisalFormService/GetAPARSkillList");
            try
            {
                var aPARSkillDtlList = appraisalRepo.GetAPARSkillDetail(skillSetID, skillTypeID);
                Mapper.Initialize(cfg =>
                cfg.CreateMap<DTOModel.GetAPARSkillDetail_Result, APARSkillSetDetails>()
                .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.DepartmentID))
                .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                .ForMember(d => d.SkillTypeID, o => o.MapFrom(s => s.SkillTypeID))
                .ForMember(d => d.SkillType, o => o.MapFrom(s => s.SkillType))
                .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                .ForMember(d => d.Skill, o => o.MapFrom(s => s.Skill))
                .ForAllOtherMembers(d => d.Ignore())
                );

                var dtoAPARSkillDtlList = Mapper.Map<List<APARSkillSetDetails>>(aPARSkillDtlList);
                if (dtoAPARSkillDtlList != null && dtoAPARSkillDtlList.Count > 0)
                    return dtoAPARSkillDtlList;
                else
                {
                    return dtoAPARSkillDtlList = new List<APARSkillSetDetails>();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public int UpdateAPARSkill(APARSkills aparSkills)
        {
            log.Info($"AppraisalFormService/InsertAPARSkill");
            try
            {
                Mapper.Initialize(cfg =>
                cfg.CreateMap<APARSkillSet, DTOModel.APARSkillSet>()
                );

                var dtoaparSkills = Mapper.Map<DTOModel.APARSkillSet>(aparSkills.APARSkillSet);
                var getdata = genericRepo.Get<DTOModel.APARSkillSet>(x => x.SkillSetID == dtoaparSkills.SkillSetID).FirstOrDefault();
                if (getdata != null)
                {
                    getdata.DepartmentID = dtoaparSkills.DepartmentID;
                    getdata.DesignationID = dtoaparSkills.DesignationID;
                    getdata.UpdatedBy = dtoaparSkills.UpdatedBy;
                    getdata.UpdatedOn = dtoaparSkills.UpdatedOn;
                    genericRepo.Update(getdata);
                }
                if (dtoaparSkills.SkillSetID > 0)
                {
                    if (aparSkills.CheckBoxListBehavioral != null)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<int, DTOModel.APARSkillSetDetail>()
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                        .ForMember(d => d.SkillSetID, o => o.UseValue(dtoaparSkills.SkillSetID))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(getdata.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoBehavioral = Mapper.Map<List<DTOModel.APARSkillSetDetail>>(aparSkills.CheckBoxListBehavioral);
                        var getBehavioral = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == dtoaparSkills.SkillSetID && x.SkillTypeID == 2).ToList<DTOModel.APARSkillSetDetail>();

                        if (getBehavioral != null || getBehavioral.Count > 0)
                            genericRepo.DeleteAll<DTOModel.APARSkillSetDetail>(getBehavioral);
                        if (dtoBehavioral != null)
                            genericRepo.AddMultipleEntity<DTOModel.APARSkillSetDetail>(dtoBehavioral);
                    }
                    if (aparSkills.CheckBoxListFunctional != null)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<int, DTOModel.APARSkillSetDetail>()
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                        .ForMember(d => d.SkillSetID, o => o.UseValue(dtoaparSkills.SkillSetID))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(getdata.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoFunctional = Mapper.Map<List<DTOModel.APARSkillSetDetail>>(aparSkills.CheckBoxListFunctional);
                        var getFunctional = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == dtoaparSkills.SkillSetID && x.SkillTypeID == 3).ToList();
                        if (getFunctional != null || getFunctional.Count > 0)
                            genericRepo.DeleteAll<DTOModel.APARSkillSetDetail>(getFunctional);
                        if (dtoFunctional != null)
                            genericRepo.AddMultipleEntity(dtoFunctional);
                    }

                }
                return dtoaparSkills.SkillSetID;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool Delete(int skillSetID, int userID)
        {
            log.Info($"AppraisalFormService/Delete");
            try
            {
                var getAPARSkill = genericRepo.Get<DTOModel.APARSkillSet>(x => x.SkillSetID == skillSetID && !x.IsDeleted).FirstOrDefault();
                if (getAPARSkill != null)
                {
                    getAPARSkill.IsDeleted = true;
                    getAPARSkill.UpdatedBy = userID;
                    getAPARSkill.UpdatedOn = DateTime.Now;
                    genericRepo.Update(getAPARSkill);
                    return true;

                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<Model.APARSkillSetFormHdr> GetEmployeeAPARSkillList(int userEmpID)
        {
            log.Info($"AppraisalFormService/GetEmployeeAPARSkillList/{userEmpID}");
            try
            {
                List<APARSkillSetFormHdr> empAppList = new List<APARSkillSetFormHdr>();
                var dtoAppraisalForms = appraisalRepo.EmployeeSelfAPARSkillList(userEmpID).ToList();
                if (dtoAppraisalForms != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.APARSkillSetFormHdr, Model.APARSkillSetFormHdr>()
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.APARHdrID, o => o.MapFrom(s => s.APARHdrID))
                        .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                        .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                        .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                        .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.Appraisal && y.ToDate == null && y.EmployeeID == s.EmployeeID)))
                        .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                        .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.DepartmentID))
                        .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                        .ForMember(d => d.formState, o => o.MapFrom(s => (APARFormState)s.StatusID));
                        cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                    });
                    return Mapper.Map<List<APARSkillSetFormHdr>>(dtoAppraisalForms);
                }
                return empAppList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public APARSkillSetFormHdr GetFormAPARHdrDetail(int? branchID, int? empID, string reportingYear)
        {
            log.Info($"AppraisalFormService/GetFormAPARHdrDetail/{branchID}/{empID}/{reportingYear}");
            try
            {
                APARSkillSetFormHdr aparFormHdr = new APARSkillSetFormHdr();
                var formGroupHdrDTO = appraisalRepo.GetFormAPARHdrDetail(branchID, empID, reportingYear);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetAPARHdrDetail_Result, APARSkillSetFormHdr>()
                       .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Name));

                  }
                  );
                aparFormHdr = Mapper.Map<List<APARSkillSetFormHdr>>(formGroupHdrDTO).FirstOrDefault();
                return aparFormHdr;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<APARSkillSetFormDetail> GetAPARFormDetail(int empID, int departmentID, int designationID, string reportingYr)
        {
            log.Info($"AppraisalFormService/GetAPARFormDetail/{empID}/{reportingYr}");
            try
            {
                List<APARSkillSetFormDetail> aparFormDetail = new List<APARSkillSetFormDetail>();
                var aparFormDetailDTO = appraisalRepo.GetAPARFormDetail(empID, departmentID, designationID, reportingYr);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetAPARFormDetail_Result, APARSkillSetFormDetail>()
                    .ForMember(d => d.enumAssessment, o => o.MapFrom(s => (EnumAssessment?)s.Grading))
                    .ForMember(d => d.SkillRemark, o => o.MapFrom(s => s.SkillRemark))
                    .ForMember(d => d.enumAssessmentReporting, o => o.MapFrom(s => (EnumAssessment?)s.ReportingGrading))
                    .ForMember(d => d.ReportingRemarks, o => o.MapFrom(s => s.ReportingRemarks));

                });
                aparFormDetail = Mapper.Map<List<APARSkillSetFormDetail>>(aparFormDetailDTO);
                return aparFormDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int InsertAPARFormData(APARForm aparForm)
        {
            log.Info($"AppraisalFormService/InsertAPARFormData");
            try
            {
                Mapper.Initialize(cfg =>
                cfg.CreateMap<APARSkillSetFormHdr, DTOModel.APARSkillSetFormHdr>()
                );

                var dtoaparForm = Mapper.Map<DTOModel.APARSkillSetFormHdr>(aparForm.APARFormHdr);
                var res = genericRepo.Insert(dtoaparForm);
                if (dtoaparForm.APARHdrID > 0)
                {
                    #region Child Table
                    if (aparForm.Part1BehavioralList != null && aparForm.Part1BehavioralList.Count > 0)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<APARSkillSetFormDetail, DTOModel.APARSkillSetFormDetail>()
                        .ForMember(d => d.APARHdrID, o => o.UseValue(dtoaparForm.APARHdrID))
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                        .ForMember(d => d.PartID, o => o.UseValue(1))
                        .ForMember(d => d.Grading, o => o.MapFrom(s => (int?)s.enumAssessment))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparForm.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                        .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                        .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                        .ForMember(d => d.ReportingGrading, o => o.MapFrom(s => (int?)s.enumAssessmentReporting))
                         .ForMember(d => d.ReportingRemarks, o => o.MapFrom(s => s.ReportingRemarks))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoBehavioral = Mapper.Map<List<DTOModel.APARSkillSetFormDetail>>(aparForm.Part1BehavioralList.ToList());
                        //var getBehavioral = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == dtoaparSkills.SkillSetID);
                        //genericRepo.RemoveMultipleEntity(getBehavioral);
                        genericRepo.AddMultipleEntity(dtoBehavioral);
                    }
                    if (aparForm.Part1FunctionalList != null && aparForm.Part1FunctionalList.Count > 0)
                    {
                        Mapper.Initialize(cfg =>
                      cfg.CreateMap<APARSkillSetFormDetail, DTOModel.APARSkillSetFormDetail>()
                            .ForMember(d => d.APARHdrID, o => o.UseValue(dtoaparForm.APARHdrID))
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                        .ForMember(d => d.PartID, o => o.UseValue(1))
                        .ForMember(d => d.Grading, o => o.MapFrom(s => (int?)s.enumAssessment))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparForm.CreatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                        .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                        .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                        .ForMember(d => d.ReportingGrading, o => o.MapFrom(s => (int?)s.enumAssessmentReporting))
                        .ForMember(d => d.ReportingRemarks, o => o.MapFrom(s => s.ReportingRemarks))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoFunctional = Mapper.Map<List<DTOModel.APARSkillSetFormDetail>>(aparForm.Part1FunctionalList);
                        //var getFunctional = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == dtoaparSkills.SkillSetID);
                        //genericRepo.RemoveMultipleEntity(getFunctional);
                        genericRepo.AddMultipleEntity(dtoFunctional);
                    }
                    //if (aparForm.Part2BehavioralList != null && aparForm.Part2BehavioralList.Count > 0)
                    //{
                    //    Mapper.Initialize(cfg =>
                    //    cfg.CreateMap<APARSkillSetFormDetail, DTOModel.APARSkillSetFormDetail>()
                    //    .ForMember(d => d.APARHdrID, o => o.UseValue(dtoaparForm.APARHdrID))
                    //    .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                    //    .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                    //    .ForMember(d => d.PartID, o => o.UseValue(1))
                    //    .ForMember(d => d.Grading, o => o.MapFrom(s => (int?)s.enumAssessment))
                    //    .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparForm.CreatedBy))
                    //    .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                    //    .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                    //    .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                    //    .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                    //    .ForAllOtherMembers(d => d.Ignore())
                    //    );
                    //    var dtoBehavioral = Mapper.Map<List<DTOModel.APARSkillSetFormDetail>>(aparForm.Part2BehavioralList);
                    //    //var getBehavioral = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == dtoaparSkills.SkillSetID);
                    //    //genericRepo.RemoveMultipleEntity(getBehavioral);
                    //    genericRepo.AddMultipleEntity(dtoBehavioral);
                    //}
                    //if (aparForm.Part2FunctionalList != null && aparForm.Part2FunctionalList.Count > 0)
                    //{
                    //    Mapper.Initialize(cfg =>
                    //  cfg.CreateMap<APARSkillSetFormDetail, DTOModel.APARSkillSetFormDetail>()
                    //    .ForMember(d => d.APARHdrID, o => o.UseValue(dtoaparForm.APARHdrID))
                    //    .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                    //    .ForMember(d => d.SkillID, o => o.MapFrom(s => s))
                    //    .ForMember(d => d.PartID, o => o.UseValue(1))
                    //    .ForMember(d => d.Grading, o => o.MapFrom(s => (int?)s.enumAssessment))
                    //    .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparForm.CreatedBy))
                    //    .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                    //    .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                    //    .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                    //    .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                    //    .ForAllOtherMembers(d => d.Ignore())
                    //    );
                    //    var dtoFunctional = Mapper.Map<List<DTOModel.APARSkillSetFormDetail>>(aparForm.Part2FunctionalList);
                    //    //var getFunctional = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == dtoaparSkills.SkillSetID);
                    //    //genericRepo.RemoveMultipleEntity(getFunctional);
                    //    genericRepo.AddMultipleEntity(dtoFunctional);
                    //}
                    #endregion

                    if (aparForm.APARFormHdr.StatusID == (int)AppraisalFormState.SubmitedByEmployee)
                    {
                        aparForm._ProcessWorkFlow.ReferenceID = dtoaparForm.APARHdrID;
                        AddProcessWorkFlow(aparForm._ProcessWorkFlow);
                    }

                }
                return dtoaparForm.APARHdrID;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public int UpdateAPARFormData(APARForm aparForm)
        {
            log.Info($"AppraisalFormService/UpdateAPARFormData");
            try
            {
                Mapper.Initialize(cfg =>
                cfg.CreateMap<APARSkillSetFormHdr, DTOModel.APARSkillSetFormHdr>()
                );

                var dtoaparForm = Mapper.Map<DTOModel.APARSkillSetFormHdr>(aparForm.APARFormHdr);

                var getAPARFormHdr = genericRepo.Get<DTOModel.APARSkillSetFormHdr>(x => x.EmployeeID == dtoaparForm.EmployeeID && x.APARHdrID == dtoaparForm.APARHdrID).FirstOrDefault();
                if (getAPARFormHdr != null)
                {
                    getAPARFormHdr.WorkedPeriodUnderROFrom = dtoaparForm.WorkedPeriodUnderROFrom;
                    getAPARFormHdr.WorkedPeriodUnderROTo = dtoaparForm.WorkedPeriodUnderROTo;
                    getAPARFormHdr.StatusID = dtoaparForm.StatusID;
                    getAPARFormHdr.UpdatedBy = dtoaparForm.UpdatedBy;
                    getAPARFormHdr.UpdatedOn = dtoaparForm.UpdatedOn;
                    genericRepo.Update(getAPARFormHdr);
                }
                if (dtoaparForm.APARHdrID > 0)
                {
                    #region Update Child Table
                    if (aparForm.Part1BehavioralList != null && aparForm.Part1BehavioralList.Count > 0)
                    {
                        Mapper.Initialize(cfg =>
                        cfg.CreateMap<APARSkillSetFormDetail, DTOModel.APARSkillSetFormDetail>()
                        .ForMember(d => d.APARHdrID, o => o.UseValue(dtoaparForm.APARHdrID))
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                        .ForMember(d => d.PartID, o => o.UseValue(1))
                        .ForMember(d => d.Grading, o => o.MapFrom(s => (int?)s.enumAssessment))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparForm.UpdatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                        .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                        .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                        .ForMember(d => d.ReportingGrading, o => o.MapFrom(s => (int?)s.enumAssessmentReporting))
                        .ForMember(d => d.ReportingRemarks, o => o.MapFrom(s => s.ReportingRemarks))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoBehavioral = Mapper.Map<List<DTOModel.APARSkillSetFormDetail>>(aparForm.Part1BehavioralList.ToList());
                        var getBehavioral = genericRepo.Get<DTOModel.APARSkillSetFormDetail>(x => x.APARHdrID == dtoaparForm.APARHdrID && x.PartID == 1 && x.SkillTypeID == 2).ToList();
                        if (getBehavioral != null && getBehavioral.Count > 0)
                            genericRepo.DeleteAll(getBehavioral);
                        genericRepo.AddMultipleEntity(dtoBehavioral);
                    }
                    if (aparForm.Part1FunctionalList != null && aparForm.Part1FunctionalList.Count > 0)
                    {
                        Mapper.Initialize(cfg =>
                      cfg.CreateMap<APARSkillSetFormDetail, DTOModel.APARSkillSetFormDetail>()
                        .ForMember(d => d.APARHdrID, o => o.UseValue(dtoaparForm.APARHdrID))
                        .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                        .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                        .ForMember(d => d.PartID, o => o.UseValue(1))
                        .ForMember(d => d.Grading, o => o.MapFrom(s => (int?)s.enumAssessment))
                        .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparForm.UpdatedBy))
                        .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                        .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                        .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                        .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                        .ForMember(d => d.ReportingGrading, o => o.MapFrom(s => (int?)s.enumAssessmentReporting))
                        .ForMember(d => d.ReportingRemarks, o => o.MapFrom(s => s.ReportingRemarks))
                        .ForAllOtherMembers(d => d.Ignore())
                        );
                        var dtoFunctional = Mapper.Map<List<DTOModel.APARSkillSetFormDetail>>(aparForm.Part1FunctionalList);
                        var getFunctional = genericRepo.Get<DTOModel.APARSkillSetFormDetail>(x => x.APARHdrID == dtoaparForm.APARHdrID && x.PartID == 1 && x.SkillTypeID == 3).ToList();
                        if (getFunctional != null && getFunctional.Count > 0)
                            genericRepo.DeleteAll(getFunctional);
                        genericRepo.AddMultipleEntity(dtoFunctional);
                    }
                    //if (aparForm.Part2BehavioralList != null && aparForm.Part2BehavioralList.Count > 0)
                    //{
                    //    Mapper.Initialize(cfg =>
                    //    cfg.CreateMap<APARSkillSetFormDetail, DTOModel.APARSkillSetFormDetail>()
                    //    .ForMember(d => d.APARHdrID, o => o.MapFrom(s => dtoaparForm.APARHdrID))
                    //    .ForMember(d => d.SkillTypeID, o => o.UseValue(2))
                    //    .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                    //    .ForMember(d => d.PartID, o => o.UseValue(2))
                    //    .ForMember(d => d.Grading, o => o.MapFrom(s => (int?)s.enumAssessment))
                    //    .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparForm.UpdatedBy))
                    //    .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                    //    .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                    //    .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                    //    .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                    //    .ForAllOtherMembers(d => d.Ignore())
                    //    );
                    //    var dtoBehavioral = Mapper.Map<List<DTOModel.APARSkillSetFormDetail>>(aparForm.Part2BehavioralList);
                    //    var getBehavioral = genericRepo.Get<DTOModel.APARSkillSetFormDetail>(x => x.APARHdrID == dtoaparForm.APARHdrID && x.PartID == 2 && x.SkillTypeID == 2).ToList();
                    //    if (getBehavioral != null && getBehavioral.Count > 0)
                    //        genericRepo.DeleteAll(getBehavioral);
                    //    genericRepo.AddMultipleEntity(dtoBehavioral);
                    //}
                    //if (aparForm.Part2FunctionalList != null && aparForm.Part2FunctionalList.Count > 0)
                    //{
                    //    Mapper.Initialize(cfg =>
                    //  cfg.CreateMap<APARSkillSetFormDetail, DTOModel.APARSkillSetFormDetail>()
                    //    .ForMember(d => d.APARHdrID, o => o.MapFrom(s => dtoaparForm.APARHdrID))
                    //    .ForMember(d => d.SkillTypeID, o => o.UseValue(3))
                    //    .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                    //    .ForMember(d => d.PartID, o => o.UseValue(2))
                    //    .ForMember(d => d.Grading, o => o.MapFrom(s => (int?)s.enumAssessment))
                    //    .ForMember(d => d.CreatedBy, o => o.UseValue(dtoaparForm.UpdatedBy))
                    //    .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                    //    .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                    //    .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                    //    .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                    //    .ForAllOtherMembers(d => d.Ignore())
                    //    );
                    //    var dtoFunctional = Mapper.Map<List<DTOModel.APARSkillSetFormDetail>>(aparForm.Part2FunctionalList);
                    //    var getFunctional = genericRepo.Get<DTOModel.APARSkillSetFormDetail>(x => x.APARHdrID == dtoaparForm.APARHdrID && x.PartID == 2 && x.SkillTypeID == 3).ToList();
                    //    if (getFunctional != null && getFunctional.Count > 0)
                    //        genericRepo.DeleteAll(getFunctional);
                    //    genericRepo.AddMultipleEntity(dtoFunctional);
                    //}
                    #endregion

                    if (aparForm.APARFormHdr.StatusID == (int)AppraisalFormState.SubmitedByEmployee || aparForm.APARFormHdr.StatusID == (int)AppraisalFormState.SubmitedByReporting || aparForm.APARFormHdr.StatusID == (int)AppraisalFormState.RejectedbyReporting)
                    {
                        aparForm._ProcessWorkFlow.ReferenceID = dtoaparForm.APARHdrID;
                        AddProcessWorkFlow(aparForm._ProcessWorkFlow);
                    }
                }
                return dtoaparForm.APARHdrID;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<APARSkillSetDetails> GetSkillDetails(int departmentID)
        {
            log.Info($"AppraisalFormService/GetSkillDetails{departmentID}");
            try
            {
                List<APARSkillSetDetails> skillDetail = new List<APARSkillSetDetails>();
                var getData = appraisalRepo.GetSkillSetDetails(departmentID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetSkillSetDetails_Result, APARSkillSetDetails>()
                    .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.DepartmentID))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.DesignationID))
                    .ForMember(d => d.Designation, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.SkillSetID, o => o.MapFrom(s => s.SkillSetID))
                    .ForMember(d => d.SkillTypeID, o => o.MapFrom(s => s.SkillTypeID))
                    .ForMember(d => d.SkillType, o => o.MapFrom(s => s.SkillType))
                    .ForMember(d => d.SkillID, o => o.MapFrom(s => s.SkillID))
                    .ForMember(d => d.Skill, o => o.MapFrom(s => s.Skill))
                    .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark));

                });
                skillDetail = Mapper.Map<List<APARSkillSetDetails>>(getData);
                return skillDetail;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateSkillRemarks(List<APARSkillSetDetails> aparSkillList)
        {
            log.Info($"AppraisalFormService/UpdateSkillRemarks");
            try
            {
                bool flag;
                DataTable skillDT = new DataTable();
                skillDT.Columns.Add("SkillSetID", typeof(int));
                skillDT.Columns.Add("SkillID", typeof(string));
                skillDT.Columns.Add("Remark", typeof(string));


                var skills = aparSkillList.Select(x => new { SkillSetID = x.SkillSetID, SkillID = x.SkillID, Remark = x.Remark }).ToList();
                skillDT = Common.ExtensionMethods.ToDataTable(skills);
                flag = appraisalRepo.UpdateSkillRemarks(skillDT);
                return flag;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetSkillSetDetailID(int skillSetID, int skillID)
        {
            var getSkillSetID = genericRepo.Get<DTOModel.APARSkillSetDetail>(x => x.SkillSetID == skillSetID && x.SkillID == skillID).FirstOrDefault();
            if (getSkillSetID != null)
                return getSkillSetID.SkillSetDtlID;
            else
                return 0;

        }
        #endregion

        public FormRulesAttributes GetFormSubmissionDate(int designationID)
        {
            log.Info($"AppraisalFormService/GetFormSubmissionDate");
            try
            {
                FormRulesAttributes frmAttributes = new FormRulesAttributes();
                var currentYr = DateTime.Now.Year;
                var formSubmissionDates = genericRepo.GetIQueryable<DTOModel.DesignationAppraisalForm>(X => X.DesignationID == designationID && !X.IsDeleted).FirstOrDefault();
                if (formSubmissionDates != null)
                {
                    frmAttributes.AcceptanceSubmissionDate = formSubmissionDates.AppraisalForm.AcceptanceAuthSubmissionDueDate.Value.ChangeYear(currentYr);
                    frmAttributes.ReviewerSubmissionDate = formSubmissionDates.AppraisalForm.ReviewerSubmissionDueDate.Value.ChangeYear(currentYr);
                    frmAttributes.ReportingSubmissionDate = formSubmissionDates.AppraisalForm.ReportingSubmissionDueDate.Value.ChangeYear(currentYr);
                    frmAttributes.EmployeeSubmissionDate = formSubmissionDates.AppraisalForm.EmployeeSubmissionDueDate.Value.ChangeYear(currentYr);
                }
                return frmAttributes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AcarAparModel> GetAcarAparDetails(AppraisalFormApprovalFilter filters, string procName)
        {
            log.Info($"AppraisalFormService/GetAcarAparDetails/");
            try
            {
                var acarAparDetails = appraisalRepo.GetAcarAparFilters(filters.selectedReportingYear, filters.selectedEmployeeID, filters.statusId, procName);
                var result = Common.ExtensionMethods.ConvertToList<AcarAparModel>(acarAparDetails);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FormRulesAttributes GetFormSubmittionDueDate(string reportingYr)
        {
            log.Info($"AppraisalFormService/GetFormSubmittionDueDate");
            try
            {
                FormRulesAttributes frmAttributes = new FormRulesAttributes();
                #region Get /Set Form Submission Due Dates/

                //  var currentYr = DateTime.Now.Year;
                var formSubmissionDates = genericRepo.GetIQueryable<DTOModel.AppraisalForm>(X => X.ReportingYr == reportingYr).FirstOrDefault();
                if(formSubmissionDates==null)
                    return frmAttributes;

                frmAttributes.AcceptanceSubmissionDate = formSubmissionDates.AcceptanceAuthSubmissionDueDate.Value;
                frmAttributes.ReviewerSubmissionDate = formSubmissionDates.ReviewerSubmissionDueDate.Value;
                frmAttributes.ReportingSubmissionDate = formSubmissionDates.ReportingSubmissionDueDate.Value;
                frmAttributes.EmployeeSubmissionDate = formSubmissionDates.EmployeeSubmissionDueDate.Value;
                return frmAttributes;
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMailMessageToReceiver(int recieverID, string empCode, string empName)
        {
            log.Info($"AppraisalFormService/SendMailMessageToReceiver/recieverID={recieverID}");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                var recieverMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == recieverID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail,
                    EmployeeCode=x.EmployeeCode
                }).FirstOrDefault();

                if (!string.IsNullOrEmpty(recieverMail.EmployeeCode))
                {
                    PushNotification notification = new PushNotification
                    {
                        UserName = recieverMail.EmployeeCode,
                        Title = "APAR Approval",
                        Message = $"Dear Sir/Madam, This is to intimate that you have received the APAR Approval, for employee { empCode + "-" + empName } with period {DateTime.Now.GetFinancialYr()}  for further evaluation."

                    };
                    Task notif = Task.Run(() => FirebaseTopicNotification(notification));
                }

                if (!String.IsNullOrEmpty(recieverMail.OfficailEmail))
                {
                    emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                 + "<p>This is to intimate that you have received the self assessed APAR Form from <b>" + empCode + "-" + empName + "</b> for further evaluation.</p>"
                 + "<p>Requesting your support for timely completion of the process by logging into: </p>"
                 + "<p>http://182.74.122.83/nafedhrms</p>"
                 + "<p>Please get in touch with Personnel Section in case of any disconnect/clarification required. </p> </div>");

                    emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                    Common.EmailMessage message = new Common.EmailMessage();
                    message.To = recieverMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : Appraisal Intimation";

                    Task t2 = Task.Run(() => SendEmail(message));

                }
                else if (!String.IsNullOrEmpty(recieverMail.MobileNo))
                {
                    emailBody.AppendFormat("Dear Sir/Madam,"
                  + "This is to intimate that you have received the self assessed APAR Form from " + empCode + "-" + empName + " for further evaluation."
                  + "Requesting your support for timely completion of the process by logging into:"
                  + "http://182.74.122.83/nafedhrms"
                  + "Please get in touch with Personnel Section in case of any disconnect/clarification required.");
                    emailBody.AppendFormat("Regards, ENAFED");

                    Task t1 = Task.Run(() => SendMessage(recieverMail.MobileNo, emailBody.ToString()));
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
        public bool SendMailMessageToEmployee(int senderID)
        {
            log.Info($"AppraisalFormService/SendMailMessageToEmployee/senderID={senderID}");
            bool flag = false;
            try
            {
                StringBuilder emailBody = new StringBuilder("");
                var senderMail = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == senderID && !x.IsDeleted).Select(x => new
                {
                    MobileNo = x.MobileNo,
                    OfficailEmail = x.OfficialEmail
                }).FirstOrDefault();

                if (!String.IsNullOrEmpty(senderMail.OfficailEmail))
                {
                    emailBody.AppendFormat("<div style='font-family:Tahoma;font-size:9pt;'> <p>Dear Sir/Madam,</p>"
                 + "<p>This is to confirm that your self-assessed APAR form has been submitted to your Reporting Officer for further review successfully.</p>");

                    emailBody.AppendFormat("<div> <p>Regards, <br/> ENAFED </p> </div>");

                    Common.EmailMessage message = new Common.EmailMessage();
                    message.To = senderMail.OfficailEmail;
                    message.Body = emailBody.ToString();
                    message.Subject = "NAFED-HRMS : Appraisal Intimation";

                    Task t2 = Task.Run(() => SendEmail(message));

                }
                else if (!String.IsNullOrEmpty(senderMail.MobileNo))
                {
                    emailBody.AppendFormat("Dear Sir/Madam,"
                  + "This is to confirm that your self-assessed APAR form has been submitted to your Reporting Officer for further review successfully.");
                    emailBody.AppendFormat("Regards, ENAFED");

                    Task t1 = Task.Run(() => SendMessage(senderMail.MobileNo, emailBody.ToString()));
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

        void SendEmail(Common.EmailMessage message)
        {
            try
            {
                var emailsetting = genericRepo.Get<DTOModel.EmailConfiguration>().FirstOrDefault();
                // EmailMessage message = new EmailMessage();
                //   message.To = ToEmailID;
                message.From = "NAFED HRMS <" + emailsetting.ToEmail + ">";
                //  message.Subject = subject;
                message.UserName = emailsetting.UserName;
                message.Password = emailsetting.Password;
                message.SmtpClientHost = emailsetting.Server;
                message.SmtpPort = emailsetting.Port;
                message.enableSSL = emailsetting.SSLStatus;
                //   message.Body = mailbody;
                message.HTMLView = true;
                message.FriendlyName = "NAFED";
                Common.EmailHelper.SendEmail(message);

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool SendMessage(string mobileNo, string message)
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

                string msgRecepient = mobileNo.Length == 10 ? "91" + mobileNo : mobileNo;
                Common.SMSParameter sms = new Common.SMSParameter();

                sms.MobileNo = msgRecepient;
                sms.Message = message;
                sms.URL = smssetting.SMSUrl;
                sms.User = smssetting.UserName;
                sms.Password = smssetting.Password;
                return Common.SMSHelper.SendSMS(sms);
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                return false;
            }

        }


        public void SetSectionValue(AppraisalForm frmProperty)
        {
            log.Info($"AppraisalFormService/SetSectionValue");
            try
            {
                int groupId = 0;
                if (frmProperty.FormID == 1)
                    groupId = frmProperty.formGroupAHdr.FormGroupID;
                else if (frmProperty.FormID == 2)
                    groupId = frmProperty.formGroupAHdr.FormGroupID;
                else if (frmProperty.FormID == 3)
                    groupId = frmProperty.formGroupAHdr.FormGroupID;
                else if (frmProperty.FormID == 4)
                    groupId = frmProperty.formGroupDHdr.FormGroupID;
                else if (frmProperty.FormID == 5)
                    groupId = frmProperty.formGroupAHdr.FormGroupID;
                else if (frmProperty.FormID == 6)
                    groupId = frmProperty.formGroupAHdr.FormGroupID;
                else if (frmProperty.FormID == 7)
                    groupId = frmProperty.formGroupGHdr.FormGroupID;
                else if (frmProperty.FormID == 7)
                    groupId = frmProperty.formGroupHHdr.FormGroupID;

                if (frmProperty.ApprovalHierarchy == 1)
                {
                    if (frmProperty.loggedInEmpID == frmProperty.ReportingTo)
                    {
                        frmProperty.empSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.EmployeeID && x.StatusID == (int)AppraisalFormState.SubmitedByEmployee && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);
                    }
                    else if (frmProperty.loggedInEmpID == frmProperty.ReviewingTo)
                    {
                        frmProperty.reportingSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.ReportingTo && x.StatusID == (int)AppraisalFormState.SubmitedByReporting && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);

                        frmProperty.empSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.EmployeeID && x.StatusID == (int)AppraisalFormState.SubmitedByEmployee && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);
                    }
                    else if (frmProperty.loggedInEmpID == frmProperty.AcceptanceAuthorityTo)
                    {
                        frmProperty.reportingSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.ReportingTo && x.StatusID == (int)AppraisalFormState.SubmitedByReporting && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);

                        frmProperty.empSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.EmployeeID && x.StatusID == (int)AppraisalFormState.SubmitedByEmployee && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);

                        frmProperty.reviewingSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.ReviewingTo && x.StatusID == (int)AppraisalFormState.SubmitedByReviewer && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);
                    }
                }
                else if (frmProperty.ApprovalHierarchy == 3)
                {
                    frmProperty.empSection = false;
                    frmProperty.reportingSection = true;
                    frmProperty.reviewingSection = true;
                    frmProperty.acceptanceSection = true;
                }
                else if (frmProperty.ApprovalHierarchy == 2)
                {
                    if (frmProperty.loggedInEmpID == frmProperty.ReportingTo || frmProperty.loggedInEmpID == frmProperty.ReportingTo)
                    {
                        frmProperty.empSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.EmployeeID && x.StatusID == (int)AppraisalFormState.SubmitedByEmployee && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);
                        frmProperty.reportingSection = true;
                        frmProperty.reviewingSection = true;
                    }
                    else if (frmProperty.loggedInEmpID == frmProperty.AcceptanceAuthorityTo)
                    {
                        frmProperty.empSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.EmployeeID && x.StatusID == (int)AppraisalFormState.SubmitedByEmployee && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);

                        frmProperty.reportingSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.ReportingTo && x.StatusID == (int)AppraisalFormState.SubmitedByReviewer && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);

                        frmProperty.reviewingSection = frmProperty.reportingSection;
                    }
                }
                else if (frmProperty.ApprovalHierarchy == 2.1)
                {
                    if (frmProperty.loggedInEmpID == frmProperty.ReviewingTo || frmProperty.loggedInEmpID == frmProperty.AcceptanceAuthorityTo)
                    {
                        frmProperty.empSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.EmployeeID && x.StatusID == (int)AppraisalFormState.SubmitedByEmployee && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);

                        frmProperty.reportingSection = genericRepo.Exists<DTOModel.ProcessWorkFlow>(x => x.ProcessID == (int)Common.WorkFlowProcess.Appraisal && x.SenderID == frmProperty.ReportingTo && x.StatusID == (int)AppraisalFormState.SubmitedByReporting && x.ReferenceID == groupId && x.EmployeeID == frmProperty.EmployeeID && !x.Readflag.HasValue);

                        frmProperty.reviewingSection = true;
                        frmProperty.acceptanceSection = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<ProcessWorkFlow> GetAppraisalHistory(int referenceId, int empId, int processID)
        {
            log.Info($"AppraisalFormService/GetAppraisalHistory");
            try
            {
                var APRhistoryObj = appraisalRepo.GetAppraisalHistory(empId, processID, referenceId);
                var result = Common.ExtensionMethods.ConvertToList<ProcessWorkFlow>(APRhistoryObj);
                return result;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }


        }

        private void AddAutoPushWorkFlow(List<Model.ProcessWorkFlow> objWrkFlow)
        {
            log.Info($"AppraisalFormService/AddAutoPushWorkFlow");
            try
            {
                var senderIDs = objWrkFlow.Select(x => (int)x.SenderID).ToArray<int>();
                var empInfo = genericRepo.Get<DTOModel.tblMstEmployee>(x => senderIDs.Any(y => y == x.EmployeeId));

                objWrkFlow.ForEach(x =>
                {
                    x.SenderDepartmentID = empInfo.FirstOrDefault(y => y.EmployeeId == x.SenderID).DepartmentID;
                    x.SenderDesignationID = empInfo.FirstOrDefault(y => y.EmployeeId == x.SenderID).DesignationID;
                    AddProcessWorkFlow(x);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<AppraisalFormHdr> GetAppraisalFormHistory(AppraisalFormApprovalFilter filters)
        {
            log.Info($"AppraisalFormService/GetAppraisalFormHistory/");
            try
            {
                List<DTOModel.AppraisalFormHdr> dtoFormHdrs = new List<DTOModel.AppraisalFormHdr>();

                var appformsHdr = genericRepo.GetIQueryable<DTOModel.AppraisalFormHdr>(x => x.tblMstEmployee.EmployeeProcessApprovals
                .Any(y => y.ProcessID == (int)Common.WorkFlowProcess.Appraisal && y.ToDate == null));

                if (filters != null)
                    dtoFormHdrs = appformsHdr.Where(x => (filters.selectedEmployeeID == 0 ? (1 > 0) : x.EmployeeID == filters.selectedEmployeeID) && (filters.selectedFormID == 0 ? (1 > 0) : x.FormID == filters.selectedFormID) && ((filters.selectedReportingYear == "" || filters.selectedReportingYear == null) ? (1 > 0) : x.ReportingYr == filters.selectedReportingYear) && (((int)filters.appraisalFormStatus == 0) ? (1 > 0) : x.StatusID == (int)filters.appraisalFormStatus)).ToList();

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.AppraisalFormHdr, Model.AppraisalFormHdr>()
                    .ForMember(d => d.AppraisalHdrID, o => o.MapFrom(s => s.AppraisalHdrID))
                    .ForMember(d => d.ReportingYr, o => o.MapFrom(s => s.ReportingYr))
                    .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                    .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                    .ForMember(d => d.EmpName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                    .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                    .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                    .ForMember(d => d.FormName, o => o.MapFrom(s => s.AppraisalForm.FormName))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                    .ForMember(d => d.EmpProceeApproval, o => o.MapFrom(s => s.tblMstEmployee.EmployeeProcessApprovals.FirstOrDefault(y => y.ProcessID == (int)Common.WorkFlowProcess.Appraisal && y.ToDate == null && y.EmployeeID == s.EmployeeID)))
                    .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                    .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.tblMstEmployee.DepartmentID))
                    .ForMember(d => d.DesignationID, o => o.MapFrom(s => s.tblMstEmployee.DesignationID))
                    .ForMember(d => d.aparReviewedSignedCopy, o => o.MapFrom(s => s.AppraisalForm.APARReviewedSignedCopies.FirstOrDefault(y => y.FormID == s.FormID && y.FormGroupID == s.FormGroupID)))
                    .ForAllOtherMembers(d => d.Ignore());
                    cfg.CreateMap<Data.Models.EmployeeProcessApproval, Model.EmployeeProcessApproval>();
                    cfg.CreateMap<Data.Models.APARReviewedSignedCopy, Model.APARReviewedSignedCopy>();
                });
                return Mapper.Map<List<Model.AppraisalFormHdr>>(dtoFormHdrs.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Upload Documents
        public bool UpdateUploadDocumentsDetails(AppraisalForm apprForm, APARReviewedSignedCopy existingFileDetails)
        {
            log.Info($"AppraisalFormService/UpdateUploadDocumentsDetails");
            bool flag = false;
            var workflowId = 0;
            try
            {
                #region Update Form State And Remarks
                UpdateFormsAcceptanceRemarks(apprForm, existingFileDetails);
                #endregion

                if (existingFileDetails == null)
                {
                    #region Insert Process Work Flow
                    if (apprForm._ProcessWorkFlow != null)
                    {
                        Data.Models.ProcessWorkFlow dtoWorkFlow = new Data.Models.ProcessWorkFlow();
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<ProcessWorkFlow, Data.Models.ProcessWorkFlow>();
                        });
                        dtoWorkFlow = Mapper.Map<Data.Models.ProcessWorkFlow>(apprForm._ProcessWorkFlow);

                        if (dtoWorkFlow.ReceiverID.HasValue && dtoWorkFlow.ReceiverID.Value > 0)
                        {
                            var receiverDtls = genericRepo.GetByID<Data.Models.tblMstEmployee>(dtoWorkFlow.ReceiverID.Value);
                            dtoWorkFlow.SenderDepartmentID = receiverDtls?.DepartmentID ?? null;
                            dtoWorkFlow.SenderDesignationID = receiverDtls?.DesignationID ?? null;
                            dtoWorkFlow.ReceiverDesignationID = receiverDtls?.DesignationID ?? null;
                            dtoWorkFlow.ReceiverDepartmentID = receiverDtls?.DepartmentID ?? null;
                        }
                        genericRepo.Insert<Data.Models.ProcessWorkFlow>(dtoWorkFlow);
                        workflowId = dtoWorkFlow.WorkflowID;
                    }
                    #endregion
                }
                else
                    workflowId = existingFileDetails.WorkFlowID.Value;

                #region Insert APAR Reviewed Signed Copy
                if (apprForm.UploadedDocList.Count > 0)
                {
                    apprForm.UploadedDocList.ForEach(x =>
                    {
                        x.WorkFlowID = workflowId;
                    });

                    var getSignedFile = genericRepo.Get<DTOModel.APARReviewedSignedCopy>(x => x.FormID == apprForm.FormID && x.FormGroupID == apprForm.frmAttributes.FormGroupID);

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.APARReviewedSignedCopy, DTOModel.APARReviewedSignedCopy>()
                        .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                        .ForMember(d => d.WorkFlowID, o => o.MapFrom(s => s.WorkFlowID))
                        .ForMember(d => d.UploadedDocName, o => o.MapFrom(s => s.UploadedDocName))
                        .ForMember(d => d.UploadedBy, o => o.MapFrom(s => s.UploadedBy))
                        .ForMember(d => d.UploadedOn, o => o.MapFrom(s => s.UploadedOn))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.UploadedOn))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoAPARSignedCopyDtls = Mapper.Map<List<DTOModel.APARReviewedSignedCopy>>(apprForm.UploadedDocList);
                    if (getSignedFile != null && getSignedFile.Count() > 0)
                        genericRepo.DeleteAll<DTOModel.APARReviewedSignedCopy>(getSignedFile);
                    if (dtoAPARSignedCopyDtls != null)
                        genericRepo.AddMultipleEntity<DTOModel.APARReviewedSignedCopy>(dtoAPARSignedCopyDtls);
                }
                flag = true;
                #endregion
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        private void UpdateFormsAcceptanceRemarks(AppraisalForm apprForm, APARReviewedSignedCopy existingFileDetails)
        {
            log.Info($"AppraisalFormService/UpdateFormsAcceptanceRemarks");
            try
            {
                #region Update HDR Table
                if (apprForm.FormID == (int)Common.AppraisalForm.FormA)
                {
                    var getFormHdrDetails = genericRepo.Get<DTOModel.FormGroupAHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr && !x.IsDeleted).FirstOrDefault();
                    if (getFormHdrDetails != null)
                    {
                        getFormHdrDetails.FormState = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getFormHdrDetails.Remarks = apprForm.UploadRemarks;
                        genericRepo.Update<DTOModel.FormGroupAHdr>(getFormHdrDetails);
                    }
                }
                else if (apprForm.FormID == (int)Common.AppraisalForm.FormB)
                {
                    var getFormHdrDetails = genericRepo.Get<DTOModel.FormGroupBHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr && !x.IsDeleted).FirstOrDefault();
                    if (getFormHdrDetails != null)
                    {
                        getFormHdrDetails.FormState = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getFormHdrDetails.Remarks = apprForm.UploadRemarks;
                        genericRepo.Update<DTOModel.FormGroupBHdr>(getFormHdrDetails);
                    }
                }
                else if (apprForm.FormID == (int)Common.AppraisalForm.FormC)
                {
                    var getFormHdrDetails = genericRepo.Get<DTOModel.FormGroupCHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr && !x.IsDeleted).FirstOrDefault();
                    if (getFormHdrDetails != null)
                    {
                        getFormHdrDetails.FormState = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getFormHdrDetails.Remarks = apprForm.UploadRemarks;
                        genericRepo.Update<DTOModel.FormGroupCHdr>(getFormHdrDetails);
                    }
                }
                else if (apprForm.FormID == (int)Common.AppraisalForm.FormD)
                {
                    var getFormHdrDetails = genericRepo.Get<DTOModel.FormGroupDHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr && !x.IsDeleted).FirstOrDefault();
                    if (getFormHdrDetails != null)
                    {
                        getFormHdrDetails.FormState = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getFormHdrDetails.AcceptanceAuthorityRemarks = apprForm.UploadRemarks;
                        genericRepo.Update<DTOModel.FormGroupDHdr>(getFormHdrDetails);
                    }
                }
                else if (apprForm.FormID == (int)Common.AppraisalForm.FormE)
                {
                    var getFormHdrDetails = genericRepo.Get<DTOModel.FormGroupEHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr && !x.IsDeleted).FirstOrDefault();
                    if (getFormHdrDetails != null)
                    {
                        getFormHdrDetails.FormState = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getFormHdrDetails.Remarks = apprForm.UploadRemarks;
                        genericRepo.Update<DTOModel.FormGroupEHdr>(getFormHdrDetails);
                    }
                }
                else if (apprForm.FormID == (int)Common.AppraisalForm.FormF)
                {
                    var getFormHdrDetails = genericRepo.Get<DTOModel.FormGroupFHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr && !x.IsDeleted).FirstOrDefault();
                    if (getFormHdrDetails != null)
                    {
                        getFormHdrDetails.FormState = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getFormHdrDetails.Remarks = apprForm.UploadRemarks;
                        genericRepo.Update<DTOModel.FormGroupFHdr>(getFormHdrDetails);
                    }
                }
                else if (apprForm.FormID == (int)Common.AppraisalForm.FormG)
                {
                    var getFormHdrDetails = genericRepo.Get<DTOModel.FormGroupGHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr).FirstOrDefault();
                    if (getFormHdrDetails != null)
                    {
                        getFormHdrDetails.FormState = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getFormHdrDetails.AcceptanceAuthorityComment = apprForm.UploadRemarks;
                        genericRepo.Update<DTOModel.FormGroupGHdr>(getFormHdrDetails);
                    }
                }
                else if (apprForm.FormID == (int)Common.AppraisalForm.FormH)
                {
                    var getFormHdrDetails = genericRepo.Get<DTOModel.FormGroupHHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr).FirstOrDefault();
                    if (getFormHdrDetails != null)
                    {
                        getFormHdrDetails.FormState = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getFormHdrDetails.AcceptanceAuthorityRemark = apprForm.UploadRemarks;
                        genericRepo.Update<DTOModel.FormGroupHHdr>(getFormHdrDetails);
                    }
                }
                #endregion

                #region Update AppraisalFormHdr table
                if (existingFileDetails == null)
                {
                    var getAppraisalFormHdr = genericRepo.Get<DTOModel.AppraisalFormHdr>(x => x.FormGroupID == apprForm.frmAttributes.FormGroupID && x.EmployeeID == apprForm.EmployeeID && x.ReportingYr == apprForm.ReportingYr).FirstOrDefault();
                    if (getAppraisalFormHdr != null)
                    {
                        getAppraisalFormHdr.StatusID = (int)AppraisalFormState.SubmitedByAcceptanceAuth;
                        getAppraisalFormHdr.UpdatedBy = apprForm.UploadedDocList.FirstOrDefault().UploadedBy;
                        getAppraisalFormHdr.UpdatedOn = DateTime.Now;
                        genericRepo.Update<DTOModel.AppraisalFormHdr>(getAppraisalFormHdr);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public APARReviewedSignedCopy GetAPARDocumentsList(int appraisalFormID, int empID, int formGroupId)
        {
            log.Info($"AppraisalFormService/GetAPARDocumentsList");
            try
            {
                var dtoAPARDocList = genericRepo.Get<DTOModel.APARReviewedSignedCopy>(x => x.FormID == appraisalFormID && x.FormGroupID == formGroupId && x.AppraisalForm.AppraisalFormHdrs.Any(y => y.EmployeeID == empID)).FirstOrDefault();

                if (dtoAPARDocList != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.APARReviewedSignedCopy, Model.APARReviewedSignedCopy>()
                        .ForMember(d => d.FormID, o => o.MapFrom(s => s.FormID))
                        .ForMember(d => d.FormGroupID, o => o.MapFrom(s => s.FormGroupID))
                        .ForMember(d => d.WorkFlowID, o => o.MapFrom(s => s.WorkFlowID))
                        .ForMember(d => d.UploadedDocName, o => o.MapFrom(s => s.UploadedDocName))
                        .ForMember(d => d.EmployeeId, o => o.UseValue(empID))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                }
                return Mapper.Map<Model.APARReviewedSignedCopy>(dtoAPARDocList);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        #endregion

        #region Export

        public bool ExportAPARStatusList(DataTable dtTable, string sFullPath, string fileName)
        {
            log.Info($"AppraisalFormService/ExportAPARStatusList/{sFullPath}/{fileName}");
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

        public bool ExportEmpAPARTraining(DataSet dsSource, string sFullPath, string fileName)
        {
            try
            {
                var flag = false;
                sFullPath = $"{sFullPath}{fileName}";
                flag = exportExcel.ExportToExcel(dsSource, sFullPath, fileName);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        #endregion

        #region Process Approval Detail
        public List<FormGroupAHdr> GetProcessApprovalDetail(CommonFilter filters)
        {
            log.Info($"AppraisalFormService/GetProcessApprovalDetail");
            try
            {
                var aprApprvaldtl = appraisalRepo.GetProcessApprovalDetail(filters.ProcessID, filters.BranchID);


                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.SP_GetProcessApprovalDetail_Result, Model.FormGroupAHdr>()
                     .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                    .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee))
                    .ForMember(d => d.PlaceOfJoin, o => o.MapFrom(s => s.Branch))
                    .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.ReportingTo))
                    .ForMember(d => d.ReviewerTo, o => o.MapFrom(s => s.ReviewerTo))
                    .ForMember(d => d.AcceptanceAuth, o => o.MapFrom(s => s.AcceptanceAuthority))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.RPTEmpCode, o => o.MapFrom(s => s.RTEmpCode))
                    .ForMember(d => d.RVEmpCode, o => o.MapFrom(s => s.RVEmpCode))
                    .ForMember(d => d.ACCEmpCode, o => o.MapFrom(s => s.ACCEmpCode))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoList = Mapper.Map<List<FormGroupAHdr>>(aprApprvaldtl);
                return dtoList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ExportApprovalList(DataTable dtTable, string sFullPath, string fileName,string tFilter)
        {
            log.Info($"AppraisalFormService/ExportApprovalList/{sFullPath}/{fileName}");
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
                        ExportApprovalListToExcel(exportHdr, dtTable, fileName, sFullPath,tFilter);
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


        public List<FormTrainingDtls> GetAPAREmployeeTrainings(int ? employeeID, string ReportingYr)
        {
            log.Info($"AppraisalFormService/GetAPAREmployeeTrainings/employeeID={employeeID}/ ReportingYr={ReportingYr}");
            try
            {
                IEnumerable<FormTrainingDtls> dtoaparTraining = Enumerable.Empty<FormTrainingDtls>();
                var aparTrainings = appraisalRepo.GetAPARTrainingbySubordinate(employeeID,ReportingYr).ToList();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetAPARTrainingbySubordinate_Result, FormTrainingDtls>()
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.FormTraining, o => o.MapFrom(s => (Model.FormPart4Training)s.TrainingID))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.EmployeeCode + " - " +s.Name))
                     .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.Reporting));

                });
                dtoaparTraining = Mapper.Map<List<FormTrainingDtls>>(aparTrainings);
                return dtoaparTraining.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<FormTrainingDtls> GetSubordinateTrainings(int? employeeID, string ReportingYr)
        {
            log.Info($"AppraisalFormService/GetSubordinateTrainings/employeeID={employeeID}/ ReportingYr={ReportingYr}");
            try
            {
                IEnumerable<FormTrainingDtls> dtoaparTraining = Enumerable.Empty<FormTrainingDtls>();
                var aparTrainings = appraisalRepo.GetSubordinateTraining(employeeID, ReportingYr);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.GetSubordinateTraining_Result, FormTrainingDtls>()
                    .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                    .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.DesignationName))
                    .ForMember(d => d.FormTraining, o => o.MapFrom(s => (Model.FormPart4Training)s.TrainingID))
                    .ForMember(d => d.Name, o => o.MapFrom(s => s.EmployeeCode + " - " + s.Name))
                    .ForMember(d => d.ReportingTo, o => o.MapFrom(s => s.Reporting));
                });
                dtoaparTraining = Mapper.Map<List<FormTrainingDtls>>(aparTrainings);
                return dtoaparTraining.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertAPARTrainingforEmployee(AppraisalForm aprObj,int userID)
        {
            #region Update FormGroupTrainingDtls
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Model.FormTrainingDtls, DTOModel.SubordinatesTraining>()
                .ForMember(d => d.CreatedOn, o => o.UseValue(DateTime.Now))
                .ForMember(d => d.CreatedBy, o => o.UseValue(userID))
                .ForMember(d => d.TrainingID, o => o.MapFrom(s => (int)s.FormTraining))
                .ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
                .ForMember(d => d.EmployeeID, o => o.MapFrom(s=> s.EmployeeID))
               .ForMember(d => d.ReportingYr, o => o.UseValue(aprObj.ReportingYr))
                .ForAllOtherMembers(d => d.Ignore())
                ;
            });
            var dtoformGroupTrainingDtls = Mapper.Map<List<DTOModel.SubordinatesTraining>>(aprObj.formGroupATrainingDtls);           
            if (dtoformGroupTrainingDtls != null)
                genericRepo.AddMultipleEntity<DTOModel.SubordinatesTraining>(dtoformGroupTrainingDtls);

            return true;
            #endregion
        }
    }
}
