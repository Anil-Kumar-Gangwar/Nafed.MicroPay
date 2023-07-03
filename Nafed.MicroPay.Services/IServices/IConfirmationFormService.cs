using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IConfirmationFormService
    {
        ConfirmationFormDetail GetEmployeeConfirmationList(int formTypeID, int? processTypeID, int? employeeID, int empProcessAppID, int formABHdrID);
        bool UpdateFormAData(ConfirmationFormHdr confirmationFormAHdr);
        List<ConfirmationFormHdr> GetConfirmationForms(int? employeeID);

        //bool isReviewOfficer(int? processTypeID, int? employeeID);
        List<SelectListModel> GetEmployeeFilter(int employeeID);

        #region Confirmation Filters
        IEnumerable<ConfirmationFormHdr> GetConfirmationFormHdr(ConfirmationFormApprovalFilter filters);

        #endregion
        ConfirmationFormRulesAttributes GetConfirmationFormRulesAttributes(int formID, int employeeID, int processId, int empProcessAppID, int FrmABHdrID);

        CommonDetails GetRTRVDetails(int? employeeID);
        bool UpdateFormBData(ConfirmationFormHdr confirmationFormAHdr);

        List<EmployeeConfirmationViewDetails> GetEmployeeConfirmationDetails(EmployeeConfirmationFormFilters filters);
        bool UpdateEmployeeConfirmationStatus(int formHdrID, int headerId, int formTypeId, int employeeID, int processID);

        CommonDetails GetAADetails(int? employeeID);

        bool PostPersonalSectionRemarks(ConfirmationPRSectionRemarks confirmationPRRemark);

        List<ConfirmationClarification> GetConfirmationClarification(int formHdrID, int empProcessAppID, int formTypeId, int childHeaderId);
        ConfirmationPRSectionRemarks GetConfirmationDetails(int formHdrID, int formTypeId, int employeeID, int childHdrID);
        ConfirmationPRSectionRemarks GetConfirmationDtlForOrdrRpt(int empID, int hdrID, int formHdrID);
        bool SendMailForOrderReport(ConfirmationPRSectionRemarks confDtlforReport,  string ordrRptPath);
        dynamic GetEmployeeNameDesignation(string empCode);

    }
}
