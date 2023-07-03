using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Common
{
    public static class DocumentUploadFilePath
    {
        public static string PanCardFilePath { get { return "Images/PANNO"; } }
        public static string AadhaarCardFilePath { get { return "Images/Aadhaar"; } }
        public static string ProfilePicFilePath { get { return "Images/Profile"; } }
        public static string ImportFilePath { get { return "~/FileUpload/"; } }
        public static string DocumentFilePath { get { return "~/Document"; } }

        public static string TCSFilePath { get { return "Document/MonthlyTCSFile"; } }


        public static string EmployeeAchievement
        {
            get
            {
                return "Document/EmployeeAchievement";
            }
        }
        public static string EmployeeCertification
        {
            get
            {
                return "Document/EmployeeCertification";
            }
        }

        public static string PassportFilePath { get { return "Images/PASSPORTNO"; } }

        public static string BankAccFilePath { get { return "Images/BANKACCNO"; } }

        #region Candidate Photo And Signature
        public static string CandidatePhoto { get { return "Images/CandidatePhoto"; } }
        public static string CandidateSignature { get { return "Images/CandidateSignature"; } }

        public static string CandidateAdmitCardPhoto { get { return "~/Images/CandidatePhoto/"; } }
        public static string CandidateAdmitCardSignature { get { return "~/Images/CandidateSignature/"; } }

        public static string AdmitCardUNCPath { get { return "~/Document/AdmitCard/"; } }


        #endregion

        public static string MonthlyImportFilePath { get { return "~/FileUpload/"; } }

        public static string TCSFileImportFilePath { get { return "~/FileUpload/"; } }

        public static string ArchiveDataPath { get { return @"D:\Destination Document"; } }

    }
}
