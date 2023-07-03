using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class Gender
    {
        public int GenderID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<DateTime> UpdateOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
