using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class FormGroupADetail1
    {
        public int FormDetailID { get; set; }
        public int FormGroupID { get; set; }
        public Nullable<byte> ActivityID { get; set; }
        public string Achievements { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
