using System;
using System.Linq;
using System.Net;


namespace Nafed.MicroPay.Common
{
    public class SMSHelper : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
     (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Dispose()
        {
        }
        public static bool SendSMS(SMSParameter parameter)
        {
            log.Info($"SMSHelper/SendSMS");
            try
            {
                using (var web = new WebClient())
                {
                    //string url = parameter.URL +
                    //        "&dest_mobileno=" + parameter.MobileNo +
                    //        "&message=" + parameter.Message +
                    //        "&response=Y"
                    //        ;

                    string url = parameter.URL +
                           "&send_to=" + parameter.MobileNo +
                           "&msg=" + parameter.Message +
                           "&msg_type=TEXT" +
                           "&userid=" + parameter.User +
                           "&auth_scheme = plain" +
                           "&password=" + parameter.Password +
                           "&v=1.1&format=text";

                    string result = web.DownloadString(url);
                  // var smsresult = result.Split(new char[] { '-' }).ToArray();
                     var smsresult= result.Split(new char[] { '|' }).ToArray();

                  //  var smscount = int.Parse(smsresult[0]);
                    //if (smscount > 0)
                    //{
                    //    return true;
                    //}
                    if (smsresult[0].ToString().Trim() == "success")
                    {
                        return true;
                    }
                    else
                    {
                        log.Error($"Some issue in delivering-{result}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Email Error-" + ex.Message + ", StackTrace-" + ex.StackTrace + ", DateTimeStamp-" + DateTime.Now);
                return false;
            }
        }
    }

    public class SMSParameter
    {
        public string URL { get; set; }
        public string MobileNo { set; get; }
        public string Message { set; get; }
        public string User { get; set; }
        public string Password { get; set; }
        public string TemplateId { get; set; }
    }
}
