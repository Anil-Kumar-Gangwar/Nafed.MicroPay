
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
    
public partial class AssetmanagementDetail
{

    public int ID { get; set; }

    public int AssetTypeID { get; set; }

    public int IMID { get; set; }

    public int EmployeeID { get; set; }

    public System.DateTime AllocationDate { get; set; }

    public Nullable<System.DateTime> DeAllocationDate { get; set; }

    public Nullable<int> StatusID { get; set; }

    public bool IsDeleted { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }

    public Nullable<int> UpdatedBy { get; set; }



    public virtual AssetType AssetType { get; set; }

    public virtual User User { get; set; }

    public virtual tblMstEmployee tblMstEmployee { get; set; }

    public virtual InventoryManagement InventoryManagement { get; set; }

    public virtual User User1 { get; set; }

}

}
