using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class Resignation
    {
        public int ResignationId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "Date of Resignation")]
        [Required(ErrorMessage = "Please select date of resignation")]
        public System.DateTime ? ResignationDate { get; set; }
        [Display(Name = "Notice Period")]
        [Required(ErrorMessage = "Please enter notice period")]
        public Nullable<int> NoticePeriod { get; set; }
        [Display(Name = "Reason")]
        [Required(ErrorMessage = "Please enter reason")]
        [Range(1,Int16.MaxValue, ErrorMessage = "Please enter reason")]
        public Nullable<int> Reason { get; set; }
        [Display(Name = "Other Reason")]
        public string OtherReason { get; set; }
        public Nullable<int> StatusId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [Display(Name ="Employee")]
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Branch { get; set; }
        [Display(Name = "Date of joining")]
        public string DOJ { get; set; }
        public string DocName { get; set; }
        public DateTime Pr_Loc_DOJ { get; set; }
        public ProcessWorkFlow _ProcessWorkFlow { get; set; }
    }
}
