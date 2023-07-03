using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class AppraisalFormHdr
    {
        public int AppraisalHdrID { get; set; }
        public int FormID { get; set; }

        public string FormName { set; get; }

        public string EmployeeCode { set; get; }
        public string DepartmentName { set; get; }
        public string DesignationName { set; get; }

        public string EmpName { set; get; }

        public string ReportingYr { get; set; }
        public int EmployeeID { get; set; }
        public int FormGroupID { get; set; }
        public int StatusID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public string Status { get; set; }

        //public int? reportingTo { get; set; }
        //public int? reviewingTo { get; set; }
        //public int? acceptanceAuthority { get; set; }

            public AppraisalFormState formState { set; get; }
        public EmployeeProcessApproval EmpProceeApproval { set; get; } = new EmployeeProcessApproval();
        public int? DepartmentID { get; set; }
        public int? DesignationID { get; set; }

        public APARReviewedSignedCopy aparReviewedSignedCopy { get; set; } = new APARReviewedSignedCopy();
    }
}
