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
    
    public partial class GetConveyanceBillDescription_Result
    {
        public int ConveyanceDescID { get; set; }
        public int VehicleID { get; set; }
        public System.DateTime Dated { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public int ConveyanceBillDetailID { get; set; }
        public int EmployeeID { get; set; }
    }
}
