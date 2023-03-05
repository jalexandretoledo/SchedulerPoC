using Akka.Actor;

namespace SchedulerPoC.GUI
{
    internal static class Program
    {

        public static SchedActorSystem SchedulerActorSystem;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SchedulerActorSystem = new SchedActorSystem();


            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}