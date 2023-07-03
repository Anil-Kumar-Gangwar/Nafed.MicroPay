using System.Web.Mvc;
using System.Web.Http;

namespace MicroPay.Web.Controllers.API
{ 
    public class BaseController : ApiController
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
    }
}