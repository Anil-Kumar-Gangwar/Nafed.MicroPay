using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class Designation
    {
        public int DesignationID { get; set; }
        public int DepartmentID { get; set; }
        [Display(Name = "Designation Code :")]
       // [Required(ErrorMessage = "Designation Code is required.")]
        public string DesignationCode { get; set; }
        [Display(Name = "Designation Name :")]
        [Required(ErrorMessage = "Designation Name is required.")]
        public string DesignationName { get; set; }
        [Display(Name = "Level :")]
        public string Level { get; set; }

        [Display(Name = "Is Officer :")]
        public bool IsOfficer { get; set; }

        [Display(Name = "Rank :")]
        public Nullable<int> Rank { get; set; }

        [Display(Name = "Cadre :")]
        public Nullable<int> CadreID { get; set; }

        [Display(Name = "Category :")]
        public Nullable<int> CateogryID { get; set; }

        [Display(Name = "LCT :")]
        public Nullable<decimal> LCT { get; set; }

        [Display(Name = "Promotion :")]
        public Nullable<decimal> Promotion { get; set; }

        [Display(Name = "Direct :")]
        public Nullable<decimal> Direct { get; set; }

        public string CadreName { get; set; }

        public string CategoryName { get; set; }

        public Nullable<bool> IsUpgradedDesignation { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<decimal> B1 { get; set; }
        public Nullable<decimal> E1 { get; set; }
        public Nullable<decimal> B2 { get; set; }
        public Nullable<decimal> E2 { get; set; }
        public Nullable<decimal> B3 { get; set; }
        public Nullable<decimal> E3 { get; set; }
        public Nullable<decimal> B4 { get; set; }
        public Nullable<decimal> E4 { get; set; }
        public Nullable<decimal> B5 { get; set; }
        public Nullable<decimal> E5 { get; set; }
        public Nullable<decimal> B6 { get; set; }
        public Nullable<decimal> E6 { get; set; }
        public Nullable<decimal> B7 { get; set; }
        public Nullable<decimal> E7 { get; set; }
        public Nullable<decimal> B8 { get; set; }
        public Nullable<decimal> E8 { get; set; }
        public Nullable<decimal> B9 { get; set; }
        public Nullable<decimal> E9 { get; set; }
        public Nullable<decimal> B10 { get; set; }
        public Nullable<decimal> E10 { get; set; }
        public Nullable<decimal> B11 { get; set; }
        public Nullable<decimal> E11 { get; set; }
        public Nullable<decimal> B12 { get; set; }
        public Nullable<decimal> E12 { get; set; }
        public Nullable<decimal> B13 { get; set; }
        public Nullable<decimal> E13 { get; set; }
        public Nullable<decimal> B14 { get; set; }
        public Nullable<decimal> E14 { get; set; }
        public Nullable<decimal> B15 { get; set; }
        public Nullable<decimal> E15 { get; set; }
        public Nullable<decimal> B16 { get; set; }
        public Nullable<decimal> E16 { get; set; }
        public Nullable<decimal> B17 { get; set; }
        public Nullable<decimal> E17 { get; set; }
        public Nullable<decimal> B18 { get; set; }
        public Nullable<decimal> E18 { get; set; }
        public Nullable<decimal> B19 { get; set; }
        public Nullable<decimal> E19 { get; set; }
        public Nullable<decimal> B20 { get; set; }
        public Nullable<decimal> E20 { get; set; }
        public Nullable<decimal> B21 { get; set; }
        public Nullable<decimal> E21 { get; set; }
        public Nullable<decimal> B22 { get; set; }
        public Nullable<decimal> E22 { get; set; }
        public Nullable<decimal> B23 { get; set; }
        public Nullable<decimal> E23 { get; set; }
        public Nullable<decimal> B24 { get; set; }
        public Nullable<decimal> E24 { get; set; }
        public Nullable<decimal> B25 { get; set; }
        public Nullable<decimal> E25 { get; set; }
        public Nullable<decimal> B26 { get; set; }
        public Nullable<decimal> E26 { get; set; }
        public Nullable<decimal> B27 { get; set; }
        public Nullable<decimal> E27 { get; set; }
        public Nullable<decimal> B28 { get; set; }
        public Nullable<decimal> E28 { get; set; }
        public Nullable<decimal> B29 { get; set; }
        public Nullable<decimal> E29 { get; set; }
        public Nullable<decimal> B30 { get; set; }
        public Nullable<decimal> E30 { get; set; }
        public Nullable<decimal> B31 { get; set; }
        public Nullable<decimal> E31 { get; set; }
        public Nullable<decimal> B32 { get; set; }
        public Nullable<decimal> E32 { get; set; }
        public Nullable<decimal> B33 { get; set; }
        public Nullable<decimal> E33 { get; set; }
        public Nullable<decimal> B34 { get; set; }
        public Nullable<decimal> E34 { get; set; }
        public Nullable<decimal> B35 { get; set; }
        public Nullable<decimal> E35 { get; set; }
        public Nullable<decimal> B36 { get; set; }
        public Nullable<decimal> E36 { get; set; }
        public Nullable<decimal> B37 { get; set; }
        public Nullable<decimal> E37 { get; set; }
        public Nullable<decimal> B38 { get; set; }
        public Nullable<decimal> E38 { get; set; }
        public Nullable<decimal> B39 { get; set; }
        public Nullable<decimal> E39 { get; set; }
        public Nullable<decimal> B40 { get; set; }
        public Nullable<decimal> E40 { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }


        public Nullable<byte> LCTInNo { get; set; }
        public Nullable<byte> PromotionInNo { get; set; }
        public Nullable<byte> DirectInNo { get; set; }
        public decimal? FamilyAssured { get; set; }

        public virtual Cadre Cadre { get; set; }
        public virtual EmployeeCategory Category { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
