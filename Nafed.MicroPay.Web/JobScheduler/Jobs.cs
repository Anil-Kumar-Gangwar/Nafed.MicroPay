using Nafed.MicroPay.Services.JobScheduler;

namespace MicroPay.Web.JobScheduler
{
    public sealed class Jobs
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger
       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Jobs() { }

        public static void GreetingJob(object state)
        {
            log.Info($"Jobs/GreetingJob: Called,");

            GreetingJobService jobService = new GreetingJobService();
            jobService.ExecuteJob();

        }
    }
}