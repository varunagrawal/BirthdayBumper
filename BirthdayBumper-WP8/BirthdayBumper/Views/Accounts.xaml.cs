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

            accounts = new List<string>() { "Facebook", "Google" };

            DataContext = accounts;
        }

        private void AccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check for Network Connectivity. If not available, then show message and exit.
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No Network Connectivity." + Environment.NewLine + "Please check if you are connected to the Internet.");

                NavigationService.GoBack();
            }

            if(AccountsList.SelectedItem == null)
                return;

            NavigationService.Navigate(new Uri("/Views/" + (string)AccountsList.SelectedItem + "LoginPage.xaml", UriKind.RelativeOrAbsolute));

        }
    }
}