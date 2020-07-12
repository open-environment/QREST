using System.ServiceProcess;

namespace QREST_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new QRESTService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
