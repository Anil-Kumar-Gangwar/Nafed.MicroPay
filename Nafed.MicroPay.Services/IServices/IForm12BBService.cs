using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using System.Data;
namespace Nafed.MicroPay.Services.IServices
{
    public  interface IForm12BBService
    {
        int CreateSection(DeductionSection dSection);

        int CreateSubSectionDescription(DeductionSubSectionDescription dSubSecDescription);
        List<DeductionSection> GetSectionList();
        bool DeleteSubSection(int sectionID, int subSectionID);
        bool DeleteSection(int sectionID);

        bool DeleteSubSectionDescription(int sectionID, int subSectionID, int descriptionID);
        Model.DeductionSection GetFormSection(int sectionID);

        Model.DeductionSubSection GetFormSubSection(int section, int subSectionID);
        bool UpdateSection(Model.DeductionSection dSection);

        List<DeductionSubSection> GetSubSectionList();

        List<DeductionSubSectionDescription> GetSubSectionDescriptions();
        List<SelectListModel> GetSectionList(string fYr);
        int CreateSubSection(DeductionSubSection dSubSection);

        bool UpdateSubSection(Model.DeductionSubSection dSubSection);
        bool UpdateSubSectionDescription(Model.DeductionSubSectionDescription dSubSectionDesc);
        List<SelectListModel> GetSubSections(int sectionID);

        Model.DeductionSubSectionDescription GetFormSubSecDesc(int sectionID, int subSectionID, int descriptionID);
        TaxDeductions GetDeductionSection(string fYear);

        Form12BBInfo GetEmployeeForm12BB(int employeeID, string fYear);

        bool PostForm12BB(Form12BBInfo frmForm12BB);

        List<Form12BBInfo> GetForm12BBList(int employeeID, string fYear);
        bool UploadForm12BBDocument(EmployeeForm12BBDocument empFrmDocument);
        List<SelectListModel> GetSubSectionDescriptions(int sectionID, int subSectionID);

        List<Form12BBInfo> GetForm12BBList(string fYear);

        DataTable GetForm12BBDetails(string fYear);

        IEnumerable<Form12BBDocumentList> GetForm12BBDocumentList(int EmpFormID);
        bool DeleteDocument(int form12BBDocumentID);
    }
}
