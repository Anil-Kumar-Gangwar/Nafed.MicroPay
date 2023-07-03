using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class ConfirmationFormAHdr
    {
        public int FormAHeaderId { get; set; }
        public int EmployeeId { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
        public int ProcessId { get; set; }
        public Nullable<System.DateTime> DueConfirmationDate { get; set; }
        public bool CertificatesReceived { get; set; }
        public bool PoliceVerification { get; set; }
        public Nullable<byte> Point8_1 { get; set; }
        public byte Point8_2 { get; set; }
        public Nullable<byte> Point8_3 { get; set; }
        public Nullable<byte> Point8_4 { get; set; }
        public Nullable<byte> Point8_5 { get; set; }
        public Nullable<byte> Point8_6 { get; set; }
        public Nullable<byte> Point8_7 { get; set; }
        public Nullable<byte> Point8_8 { get; set; }
        public Nullable<byte> Point8_9 { get; set; }
        public Nullable<byte> Point8_10 { get; set; }
        public Nullable<byte> Point8_11 { get; set; }
        public Nullable<byte> Point8_12 { get; set; }
        public Nullable<byte> Point8_13 { get; set; }
        public bool Point9 { get; set; }
        public string Point9_Remark { get; set; }
        public bool Point10 { get; set; }
        public string Point10_Remark { get; set; }
        public bool Point11 { get; set; }
        public string Point11_Remark { get; set; }
        public bool Point12 { get; set; }
        public string Point12_Remark { get; set; }
        public bool ConfirmationRecommended { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public Nullable<DateTime> DOJ { get; set; }
        public Nullable<DateTime> appointmentConfirmationDate { get; set; }
        public Nullable<DateTime> promotionConfirmationDate { get; set; }

        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeName { get; set; }

        public int FormHdrID { get; set; }
    }
}
