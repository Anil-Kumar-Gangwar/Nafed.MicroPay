using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConveyanceBillDescription
    {
        public int sno { set; get; }
        public int ConveyanceDescID { get; set; }
        public int ConveyanceBillDetailID { get; set; }
        [Required(ErrorMessage = "Please enter vehicle Info.")]
        public int VehicleID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid Date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter date")]
        public Nullable<System.DateTime> Dated { get; set; }
        [Required(ErrorMessage = "Please enter from place Info")]
        public string From { get; set; }
        [Required(ErrorMessage = "Please enter to place Info")]
        public string To { get; set; }
        [Required(ErrorMessage = "Please enter amount")]
        [Range(1.00, double.MaxValue, ErrorMessage = "Please enter amount")]
        public decimal Amount { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [Required(ErrorMessage = "Please Select Vehicle")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Vehicle")]
        public ConveyanceBillDesc conveyanceBillDesc { set; get; }
    }

    public enum ConveyanceBillDesc
    {
        Car = 1,
        Scooter = 2,
        Taxi = 3
    }
}
