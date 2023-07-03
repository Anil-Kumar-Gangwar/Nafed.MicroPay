using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;
using AutoMapper;
using DTOModel = Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;

namespace Nafed.MicroPay.Services
{
    public class FileTrackingSytemService : BaseService, IFileTrackingSytemService
    {
        private readonly IGenericRepository genericRepo;
        private readonly IFileManagementSystemRepository fileManagRepo;
        public FileTrackingSytemService(IGenericRepository genericRepo, IFileManagementSystemRepository fileManagRepo)
        {
            this.genericRepo = genericRepo;
            this.fileManagRepo = fileManagRepo;
        }
        public bool InsertFileTracking(FileWorkflow fileWorkFlow)
        {
            log.Info($"FileTrackingSytemService/InsertFileTracking");
            try
            {
                bool flag = false;

                for (int i = 0; i < fileWorkFlow.intEmployeeID.Length; i++)
                {
                    int designationid = 0;
                    int empid = fileWorkFlow.intEmployeeID[i];
                    var designation = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == empid).FirstOrDefault();
                    if (designation != null)
                    {
                        designationid = designation.DesignationID;
                    }
                    Mapper.Initialize(cfg => cfg.CreateMap<FileWorkflow, DTOModel.FileWorkflow>()
                    .ForMember(d => d.initiateDepartmentID, o => o.MapFrom(s => s.initiateDepartmentID))
                    .ForMember(d => d.initiateDesignationID, o => o.UseValue(designationid))
                    .ForMember(d => d.initiateEmployeeID, o => o.MapFrom(s => s.intEmployeeID[i]))
                    .ForMember(d => d.ParkByDepartmentID, o => o.MapFrom(s => s.ParkByDepartmentID))
                    .ForMember(d => d.ParkByDesignationID, o => o.MapFrom(s => s.ParkByDesignationID))
                    .ForMember(d => d.ParkByEmployeeID, o => o.MapFrom(s => s.ParkByEmployeeID))
                    .ForMember(d => d.fromdate, o => o.MapFrom(s => s.fromdate))
                    .ForMember(d => d.created_by, o => o.MapFrom(s => s.created_by))
                    .ForMember(d => d.created_on, o => o.MapFrom(s => s.created_on))
                    .ForAllOtherMembers(d => d.Ignore())
                    );
                    var dtoFileWorkFlow = Mapper.Map<DTOModel.FileWorkflow>(fileWorkFlow);
                    genericRepo.Insert(dtoFileWorkFlow);
                    flag = true;
                    if (flag)
                    {
                        var getData = genericRepo.GetByID<DTOModel.FileWorkflow>(dtoFileWorkFlow.ID);
                        if (getData != null)
                        {
                            getData.FileWorkFlowID = dtoFileWorkFlow.ID;
                            genericRepo.Update(getData);
                            flag = true;
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }

        }

        public bool UpdateFileTracking(FileWorkflow fileWorkFlow)
        {
            log.Info($"FileTrackingSytemService/UpdateFileTracking");
            try
            {
                bool flag = false;

                var getData = genericRepo.GetByID<DTOModel.FileWorkflow>(fileWorkFlow.WorkFlowID);
                if (getData != null)
                {
                    getData.todate = fileWorkFlow.fromdate.Value.AddDays(-1);
                    genericRepo.Update(getData);
                    flag = true;
                }
                if (flag)
                {
                    for (int i = 0; i < fileWorkFlow.intEmployeeID.Length; i++)
                    {
                        int designationid = 0;
                        int empid = fileWorkFlow.intEmployeeID[i];
                        var designation = genericRepo.Get<DTOModel.tblMstEmployee>(x => x.EmployeeId == empid).FirstOrDefault();
                        if (designation != null)
                        {
                            designationid = designation.DesignationID;
                        }
                        Mapper.Initialize(cfg => cfg.CreateMap<FileWorkflow, DTOModel.FileWorkflow>()
                   .ForMember(d => d.initiateDepartmentID, o => o.MapFrom(s => s.initiateDepartmentID))
                   .ForMember(d => d.initiateDesignationID, o => o.UseValue(designationid))
                   .ForMember(d => d.initiateEmployeeID, o => o.MapFrom(s => s.intEmployeeID[i]))
                   .ForMember(d => d.ParkByDepartmentID, o => o.MapFrom(s => s.ParkByDepartmentID))
                   .ForMember(d => d.ParkByDesignationID, o => o.MapFrom(s => s.ParkByDesignationID))
                   .ForMember(d => d.ParkByEmployeeID, o => o.MapFrom(s => s.ParkByEmployeeID))
                   .ForMember(d => d.fromdate, o => o.MapFrom(s => s.fromdate))
                   .ForMember(d => d.created_by, o => o.MapFrom(s => s.created_by))
                   .ForMember(d => d.created_on, o => o.MapFrom(s => s.created_on))
                   // .ForMember(d => d.FileWorkFlowID, o => o.UseValue(fileWorkFlow.WorkFlowID))
                   .ForAllOtherMembers(d => d.Ignore())
                   );
                        var dtoFileWorkFlow = Mapper.Map<DTOModel.FileWorkflow>(fileWorkFlow);
                        genericRepo.Insert(dtoFileWorkFlow);
                        flag = true;

                        if (flag)
                        {
                            getData = genericRepo.GetByID<DTOModel.FileWorkflow>(dtoFileWorkFlow.ID);
                            if (getData != null)
                            {
                                getData.FileWorkFlowID = dtoFileWorkFlow.ID;
                                genericRepo.Update(getData);
                                flag = true;
                            }
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }

        }

        public List<FileWorkflow> GetFileTrackingList()
        {
            log.Info($"FileTrackingSytemService/GetFileTrackingList");
            try
            {
                var getData = genericRepo.Get<DTOModel.FileWorkflow>(x => x.todate == null);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileWorkflow, FileWorkflow>()
                   .ForMember(d => d.initiateDepartmentID, o => o.MapFrom(s => s.initiateDepartmentID))
                   .ForMember(d => d.initiateDesignationID, o => o.MapFrom(s => s.initiateDesignationID))
                   .ForMember(d => d.initiateEmployeeID, o => o.MapFrom(s => s.initiateEmployeeID))
                   .ForMember(d => d.ParkByDepartmentID, o => o.MapFrom(s => s.ParkByDepartmentID))
                   .ForMember(d => d.ParkByDesignationID, o => o.MapFrom(s => s.ParkByDesignationID))
                   .ForMember(d => d.ParkByEmployeeID, o => o.MapFrom(s => s.ParkByEmployeeID))
                   .ForMember(d => d.fromdate, o => o.MapFrom(s => s.fromdate))
                   .ForMember(d => d.WorkFlowID, o => o.MapFrom(s => s.ID))
                   .ForMember(d => d.InitiatedDepartment, o => o.MapFrom(s => s.Department.DepartmentName))
                   .ForMember(d => d.InitiateEmployee, o => o.MapFrom(s => s.tblMstEmployee.Name))
                   .ForMember(d => d.ParkedEmployee, o => o.MapFrom(s => s.tblMstEmployee1.Name))
                   .ForAllOtherMembers(d => d.Ignore())
                   );
                var dtoFileWorkFlow = Mapper.Map<List<FileWorkflow>>(getData);
                return dtoFileWorkFlow;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public List<FileWorkflow> GetFileTrackingListForPopup(int departmentID)
        {
            log.Info($"FileTrackingSytemService/GetFileTrackingListForPopup");
            try
            {
                var getData = genericRepo.Get<DTOModel.FileWorkflow>(x => x.initiateDepartmentID == departmentID);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileWorkflow, FileWorkflow>()
                   .ForMember(d => d.fromdate, o => o.MapFrom(s => s.fromdate))
                   .ForMember(d => d.todate, o => o.MapFrom(s => s.todate))
                   .ForMember(d => d.InitiatedDepartment, o => o.MapFrom(s => s.Department.DepartmentName))
                   .ForMember(d => d.ParkedDepartment, o => o.MapFrom(s => s.Department1.DepartmentName))
                   .ForMember(d => d.InitiatedDesignation, o => o.MapFrom(s => s.Designation.DesignationName))
                   .ForMember(d => d.ParkedDesignation, o => o.MapFrom(s => s.Designation1.DesignationName))
                   .ForMember(d => d.InitiateEmployee, o => o.MapFrom(s => s.tblMstEmployee.Name))
                   .ForMember(d => d.ParkedEmployee, o => o.MapFrom(s => s.tblMstEmployee1.Name))
                   .ForAllOtherMembers(d => d.Ignore())
                   );
                var dtoFileWorkFlow = Mapper.Map<List<FileWorkflow>>(getData);
                return dtoFileWorkFlow;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        #region File Tracking Type

        public List<Model.FileTrackingType> GetFileTrackingTypeList()
        {
            log.Info($"FileTrackingSytemService/GetFileTrackingTypeList");
            try
            {
                var result = genericRepo.Get<DTOModel.FileTrackingType>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.FileTrackingType, Model.FileTrackingType>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.FileType, c => c.MapFrom(s => s.FileType))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listFileType = Mapper.Map<List<Model.FileTrackingType>>(result);
                return listFileType;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool FileTypeExists(string fileType)
        {
            return genericRepo.Exists<DTOModel.FileTrackingType>(x => x.FileType == fileType && x.IsDeleted == false);
        }

        public int InsertFileTrackingType(FileTrackingType fileType)
        {
            log.Info($"FileTrackingSytemService/InsertFileTrackingType");
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.FileTrackingType, DTOModel.FileTrackingType>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.FileType, c => c.MapFrom(s => s.FileType))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoFileType = Mapper.Map<DTOModel.FileTrackingType>(fileType);
                genericRepo.Insert<DTOModel.FileTrackingType>(dtoFileType);
                return dtoFileType.ID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public Model.FileTrackingType GetFileTrackingTypeByID(int id)
        {
            log.Info($"FileTrackingSytemService/InsertFileTrackingType/{id}");
            try
            {
                var fileTypeObj = genericRepo.GetByID<DTOModel.FileTrackingType>(id);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.FileTrackingType, Model.FileTrackingType>()
                    .ForMember(c => c.ID, c => c.MapFrom(s => s.ID))
                    .ForMember(c => c.FileType, c => c.MapFrom(s => s.FileType))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.FileTrackingType, Model.FileTrackingType>(fileTypeObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool UpdateFileTrackingType(Model.FileTrackingType fileType)
        {
            log.Info($"FileTrackingSytemService/UpdateFileTrackingType");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.FileTrackingType>(fileType.ID);
                if (dtoObj != null)
                {
                    dtoObj.FileType = fileType.FileType;
                    genericRepo.Update<DTOModel.FileTrackingType>(dtoObj);
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

        public bool Delete(int id)
        {
            log.Info($"FileTrackingSytemService/Delete/{id}");
            bool flag = false;
            try
            {

                var dtofileType = genericRepo.GetByID<DTOModel.FileTrackingType>(id);
                if (dtofileType != null)
                {
                    dtofileType.IsDeleted = true;
                    genericRepo.Update(dtofileType);
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
        #endregion

        #region  File No Generate
        public bool IsEmpEligibleForFTMS(int employeeID)
        {
            log.Info($"FileTrackingSytemService/IsEmpEligibleForFTMS/{employeeID}");
            try
            {
                return genericRepo.Exists<DTOModel.FileWorkflow>(x => x.todate == null && x.initiateEmployeeID == employeeID);
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }

        }

        public List<FileManagement> GetFileList(int userID)
        {
            log.Info($"FileTrackingSytemService/GetFileList");
            try
            {
                var getData = genericRepo.Get<DTOModel.FileManagement>(x => x.CreatedBy == userID);
                Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileManagement, FileManagement>()
                .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                .ForMember(d => d.DiaryNo, o => o.MapFrom(s => s.DiaryNo))
                .ForMember(d => d.OtherDiaryNo, o => o.MapFrom(s => s.OtherDiaryNo))
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                .ForMember(d => d.FileSubject, o => o.MapFrom(s => s.FileSubject))
                //.ForMember(d => d.FileName, o => o.MapFrom(s => s.FileName))
                .ForMember(d => d.FileWorkFlowID, o => o.MapFrom(s => s.FileWorkFlowID))
                .ForMember(d => d.FileTypeName, o => o.MapFrom(s => s.FileTrackingType.FileType))
                .ForMember(d => d.FilePutup, o => o.MapFrom(s => s.FilePutup))
                .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                 .ForMember(d => d.FileLastStatus, o => o.MapFrom(s => s.FileLastStatus))
                .ForAllOtherMembers(d => d.Ignore())
                   );
                var dtoFileManagement = Mapper.Map<List<FileManagement>>(getData);
                return dtoFileManagement;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        public FileManagement GetFileInitiatorDetail(int employeeID)
        {
            log.Info($"FileTrackingSytemService/GetFileInitiatorDetail");
            try
            {
                var getData = genericRepo.Get<DTOModel.FileWorkflow>(x => x.todate == null && x.initiateEmployeeID == employeeID).FirstOrDefault();
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileWorkflow, FileManagement>()
                       .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                       .ForMember(d => d.DesignationName, o => o.MapFrom(s => s.Designation.DesignationName))
                        .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.initiateDepartmentID))
                       .ForMember(d => d.FileWorkFlowID, o => o.MapFrom(s => s.FileWorkFlowID))
                       .ForAllOtherMembers(d => d.Ignore())
                       );
                    var dtoFileInitiator = Mapper.Map<FileManagement>(getData);
                    return dtoFileInitiator;
                }
                else
                    return new FileManagement();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public bool GenerateFile(FileManagementSystem fileManagement)
        {
            log.Info($"FileTrackingSytemService/GenerateFile");
            try
            {
                bool flag = false;
                Mapper.Initialize(cfg => cfg.CreateMap<FileManagement, DTOModel.FileManagement>()
                .ForMember(d => d.DiaryNo, o => o.MapFrom(s => s.DiaryNo))
                .ForMember(d => d.OtherDiaryNo, o => o.MapFrom(s => s.OtherDiaryNo))
                .ForMember(d => d.DepartmentID, o => o.MapFrom(s => s.DepartmentID))
                .ForMember(d => d.FileSubject, o => o.MapFrom(s => s.FileSubject))
                //.ForMember(d => d.FileName, o => o.MapFrom(s => s.FileName))
                .ForMember(d => d.FileWorkFlowID, o => o.MapFrom(s => s.FileWorkFlowID))
                .ForMember(d => d.FileTypeID, o => o.MapFrom(s => s.FileTypeID))
                .ForMember(d => d.FilePutup, o => o.MapFrom(s => s.FilePutup))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                .ForMember(d => d.StatusID, o => o.MapFrom(s => s.StatusID))
                .ForAllOtherMembers(d => d.Ignore())
                );
                var dtoFileManagement = Mapper.Map<DTOModel.FileManagement>(fileManagement.fileManagement);
                genericRepo.Insert(dtoFileManagement);
                flag = true;
                if (flag)
                {
                    if (fileManagement.fileDocumentsList.Count > 0)
                    {
                        Mapper.Initialize(cfg => cfg.CreateMap<FileTrackingDocuments, DTOModel.FileTrackingDocument>()
                                        .ForMember(d => d.FileID, o => o.UseValue(dtoFileManagement.FileID))
                                        .ForMember(d => d.DocName, o => o.MapFrom(s => s.DocName))
                                        .ForMember(d => d.DocPathName, o => o.MapFrom(s => s.DocPathName))
                                        .ForMember(d => d.DocOrignalName, o => o.MapFrom(s => s.DocOrignalName))
                                        .ForMember(d => d.Sno, o => o.MapFrom(s => s.Sno))
                                        .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                        .ForAllOtherMembers(d => d.Ignore())
                                        );
                        var dtoFileDocuments = Mapper.Map<List<DTOModel.FileTrackingDocument>>(fileManagement.fileDocumentsList);
                        genericRepo.AddMultipleEntity(dtoFileDocuments);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }

        }

        public FileManagementSystem GetFileDetails(int fileID)
        {
            log.Info($"FileTrackingSytemService/GetFileDetails");
            try
            {
                FileManagementSystem fileManagement = new FileManagementSystem();
                var getData = genericRepo.Get<DTOModel.FileManagement>(x => x.FileID == fileID).FirstOrDefault();
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileManagement, FileManagement>()
                    .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                    .ForMember(d => d.DiaryNo, o => o.MapFrom(s => s.DiaryNo))
                    .ForMember(d => d.OtherDiaryNo, o => o.MapFrom(s => s.OtherDiaryNo))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.DepartmentName))
                    .ForMember(d => d.FileSubject, o => o.MapFrom(s => s.FileSubject))
                    //.ForMember(d => d.FileName, o => o.MapFrom(s => s.FileName))
                    .ForMember(d => d.FileWorkFlowID, o => o.MapFrom(s => s.FileWorkFlowID))
                    .ForMember(d => d.FileTypeID, o => o.MapFrom(s => s.FileTypeID))
                    .ForMember(d => d.FilePutup, o => o.MapFrom(s => s.FilePutup))
                    .ForAllOtherMembers(d => d.Ignore())
                       );

                    var dtoFileManagement = Mapper.Map<FileManagement>(getData);

                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileTrackingDocument, FileTrackingDocuments>()
                   .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                   .ForMember(d => d.DocName, o => o.MapFrom(s => s.DocName))
                   .ForMember(d => d.DocOrignalName, o => o.MapFrom(s => s.DocOrignalName))
                   .ForMember(d => d.DocPathName, o => o.MapFrom(s => s.DocPathName))
                   .ForMember(d => d.Sno, o => o.MapFrom(s => s.Sno))
                    .ForMember(d => d.DocID, o => o.MapFrom(s => s.DocID))
                   .ForAllOtherMembers(d => d.Ignore())
                      );
                    var dtodtoFileDoc = Mapper.Map<List<FileTrackingDocuments>>(getData.FileTrackingDocuments);
                    fileManagement.fileManagement = dtoFileManagement;
                    fileManagement.fileDocumentsList = dtodtoFileDoc;
                    return fileManagement;
                }
                else
                    return new FileManagementSystem();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        public bool UpdateFileDetail(FileManagementSystem fileManagement)
        {
            log.Info($"FileTrackingSytemService/UpdateFileDetail");
            try
            {
                bool flag = false;
                var getData = genericRepo.GetByID<DTOModel.FileManagement>(fileManagement.fileManagement.FileID);
                if (getData != null)
                {
                    getData.DiaryNo = fileManagement.fileManagement.DiaryNo;
                    getData.OtherDiaryNo = fileManagement.fileManagement.OtherDiaryNo;
                    getData.FileSubject = fileManagement.fileManagement.FileSubject;
                    //getData.FileName = fileManagement.FileName;
                    getData.FileTypeID = fileManagement.fileManagement.FileTypeID;
                    getData.FilePutup = (DateTime)fileManagement.fileManagement.FilePutup;
                    getData.UpdatedBy = fileManagement.fileManagement.UpdatedBy;
                    getData.UpdatedOn = fileManagement.fileManagement.UpdatedOn;
                    genericRepo.Update(getData);
                    flag = true;
                    if (flag)
                    {
                        if (fileManagement.fileDocumentsList.Count > 0)
                        {
                            Mapper.Initialize(cfg => cfg.CreateMap<FileTrackingDocuments, DTOModel.FileTrackingDocument>()
                                            .ForMember(d => d.FileID, o => o.UseValue(fileManagement.fileManagement.FileID))
                                            .ForMember(d => d.DocName, o => o.MapFrom(s => s.DocName))
                                            .ForMember(d => d.DocPathName, o => o.MapFrom(s => s.DocPathName))
                                            .ForMember(d => d.DocOrignalName, o => o.MapFrom(s => s.DocOrignalName))
                                            .ForMember(d => d.Sno, o => o.MapFrom(s => s.Sno))
                                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                            .ForAllOtherMembers(d => d.Ignore())
                                            );
                            var dtoFileDocuments = Mapper.Map<List<DTOModel.FileTrackingDocument>>(fileManagement.fileDocumentsList);
                            //   var dtoDocumentList = genericRepo.Get<DTOModel.FileTrackingDocument>(x => x.FileID == fileManagement.fileManagement.FileID && x.CreatedBy == fileManagement.fileManagement.UpdatedBy).ToList();
                            // if (dtoDocumentList != null && dtoDocumentList.Count > 0)
                            //  genericRepo.DeleteAll<DTOModel.FileTrackingDocument>(dtoDocumentList);
                            genericRepo.AddMultipleEntity(dtoFileDocuments);
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }

        }

        public bool DeleteDocument(int docID, out string DocName)
        {
            log.Info($"FileTrackingSytemService/DeleteDocument/docID={docID}");
            bool flag = false;
            DocName = "";
            try
            {
                var dtofileType = genericRepo.GetByID<DTOModel.FileTrackingDocument>(docID);
                if (dtofileType != null)
                {
                    DocName = dtofileType.DocPathName;
                    genericRepo.Delete(dtofileType);
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
        #endregion

        #region Forward File
        public bool ForwardFile(ProcessWorkFlow pWorkFlow)

        {
            log.Info($"FileTrackingSytemService/UpdateFileDetail");
            try
            {
                bool flag = false;
                var getData = genericRepo.GetByID<DTOModel.FileManagement>(pWorkFlow.ReferenceID);
                if (getData != null)
                {
                    getData.StatusID = 2;
                    getData.FileLastStatus = pWorkFlow.StatusID == 1 ? 1 : 2;
                    genericRepo.Update(getData);
                    flag = true;
                }
                if (flag)
                {
                    flag = false;
                    //  pWorkFlow.Senddate = null;
                    flag = AddProcessWorkFlow(pWorkFlow);
                }
                return flag;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public List<FileManagement> GetInboxFileList(int receiverID)
        {
            log.Info($"FileTrackingSytemService/GetInboxFileList/ReceiverID={receiverID}");
            try
            {
                var getData = fileManagRepo.GetInboxFilesList(receiverID);
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetInboxFiles_Result, FileManagement>()
                    .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                    .ForMember(d => d.ReferenceID, o => o.MapFrom(s => s.ReferenceID))
                    .ForMember(d => d.DiaryNo, o => o.MapFrom(s => s.DiaryNo))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.SenderDepartment))
                    .ForMember(d => d.SenderName, o => o.MapFrom(s => s.Sender))
                    .ForMember(d => d.FileSubject, o => o.MapFrom(s => s.FileSubject))
                    .ForMember(d => d.Remarks, o => o.MapFrom(s => s.Scomments))
                    .ForMember(d => d.FileTypeName, o => o.MapFrom(s => s.FileType))
                    .ForMember(d => d.FilePutup, o => o.MapFrom(s => s.FilePutup))
                    .ForMember(d => d.FileWorkFlowID, o => o.MapFrom(s => s.WorkflowID))
                    // .ForMember(d => d.ParkByID, o => o.MapFrom(s => s.ParkByEmployeeID))
                    .ForMember(d => d.FileLastStatus, o => o.MapFrom(s => s.FileStatus))
                    .ForMember(d => d.Readdate, o => o.MapFrom(s => s.Readdate))
                    .ForMember(d => d.Readflag, o => o.MapFrom(s => s.Readflag))
                    .ForMember(d => d.Purpose, o => o.MapFrom(s => s.Purpose))
                    .ForMember(d => d.ReceiverName, o => o.MapFrom(s => s.Receiver))
                    .ForMember(d => d.ReceiverDepartment, o => o.MapFrom(s => s.ReceiverDepartment))
                    .ForMember(d => d.ForwardThrough, o => o.MapFrom(s => s.ForwardThrough))
                    .ForAllOtherMembers(d => d.Ignore()));
                    var dtoFileManagement = Mapper.Map<List<FileManagement>>(getData);
                    if (dtoFileManagement != null && dtoFileManagement.Count > 0)
                    {
                        var fileID = 0;
                        for (int i = 0; i < dtoFileManagement.Count; i++)
                        {
                            fileID = dtoFileManagement[i].FileID;

                            var getFileDocumentData = genericRepo.Get<DTOModel.FileTrackingDocument>(x => x.FileID == fileID).ToList();
                            Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileTrackingDocument, FileTrackingDocuments>()
                            .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                            .ForMember(d => d.Sno, o => o.MapFrom(s => s.Sno))
                            .ForMember(d => d.DocName, o => o.MapFrom(s => s.DocName))
                            .ForMember(d => d.DocOrignalName, o => o.MapFrom(s => s.DocOrignalName))
                            .ForMember(d => d.DocPathName, o => o.MapFrom(s => s.DocPathName))
                            .ForAllOtherMembers(d => d.Ignore()));
                            var fileDocumentList = Mapper.Map<List<FileTrackingDocuments>>(getFileDocumentData);
                            dtoFileManagement[i].fileDocumentsList = fileDocumentList;
                        }
                    }
                    return dtoFileManagement;
                }
                else
                    return new List<FileManagement>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public List<FileManagement> GetOutboxFileList(int senderID)
        {
            log.Info($"FileTrackingSytemService/GetOutboxFileList/senderID={senderID}");
            try
            {
                var getData = fileManagRepo.GetOutboxFilesList(senderID);
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetOutboxFiles_Result, FileManagement>()
                    .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                    .ForMember(d => d.ReferenceID, o => o.MapFrom(s => s.ReferenceID))
                    .ForMember(d => d.DiaryNo, o => o.MapFrom(s => s.DiaryNo))
                    .ForMember(d => d.ReceiverDepartment, o => o.MapFrom(s => s.ReceiverDepartment))
                    .ForMember(d => d.ReceiverName, o => o.MapFrom(s => s.Receiver))
                    .ForMember(d => d.FileSubject, o => o.MapFrom(s => s.FileSubject))
                    .ForMember(d => d.Remarks, o => o.MapFrom(s => s.Scomments))
                    .ForMember(d => d.FileTypeName, o => o.MapFrom(s => s.FileType))
                    .ForMember(d => d.FilePutup, o => o.MapFrom(s => s.FilePutup))
                    .ForAllOtherMembers(d => d.Ignore()));
                    var dtoFileManagement = Mapper.Map<List<FileManagement>>(getData);
                    if (dtoFileManagement != null && dtoFileManagement.Count > 0)
                    {
                        var fileID = 0;
                        for (int i = 0; i < dtoFileManagement.Count; i++)
                        {
                            fileID = dtoFileManagement[i].FileID;

                            var getFileDocumentData = genericRepo.Get<DTOModel.FileTrackingDocument>(x => x.FileID == fileID).ToList();
                            Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileTrackingDocument, FileTrackingDocuments>()
                            .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                            .ForMember(d => d.Sno, o => o.MapFrom(s => s.Sno))
                            .ForMember(d => d.DocName, o => o.MapFrom(s => s.DocName))
                            .ForMember(d => d.DocOrignalName, o => o.MapFrom(s => s.DocOrignalName))
                            .ForMember(d => d.DocPathName, o => o.MapFrom(s => s.DocPathName))
                            .ForAllOtherMembers(d => d.Ignore()));
                            var fileDocumentList = Mapper.Map<List<FileTrackingDocuments>>(getFileDocumentData);
                            dtoFileManagement[i].fileDocumentsList = fileDocumentList;
                        }
                    }
                    return dtoFileManagement;
                }
                else
                    return new List<FileManagement>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public List<FileManagement> GetFileForwardDetails(int referenceID)
        {
            log.Info($"FileTrackingSytemService/GetFileForwardDetails/ReferenceID={referenceID}");
            try
            {
                var getData = fileManagRepo.GetFileForwardList(referenceID);
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetFileForwardDetails_Result, FileManagement>()
                    .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                    .ForMember(d => d.DiaryNo, o => o.MapFrom(s => s.DiaryNo))
                    .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.SenderDepartment))
                    .ForMember(d => d.SenderName, o => o.MapFrom(s => s.Sender))
                    .ForMember(d => d.SendDate, o => o.MapFrom(s => s.Senddate))
                    .ForMember(d => d.ReceiverDepartment, o => o.MapFrom(s => s.RevDepartment))
                    .ForMember(d => d.ReceiverName, o => o.MapFrom(s => s.RevName))
                    .ForMember(d => d.Remarks, o => o.MapFrom(s => s.Scomments))
                     .ForMember(d => d.TAT, o => o.MapFrom(s => s.TAT))
                    .ForAllOtherMembers(d => d.Ignore()));
                    var dtoFileManagement = Mapper.Map<List<FileManagement>>(getData);
                    return dtoFileManagement;
                }
                else
                    return new List<FileManagement>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public bool TabForwardFile(FileManagementSystem fileManagement)
        {
            log.Info($"FileTrackingSytemService/UpdateFileDetail");
            try
            {
                bool flag = false;
                var getData = genericRepo.GetByID<DTOModel.ProcessWorkFlow>(fileManagement.processWorkFlow.WorkflowID);
                if (getData != null)
                {
                    getData.StatusID = 2;
                    getData.UpdatedBy = fileManagement.processWorkFlow.CreatedBy;
                    getData.UpdatedOn = DateTime.Now;
                    getData.Senddate = fileManagement.processWorkFlow.Senddate;
                    genericRepo.Update(getData);
                    flag = true;
                }
                if (flag)
                {
                    if (fileManagement.fileDocumentsList.Count > 0)
                    {
                        flag = false;
                        Mapper.Initialize(cfg => cfg.CreateMap<FileTrackingDocuments, DTOModel.FileTrackingDocument>()
                                       .ForMember(d => d.FileID, o => o.UseValue(fileManagement.processWorkFlow.ReferenceID))
                                       .ForMember(d => d.DocName, o => o.MapFrom(s => s.DocName))
                                       .ForMember(d => d.DocPathName, o => o.MapFrom(s => s.DocPathName))
                                       .ForMember(d => d.DocOrignalName, o => o.MapFrom(s => s.DocOrignalName))
                                       .ForMember(d => d.Sno, o => o.MapFrom(s => s.Sno))
                                       .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                       .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                       .ForAllOtherMembers(d => d.Ignore())
                                       );
                        var dtoFileDocuments = Mapper.Map<List<DTOModel.FileTrackingDocument>>(fileManagement.fileDocumentsList);
                        if (dtoFileDocuments != null && dtoFileDocuments.Count > 0)
                            genericRepo.AddMultipleEntity(dtoFileDocuments);
                    }
                    fileManagement.processWorkFlow.Senddate = null;
                    var getpFlow = genericRepo.Get<DTOModel.ProcessWorkFlow>(x => x.ReferenceID == fileManagement.processWorkFlow.ReferenceID && x.ProcessID == fileManagement.processWorkFlow.ProcessID).OrderByDescending(x => x.WorkflowID).FirstOrDefault();
                    if (getpFlow != null)
                    {
                        fileManagement.processWorkFlow.SenderID = getpFlow.ReceiverID;
                        fileManagement.processWorkFlow.SenderDepartmentID = getpFlow.ReceiverDepartmentID;
                        fileManagement.processWorkFlow.SenderDesignationID = getpFlow.ReceiverDesignationID;
                    }
                    flag = AddProcessWorkFlow(fileManagement.processWorkFlow);
                }
                return flag;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public List<FileManagement> GetFileListForGridView(FileManagement fileManagement)
        {
            log.Info($"FileTrackingSytemService/GetFileListForGridView");
            try
            {
                var getData = fileManagRepo.GetFileListForGridView(fileManagement.DiaryNo, fileManagement.FilePutup, fileManagement.SendDate);
                if (getData != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.GetFileTrackingListForSearch_Result, FileManagement>()
                    .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                    .ForMember(d => d.ReferenceID, o => o.MapFrom(s => s.ReferenceID))
                    .ForMember(d => d.DiaryNo, o => o.MapFrom(s => s.DiaryNo))
                    .ForMember(d => d.ReceiverDepartment, o => o.MapFrom(s => s.ReceiverDepartment))
                    .ForMember(d => d.ReceiverName, o => o.MapFrom(s => s.Receiver))
                    .ForMember(d => d.FileSubject, o => o.MapFrom(s => s.FileSubject))
                    .ForMember(d => d.Remarks, o => o.MapFrom(s => s.Scomments))
                    .ForMember(d => d.FileTypeName, o => o.MapFrom(s => s.FileType))
                    .ForMember(d => d.FilePutup, o => o.MapFrom(s => s.FilePutup))
                    .ForMember(d => d.FileWorkFlowID, o => o.MapFrom(s => s.ReferenceID))
                    .ForAllOtherMembers(d => d.Ignore()));
                    var dtoFileManagement = Mapper.Map<List<FileManagement>>(getData);
                    if (dtoFileManagement != null && dtoFileManagement.Count > 0)
                    {
                        var fileID = 0;
                        for (int i = 0; i < dtoFileManagement.Count; i++)
                        {
                            fileID = dtoFileManagement[i].FileID;

                            var getFileDocumentData = genericRepo.Get<DTOModel.FileTrackingDocument>(x => x.FileID == fileID).ToList();
                            Mapper.Initialize(cfg => cfg.CreateMap<DTOModel.FileTrackingDocument, FileTrackingDocuments>()
                            .ForMember(d => d.FileID, o => o.MapFrom(s => s.FileID))
                            .ForMember(d => d.Sno, o => o.MapFrom(s => s.Sno))
                            .ForMember(d => d.DocName, o => o.MapFrom(s => s.DocName))
                            .ForMember(d => d.DocOrignalName, o => o.MapFrom(s => s.DocOrignalName))
                            .ForMember(d => d.DocPathName, o => o.MapFrom(s => s.DocPathName))
                            .ForAllOtherMembers(d => d.Ignore()));
                            var fileDocumentList = Mapper.Map<List<FileTrackingDocuments>>(getFileDocumentData);
                            dtoFileManagement[i].fileDocumentsList = fileDocumentList;
                        }
                    }
                    return dtoFileManagement;
                }
                else
                    return new List<FileManagement>();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public bool FileClosed(ProcessWorkFlow pWorkFlow)

        {
            log.Info($"FileTrackingSytemService/FileClosed");
            try
            {
                bool flag = false;
                var getData = genericRepo.GetByID<DTOModel.FileManagement>(pWorkFlow.ReferenceID);
                if (getData != null)
                {
                    getData.StatusID = 3;
                    getData.UpdatedBy = pWorkFlow.CreatedBy;
                    getData.UpdatedOn = DateTime.Now;
                    genericRepo.Update(getData);
                    flag = true;
                    var getWrkFlow = genericRepo.GetByID<DTOModel.ProcessWorkFlow>(pWorkFlow.WorkflowID);
                    if (getWrkFlow != null)
                    {
                        getWrkFlow.UpdatedBy = pWorkFlow.CreatedBy;
                        getWrkFlow.UpdatedOn = DateTime.Now;
                        genericRepo.Update(getWrkFlow);
                        flag = true;
                    }
                }
                return flag;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public bool FileReceive(ProcessWorkFlow pWorkFlow)

        {
            log.Info($"FileTrackingSytemService/FileReceive");
            try
            {
                bool flag = false;

                var getWrkFlow = genericRepo.GetByID<DTOModel.ProcessWorkFlow>(pWorkFlow.WorkflowID);
                if (getWrkFlow != null)
                {
                    getWrkFlow.Purpose = pWorkFlow.Purpose;// received through
                    getWrkFlow.Readdate = pWorkFlow.Readdate;
                    getWrkFlow.Readflag = 1;
                    getWrkFlow.ReadBy = pWorkFlow.CreatedBy;
                    genericRepo.Update(getWrkFlow);

                    var getData = genericRepo.GetByID<DTOModel.FileManagement>(pWorkFlow.ReferenceID);
                    if (getData != null)
                    {
                        getData.FileLastStatus = 2;
                        genericRepo.Update(getData);
                    }
                    flag = true;
                }

                return flag;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }
        #endregion

        public bool FileRollback(ProcessWorkFlow pWorkFlow)
        {
            log.Info($"FileTrackingSytemService/FileRollback");
            try
            {
                bool flag = false;
                var getpFlow = genericRepo.Get<DTOModel.ProcessWorkFlow>(x => x.ReferenceID == pWorkFlow.ReferenceID && x.ProcessID == pWorkFlow.ProcessID).OrderByDescending(x => x.WorkflowID).FirstOrDefault();
                if (getpFlow != null)
                {                   
                    getpFlow.UpdatedBy = pWorkFlow.CreatedBy;
                    getpFlow.UpdatedOn = DateTime.Now;
                    getpFlow.StatusID = 2;
                    getpFlow.Senddate = pWorkFlow.Senddate;
                    genericRepo.Update(getpFlow);
                    flag = true;
                }
                if (flag)
                {
                    var getData = genericRepo.GetByID<DTOModel.FileManagement>(pWorkFlow.ReferenceID);
                    if (getData != null)
                    {
                        getData.StatusID = 1;
                        genericRepo.Update(getData);
                        flag = true;
                    }
                    pWorkFlow.SenderID = getpFlow.ReceiverID;
                    pWorkFlow.Senddate = null;
                    flag = AddProcessWorkFlow(pWorkFlow);
                }
                return flag;

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw;
            }
        }

        public bool ValidateUser(ValidateLogin userCredential,out int userId)
        {
            log.Info("LoginService/ValidateUser");         
            string sUserName = userCredential.userName;
            string sPassword = userCredential.password;
            string dbPassword = string.Empty;
            bool isAuthenticated = false;
            bool isExists = false;
            userId = 0;
            try
            {
                var getLogin = genericRepo.Get<DTOModel.FTSUser>(x => x.UserName == userCredential.userName && !x.IsDeleted).FirstOrDefault();
                if(getLogin !=null)
                {
                    isExists = true;
                    dbPassword = getLogin.Password;
                }
                if (isExists)
                {
                    bool isMatch = Password.VerifyPassword(sPassword, dbPassword, Password.PASSWORD_SALT);
                    if (isMatch)
                    {
                        userId = getLogin.UserId;
                        isAuthenticated = true;
                    }
                    else
                    {
                        isAuthenticated = false;                      
                    }
                }
                else
                {
                    isAuthenticated = false;
                  
                }
            }
            catch (System.Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return isAuthenticated;
        }

    }
}
