using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace Nafed.MicroPay.Model
{
    public class Holiday
    {
        [DisplayName("Calendar Year :")]
        public Nullable<int> CYear { get; set; }

        public int HolidayID { get; set; }
        [DisplayName("Holiday Name :")]
        [Required(ErrorMessage = "Please enter holiday name")]
        public string HolidayName { get; set; }

        public List<SelectListModel> CYearList { set; get; } = new List<SelectListModel>();

        [DisplayName("Holiday Date :")]
        [Required(ErrorMessage = "Please enter holiday date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> HolidayDate { get; set; }

        [DisplayName("Branch :")]
        [Required(ErrorMessage = "Please Select Branch")]
        public Nullable<System.Int32> BranchId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }

        public string BranchName { get; set; }
    }
}
