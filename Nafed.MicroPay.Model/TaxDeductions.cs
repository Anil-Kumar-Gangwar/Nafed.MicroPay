using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class TaxDeductions
    {
        public string Fyear { set; get; }
        public List<DeductionSection> sections { set; get; } = new List<DeductionSection>();

        public List<DeductionSubSection> subSections { set; get; } = new List<DeductionSubSection>();

        public List<DeductionSubSectionDescription> subSectionDescriptions { set; get; } = new List<DeductionSubSectionDescription>();
    }
}
