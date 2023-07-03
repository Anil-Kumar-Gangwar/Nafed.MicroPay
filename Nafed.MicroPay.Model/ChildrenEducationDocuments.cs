using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ChildrenEducationDocuments
    {
        public int ReceiptID { get; set; }
        public int EmployeeID { get; set; }
        public int ChildrenEduHdrID { get; set; }
        public string FilePath { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string EmployeeName { get; set; }
    }
}
