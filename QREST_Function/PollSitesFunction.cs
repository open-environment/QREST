using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using QRESTModel.DAL;

namespace QREST_Function
{
    public static class PollSitesFunction
    {
        [FunctionName("PollSitesFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            //1. query active sites and pull air monitoring data and insert into database


            int SuccInd = db_Ref.CreateT_QREST_SYS_LOG("test", "test", "test");
            log.Info(SuccInd.ToString());
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
