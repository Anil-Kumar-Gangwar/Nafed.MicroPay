using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class CandidateLanguageKnown
    {
        public int Counter { get; set; }
        public int RegistrationID { get; set; }
        public int LanguageID { get; set; }
        public Nullable<bool> Speak { get; set; }
        public Nullable<bool> Writing { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CraetedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
