using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MicroPay.Web.JobScheduler;
using System.Configuration;
using System.Threading;
using System.Web.Configuration;
using System.Web.SessionState;
using System.Text;
using System.Security.Cryptography;

namespace MicroPay.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Timer _timer;
        long TimerIntervalInMilliseconds;
        //  Scheduler objmyScheduler = new Scheduler();
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            log.Info($"Application_Start Called : {DateTime.Now}");

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();
            log4net.Config.XmlConfigurator.Configure();

            ///// Code that runs on application startup //========

            TimerIntervalInMilliseconds = Convert.ToInt32(ConfigurationManager.AppSettings["TimerIntervalInMilliseconds"]);

            _timer = new Timer(new TimerCallback(timerSchedulerElapsed),
                null, Timeout.Infinite, Timeout.Infinite);
            _timer.Change(0, TimerIntervalInMilliseconds);

            //  Scheduler objmyScheduler = new Scheduler();
            //objmyScheduler.Scheduler_Start();

            //#region ====== Register & Configure Job Scheduler ========

            //if (DateTime.Now.TimeOfDay.Equals(new TimeSpan(11,15, 0)))
            //{
            //    log.Info($"Greeting job scheduler started at :{DateTime.Now}");
            //    GreetingJobService gJob = new GreetingJobService();
            //    gJob.ExecuteJob();
            //}
            //#endregion
                       

        }

        protected void Application_BeginRequest()
        {            
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();            
        }

        private void timerSchedulerElapsed(object state)
        {
            DateTime schedulerStartTime = DateTime.Parse(WebConfigurationManager.AppSettings["TimerStartTime"]);
            TimerIntervalInMilliseconds = Convert.ToInt32(ConfigurationManager.AppSettings["TimerIntervalInMilliseconds"]);//

            DateTime LatestRunTime = schedulerStartTime.AddMilliseconds(TimerIntervalInMilliseconds);

            // Get the current system time
            DateTime CurrentSystemTime = DateTime.Now;
            log.Info($"Timer Event Handler Called: {CurrentSystemTime}");

            // If within the (Start Time) to (Start Time+Interval) time frame - run the processes
            if ((CurrentSystemTime.CompareTo(schedulerStartTime) >= 0) && (CurrentSystemTime.CompareTo(LatestRunTime) <= 0))
            {
                //   t.Stop();
                //  t.Enabled = false; t.Dispose();
                // RUN YOUR PROCESSES HERE
                Jobs.GreetingJob(state);
            }
        }

        protected void Application_EndRequest()
        {
            
        }
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }       
        
    }
}
