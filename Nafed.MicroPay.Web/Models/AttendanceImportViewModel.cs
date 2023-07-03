using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class AttendanceImportViewModel
    {

        public List<EmpAttendanceForm> attendanceData { set; get; }
        public Dictionary<string, string> ErrorMsgCollection { get; set; }
        public int NoOfErrors { get; set; }
        public string Message { get; set; }
    }
}