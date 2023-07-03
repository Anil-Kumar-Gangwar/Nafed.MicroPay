using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
    using System;
    using System.Collections.Generic;
    
    public class EmployeeDeputation
    {
        public int EmpDeputationID { get; set; }
        public int EmployeeID { get; set; }

        [DisplayName("From Date :")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter from date")]

        public System.DateTime FromDate { get; set; }

      
        [DisplayName("To Date :")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter to date")]

        public System.DateTime ToDate { get; set; }

        [DisplayName("Organization Name :")]
        [Required(ErrorMessage = "Please enter organization name")]
        public string OrganizationName { get; set; }

        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public string AlertMessage { get; set; }
    
      
    }
}
