
using System;
using System.Web.Configuration;

namespace MicroPay.Web.JobScheduler
{
    public sealed class Scheduler
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static double TimerIntervalInMilliseconds =
            Convert.ToDouble(WebConfigurationManager.AppSettings["TimerIntervalInMilliseconds"]);

        System.Timers.Timer t = new System.Timers.Timer();
        public Scheduler() {

            t.Interval = Convert.ToDouble(TimerIntervalInMilliseconds);  //--- 50 min
            t.Enabled = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Start();

        }

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            log.Info($"t_Elapsed");            
            DateTime schedulerStartTime = DateTime.Parse(WebConfigurationManager.AppSettings["TimerStartTime"]);
            DateTime LatestRunTime = schedulerStartTime.AddMilliseconds(t.Interval);

            // Get the current system time
            DateTime CurrentSystemTime = DateTime.Now;
            log.Info($"Timer Event Handler Called: {CurrentSystemTime}");
            
            // If within the (Start Time) to (Start Time+Interval) time frame - run the processes
            if ((CurrentSystemTime.CompareTo(schedulerStartTime) >= 0) && (CurrentSystemTime.CompareTo(LatestRunTime) <= 0))
            {
                //  t.Stop();
               //  t.Enabled = false; t.Dispose();
                // RUN YOUR PROCESSES HERE
                  Jobs.GreetingJob(sender);
            }
        }

      
    }
}