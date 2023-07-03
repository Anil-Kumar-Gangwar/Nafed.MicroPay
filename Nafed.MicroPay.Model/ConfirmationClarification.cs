using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConfirmationClarification
    {
        public int Sno { get; set; }
        public int ClarificationID { get; set; }

        public string ROClarification { get; set; }

        public int FormHdrID { get; set; }
        public string ClarificationRemarks { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int EmpProcessAppID { get; set; }

        public int FormABHeaderID { get; set; }
        public int FormTypeID { get; set; }
    }
}
