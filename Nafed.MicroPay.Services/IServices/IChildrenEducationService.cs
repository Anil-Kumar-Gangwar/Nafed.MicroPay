using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IChildrenEducationService
    {
        bool InsertChildrenEducationData(Model.ChildrenEducationHdr childrenEduForm, out int chldrenHdrId);
        List<Model.ChildrenEducationHdr> GetEmployeeChildrenEducationList(int userEmpID);
        Model.ChildrenEducationHdr GetChildrenEducation(int empId, int childrenEduHdrId);
        List<Model.ChildrenEducationDetails> GetChildrenEducationDetails(int empId, int childrenEduHdrId);
        List<SelectListModel> GetDependentList(int empID);
        bool UpdateChildrenEducationData(Model.ChildrenEducationHdr childrenEduForm);
        int ChildrenEducationExist(int employeeId, string reportingYr);
        List<Model.ChildrenEducationHistoryModel> GetChildrenEducationForAdmin(AppraisalFormApprovalFilter filters);
        List<Model.ChildrenEducationDocuments> GetChildrenEducationDocumentsList(int employeeId, int childrenEduHdrId);
        bool DeleteReceiptsDocuments(int receiptId);

        bool UpdateEmployeeDependent(List<ChildrenEducationDetails> childrenEducationDetails, IEnumerable<ChildrenEducationDetails> deletedResult);
        List<Model.ChildrenEducationHdr> GetEmployeeChildrenEducationYearWise(string reportingYr);
    }
}
