
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
    
public partial class DeductionSection
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public DeductionSection()
    {

        this.DeductionSubSections = new HashSet<DeductionSubSection>();

        this.DeductionSubSectionDescriptions = new HashSet<DeductionSubSectionDescription>();

        this.EmpDeductionUnderChapterVI_A = new HashSet<EmpDeductionUnderChapterVI_A>();

    }


    public int SectionID { get; set; }

    public string SectionName { get; set; }

    public string FYear { get; set; }

    public int CreatedBy { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public bool IsDeleted { get; set; }

    public Nullable<int> UpdatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }



    public virtual User User { get; set; }

    public virtual User User1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DeductionSubSection> DeductionSubSections { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DeductionSubSectionDescription> DeductionSubSectionDescriptions { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EmpDeductionUnderChapterVI_A> EmpDeductionUnderChapterVI_A { get; set; }

}

}
