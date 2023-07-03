using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MicroPay.Web.Startup))]
namespace MicroPay.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           // ConfigureAuth(app);
        }
    }
}
