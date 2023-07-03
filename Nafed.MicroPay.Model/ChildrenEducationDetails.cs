using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ChildrenEducationDetails
    {
        public int sno { get; set; }
        public int ChildrenEduDetailID { get; set; }
        public int ChildrenEduHdrID { get; set; }
        public int EmpDependentID { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public bool NotApplicable { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DOB { get; set; }
        public int EmployeeId { get; set; }
    }
}
