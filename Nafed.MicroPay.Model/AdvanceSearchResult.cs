using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Nafed.MicroPay.Model
{

    public class AdvanceSearchDateFilter
    {
        public int FilterID { set; get; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateFrom { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateTo { get; set; }= DateTime.Now;

    }

    public class AdvanceSearchResult
    {
        public SelectMoreFields SelectMoreFields { set; get; } = new SelectMoreFields();
        public DynamicGridRow DefaultTableSchema { set; get; }
        public DynamicGrid GridView { get; set; }

        public DataSet SearchedResultDT { set; get; }

    }
    public class DynamicGrid
    {
        public List<string> Columns { get; set; }
        public List<DynamicGridRow> Rows { get; set; }
       
    }
    public class DynamicGridRow
    {
        [Display(Name ="Branch Code")]
        public string BranchCode { get; set; }

        [Display(Name = "Employee Code")]
        public string  EmployeeCode { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Designation Code")]
        public string DesignationCode { get; set; }

        [Display(Name = "Employee Type")]
        public string EmployeeType { get; set; }
        public List<string> Values { get; set; }       
    }

    public class SelectMoreFields : AdvanceSearchDateFilter
    {
        public IEnumerable<string> CheckedChkName
        {
            get
            {
                return GetType().GetProperties().Where(p => p.PropertyType == typeof(bool) 
                && p.GetValue(this,null).Equals(true)).Select(p=>p.Name).ToList(); 
            }
        }

        public IEnumerable<string> CheckedChkDisplayName
        {
            get
            {
                return GetType().GetProperties().Where(p => p.PropertyType == typeof(bool)
                && p.GetValue(this, null).Equals(true))
                    .Select(f => f.GetCustomAttribute<DisplayAttribute>())
                    .Where(x => x != null)
                    .Select(x => x.Name)
                    .ToList();
            }
        }
        public int FilterTypeID { set; get; }

        public int SelectedEmployeeType { set; get; }

        public int[] PreviousFields { get; set; }

        [Display(Name = "Title")]
        public bool TitleName { get; set; }

        [Display(Name = "Current Designation")]
        public bool DesignationName { get; set; }

        [Display(Name = "Date of Joining in Current branch")]
        public bool DOJ { set; get; }

        [Display(Name = "Date Of Super Annuatation")]
        public bool DOSupAnnuating { get; set; }

        [Display(Name = "Order Of Appointment")]
        public bool orderofpromotion { get; set; }


        [Display(Name = "Date Of Birth")]
        public bool DOB { get; set; }

        [Display(Name = "First Designation")]
        public bool FirstDesgID { get; set; }

        [Display(Name = "First Place of Posting")]
        public bool FirstBranchID { get; set; }

        [Display(Name = "First DOJ")]
        public bool Pr_Loc_DOJ { get; set; }

        [Display(Name = "Basic Pay")]
        public bool E_Basic { set; get; }

        [Display(Name = "Last Basic")]
        public bool LastBasic { set; get; }

        [Display(Name = "Current Place of Posting")]
        public bool BranchName { get; set; }
        [Display(Name = "Cadre")]
        public bool CadreName { get; set; }

        [Display(Name = "Date Of Leaving")]
        public bool DOLeaveOrg { get; set; }       

             

        [Display(Name = "Increment Month")]
        public bool IncrementMonth { get; set; }

        [Display(Name = "Loan Type")]
        public bool LoanDesc { get; set; }
        
        
        [Display(Name = "Date Of Confirmation")]
        public bool ConfirmationDate { get; set; }

        [Display(Name = "Department/Section")]
        public bool DepartmentName { get; set; }

        [Display(Name = "Seniority Code")]
        public bool Sen_Code { get; set; }

        

        [Display(Name = "Category")]
        public bool CategoryName { get; set; }
        [Display(Name = "Date Of Promotion")]
        public bool PromotionDate { get; set; }

        [Display(Name = "ACR")]
        public bool ACR_No { get; set; }

        [Display(Name = "Address")]
        public bool PmtAdd { get; set; }

        [Display(Name = "File No.")]
        public bool FileNo { get; set; }

        [Display(Name = "PF No.")]
        public bool PFNO { get; set; }

        [Display(Name = "Folio No.")]
        public bool Folio_No { get; set; }
        [Display(Name = "Husband/Father Name")]
        public bool HBName { get; set; }

        [Display(Name = "Reason Of Leaving")]
        public bool ReasonOfLeaving { get; set; }

     

        [Display(Name = "Blood Group")]
        public bool BloodGroupName { get; set; }

       

        [Display(Name = "Level")]
        public bool Level { get; set; }

        [Display(Name = "Academic Qualification")]
        public bool QAcademicID { set; get; }

        [Display(Name = "Professional Qualification")]
        public bool QProfessionalID { set; get; }

        [Display(Name = "CL")]
        public bool CL { set; get; }

        [Display(Name = "EL")]
        public bool EL { set; get; }

        [Display(Name = "Medical")]
        public bool Medical { set; get; }

        [Display(Name = "Medical Extra")]
        public bool Medical_Extra { set; get; }       

        [Display(Name = "Pay Scale")]
        public bool PayScale { set; get; }

        [Display(Name = "Mobile No.")]
        public bool MobileNo { set; get; }

        [Display(Name = "E-Mail")]
        public bool OfficialEmail { set; get; }


    }
}
