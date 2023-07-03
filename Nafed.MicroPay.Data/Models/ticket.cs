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
    
    public partial class ticket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ticket()
        {
            this.ticket_attachments = new HashSet<ticket_attachments>();
        }
    
        public int ID { get; set; }
        public Nullable<int> status_id { get; set; }
        public Nullable<int> priority_id { get; set; }
        public int type_id { get; set; }
        public int DepartmentID { get; set; }
        public Nullable<int> customer_id { get; set; }
        public Nullable<int> agent_id { get; set; }
        public Nullable<int> group_id { get; set; }
        public string name { get; set; }
        public string Email { get; set; }
        public string subject { get; set; }
        public string Message { get; set; }
        public Nullable<int> reference_ids { get; set; }
        public bool is_new { get; set; }
        public bool is_replied { get; set; }
        public bool is_reply_enabled { get; set; }
        public bool is_starred { get; set; }
        public bool is_trashed { get; set; }
        public bool is_agent_viewed { get; set; }
        public bool is_customer_viewed { get; set; }
        public Nullable<int> subGroup_id { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int DesignationID { get; set; }
        public string PreviousTicket { get; set; }
        public string ContactNo { get; set; }
    
        public virtual Department Department { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual Support_group Support_group { get; set; }
        public virtual Support_team Support_team { get; set; }
        public virtual tblMstEmployee tblMstEmployee { get; set; }
        public virtual tblMstEmployee tblMstEmployee1 { get; set; }
        public virtual ticket_priority ticket_priority { get; set; }
        public virtual ticket_status ticket_status { get; set; }
        public virtual ticket_type ticket_type { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ticket_attachments> ticket_attachments { get; set; }
    }
}
