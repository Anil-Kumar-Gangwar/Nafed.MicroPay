using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class PromotionFilters
    {
        [Display(Name = "Branch")]
        public int? branchId { get; set; }
        public List<SelectListModel> branchList { set; get; }

        [Display(Name = "Month")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Month")]
        public int? monthId { get; set; }
        public List<SelectListModel> monthList { set; get; }

        [Display(Name = "Year")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Year")]
        public int? yearId { get; set; }

        [Display(Name = "Staff Budget Year")]
        [Required(ErrorMessage = "Staff budget year required")]
        public string staffBudgetYearId { get; set; }
        public List<SelectListModel> staffBudgetYearList { set; get; }
        [Display(Name = "All Branch")]
        public bool AllBranch { get; set; }
    }
}
