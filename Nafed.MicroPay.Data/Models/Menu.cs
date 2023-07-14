
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
    
public partial class Menu
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Menu()
    {

        this.DepartmentRights = new HashSet<DepartmentRight>();

        this.UserRights = new HashSet<UserRight>();

    }


    public int MenuID { get; set; }

    public string MenuName { get; set; }

    public Nullable<int> ParentID { get; set; }

    public Nullable<int> SequenceNo { get; set; }

    public string Url { get; set; }

    public string IconClass { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public string Help { get; set; }

    public bool ActiveOnMobile { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DepartmentRight> DepartmentRights { get; set; }

    public virtual User User { get; set; }

    public virtual User User1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<UserRight> UserRights { get; set; }

}

}
