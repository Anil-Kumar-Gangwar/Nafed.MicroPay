
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Nafed.MicroPay.Data.Models
{

using System;
    using System.Collections.Generic;
    
public partial class AcadmicProfessionalDetail
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public AcadmicProfessionalDetail()
    {

        this.tblEmployeeQualificationdetails = new HashSet<tblEmployeeQualificationdetail>();

        this.tblMstEmployees = new HashSet<tblMstEmployee>();

        this.tblMstEmployees1 = new HashSet<tblMstEmployee>();

        this.tblMstEmployees2 = new HashSet<tblMstEmployee>();

        this.tblMstEmployees3 = new HashSet<tblMstEmployee>();

    }


    public int ID { get; set; }

    public int TypeID { get; set; }

    public string Value { get; set; }

    public bool IsDeleted { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }



    public virtual User User { get; set; }

    public virtual User User1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblEmployeeQualificationdetail> tblEmployeeQualificationdetails { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblMstEmployee> tblMstEmployees { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblMstEmployee> tblMstEmployees1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblMstEmployee> tblMstEmployees2 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblMstEmployee> tblMstEmployees3 { get; set; }

}

}
