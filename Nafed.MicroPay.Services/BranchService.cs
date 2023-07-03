using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services
{
    public class BranchService : BaseService, IBranchService
    {
        private readonly IGenericRepository genericRepo;

        public BranchService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }

        public List<Model.Branch> GetBranchList()
        {
            log.Info($"GetBranchList");
            try
            {
                var result = genericRepo.Get<DTOModel.Branch>(x => !x.IsDeleted);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.Branch, Model.Branch>()
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                    .ForMember(c => c.BranchCode, c => c.MapFrom(s => s.BranchCode))
                    .ForMember(c => c.BranchName, c => c.MapFrom(s => s.BranchName))
                    .ForMember(c => c.IsHillComp, c => c.MapFrom(s => s.IsHillComp))
                    .ForMember(c => c.Address1, c => c.MapFrom(s => s.Address1))
                    .ForMember(c => c.Address2, c => c.MapFrom(s => s.Address2))
                    .ForMember(c => c.Address3, c => c.MapFrom(s => s.Address3))
                    .ForMember(c => c.Pin, c => c.MapFrom(s => s.Pin))
                    .ForMember(c => c.CityID, c => c.MapFrom(s => s.CityID))
                    .ForMember(c => c.GradeID, c => c.MapFrom(s => s.GradeID))
                    .ForMember(c => c.Region, c => c.MapFrom(s => s.Region))
                    .ForMember(c => c.PhoneSTD, c => c.MapFrom(s => s.PhoneSTD))
                    .ForMember(c => c.PhoneNo, c => c.MapFrom(s => s.PhoneNo))
                    .ForMember(c => c.FaxSTD, c => c.MapFrom(s => s.FaxSTD))
                    .ForMember(c => c.FaxNo, c => c.MapFrom(s => s.FaxNo))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var listBranch = Mapper.Map<List<Model.Branch>>(result);
                return listBranch.OrderBy(x => x.BranchName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool BranchNameExists(string branchName)
        {
            return genericRepo.Exists<DTOModel.Branch>(x => x.BranchName == branchName && !x.IsDeleted);
        }

        public int InsertBranch(Model.Branch createBrach)
        {
            log.Info($"InsertBranch");
            try
            {
         

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Branch, DTOModel.Branch>()
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                     .ForMember(c => c.BranchCode, c => c.MapFrom(s => s.BranchCode))
                     .ForMember(c => c.BranchName, c => c.MapFrom(s => s.BranchName))
                    .ForMember(c => c.IsHillComp, c => c.MapFrom(s => s.IsHillComp))
                    .ForMember(c => c.Address1, c => c.MapFrom(s => s.Address1))
                     .ForMember(c => c.Address2, c => c.MapFrom(s => s.Address2))
                    .ForMember(c => c.Address3, c => c.MapFrom(s => s.Address3))
                    .ForMember(c => c.Pin, c => c.MapFrom(s => s.Pin))
                     .ForMember(c => c.CityID, c => c.MapFrom(s => s.CityID))
                    .ForMember(c => c.GradeID, c => c.MapFrom(s => s.GradeID))
                     .ForMember(c => c.Region, c => c.MapFrom(s => s.Region))
                    .ForMember(c => c.PhoneSTD, c => c.MapFrom(s => s.PhoneSTD))
                     .ForMember(c => c.PhoneNo, c => c.MapFrom(s => s.PhoneNo))
                    .ForMember(c => c.FaxSTD, c => c.MapFrom(s => s.FaxSTD))
                     .ForMember(c => c.FaxNo, c => c.MapFrom(s => s.FaxNo))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                     .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                   .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoBranch = Mapper.Map<DTOModel.Branch>(createBrach);
                genericRepo.Insert<DTOModel.Branch>(dtoBranch);
                return dtoBranch.BranchID;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }           
        }

        public bool UpdateBranch(Model.Branch editBranch)
        {
            log.Info($"UpdateBranch");
            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<DTOModel.Branch>(editBranch.BranchID);
                dtoObj.BranchCode = editBranch.BranchCode;
                dtoObj.BranchName = editBranch.BranchName;
                dtoObj.IsHillComp = editBranch.IsHillComp;
                dtoObj.Address1 = editBranch.Address1;
                dtoObj.Address2 = editBranch.Address2;
                dtoObj.Address3 = editBranch.Address3;
                dtoObj.Pin = editBranch.Pin;
                dtoObj.CityID = editBranch.CityID == 0 ? null : editBranch.CityID;
                dtoObj.GradeID = editBranch.GradeID == 0 ? null : editBranch.GradeID;
                dtoObj.Region = editBranch.Region;
                dtoObj.PhoneSTD = editBranch.PhoneSTD;
                dtoObj.PhoneNo = editBranch.PhoneNo;
                dtoObj.FaxSTD = editBranch.FaxSTD;
                dtoObj.FaxNo = editBranch.FaxNo;
                dtoObj.Remarks = editBranch.Remarks;
                dtoObj.UpdatedOn = editBranch.UpdatedOn;
                dtoObj.UpdatedBy = editBranch.UpdatedBy;
                genericRepo.Update<DTOModel.Branch>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }


        public Model.Branch GetBranchByID(int branchID)
        {
            log.Info($"GetBranchByID {branchID}");
            try
            {
                var branchObj = genericRepo.GetByID<DTOModel.Branch>(branchID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Branch, Model.Branch>()
                    .ForMember(c => c.BranchID, c => c.MapFrom(s => s.BranchID))
                    .ForMember(c => c.BranchCode, c => c.MapFrom(s => s.BranchCode))
                    .ForMember(c => c.BranchName, c => c.MapFrom(s => s.BranchName))
                    .ForMember(c => c.IsHillComp, c => c.MapFrom(s => s.IsHillComp))
                    .ForMember(c => c.Address1, c => c.MapFrom(s => s.Address1))
                    .ForMember(c => c.Address2, c => c.MapFrom(s => s.Address2))
                    .ForMember(c => c.Address3, c => c.MapFrom(s => s.Address3))
                    .ForMember(c => c.Pin, c => c.MapFrom(s => s.Pin))
                    .ForMember(c => c.CityID, c => c.MapFrom(s => s.CityID))
                    .ForMember(c => c.GradeID, c => c.MapFrom(s => s.GradeID))
                    .ForMember(c => c.Region, c => c.MapFrom(s => s.Region))
                    .ForMember(c => c.PhoneSTD, c => c.MapFrom(s => s.PhoneSTD))
                    .ForMember(c => c.PhoneNo, c => c.MapFrom(s => s.PhoneNo))
                    .ForMember(c => c.FaxSTD, c => c.MapFrom(s => s.FaxSTD))
                    .ForMember(c => c.FaxNo, c => c.MapFrom(s => s.FaxNo))
                    .ForMember(c => c.Remarks, c => c.MapFrom(s => s.Remarks))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(s => s.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(s => s.CreatedOn))
                    .ForMember(c => c.UpdatedBy, c => c.MapFrom(s => s.UpdatedBy))
                    .ForMember(c => c.UpdatedOn, c => c.MapFrom(s => s.UpdatedOn))
                    .ForMember(c => c.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<DTOModel.Branch, Model.Branch>(branchObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }


        public bool Delete(int branchID)
        {
            log.Info($"DeleteByID {branchID}");
            bool flag = false;
            try
            {
                DTOModel.Branch dtoBranch = new DTOModel.Branch();
                dtoBranch = genericRepo.GetByID<DTOModel.Branch>(branchID);

                dtoBranch.IsDeleted = true;

                genericRepo.Update<DTOModel.Branch>(dtoBranch);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }



        #region Branch Transfer 
        public BranchTransfer GetBranchTransferDtls(int? employeeID)
        {
            log.Info($"BranchService/GetBranchTransferDtls/{employeeID}");
            try
            {
                BranchTransfer bTransfer = new BranchTransfer();
                var employeeInfo = genericRepo.GetByID<DTOModel.tblMstEmployee>(employeeID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblMstEmployee, Model.BranchTransfer>()
                     .ForMember(d => d.employeeCode, o => o.MapFrom(s => s.EmployeeCode))
                     .ForMember(d => d.employeeName, o => o.MapFrom(s => s.Name))
                     .ForMember(d => d.employeeID, o => o.MapFrom(s => s.EmployeeId))
                     .ForMember(d => d.cadre, o => o.MapFrom(s => s.Cadre.CadreName))
                     .ForMember(d => d.designation, o => o.MapFrom(s => s.Designation.DesignationName))
                     .ForMember(d => d.Sen_Code, o => o.MapFrom(s => s.Sen_Code))
                     .ForMember(d => d.basicSalary, o => o.MapFrom(s => s.TblMstEmployeeSalaries.FirstOrDefault(y => y.EmployeeID == employeeID).E_Basic))
                     .ForMember(d => d.CurrentCadreID, o => o.MapFrom(s => s.CadreID))
                     .ForAllOtherMembers(d => d.Ignore());
                });
                bTransfer = Mapper.Map<Model.BranchTransfer>(employeeInfo);

                if (genericRepo.Exists<DTOModel.tblBranchManagerDetail>(x => x.EmployeeID == employeeID))
                {
                    var dtoBranchTransferList = genericRepo.Get<DTOModel.tblBranchManagerDetail>(x => x.EmployeeID == employeeID && !x.IsDeleted);
                    Mapper.Initialize(cfg =>
                    {

                        cfg.CreateMap<DTOModel.tblBranchManagerDetail, BranchManagerDetails>()
                          .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                          .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                          .ForMember(d => d.branchID, o => o.MapFrom(s => s.branchID))
                          .ForMember(d => d.BranchCode, o => o.MapFrom(s => s.BranchCode))
                          .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                          .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.BranchName))
                          .ForMember(d => d.DateFrom, o => o.MapFrom(s => s.DateFrom))
                          .ForMember(d => d.DateTo, o => o.MapFrom(s => s.DateTo))
                          .ForAllOtherMembers(d => d.Ignore());
                    });
                    bTransfer.branchTransferList = Mapper.Map<List<Model.BranchManagerDetails>>(dtoBranchTransferList);
                }
                return bTransfer;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public BranchManagerDetails GetBranchTransferForm(int? employeeID, int? transID)
        {
            log.Info($"BranchService/GetBranchTransferForm/{employeeID}/{transID}");
            try
            {
                BranchManagerDetails branchDetail = new BranchManagerDetails();
                var result = genericRepo.Get<DTOModel.tblBranchManagerDetail>(x => x.EmployeeID == employeeID && x.Id == transID);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<DTOModel.tblBranchManagerDetail, Model.BranchManagerDetails>()
                            .ForMember(d => d.branchID, o => o.MapFrom(s => s.branchID))
                            .ForMember(d => d.DateFrom, o => o.MapFrom(s => s.DateFrom))
                            .ForMember(d => d.DateTo, o => o.MapFrom(s => s.DateTo))
                            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                            .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                            .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                            .ForAllOtherMembers(d => d.Ignore());
                });
                return Mapper.Map<Model.BranchManagerDetails>(result.FirstOrDefault());
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public BranchManagerDetails ChangeBranch(BranchManagerDetails branchDtls)
        {
            log.Info($"BranchService/ChangeBranch/");
            try
            {
                if (branchDtls.FormActionType == "Create" && ValidateBranchTransferFormInputs(branchDtls.EmployeeCode))
                {
                    branchDtls.IsValidInputs = false;
                    branchDtls.ValidationMessage = "You can not process new transaction until the details of previous transaction is completed";
                }
                else
                {
                    var isAlreadyAssigned = false; DateTime? fromDate = null, toDate = null;
                    var branchTransEntities = genericRepo.GetIQueryable<DTOModel.tblBranchManagerDetail>
                        (x => x.EmployeeCode == branchDtls.EmployeeCode && !x.IsDeleted).ToList();

                    branchTransEntities.ForEach(x =>
                    {
                        isAlreadyAssigned = x.DateTo.HasValue &&
                        (branchDtls.DateFrom.Value < x.DateTo.Value) ? true : isAlreadyAssigned;
                        fromDate = x.DateFrom;
                        toDate = x.DateTo;
                    });
                    if (branchDtls.FormActionType == "Create" && isAlreadyAssigned)
                    {
                        branchDtls.IsValidInputs = false;
                        branchDtls.ValidationMessage = $"This employee is already working on Previous Branch at this time From {fromDate.Value.ToString("dd/MM/yyyy")} & To {toDate.Value.ToString("dd/MM/yyyy")},So please select from date greater than pevious to date";
                        //  $"This employee already worked on this designation From {fromDate.Value.ToString("dd/MM/yyyy")} & Todate {toDate.Value.ToString("dd/MM/yyyy")}";
                    }
                }
                if (branchDtls.IsValidInputs)
                {
                    if (branchDtls.FormActionType == "Create")
                    {
                        var current_Emp = genericRepo.Get<DTOModel.tblMstEmployee>(x =>
                        x.EmployeeCode == branchDtls.EmployeeCode && !x.IsDeleted).FirstOrDefault();
                        current_Emp.UpdatedBy = branchDtls.FormActionType == "Create" ? branchDtls.CreatedBy : branchDtls.UpdatedBy;
                        current_Emp.UpdatedOn = branchDtls.FormActionType == "Create" ? branchDtls.CreatedOn : branchDtls.UpdatedOn;
                        current_Emp.BranchID = branchDtls.branchID;
                        genericRepo.Update<DTOModel.tblMstEmployee>(current_Emp);

                        if (branchDtls.FormActionType == "Create")
                        {
                            Mapper.Initialize(cfg =>
                            {
                                cfg.CreateMap<BranchManagerDetails, DTOModel.tblBranchManagerDetail>()
                                .ForMember(d => d.EmployeeID, o => o.MapFrom(s => s.EmployeeID))
                                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.EmployeeCode))
                                .ForMember(d => d.branchID, o => o.MapFrom(s => s.branchID))
                                .ForMember(d => d.DateFrom, o => o.MapFrom(s => s.DateFrom))
                                .ForMember(d => d.DateTo, o => o.MapFrom(s => s.DateTo))
                                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
                                .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
                                .ForMember(d => d.IsDeleted, o => o.UseValue(false))
                                .ForAllOtherMembers(d => d.Ignore());
                            });
                            var dtoBranchDtls = Mapper.Map<DTOModel.tblBranchManagerDetail>(branchDtls);
                            int newtransID = genericRepo.Insert<DTOModel.tblBranchManagerDetail>(dtoBranchDtls);
                            branchDtls.Saved = true;
                        }
                    }
                    else
                    {
                        var prevTransRow = genericRepo.Get<DTOModel.tblBranchManagerDetail>(x => x.Id == branchDtls.Id
                          && x.EmployeeCode == branchDtls.EmployeeCode).FirstOrDefault();
                        prevTransRow.DateTo = branchDtls.DateTo;
                        prevTransRow.DateFrom = branchDtls.DateFrom.Value;
                        prevTransRow.branchID = branchDtls.branchID;
                        prevTransRow.UpdatedBy = branchDtls.UpdatedBy;
                        prevTransRow.UpdatedOn = branchDtls.UpdatedOn;

                        // prevTransRow.
                        genericRepo.Update<DTOModel.tblBranchManagerDetail>(prevTransRow);
                        branchDtls.Saved = true;

                    }
                }

                return branchDtls;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool DeleteBranchTransEntry(int transID)
        {
            log.Info($"BranchService/DeleteBranchTransEntry/{transID}");
            bool flag = false;
            try
            {
                var transRow = genericRepo.GetByID<DTOModel.tblBranchManagerDetail>(transID);
                transRow.IsDeleted = true;
                genericRepo.Update<DTOModel.tblBranchManagerDetail>(transRow);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;

        }

        private bool ValidateBranchTransferFormInputs(string employeeCode)
        {
            log.Info($"ValidateBranchTransferFormInputs/employeeCode");

            var lastTrans = genericRepo.GetIQueryable<DTOModel.tblBranchManagerDetail>
                (x => x.EmployeeCode == employeeCode && !x.IsDeleted).OrderByDescending(y => y.Id).Take(1);
            return lastTrans.Any(z => z.DateTo == null);
        }

        #endregion
    }
}
