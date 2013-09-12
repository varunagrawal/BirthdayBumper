using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Linq;
using System.Net;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using System.IO.IsolatedStorage;

using Facebook;
using Facebook.Client;

namespace BirthdayBumper.Models
{
    class BBFacebook
    {
        private static BBFacebook instance;
        private static readonly IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        
        public static readonly string App_Id = "587441381302332";
        private string clientSecret = "";
        public const string ExtendedPermissions = "user_about_me,read_stream,friends_birthday";

        private static string accessToken = String.Empty;
        public static string FacebookId = String.Empty;
        public static bool isAuthenticated = false;
        public static FacebookSessionClient fbSessionClient = new FacebookSessionClient(App_Id);

        public BBFacebook()
        {
            try
            {
                accessToken = (string)appSettings["accessToken"];
            }
            catch (KeyNotFoundException e)
            {
                MessageBox.Show(e.Message);
                accessToken = "";            
            }
        }

        public static BBFacebook Instance
        {
            get
            {
                if (instance == null)
                    instance = new BBFacebook();
                return instance;
            }

            set
            {
                instance = value;
            }

        }

        public static string AccessToken
        {
            get 
            {
                return accessToken;
            }

            set
            {
                accessToken = value;
                if (accessToken.Equals(""))
                    appSettings.Remove("accessToken");
                else
                    appSettings.Add("accessToken", accessToken);
            }
        }


    }
}
