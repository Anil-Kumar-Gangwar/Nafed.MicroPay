namespace Nafed.MicroPay.Services.API
{
    public class BaseService
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Dispose()
        {

        }
    }
}
