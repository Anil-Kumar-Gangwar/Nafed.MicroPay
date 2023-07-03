using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
    public class FileWorkflow
    {
        public int WorkFlowID { get; set; }
        [Display(Name = "Designation")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Designation")]
        public Nullable<int> initiateDesignationID { get; set; }
        [Display(Name = "Designation")]
      //  [Required]
      //  [Range(1, Int32.MaxValue, ErrorMessage = "Please select Designation")]
        public Nullable<int> ParkByDesignationID { get; set; }
        public Nullable<System.DateTime> todate { get; set; }
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "Please select From Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fromdate { get; set; }
        public Nullable<System.DateTime> created_on { get; set; }
        public Nullable<int> created_by { get; set; }
        [Display(Name = "Department")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Department")]
        public Nullable<int> initiateDepartmentID { get; set; }
        [Display(Name = "Department")]
      //  [Required]
      //  [Range(1, Int32.MaxValue, ErrorMessage = "Please select Department")]
        public Nullable<int> ParkByDepartmentID { get; set; }

        [Display(Name = "Employee")]
        //[Required]
        //[Range(1, Int32.MaxValue, ErrorMessage = "Please select Employee")]
        public int initiateEmployeeID { get; set; }

        [Display(Name = "Employee")]
       // [Required]
       // [Range(1, Int32.MaxValue, ErrorMessage = "Please select Employee")]
        public int[] intEmployeeID { get; set; }

        [Display(Name = "Employee")]
       // [Required]
       // [Range(1, Int32.MaxValue, ErrorMessage = "Please select Employee")]
        public Nullable<int> ParkByEmployeeID { get; set; }
        public string InitiatedDepartment { get; set; }
        public string ParkedDepartment { get; set; }
        public string InitiatedDesignation { get; set; }
        public string ParkedDesignation { get; set; }
        public string InitiateEmployee { get; set; }
        public string ParkedEmployee { get; set; }

    }
}
