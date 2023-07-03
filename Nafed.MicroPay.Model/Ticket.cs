using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
    public class Ticket
    {
        public int ID { get; set; }
        public Nullable<int> status_id { get; set; }
        [Display(Name = "Department")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Department")]
        public int DepartmentID { get; set; }
        [Display(Name = "Priority")]
        public Nullable<int> priority_id { get; set; }
        [Display(Name = "Type")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Type")]
        public int type_id { get; set; }
        public Nullable<int> customer_id { get; set; }
        public Nullable<int> agent_id { get; set; }
        public Nullable<int> group_id { get; set; }
        [Display(Name = "Name")]
        // [Required(ErrorMessage = "Please enter Name")]
        public string name { get; set; }
        [Display(Name = "Personal email")]
        //[Required(ErrorMessage = "Please enter Email")]
        // [StringLength(50, ErrorMessage = "{0} not be exceed 50 char")]
        // [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email.")]
        public string Email { get; set; }
        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Please enter Subject")]
        public string subject { get; set; }
        [Display(Name = "Message")]
        [Required(ErrorMessage = "Please enter Message")]
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
        public string TicketType { get; set; }
        public int DesignationID { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Remark { get; set; }
        public string TicketSolverRmk { get; set; }
        [Display(Name = "Previous Reference Ticket No. (If any)")]
        public string PreviousTicket { get; set; }
        [StringLength(10,ErrorMessage ="Contact No. can't be exceed more then 10 digits")]
        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }
        [Display(Name ="Employee Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "First Date of Joining")]
        public DateTime? DOJ { get; set; }
        [Display(Name = "Last Date of Working")]
        public DateTime? LastWorking { get; set; }
        [Display(Name = "Last Branch")]
        public int? BranchId { get; set; }

        [Display(Name = "Last Department")]
        public int? LastDepartment { get; set; }
        [Display(Name = "Name")]
        public String EmployeeName { get; set; }
        public TicketAttachment TicketDocument { get; set; }
        public List<TicketAttachment> TicketDocumentList { get; set; } = new List<TicketAttachment>();
        public List<TicketForwardDetails> TForwardList { get; set; }
    }
    public enum TicketTypeStatus
    {

        Open = 1,
        Pending = 2,
        Answered = 3,
        Resolved = 4
    }
}
