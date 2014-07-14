using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Facebook;

using BirthdayBumper.ViewModels;
using System.IO.IsolatedStorage;
using Facebook.Client;
using System.Threading.Tasks;

namespace BirthdayBumper.Views
{
    public partial class FacebookLoginPage : PhoneApplicationPage
    {
        private readonly FacebookClient _fb = new FacebookClient();

        public FacebookLoginPage()
        {
            InitializeComponent();
            this.Loaded += FacebookLoginPage_Loaded;
        }

        private async void FacebookLoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(!FacebookAccount.isAuthenticated)
            {
                await Authenticate();
            }
        }

        private async Task Authenticate()
        {
            ShowProgressBar(true);

            FacebookSession session;
            FacebookSessionClient fbclient = new FacebookSessionClient(FacebookAccount.App_Id);

            try
            {
                session = await fbclient.LoginAsync(FacebookAccount.ExtendedPermissions);

                FacebookAccount.AccessToken = session.AccessToken;
                FacebookAccount.FacebookId = session.FacebookId;

                FacebookAccount.IsConnected = true;
                FacebookAccount.isAuthenticated = true;

                NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));
            }
            catch (InvalidOperationException ioe)
            {
                MessageBox.Show("Login Failed! Exception: " + ioe.Message);
            }
            finally
            {
                ShowProgressBar(false);
            }
        }

        private void ShowProgressBar(bool set)
        {
            if (set)
            {
                SystemTray.ProgressIndicator = new ProgressIndicator();
            }
            
            if (SystemTray.ProgressIndicator != null)
            {
                SystemTray.ProgressIndicator.IsVisible = set;
                SystemTray.ProgressIndicator.IsIndeterminate = set;
            }
        }

    }
}