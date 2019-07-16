using System.ServiceProcess;

namespace NeeoFileCleanupService
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
                new NeeoFileCleanupSvc() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
