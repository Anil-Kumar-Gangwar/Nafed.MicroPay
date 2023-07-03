using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class JobRequirementLocation
    {
        public int JLocId { get; set; }
        public int RequirementId { get; set; }
        public int LocTypeId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> ZoneId { get; set; }

        public string BranchName { get; set; }
        //public EnumZoneAppliedFor ZoneName { get; set; }
        public string ZoneName { set; get; }
    }
}
