using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BirthdayBumper.Resources;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Net.NetworkInformation;

using BirthdayBumper.ViewModels;

namespace BirthdayBumper
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += MainPage_Loaded;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            // Setup Toast Notifications for the App
            //ToastNotificationSetup();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {   
            // Navigate to the Birthdays Page
            if (!GoogleAccount.IsConnected && !FacebookAccount.IsConnected)
            {
                NavigationService.Navigate(new Uri("/Views/Accounts.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));
            }
        }



        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        
        private void ToastNotificationSetup()
        {
            PeriodicTask periodicTask;
            string periodicTaskName = "PeriodicAgent";
        
            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);
            
            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "BirthdayBumper Wish reminding periodic task.";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);

                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
                #if(DEBUG_AGENT)
                    ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
                #endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }

            }
            catch (SchedulerServiceException)
            {
                // No user action required.  
            }
        }


        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }
    }
}