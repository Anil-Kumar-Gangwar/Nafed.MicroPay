using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
    public class EPFNomination
    {
        public string EmployeeName { get; set; }
        public string HBName { get; set; }
        public string Gender { get; set; }
        public string MaritalStatusName { get; set; }
        public string PlaceOfJoin { get; set; }
        public string Qualification { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public string ReportingName { get; set; }
        public string ReviewerToName { get; set; }
        public string DesignationName { get; set; }
        public string ReportingDesignation { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<System.DateTime> RTSenddate { get; set; }
        public Nullable<System.DateTime> EmpSenddate { get; set; }
        public int EPFNoID { get; set; }
        public short StatusID { get; set; }  
        public List<EPFDetail> EPFDetailList { get; set; }
        public List<EPSDetail> EPSDetailList { get; set; }
        public List<EPSDetail> EPSMaleEmpNomList { get; set; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; }
        public int loggedInEmpID { get; set; }
        public int ReportingTo { get; set; }
        public string PresentAddress { get; set; }
        public string PmtAddress { get; set; }
        public string RTPlaceOfJoin { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public List<SelectListModel> RelationList { set; get; }

        public EmployeeProcessApproval approvalSettings { get; set; }
    }
}
