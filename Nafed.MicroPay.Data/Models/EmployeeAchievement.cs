
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
    
public partial class EmployeeAchievement
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public EmployeeAchievement()
    {

        this.EmpAchievementAndCertificationDocuments = new HashSet<EmpAchievementAndCertificationDocument>();

    }


    public int EmpAchievementID { get; set; }

    public int EmployeeID { get; set; }

    public Nullable<System.DateTime> DateOfAchievement { get; set; }

    public string AchievementName { get; set; }

    public string AchievementRemark { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }



    public virtual tblMstEmployee tblMstEmployee { get; set; }

    public virtual User User { get; set; }

    public virtual User User1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EmpAchievementAndCertificationDocument> EmpAchievementAndCertificationDocuments { get; set; }

}

}
