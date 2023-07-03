using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EPFDetail
    {
        public int sno { set; get; }
        public int EPFIDDetail { get; set; }
        public int EPFID { get; set; }
        public int NomineeID { get; set; }
        public string GuardianName { get; set; }
        public int ? GuardianRelationID { get; set; }
        public string GuardianAddress { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string RelationName { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<decimal> PFDistribution { get; set; }
        public int EmpDependentID { get; set; }
        public string DependentName { get; set; }
        public string Address { get; set; }
        public bool  IsMinor { get; set; }
        
    }
}
