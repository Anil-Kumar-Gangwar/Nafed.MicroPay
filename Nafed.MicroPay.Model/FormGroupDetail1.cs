using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class FormGroupDetail1
    {
        public int sno { set; get; }
        public int FormDetailID { get; set; }
        public int FormGroupID { get; set; }
        public string Activities { get; set; }
        public string Achievements { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
      
    }
}
