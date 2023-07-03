using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmployeeProfile
    {
        public int EmployeeID { set; get; }
        public string EmpName { set; get; }

        public string EmpCode { set; get; }

        public string BloodGroupName { set; get; }

        public string PANNo { set; get; }

        public string AadhaarNo { set; get; }

        public string Title { set; get; }

        public string HBName { set; get; }

        public string Gender { set; get; }

        public string Nationality { set; get; }

        public string MaritalStatus { set; get; }

        public string SupervisorName { set; get; }

        public string EmailID { set; get; }

        public string MobileNo { set; get; }

        public string PresentAddress { set; get; }

        public string PresentState { set; get; }

        public string PresentCity { set; get; }

        public string PresentPin { set; get; }

        public string PermanentAddress { set; get; }

        public string PermanentState { set; get; }

        public string PermanentCity { set; get; }

        public string PermanentPin { set; get; }

        public string ProfessionalQualification { set; get; }

        public string AcademicQualification { set; get; }

        public string SpecialSkills { set; get; }

        public string EmpProfilePhotoUNCPath { set; get; }

        public int? ReportingTo { set; get; }

        public Nullable<System.DateTime> DOB { get; set; }

        public System.DateTime DOJ { get; set; }

        public string Category { set; get; }

        public string Branch { set; get; }

        public string Department { set; get; }

        public string Designation { set; get; }

        public string Section { set; get; }


        public int? PFNo { set; get; }
        public string ACRNo { set; get; }

        public string UANNo { set; get; }

        public string Bank { set; get; }
        public string BankAccountNo { set; get; }
        public DateTime DateOfJoining { set; get; }
        public int? ReviewerTo { set; get; }
        public int? AcceptanceAuthority { set; get; }
        public string ReviewerName { set; get; }
        public string AcceptanceAuthorityName { set; get; }
        public int? Sen_Code { set; get; }
        public string PassportNo { set; get; }
        public string IDMark { set; get; }
        public string FileNo { set; get; }
        public int? IncrementMonth { get; set; }
        public string Nominee { set; get; }
        public string Relation { set; get; }
        public string PayScale { set; get; }
        public DateTime SuperAnnuation { set; get; }
        public string AadhaarCardFilePath { get; set; }
        public string PanCardFilePath { get; set; }
        public string PanCardUNCFilePath { set; get; }
        public string AadhaarCardUNCFilePath { set; get; }
        public string MotherName { get; set; }
        public string IFSCCode { get; set; }
        public int? QProfessionalID { set; get; }
        public int? QAcademicID { set; get; }
        public List<SelectListModel> ddlQAcademic = new List<SelectListModel>();
        public List<SelectListModel> ddlQProfessional = new List<SelectListModel>();
        public List<SelectListModel> ddlBloodGroup = new List<SelectListModel>();
        public int? BloodGroupID { set; get; }
        public int? PresentStateID { set; get; }
        public int? PmtStateID { set; get; }

        public List<SelectListModel> ddlPresentState = new List<SelectListModel>();
        public List<SelectListModel> ddlPermanentState = new List<SelectListModel>();
        public List<AcadmicProfessionalDetailsModel> QualificationList = new List<Model.AcadmicProfessionalDetailsModel>();


        public List<EmployeeAchievement> achievements = new List<EmployeeAchievement>();
        public List<EmployeeCertification> certifications = new List<EmployeeCertification>();

        public string PensionNumber { get; set; }
        public string EPFOMemberID { get; set; }

        public Insurance EmployeeInsurance { get; set; }


    }

}
