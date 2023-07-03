using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
  public  class JobRequirementQualification:CandidateEducationSummary
    {
        public int RequirementID { get; set; }
        public Nullable<int> JQualificationTypeID { get; set; }
        public int SelectedQualificationID { get; set; }

        public string Qualification { get; set; }
        public int JQualifactionID { get; set; }
    }
}
