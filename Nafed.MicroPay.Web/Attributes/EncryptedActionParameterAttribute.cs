using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MicroPay.Web
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            if (HttpContext.Current.Request.QueryString.Get("q") != null)

            {
                string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("q");
                string decrptedString = Decrypt(encryptedQueryString.ToString());
                string[] paramsArrs = decrptedString.Split('?');

                for (int i = 0; i < paramsArrs.Length; i++)
                {
                    string[] paramArr = paramsArrs[i].Split('=');

                    if (paramArr[1].All(char.IsDigit))
                    {
                        if (paramArr[1].Length > 1 && paramArr[1].First().ToString() == "0")
                            decryptedParameters.Add(paramArr[0], Convert.ToString(paramArr[1]));
                        else
                        {
                            if(paramArr[0]== "userName")
                            {
                                decryptedParameters.Add(paramArr[0], Convert.ToString(paramArr[1]));
                            }
                            else { decryptedParameters.Add(paramArr[0], paramArr[1] == "" ? (int?)null : Convert.ToInt32(paramArr[1])); }

                        }
                           
                    }

                    else if (Convert.ToString(paramArr[1]).ToUpper() == "TRUE" || Convert.ToString(paramArr[1]).ToUpper() == "FALSE")
                        decryptedParameters.Add(paramArr[0], paramArr[1] == "" ? (bool?)null : Convert.ToBoolean(paramArr[1]));
                    else
                        decryptedParameters.Add(paramArr[0], Convert.ToString(paramArr[1]));
                }
            }
            
            for (int i = 0; i < decryptedParameters.Count; i++)
            {
                filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
            }
            base.OnActionExecuting(filterContext);

        }

        private string Decrypt(string encryptedText)
        {
            string key = "@m2qJzeyjXLBK!axPV$Bvg3QUP";
            byte[] DecryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            byte[] inputByte = new byte[encryptedText.Length];

            DecryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByte = Convert.FromBase64String(encryptedText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByte, 0, inputByte.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

    }
}
