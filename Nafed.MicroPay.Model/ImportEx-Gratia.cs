using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ImportEx_Gratia
    {
        [Display(Name ="Month")]
        public int SalMonth { set; get; }

        [Display(Name ="Year")]
        public int SalYear { set; get; }
        public List<SelectListModel> branches { set; get; }

        public Nullable<int> BranchID { get; set; }

    }
}
