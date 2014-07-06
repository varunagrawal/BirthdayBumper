using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Net.NetworkInformation;

namespace BirthdayBumper.Views
{
    public partial class Accounts : PhoneApplicationPage
    {
        public static List<string> accounts;

        public Accounts()
        {
            InitializeComponent();

            // Check for Network Connectivity. If not available, then show message and exit.
            //if (!NetworkInterface.GetIsNetworkAvailable())
            //{
            //    MessageBox.Show("No Network Connectivity." + Environment.NewLine + "Please check if you are connected to the Internet.");
                
            //    NavigationService.RemoveBackEntry();
            //    NavigationService.GoBack();
            //}

            accounts = new List<string>() { "Facebook", "Google" };

            DataContext = accounts;
        }

        private void AccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(AccountsList.SelectedItem == null)
                return;

            if((string)AccountsList.SelectedItem == "Google")
            {
                NavigationService.Navigate(new Uri("/Views/GoogleLoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else if ((string)AccountsList.SelectedItem == "Facebook")
            {
                NavigationService.Navigate(new Uri("/Views/FacebookLoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}