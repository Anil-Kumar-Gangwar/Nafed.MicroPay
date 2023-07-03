using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nafed.MicroPay.Model
{
    public class User
    {
        public int UserID { get; set; }
        [DisplayName("User Name :")]
        [Required(ErrorMessage = "Please enter user name")]
        public string UserName { get; set; }
        public string hdUserName { get; set; }
        [DisplayName("Full Name :")]
        [Required(ErrorMessage = "Please enter full name")]
        public string FullName { get; set; }
        [DisplayName("Password :")]
        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }

        public string hdPassword { get; set; }

        [DisplayName("Confirm Password :")]
        [Required(ErrorMessage = "Please enter confirm password")]
        public string ConfirmPassword { get; set; }
        public string hdConfirmPassword { get; set; }
        [DisplayName("Department:")]
      //  [Required(ErrorMessage = "Please select department")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select department")]
        public int DepartmentID { get; set; }

        [DisplayName("User Type :")]
        [Range(1,Int32.MaxValue,ErrorMessage = "Please enter user type")]
      //  [Required(ErrorMessage = "Please enter user type")]
        public int UserTypeID { get; set; }
        [DisplayName("Mobile No :")]
    //    [Required(ErrorMessage = "Please enter mobile no")]
        public string MobileNo { get; set; }
        [DisplayName("Email-ID :")]
    //    [Required(ErrorMessage = "Please enter email id")]
        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        public string EmailID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        [DisplayName("Image Name :")]
      //  [Required(ErrorMessage = "Please select image")]
        public string ImageName  { get; set; }
        public bool IsActive { get; set; }

        [DisplayName("Employee :")]
        [Required(ErrorMessage ="Please select employee ")]
        public int EmployeeID { set; get; }
        public string DepartmentName { get; set; }
        public string UserTypeName { get; set; }
        public int DesignationID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }

        public string UserProfilePhotoUNCPath { set; get; }

        public List<SelectListModel> EmployeeList { set; get; }
        public Nullable<byte> WrongAttemp { get; set; }
        public Nullable<System.DateTime> LockDate { get; set; }


    }
}
