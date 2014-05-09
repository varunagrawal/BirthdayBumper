using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;

using Google.Apis.Auth.OAuth2;

using BirthdayBumper.ViewModels;
using System.ComponentModel;

namespace BirthdayBumper.Views
{
    public partial class GoogleLoginPage : PhoneApplicationPage
    {
        public GoogleLoginPage()
        {
            InitializeComponent();
            this.Loaded += GoogleLoginPage_Loaded;
        }

        void GoogleLoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!GoogleAccount.IsConnected)
            {
                ShowProgressBar(true);

                // code to redirect to Google OAuth2 URL
                var oauthUrl = GetOauthUrl();
                GLoginBrowser.Navigate(oauthUrl);

            }
            else 
            {
                NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));
            }
            
        }

        private Uri GetOauthUrl()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("scope", GoogleAccount.Scope);
            dict.Add("redirect_uri", GoogleAccount.RedirectUri);
            dict.Add("response_type", "code");
            dict.Add("client_id", GoogleAccount.ClientId);
            
            return new Uri(GoogleAccount.AuthUri + "?" + string.Join("&", dict.Select(item => item.Key + "=" + item.Value).ToArray()));
        }

        private async void GLoginBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            ShowProgressBar(true);

            string token = e.Uri.Query;
            if (e.Uri.Query.Contains("xsrf"))
            {

                string source = GLoginBrowser.SaveToString();
                int start = source.IndexOf("<title>") + 20;
                int end = source.IndexOf("</title>");
                string authCode = source.Substring(start, end - start);

                bool completed = await GoogleAccount.Authorize(authCode);

                while (!completed) ;

                ShowProgressBar(false);

                NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));
                    
            }
            
        }

        private void GLoginBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ShowProgressBar(false);
        }

        private void ShowProgressBar(bool set)
        {
            if (set)
            {
                SystemTray.ProgressIndicator = new ProgressIndicator();
            }
            SystemTray.ProgressIndicator.IsVisible = set;
            SystemTray.ProgressIndicator.IsIndeterminate = set;

        }

    }
}