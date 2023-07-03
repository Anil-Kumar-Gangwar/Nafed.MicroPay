using System;
using System.Collections.Generic;

namespace Nafed.MicroPay.Model
{
    public class EmpAttendanceForm
    {
        public int Sno { set; get; }
        public int EmployeeID { set; get; }
        public string EmpCode { set; get; }
        public string EmployeeName { set; get; }
        public string InDate { set; get; }
      
        public string InTime { set; get; }
        public string OutTime { set; get; }
        public string Remarks { set; get; }
        public string error { set; get; }
        public string warning { set; get; }

        public bool isDuplicatedRow { set; get; }
    }

    public class AttendanceImportColValues
    {
        public List<string> empCode { set; get; }
    }
}
