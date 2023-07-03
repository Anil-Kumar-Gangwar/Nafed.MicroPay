using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class TicketWorkFlow
    {
        public int WflowID { get; set; }
        [Display(Name = "Department")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Department")]
        public int DepartmentID { get; set; }
        [Display(Name = "Type")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Type")]
        public int TicketTypeID { get; set; }
        [Display(Name = "Designation")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Designation")]
        public int DesignationID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string TicketType { get; set; }
    }
}
