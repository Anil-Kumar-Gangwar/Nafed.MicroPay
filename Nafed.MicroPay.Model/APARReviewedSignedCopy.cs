using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class APARReviewedSignedCopy
    {
        public int FormID { get; set; }
        public int FormGroupID { get; set; }
        public Nullable<int> WorkFlowID { get; set; }
        public string UploadedDocName { get; set; }
        public int UploadedBy { get; set; }
        public System.DateTime UploadedOn { get; set; }

        public int EmployeeId { get; set; }
    }
}
