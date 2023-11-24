using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class Branch
    {
        public int BranchID { get; set; }
        [DisplayName("Branch Code :")]
        //[Required(ErrorMessage = "Please enter branch code")]
        public string BranchCode { get; set; }
        [DisplayName("Branch Name :")]
        [Required(ErrorMessage = "Please enter branch name")]
        public string BranchName { get; set; }
        public bool IsHillComp { get; set; }
        [DisplayName("Address1 :")]
        [Required(ErrorMessage = "Please enter Address")]
        public string Address1 { get; set; }
        [DisplayName("Address2 :")]
        public string Address2 { get; set; }
        [DisplayName("Address3 :")]
        public string Address3 { get; set; }
        [DisplayName("Pin Code :")]
        public string Pin { get; set; }
        [DisplayName("City :")]
        public Nullable<int> CityID { get; set; }
        [DisplayName("Grade :")]
        public Nullable<int> GradeID { get; set; }
        [DisplayName("Region :")]
        public string Region { get; set; }
        [DisplayName("PhoneSTD :")]
        public string PhoneSTD { get; set; }
        [DisplayName("PhoneNo :")]
        public string PhoneNo { get; set; }
        [DisplayName("FaxSTD :")]
        public string FaxSTD { get; set; }
        [DisplayName("FaxNo :")]
        public string FaxNo { get; set; }
        [DisplayName("Remarks :")]
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string GradeName { get; set; }

        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        public string EmailId { get; set; }
    }
}
