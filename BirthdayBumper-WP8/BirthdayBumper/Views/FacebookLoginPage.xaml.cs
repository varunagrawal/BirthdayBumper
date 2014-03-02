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

using BirthdayBumper.Models;

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

        private void FacebookLoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(BBFacebook.accessToken))
            {    
                var loginUrl = GetFacebookLoginUrl(BBFacebook.App_Id, BBFacebook.ExtendedPermissions);
                FBLoginBrowser.Navigate(loginUrl);
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

            string loginUri = _fb.GetLoginUrl(parameters).ToString().Replace("www.facebook.com", "m.facebook.com");

            return new Uri(loginUri);
        }



        private void FBLoginBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;
            if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            if (oauthResult.IsSuccess)
            {
                var token = oauthResult.AccessToken;
                BBFacebook.AccessToken = oauthResult.AccessToken;
                NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                // user cancelled
                MessageBox.Show(oauthResult.ErrorDescription);
            }
        }
    }
}