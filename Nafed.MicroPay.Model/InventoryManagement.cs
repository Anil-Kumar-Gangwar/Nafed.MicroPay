using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class InventoryManagement
    {
       
        public int ID { get; set; }
        [Display(Name = "Asset Name")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Asset Name")]
        public int IMID { get; set; }

        [Display(Name = "Asset Type")]
        [Range(1, Int32.MaxValue,ErrorMessage = "Please select Asset Type")]
        public int AssetTypeID { get; set; }
        [Display(Name = "Manufacturer")]
        [Range(1,Int32.MaxValue,ErrorMessage = "Please select Manufacturer")]
        public int ManufacturerID { get; set; }
        [Display(Name = "Serial No.")]
        [Required(ErrorMessage = "Please enter Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Price")]
        [Range(1,999999,ErrorMessage = "Please enter Price")]
        public decimal Price { get; set; }
        [Display(Name = "Asset Name")]
        [Required(ErrorMessage = "Please enter Asset")]
        public string AssetName { get; set; }
        public bool Consumable { get; set; }
        public string Remarks { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Status")]
        [Display(Name = "Status")]
        public Nullable<int> StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string AssetTypeName { get; set; }
        public string ManufacturerName { get; set; }

        [Display(Name = "Employee")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Employee")]
        public int EmployeeID { get; set; }

        public int Employeecode { get; set; }
        public string EmployeeName { get; set; }

        [Display(Name = "Allocation Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter Allocation Date")]
        public Nullable<System.DateTime> AllocationDate { get; set; }

        [Display(Name = "Deallocation Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime>DeAllocationDate { get; set; }

        [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email.")]
        public string Email { get; set; }
        public string empEmail { get; set; }
        public string domain { get; set; }

        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
        [Display(Name = "Manufacturing Year")]
        [Required(ErrorMessage ="Please select {0}.")]
        public string ManufacturingYr { get; set; }
    }
}
