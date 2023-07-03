using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Nafed.MicroPay.Model
{
    public class EmployeeAttendanceDetails
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string ReportingOfficer { get; set; }
        
        public string D1 {get; set;}
        public string D2 { get; set; }
        public string D3 { get; set; }
        public string D4 { get; set; }
        public string D5 { get; set; }
        public string D6 { get; set; }
        public string D7 { get; set; }
        public string D8 { get; set; }
        public string D9 { get; set; }
        public string D10 { get; set; }
        public string D11 { get; set; }
        public string D12 { get; set; }
        public string D13 { get; set; }
        public string D14 { get; set; }
        public string D15 { get; set; }
        public string D16 { get; set; }
        public string D17 { get; set; }
        public string D18 { get; set; }
        public string D19 { get; set; }
        public string D20 { get; set; }
        public string D21 { get; set; }
        public string D22 { get; set; }
        public string D23 { get; set; }
        public string D24 { get; set; }
        public string D25 { get; set; }
        public string D26 { get; set; }
        public string D27 { get; set; }
        public string D28 { get; set; }
        public string D29 { get; set; }
        public string D30 { get; set; }
        public string D31 { get; set; }

        public int BranchId { get; set; }
        public int EmployeeId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        public string Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Hours { get; set; }

        public int DisplayType { get; set; }
        public string DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
        public MyAttendanceDetails MyAttendance { get; set; }

        public List<MyAttendanceDetails> InTimeOutTimes { get; set; } = new List<MyAttendanceDetails>();


        [DisplayName("From Date :")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public Nullable<System.DateTime> fromdate { get; set; }

        [DisplayName("To Date :")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> todate { get; set; }
        public string BranchName { get; set; }

        public string CellbgColor { get; set; }
    }

    public enum Colors{
        Red,
        Yellow,
        orange,
        blue,
        gray
    }

    //return $(this).text() == "NA";
    //        if ($(this).text().trim() == "NA") {
    //            $(this).css("background-color", "yellow");
    //        }

    //        if ($(this).text().trim() == "W") {
    //            $(this).css("background-color", "gray");
    //        }
    //        if ($(this).text().trim() == "CS") {
    //            $(this).css("color", "orange");
    //        }
    //        if ($(this).text().trim() == "WP") {
    //            $(this).css("color", "red");
    //        }
    //        if ($(this).text().trim() == "EL") {
    //            $(this).css("color", "blue");


    public class MyAttendanceDetails {



        public int Day
        {
            set { }
            get
            {
                return Convert.ToDateTime(Date).Day;
            }
        }
        public int EmployeeId { get; set; }
        public string Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Hours { get; set; }
        public int Editable { get; set; }
        public int EmpAttendanceID { get; set; }
         
    }
}
