using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Nafed.MicroPay.Common
{
    public class ConfigManager
    {
        private ConfigManager()
        {

        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Method to get appsetting value using key name 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Value(string key)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                return value;
            }
            catch
            {
                log.Error($"Config error : config value {key} not found.");

                return string.Empty;
            }
        }
        /// <summary>
        /// Get Connection string 
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["MicroPayEntities"].ToString();
        }
    }
}
