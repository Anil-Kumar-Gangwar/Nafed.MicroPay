using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Nafed.MicroPay.Model
{


    public class PR
    {
        public int Counter { get; set; }
        public int PRID { get; set; }
        [DisplayName("Year :")]
        public Nullable<int> Year { get; set; }
        public List<SelectListModel> CYearList { set; get; } = new List<SelectListModel>();
        //public IEnumerable<dynamic> CYearList { set; get; } = new List<dynamic>();
        public IEnumerable<dynamic> EmployeeList { set; get; } = new List<dynamic>();
       
        [DisplayName("Employee :")]
         [Range(1, Int64.MaxValue, ErrorMessage = "Please select employee")]
        public Nullable<int> EmployeeId { get; set; }
        public string Employeecode { get; set; }
        public string Employeename { get; set; }

        public string BranchName { get; set; }



        [Required(ErrorMessage = "Please enter details where property is situated")]
        [DisplayName("Name of District, Sub-Division, Taluk, Village in which property is situated :")]
        public string PropertySituated { get; set; }

        public IEnumerable<dynamic> PropertyTypeDetails { set; get; } = new List<dynamic>();

        [Required(ErrorMessage = "Please select Property Type")]
        [DisplayName("Property Type :")]
        public Nullable<int> PropertyType { get; set; }

        [DisplayName("Property Details :")]
        public string PropertyDetails { get; set; }

        [DisplayName("Present Value :")]
        public decimal PresentValue { get; set; }

        [DisplayName("Own Name :")]
        public bool SelfProperty { get; set; }

        [DisplayName("Property Owner  :")]
        public string PropertyOwner { get; set; }
        public IEnumerable<dynamic> RelationDetails { set; get; } = new List<dynamic>();

        [DisplayName("Relation :")]
        public Nullable<int> RelationID { get; set; }

        public IEnumerable<dynamic> AcquiredTypeDetails { set; get; } = new List<dynamic>();

        [DisplayName("Acquired Type :")]
        public Nullable<int> AcquiredTypeID { get; set; }
        


        [DisplayName("Acquisition Date :")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AcquisitionDate { get; set; }


        
        [DisplayName("Persons from whom acquired  : ")]
        public string AcquiredPerson { get; set; }

        [DisplayName("Acquired Details : ")]
        public string AcquiredDetails { get; set; }

        [DisplayName("Property Income : ")]
        public decimal PropertyIncome { get; set; }

        [DisplayName("Remarks : ")]
        public string Remarks { get; set; }

        public Nullable<System.DateTime> Duedate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

        public string SelectedYear
        { get; set; }
        public string SelectedEmployeeName
        { get; set; }

        public string SelectedPropertyType
        { get; set; }

        public string SelectedAcquiredType
        { get; set; }
        public string SelectedRelationName
        { get; set; }


        public Nullable<int> StatusID { get; set; }
        public Model.FormGroupAHdr formGroupAHdr { get; set; } = new Model.FormGroupAHdr();

        public decimal E_Basic { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }

        public bool NotApplicable { get; set; }
    }
}
