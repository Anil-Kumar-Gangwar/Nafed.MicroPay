using System.Collections.Generic;

namespace MicroPay.Web.Models
{
    public class BaseReportModel
    {
        public string rptName { set; get; }
        //public string dsUserName { set; get; }
        //public string dsPassword { set; get; } 
        //public string dsServerName { set; get; } = "odbcNAFEDHR";
        //public string dataBaseName { set; get; } = "HRMS";
        public int reportType { set; get; } = 1; //== 1- Personal , 2- Payroll 
        public string ReportFilePath
        {
            get
            {
                if (reportType == 1)
                    return $"~/CrystalRpts/Personal/";
                else
                    return $"~/CrystalRpts/Payroll/";

            }
        }

        public string CandidateImage
        {
            get
            {
                return Nafed.MicroPay.Common.DocumentUploadFilePath.CandidateAdmitCardPhoto;
            }
        }
        public string CandidateSignature
        {
            get
            {
                return Nafed.MicroPay.Common.DocumentUploadFilePath.CandidateAdmitCardSignature;
            }
        }

        public string AdmitCardCPath
        {
            get
            {
                return Nafed.MicroPay.Common.DocumentUploadFilePath.AdmitCardUNCPath;
            }
        }
        public List<ReportParameter> reportParameters { set; get; }
    }

    public class ReportParameter
    {
        public string name { set; get; }
        public object value { set; get; }
        public string subReport { set; get; }
    }
}
