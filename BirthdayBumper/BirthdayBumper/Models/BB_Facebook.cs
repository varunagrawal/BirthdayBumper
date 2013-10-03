using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace BirthdayBumper.Models
{
    class BB_Facebook
    {
        public static readonly string App_Id = "587441381302332";
        public static readonly string AppSecret = "7a8df6147a0e50797ef5736d5cfd7391";
        public const string ExtendedPermissions = "user_about_me,read_stream,friends_birthday";

        public static string accessToken = String.Empty;
        public static bool isAuthenticated = false;

        private static readonly IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

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
                    var error = e.Message;
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
                {
                    if (!appSettings.Contains("accessToken"))
                    {
                        appSettings.Remove("accessToken");
                        appSettings.Add("accessToken", accessToken);
                    }
                }
                    
            }
        }
    }
}
