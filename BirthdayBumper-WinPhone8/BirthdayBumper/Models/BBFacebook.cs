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
        public static readonly string AppSecret = "7a8df6147a0e50797ef5736d5cfd7391";
        public const string ExtendedPermissions = "user_about_me,read_stream,friends_birthday";

        private static string accessToken = String.Empty;
        public static string FacebookId = String.Empty;
        public static bool isAuthenticated = false;
        public static FacebookSessionClient fbSessionClient = new FacebookSessionClient(App_Id);

        public BBFacebook()
        {
            
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
                try
                {
                    accessToken = (string)appSettings["accessToken"];
                }
                catch (KeyNotFoundException e)
                {
                    accessToken = "";
                }
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
