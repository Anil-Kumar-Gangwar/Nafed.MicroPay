using Nafed.MicroPay.ImportExport.Interfaces;

namespace Nafed.MicroPay.ImportExport
{
   abstract public class Base : IBase
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Dispose()
        {
           
        }
    }
}
