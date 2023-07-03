using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class EmployeeCategory
    {
        public int EmplCatID { get; set; }
        public string EmplCatCode { get; set; }
        public string EmplCatName { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
