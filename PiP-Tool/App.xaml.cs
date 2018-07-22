using System;
using PiP_Tool.Shared;

namespace PiP_Tool
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledException;
        }

        /// <summary>
        /// Catch unhandled exception
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event arguments</param>
        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Logger.Instance.Fatal("UnhandledException caught : " + ex.Message);
            Logger.Instance.Fatal("UnhandledException StackTrace : " + ex.StackTrace);
            Logger.Instance.Fatal("Runtime terminating : " + e.IsTerminating);
        }

    }
}
