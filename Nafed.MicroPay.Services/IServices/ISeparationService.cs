using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
namespace Nafed.MicroPay.Services.IServices
{
   public interface ISeparationService
    {
        List<SuperAnnuating> GetSuperAnnuating();
        bool StartProcess(SuperAnnuating objSup);
        bool UploadDocument(SuperAnnuating objSup);
        bool SendForClearance(EmployeeProcessApprovalVM empProcessApproval);
        List<SeparationClearance> GetClearanceApprovalStatus(int sepId, int aprType);
        List<SeparationClearance> GetApprovalClearanceList(int empId);
        bool ApproveRejectClearance(SeparationClearance objSep);
        bool UploadCircular(SuperAnnuating objSup);
        Resignation GetEmployeeDetail(int empId);
        bool Resignation(Resignation objSup);
        int? CheckForResignation(int empId);
    }
}
