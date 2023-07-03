using System.Collections.Generic;

namespace Nafed.MicroPay.Common
{
    public class ImportValidationMessage
    {
        //private ImportValidationMessage()
        //{
        //}
        public static Dictionary<string, string> ImportErrMsg()
        {
            Dictionary<string, string> dicErrMessage = new Dictionary<string, string>();

            dicErrMessage.Add("HDR", "'{0}' Header(s) are missing in source sheet, please correct the source sheet and upload again.");
            dicErrMessage.Add("NF", "{0} Invalid.");
           
            dicErrMessage.Add("B", "{0} should not be blank.");
            dicErrMessage.Add("I", "Should be numeric.");
            dicErrMessage.Add("DT", "Should be DateTime.");
            dicErrMessage.Add("T", "Should be Text.");
            dicErrMessage.Add("BO", "Should be boolean.");
            dicErrMessage.Add("BOA", "Should be YES/NO or TRUE/FALSE or Y/N");
            dicErrMessage.Add("Z", "{0} can not be zero.");
            dicErrMessage.Add("FD", "Should be future Date.");
            dicErrMessage.Add("EV", "Should be less than 53.");
            dicErrMessage.Add("NDF", "Invalid Date Format.");
            dicErrMessage.Add("NTF", "Invalid Time Format.");
            dicErrMessage.Add("DI", "Found duplicate.");

          
            dicErrMessage.Add("SZ", "Should be Zero.");
            dicErrMessage.Add("SGZ", "Should be greater then Zero.");
          
            dicErrMessage.Add("OTDP", "Should be positive number up to two decimal places.");
            
            dicErrMessage.Add("NOREC", "No record found.");
            dicErrMessage.Add("NBPODNAME", "{0} can not be blank.");
            dicErrMessage.Add("BPODNAME", "{0} should be blank.");
           
          
            dicErrMessage.Add("UIIS", "{0} Imported item(s)  updated successfully");
            dicErrMessage.Add("UIIU", "Item  update failed.");
            dicErrMessage.Add("PI", "Should be postive integer and greater than zero ");
            dicErrMessage.Add("PN", "Should be postive number and greater than zero");
            dicErrMessage.Add("PD", "The date in this cell cannot be in the past");
            dicErrMessage.Add("OPD", "Invalid date,Only one day back date attendance is allowed.");
            dicErrMessage.Add("IP", "Should be postive integer");
           
            dicErrMessage.Add("PRPI", "Should be postive number");
         
            dicErrMessage.Add("DUPE", "Found duplicate.");
            dicErrMessage.Add("MAXLEN", "Value should not be more than 200 characters including spaces.");
            dicErrMessage.Add("IIS", "{0} Item(s) imported successfully.");
            dicErrMessage.Add("ITLTT", "InTime should not be greater than OutTime.");
            dicErrMessage.Add("OTLTT", "OutTime should not be less than InTime.");
            dicErrMessage.Add("IVDF", "Date should be in dd/MM/yyyy format.");
            dicErrMessage.Add("FDNA", "Invalid date,Future date attendance is not allowed.");
            dicErrMessage.Add("WKND", "Invalid date,Weekend attendance is not allowed.");
            return dicErrMessage;

        }
    }
}
