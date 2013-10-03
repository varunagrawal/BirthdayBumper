//#define DEBUG_AGENT

using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace BirthdayReminder
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
            string toastMessage = "Wish Your Friends On Their Birthday!";

            ShellToast toast = new ShellToast();
            toast.Title = "Birthday Bumper";
            toast.Content = toastMessage;

            DateTime d = DateTime.Now;
            DateTime checkPoint = new DateTime(d.Year, d.Month, d.Day, 6, 0, 0);

            if(DateTime.Compare(d, checkPoint) > 0)
                toast.Show();

            #if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(task.Name,TimeSpan.FromSeconds(60));
            #endif

            NotifyComplete();
        }
    }
}