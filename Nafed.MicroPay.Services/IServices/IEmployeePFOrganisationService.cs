using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IEmployeePFOrganisationService
    {
        List<Model.EmployeePFORG> GetEmpPFHList(int EmployeeID);
        bool GetEmpdetails(int EmployeeID, out EmployeePFORG EMPPFdetail);
        bool InsertEmployeePFDetails(EmployeePFORG createEMPPFOrg, ProcessWorkFlow workFlow);
        EmployeePFORG GetEMPPFOrgDetails(int ID, int EmpPFID, int statusID);
        List<EmployeePFORG> checkexistdata(int EmpPFID);
        bool UpdateEmployeePFDetails(EmployeePFORG createEMPPFOrg);
        bool UpdatePFNo(int employeeID, int pfNo);
        bool UpdateEmpAccountDetail(int eID, int? pfNo, string uan, string epfo, string pan, string aadhar, string ac, string bankCode, string ifscCode, int userID);
        IEnumerable<UnAssignedPFRecord> GetUnAssignedPFRecords(int? branchID);
        bool UpdateEmployeePFStatus(EmployeePFORG createEMPPFOrg, ProcessWorkFlow workFlow);

        IEnumerable<EmployeePFORG> GetForm11Hdr(Form11FormApprovalFilter filters);
        bool SendMail(EmployeePFORG createEMPPFOrg);

    }
}
