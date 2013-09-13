using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;

using Facebook;
using Facebook.Client;
using BirthdayBumper.Models;

namespace BirthdayBumper.Views
{
    public partial class FacebookLogin : PhoneApplicationPage
    {
        
        private readonly FacebookClient _fb = new FacebookClient();
        private Dictionary<string, object> facebookData = new Dictionary<string, object>();

        public FacebookLogin()
        {
            InitializeComponent();
            this.Loaded += FacebookLogin_Loaded;
        }

        void FacebookLogin_Loaded(object sender, RoutedEventArgs e)
        {
            //if (!BBFacebook.isAuthenticated)
            //{
            //    BBFacebook.isAuthenticated = true;
            //    await Authenticate();
            //}

            if (String.IsNullOrEmpty(BBFacebook.AccessToken))
            {
                var loginUrl = GetFacebookLoginUrl(BBFacebook.App_Id, BBFacebook.ExtendedPermissions);
                FBLogin.Navigate(loginUrl);
            }
            else 
            {
                NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));       
            }
            
        }

        private Uri GetFacebookLoginUrl(string appId, string extendedPermissions)
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = appId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "touch";

            // add the 'scope' only if we have extendedPermissions.
            if (!string.IsNullOrEmpty(extendedPermissions))
            {
                // A comma-delimited list of permissions
                parameters["scope"] = extendedPermissions;
            }

            return _fb.GetLoginUrl(parameters);
        }
        

        
        private void FBLogin_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;
            if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            if (oauthResult.IsSuccess)
            {
                BBFacebook.AccessToken = oauthResult.AccessToken;
                NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                // user cancelled
                MessageBox.Show(oauthResult.ErrorDescription);
            }
        }

        
        

        void GetAccessToken()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (string.IsNullOrEmpty(BBFacebook.AccessToken))
                {
                    MessageBox.Show("AccessToken not valid");
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));       
                }
            });
        }


        
        private FacebookSession session;
        private async Task Authenticate()
        {
            string message = String.Empty;

            try
            {
                session = await BBFacebook.fbSessionClient.LoginAsync(BBFacebook.ExtendedPermissions);

                BBFacebook.AccessToken = session.AccessToken;
                BBFacebook.FacebookId = session.FacebookId;

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute)));

            }
            catch (Exception e)
            {
                message = "Login failed! Exception details: " + e.Message;
                MessageBox.Show(message);
            }
        }

    }

}