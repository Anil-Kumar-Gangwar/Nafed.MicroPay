using AutoMapper;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using DTOModel = Nafed.MicroPay.Data.Models;
using System.Linq;
using Nafed.MicroPay.ImportExport.Interfaces;


namespace Nafed.MicroPay.Services
{
    public class ChildrenEducationService : BaseService, IChildrenEducationService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IChildrenEducationRepository childrenRepo;
        private readonly IExport exportExcel;
        public ChildrenEducationService(IGenericRepository genericRepo, IChildrenEducationRepository childrenRepo, IExport exportExcel)
        {
            this.genericRepo = genericRepo;
            this.childrenRepo = childrenRepo;
            this.exportExcel = exportExcel;
        }
        public bool InsertChildrenEducationData(Model.ChildrenEducationHdr childrenEduForm, out int chldrenHdrId)
        {
            log.Info($"ChildrenEducationService/InsertChildrenEducationData");
            bool flag = false;
            chldrenHdrId = 0;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.ChildrenEducationHdr, DTOModel.ChildrenEducationHdr>()
                    .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                    .ForMember(d => d.ReportingYear, o => o.MapFrom(s => s.ReportingYear))
                    .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                    .ForMember(d => d.DepartmentId, o => o.MapFrom(s => s.DepartmentId))
                    .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                    .ForMember(d => d.ReceiptNo, o => o.MapFrom(s => s.ReceiptNo))
                    .ForMember(d => d.ReceiptDate, o => o.MapFrom(s => s.ReceiptDate))
                    .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                    .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                    .ForMember(d => d.StatusId, o => o.MapFrom(s => s.StatusId))
                    .ForMember(d => d.IsDeleted, o => o.UseValue(false))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoChildrenEducationHdr = Mapper.Map<DTOModel.ChildrenEducationHdr>(childrenEduForm);
                genericRepo.Insert<DTOModel.ChildrenEducationHdr>(dtoChildrenEducationHdr);
                int result = dtoChildrenEducationHdr.ChildrenEduHdrID;
                chldrenHdrId = result;
                if (result > 0)
                {
                    #region Add Children Education Details ============

                    var dobCheck = DateTime.Now.AddYears(-4);
                    var dependentDetails = genericRepo.Get<DTOModel.EmployeeDependent>(x => x.EmployeeId == childrenEduForm.EmployeeId && !x.IsDeleted && (x.RelationID == 6 || x.RelationID == 7) && x.DOB < dobCheck).ToList();
                    List<Model.ChildrenEducationDetails> childrenEduDetails = new List<Model.ChildrenEducationDetails>();
                    foreach (var dep in dependentDetails)
                    {
                        Model.ChildrenEducationDetails addChildrenEducationDetails = new Model.ChildrenEducationDetails();
                        addChildrenEducationDetails = new ChildrenEducationDetails
                        {
                            CreatedBy = childrenEduForm.CreatedBy,
                            CreatedOn = childrenEduForm.CreatedOn,
                            ChildrenEduHdrID = result,
                            EmpDependentID = dep.EmpDependentID,
                            NotApplicable = false
                        };
                        childrenEduDetails.Add(addChildrenEducationDetails);
                    }

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ChildrenEducationDetails, DTOModel.ChildrenEducationDetail>()
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.ChildrenEduHdrID, o => o.MapFrom(s => s.ChildrenEduHdrID))
                        .ForMember(d => d.EmpDependentID, o => o.MapFrom(s => s.EmpDependentID))
                        .ForMember(d => d.NotApplicable, o => o.MapFrom(s => s.NotApplicable))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoEducationDetails = Mapper.Map<List<DTOModel.ChildrenEducationDetail>>(childrenEduDetails);
                    if (dtoEducationDetails != null)
                        genericRepo.AddMultipleEntity<DTOModel.ChildrenEducationDetail>(dtoEducationDetails);
                    #endregion
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

        public List<Model.ChildrenEducationHdr> GetEmployeeChildrenEducationList(int userEmpID)
        {
            log.Info($"ChildrenEducationService/GetEmployeeChildrenEducationList");
            try
            {
                var dtoChildrenEducation = genericRepo.Get<DTOModel.ChildrenEducationHdr>(x => x.EmployeeId == userEmpID && !x.IsDeleted).ToList();

                if (dtoChildrenEducation != null && dtoChildrenEducation.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.ChildrenEducationHdr, Model.ChildrenEducationHdr>()
                        .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                        .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                        .ForMember(d => d.ReportingYear, o => o.MapFrom(s => s.ReportingYear))
                        .ForMember(d => d.ChildrenEduHdrID, o => o.MapFrom(s => s.ChildrenEduHdrID))
                        .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount.HasValue ? (s.Amount.Value.ToString("0.##")) : "0"));
                    });
                }
                return Mapper.Map<List<Model.ChildrenEducationHdr>>(dtoChildrenEducation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public Model.ChildrenEducationHdr GetChildrenEducation(int empId, int childrenEduHdrId)
        {
            log.Info($"ChildrenEducationService/GetChildrenEducation");
            try
            {
                var dtoChildrenEducation = genericRepo.Get<DTOModel.ChildrenEducationHdr>(x => x.EmployeeId == empId && x.ChildrenEduHdrID == childrenEduHdrId && !x.IsDeleted).FirstOrDefault();

                if (dtoChildrenEducation != null)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.ChildrenEducationHdr, Model.ChildrenEducationHdr>()
                        .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(d => d.Branch, o => o.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                        .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                        .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                        .ForMember(d => d.ReportingYear, o => o.MapFrom(s => s.ReportingYear))
                        .ForMember(d => d.DesignationId, o => o.MapFrom(s => s.DesignationId))
                        .ForMember(d => d.DepartmentId, o => o.MapFrom(s => s.DepartmentId))
                        .ForMember(d => d.ReceiptNo, o => o.MapFrom(s => s.ReceiptNo))
                        .ForMember(d => d.ReceiptNo2, o => o.MapFrom(s => s.ReceiptNo2))
                        .ForMember(d => d.ReceiptDate, o => o.MapFrom(s => s.ReceiptDate))
                        .ForMember(d => d.ReceiptDate2, o => o.MapFrom(s => s.ReceiptDate2))
                        .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount.HasValue ? (s.Amount.Value.ToString("0.##")) : "0"))
                        .ForMember(d => d.ChildrenEduHdrID, o => o.MapFrom(s => s.ChildrenEduHdrID))
                        .ForMember(d => d.StatusId, o => o.MapFrom(s => s.StatusId))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                }
                return Mapper.Map<Model.ChildrenEducationHdr>(dtoChildrenEducation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Model.ChildrenEducationDetails> GetChildrenEducationDetails(int empID, int childrenEduHdrId)
        {
            log.Info($"ChildrenEducationService/GetChildrenEducationDetails/{empID}/{childrenEduHdrId}");
            try
            {
                //IEnumerable<ChildrenEducationDetails> childrenEducationDescription = Enumerable.Empty<ChildrenEducationDetails>();
                var formChildrenEducationDTO = childrenRepo.GetChildrenEducationDetails(empID, childrenEduHdrId);
                Mapper.Initialize(
                  cfg =>
                  {
                      cfg.CreateMap<DTOModel.GetChildrenEducationDetails_Result, ChildrenEducationDetails>();
                  }
                  );
               var childrenEducationDescription = Mapper.Map<List<ChildrenEducationDetails>>(formChildrenEducationDTO.ToList());
                if (childrenEducationDescription.Count == 0)
                    childrenEducationDescription = new List<ChildrenEducationDetails>();
                return childrenEducationDescription;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<SelectListModel> GetDependentList(int empID)
        {
            log.Info($"ChildrenEducationService/GetDependentList");
            try
            {
                var dobCheck = DateTime.Now.AddYears(-4);
                var dependentList = genericRepo.Get<DTOModel.EmployeeDependent>(x => x.EmployeeId == empID && (x.RelationID == 6 || x.RelationID == 7) && x.DOB < dobCheck && !x.IsDeleted).ToList();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.EmployeeDependent, Model.SelectListModel>()
                    .ForMember(d => d.id, o => o.MapFrom(s => s.EmpDependentID))
                    .ForMember(d => d.value, o => o.MapFrom(s => s.DependentName))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoDependentList = Mapper.Map<List<Model.SelectListModel>>(dependentList);
                return dtoDependentList;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateChildrenEducationData(Model.ChildrenEducationHdr childrenEduForm)
        {
            log.Info($"ChildrenEducationService/UpdateChildrenEducationData");
            bool flag = false;
            try
            {
                childrenRepo.UpdateChildrenEducationData(childrenEduForm.ChildrenEduHdrID, childrenEduForm.Amount, childrenEduForm.ReceiptDate, childrenEduForm.ReceiptDate2, childrenEduForm.ReceiptNo, childrenEduForm.ReceiptNo2, childrenEduForm.StatusId);

                var getChildrenEducationhdr = genericRepo.Get<DTOModel.ChildrenEducationHdr>(x => x.ChildrenEduHdrID == childrenEduForm.ChildrenEduHdrID && !x.IsDeleted).FirstOrDefault();
                if (getChildrenEducationhdr != null)
                {
                    getChildrenEducationhdr.Amount = childrenEduForm.Amount;
                    getChildrenEducationhdr.ReceiptDate = childrenEduForm.ReceiptDate;
                    getChildrenEducationhdr.ReceiptDate2 = childrenEduForm.ReceiptDate2;
                    getChildrenEducationhdr.ReceiptNo = childrenEduForm.ReceiptNo;
                    getChildrenEducationhdr.ReceiptNo2 = childrenEduForm.ReceiptNo2;
                    getChildrenEducationhdr.StatusId = childrenEduForm.StatusId;



                    // genericRepo.Update<DTOModel.ChildrenEducationHdr>(getChildrenEducationhdr);
                }
                int result = getChildrenEducationhdr.ChildrenEduHdrID;
                #region Insert Children Education
                var getChildrenEducationDetails = genericRepo.Get<DTOModel.ChildrenEducationDetail>(x => x.ChildrenEduHdrID == result).ToList();
                var formChildrenEducationDetails = childrenEduForm.ChildrenEducationDetailsList;
                if (formChildrenEducationDetails.Count > 0)
                {
                    var sno = 1;
                    formChildrenEducationDetails.ForEach(x =>
                    {
                        x.sno = sno++;
                    });
                }
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.ChildrenEducationDetails, DTOModel.ChildrenEducationDetail>()
                    .ForMember(d => d.CreatedOn, o => o.UseValue(childrenEduForm.UpdatedOn))
                    .ForMember(d => d.CreatedBy, o => o.UseValue(childrenEduForm.UpdatedBy))
                    .ForMember(d => d.ChildrenEduHdrID, o => o.UseValue(result))
                    .ForMember(d => d.EmpDependentID, o => o.MapFrom(s => s.EmpDependentID))
                    .ForMember(d => d.SchoolName, o => o.MapFrom(s => s.SchoolName))
                    .ForMember(d => d.ClassName, o => o.MapFrom(s => s.ClassName))
                    .ForMember(d => d.NotApplicable, o => o.MapFrom(s => s.NotApplicable))
                    .ForAllOtherMembers(d => d.Ignore());
                });
                var dtoChildrenEducationDescription = Mapper.Map<List<DTOModel.ChildrenEducationDetail>>(formChildrenEducationDetails);
                if (getChildrenEducationDetails != null && getChildrenEducationDetails.Count > 0)
                    genericRepo.DeleteAll<DTOModel.ChildrenEducationDetail>(getChildrenEducationDetails);
                if (dtoChildrenEducationDescription != null && dtoChildrenEducationDescription.Count() > 0)
                    genericRepo.AddMultipleEntity<DTOModel.ChildrenEducationDetail>(dtoChildrenEducationDescription);
                #endregion
                #region Insert Children Document
                if (childrenEduForm.ChildrenEducationDocumentsList.Count > 0)
                {
                    var getChildrenEducationDoc = genericRepo.Get<DTOModel.ChildrenEducationDocument>(x => x.ChildrenEduHdrID == result && x.EmployeeID == childrenEduForm.EmployeeId).ToList();

                    if (childrenEduForm.ChildrenEducationDocumentsList.Count > 0)
                    {
                        childrenEduForm.ChildrenEducationDocumentsList.ForEach(x =>
                        {
                            x.ChildrenEduHdrID = result;
                        });
                    }
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ChildrenEducationDocuments, DTOModel.ChildrenEducationDocument>()
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.ChildrenEduHdrID, o => o.MapFrom(s => s.ChildrenEduHdrID))
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.FilePath, o => o.MapFrom(s => s.FilePath))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoChildrenEducationDoc = Mapper.Map<List<DTOModel.ChildrenEducationDocument>>(childrenEduForm.ChildrenEducationDocumentsList);
                    //if (getChildrenEducationDoc != null && getChildrenEducationDoc.Count > 0)
                    //    genericRepo.DeleteAll<DTOModel.ChildrenEducationDocument>(getChildrenEducationDoc);
                    if (dtoChildrenEducationDoc != null && dtoChildrenEducationDoc.Count() > 0)
                        genericRepo.AddMultipleEntity<DTOModel.ChildrenEducationDocument>(dtoChildrenEducationDoc);
                }
                #endregion
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public int ChildrenEducationExist(int employeeId, string reportingYr)
        {
            log.Info($"ChildrenEducationService/ChildrenEducationExist");
            try
            {
                int result = 0;
                if (genericRepo.Exists<DTOModel.ChildrenEducationHdr>(x => x.EmployeeId == employeeId && x.ReportingYear == reportingYr && !x.IsDeleted))
                {
                    result = genericRepo.Get<DTOModel.ChildrenEducationHdr>(x => x.EmployeeId == employeeId && x.ReportingYear == reportingYr && !x.IsDeleted).FirstOrDefault().ChildrenEduHdrID;
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public List<Model.ChildrenEducationHistoryModel> GetChildrenEducationForAdmin(AppraisalFormApprovalFilter filters)
        {
            log.Info($"ChildrenEducationService/GetChildrenEducationForAdmin/");
            try
            {
                var dtoChildrenEducation = genericRepo.Get<DTOModel.ChildrenEducationHdr>(x => (filters.selectedBranchId != 0 ? x.BranchId == filters.selectedBranchId : 1 > 0) && (filters.selectedEmployeeID != 0 ? x.EmployeeId == filters.selectedEmployeeID : 1 > 0) && (filters.selectedReportingYear != null ? x.ReportingYear == filters.selectedReportingYear : 1 > 0) && !x.IsDeleted && x.StatusId == (int)ChildrenEducationStatus.SubmitedByEmployee).ToList();

                if (dtoChildrenEducation != null && dtoChildrenEducation.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.ChildrenEducationHdr, Model.ChildrenEducationHistoryModel>()
                        .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(d => d.Branch, o => o.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                        .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                        .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                        .ForMember(d => d.ReportingYear, o => o.MapFrom(s => s.ReportingYear))
                        .ForMember(d => d.ReceiptNo, o => o.MapFrom(s => s.ReceiptNo))
                        .ForMember(d => d.ReceiptDate, o => o.MapFrom(s => s.ReceiptDate))
                        .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount.HasValue ? (s.Amount.Value.ToString("0.##")) : "0"))
                        .ForMember(d => d.ChildrenEduHdrID, o => o.MapFrom(s => s.ChildrenEduHdrID))
                        .ForMember(d => d.StatusId, o => o.MapFrom(s => s.StatusId))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                }
                return Mapper.Map<List<Model.ChildrenEducationHistoryModel>>(dtoChildrenEducation);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Model.ChildrenEducationDocuments> GetChildrenEducationDocumentsList(int employeeId, int childrenEduHdrId)
        {
            log.Info($"ChildrenEducationService/GetChildrenEducationDocumentsList/{employeeId}/{childrenEduHdrId}");
            try
            {
                var dtoChildrenEducationDoc = genericRepo.Get<DTOModel.ChildrenEducationDocument>(x => x.EmployeeID == employeeId && x.ChildrenEduHdrID == childrenEduHdrId).ToList();

                if (dtoChildrenEducationDoc != null && dtoChildrenEducationDoc.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.ChildrenEducationDocument, Model.ChildrenEducationDocuments>()
                        .ForMember(d => d.ReceiptID, o => o.MapFrom(s => s.ReceiptID))
                        .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                        .ForMember(d => d.ChildrenEduHdrID, o => o.MapFrom(s => s.ChildrenEduHdrID))
                        .ForMember(d => d.FilePath, o => o.MapFrom(s => s.FilePath))
                        .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode + "-" + s.tblMstEmployee.Name))
                        .ForAllOtherMembers(d => d.Ignore()); ;
                    });
                }
                return Mapper.Map<List<Model.ChildrenEducationDocuments>>(dtoChildrenEducationDoc);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DeleteReceiptsDocuments(int receiptId)
        {
            log.Info($"ChildrenEducationService/DeleteReceiptsDocuments");
            bool flag = false;
            try
            {
                genericRepo.Delete<DTOModel.ChildrenEducationDocument>(receiptId);
                flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateEmployeeDependent(List<ChildrenEducationDetails> childrenEducationDetails, IEnumerable<ChildrenEducationDetails> deletedResult)
        {
            log.Info($"ChildrenEducationService/UpdateEmployeeDependent");
            try
            {
                bool flag = false;
                if (childrenEducationDetails.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Model.ChildrenEducationDetails, DTOModel.ChildrenEducationDetail>()
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                        .ForMember(d => d.ChildrenEduHdrID, o => o.MapFrom(s => s.ChildrenEduHdrID))
                        .ForMember(d => d.EmpDependentID, o => o.MapFrom(s => s.EmpDependentID))
                        .ForMember(d => d.SchoolName, o => o.MapFrom(s => s.SchoolName))
                        .ForMember(d => d.ClassName, o => o.MapFrom(s => s.ClassName))
                        .ForMember(d => d.NotApplicable, o => o.MapFrom(s => s.NotApplicable))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                    var dtoChildrenEducationDescription = Mapper.Map<List<DTOModel.ChildrenEducationDetail>>(childrenEducationDetails);
                    if (dtoChildrenEducationDescription != null && dtoChildrenEducationDescription.Count() > 0)
                        genericRepo.AddMultipleEntity<DTOModel.ChildrenEducationDetail>(dtoChildrenEducationDescription);
                    flag = true;
                }
                if (deletedResult != null && deletedResult.Count() > 0)
                {
                    foreach (var item in deletedResult)
                    {
                        var details = genericRepo.Get<DTOModel.ChildrenEducationDetail>(x => x.EmpDependentID == item.EmpDependentID && x.ChildrenEduHdrID == item.ChildrenEduHdrID).FirstOrDefault();
                        if (details != null)
                            genericRepo.Delete<DTOModel.ChildrenEducationDetail>(details.ChildrenEduDetailID);
                        flag = true;
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

        public List<Model.ChildrenEducationHdr> GetEmployeeChildrenEducationYearWise(CommonFilter filters)
        {
            log.Info($"ChildrenEducationService/GetEmployeeChildrenEducationYearWise");
            try
            {
                var dtoChildrenEducation = genericRepo.Get<DTOModel.ChildrenEducationHdr>(x => (filters.BranchID.HasValue && filters.BranchID != 0 ? x.BranchId == filters.BranchID.Value : 1 > 0) && (filters.EmployeeID.HasValue && filters.EmployeeID != 0 ? x.EmployeeId == filters.EmployeeID.Value : 1 > 0) && (filters.ReportingYear != null ? x.ReportingYear == filters.ReportingYear : 1 > 0) && !x.IsDeleted).ToList();
                if (dtoChildrenEducation != null && dtoChildrenEducation.Count > 0)
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<DTOModel.ChildrenEducationHdr, Model.ChildrenEducationHdr>()
                        .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId))
                        .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.tblMstEmployee.EmployeeCode))
                        .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.tblMstEmployee.Name))
                        .ForMember(d => d.Branch, o => o.MapFrom(s => s.tblMstEmployee.Branch.BranchName))
                        .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.tblMstEmployee.Designation.DesignationName))
                        .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.tblMstEmployee.Department.DepartmentName))
                        .ForMember(d => d.ReportingYear, o => o.MapFrom(s => s.ReportingYear))
                        .ForMember(d => d.ReceiptNo, o => o.MapFrom(s => s.ReceiptNo))
                        .ForMember(d => d.ReceiptDate, o => o.MapFrom(s => s.ReceiptDate))
                        .ForMember(d => d.ChildrenEduHdrID, o => o.MapFrom(s => s.ChildrenEduHdrID))
                        .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount.HasValue ? (s.Amount.Value.ToString("0.##")) : "0"))
                        .ForAllOtherMembers(d => d.Ignore());
                    });
                }
                return Mapper.Map<List<Model.ChildrenEducationHdr>>(dtoChildrenEducation);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DownloadExcel(System.Data.DataSet dsSource, string sFullPath, string fileName)
        {
            try
            {
                var flag = false;
                sFullPath = $"{sFullPath}{fileName}";
                flag = exportExcel.ExportFormatedExcel(dsSource, sFullPath, fileName);
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
    }
}
