using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class DeductionSubSectionDescription
    {
        public int SectionID { get; set; }

        public string SectionName { set; get; }
        public int SubSectionID { get; set; }
        public string SubSectionName { set; get; }

        public decimal? Amount { set; get; }

        public int DescriptionID { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }


    }
}
