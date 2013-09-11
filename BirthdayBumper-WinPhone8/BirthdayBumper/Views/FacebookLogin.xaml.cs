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

using Facebook;
using Facebook.Client;
using BirthdayBumper.Models;

namespace BirthdayBumper.Views
{
    public partial class FacebookLogin : PhoneApplicationPage
    {
        private const string ExtendedPermissions = "user_about_me,read_stream,friends_birthday";
        private FacebookSession session;
       
        public FacebookLogin()
        {
            InitializeComponent();
            this.Loaded += FacebookLogin_Loaded;
        }

        async void FacebookLogin_Loaded(object sender, RoutedEventArgs e)
        {
            if (!BBFacebook.isAuthenticated)
            {
                BBFacebook.isAuthenticated = true;

                BBFacebook.AccessToken = GetSavedAccessToken();
                if (BBFacebook.AccessToken == null || String.IsNullOrEmpty(BBFacebook.AccessToken))
                {
                    await Authenticate();
                }
                
                NavigationService.Navigate(new Uri("/Views/Birthdays.xaml", UriKind.RelativeOrAbsolute));
            }
        }
        

        private async Task Authenticate()
        {
            string message = String.Empty;

            try
            {
                session = await BBFacebook.fbSessionClient.LoginAsync(ExtendedPermissions);
                
                BBFacebook.AccessToken = session.AccessToken;
                BBFacebook.FacebookId = session.FacebookId;

                if (SaveAccessToken(BBFacebook.AccessToken))
                {
                    MessageBox.Show("Successful Login");
                }

            }
            catch (Exception e)
            {
                message = "Login failed! Exception details: " + e.Message;
                MessageBox.Show(message);
            }
        }




        private string GetSavedAccessToken()
        {
            System.IO.IsolatedStorage.IsolatedStorageFile local =
                System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();

            if (!local.FileExists(@"DataFolder\AccessToken.txt"))
            {
                return null;
            }

            using (var isoFileStream =
                    new System.IO.IsolatedStorage.IsolatedStorageFileStream(
                        @"DataFolder\AccessToken.txt", System.IO.FileMode.Open, local))
            {
                using (var isoFileReader = new System.IO.StreamReader(isoFileStream))
                {
                    string line = isoFileReader.ReadLine();
                    string[] index = line.Split(':');

                    if (index[0] == "AccessToken" && !String.IsNullOrEmpty(index[1]))
                    {   
                        return index[1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }


        private bool SaveAccessToken(string accessToken)
        {
            try
            {
                System.IO.IsolatedStorage.IsolatedStorageFile local = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();

                if (!local.DirectoryExists("DataFolder"))
                {
                    local.CreateDirectory("DataFolder");
                }


                using (var isoFileStream = new System.IO.IsolatedStorage.IsolatedStorageFileStream(@"DataFolder\AccessToken.txt", System.IO.FileMode.OpenOrCreate, local))
                {
                    using (var isoFileWriter = new System.IO.StreamWriter(isoFileStream))
                    {
                        isoFileWriter.WriteLine("AccessToken:" + accessToken);
                    }
                }

                return true;

            }
            catch (Exception e)
            { 
                MessageBox.Show("Error: " + e.Message);
                return false;
            }
        }

    }
}