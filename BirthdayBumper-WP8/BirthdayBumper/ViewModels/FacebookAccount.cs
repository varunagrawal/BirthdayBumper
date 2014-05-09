using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using BirthdayBumper.Models;
using Facebook;
using System.Windows;

namespace BirthdayBumper.ViewModels
{
    class FacebookAccount
    {
        public static readonly string App_Id = "587441381302332";
        public static readonly string AppSecret = "7a8df6147a0e50797ef5736d5cfd7391";
        public const string ExtendedPermissions = "user_about_me,read_stream,friends_birthday";

        private static string accessToken = String.Empty;
        private static bool isConnected = false;
        public static bool isAuthenticated = false;

        private static readonly IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        public static string AccessToken
        {
            get
            {
                try
                {
                    accessToken = (string)appSettings["facebook_accessToken"];
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
                    appSettings.Remove("facebook_accessToken");
                else
                {
                    if (!appSettings.Contains("facebook_accessToken"))
                    {
                        appSettings.Remove("facebook_accessToken");
                        appSettings.Add("facebook_accessToken", accessToken);
                    }
                    else
                    {
                        appSettings["facebool_accessToken"] = accessToken;
                    }
                }
            }
        }

        public static bool IsConnected
        {
            get
            {
                try
                {
                    isConnected = (bool)appSettings["facebook_isOnline"];
                }
                catch (KeyNotFoundException e)
                {
                    var error = e.Message;
                    isConnected = false;
                }
                return isConnected;
            }

            set
            {
                isConnected = value;

                if (!appSettings.Contains("facebook_isOnline"))
                {
                    appSettings.Add("facebook_isOnline", isConnected);
                }
                else
                {
                    appSettings["facebook_isOnline"] = isConnected;
                }

            }
        }


        /// <summary>
        /// Get Birthdays of Facebook Friends
        /// </summary>
        public static async Task<List<Friend>> GetFacebookBirthdays()
        {
            FacebookClient fb = new FacebookClient(FacebookAccount.AccessToken);

            List<Friend> Friends = new List<Friend>();

            string month = DateTime.Now.ToString("MMMM");
            int day = DateTime.Now.Day;

            try
            {
                object fb_data = await fb.GetTaskAsync("fql",
                    new
                    {
                        q = string.Format("SELECT uid, name, birthday, pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=me()) AND strpos(birthday, '{0} {1}') >= 0", month, day)
                        //q = string.Format("SELECT uid, name, birthday, pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=me()) >= 0")
                    });

                var result = fb_data as IDictionary<string, object>;
                var data = (IEnumerable<object>)result["data"];

                foreach (var item in data)
                {
                    var friend = (IDictionary<string, object>)item;
                    if (friend.ContainsKey("birthday"))
                    {
                        string[] Birthdate = ((string)friend["birthday"]).Split(new char[] { ' ', ',' });
                        string m = DateTime.Now.ToString("MMMM");
                        string d = DateTime.Now.Day.ToString();
                        string year = Birthdate.Length == 3 ? Birthdate[2] : "";

                        if (Birthdate[0] == m && Birthdate[1] == d)
                        {
                            Friends.Add(new FacebookFriend
                            (
                                friend["uid"].ToString(),
                                (string)friend["name"],
                                Birthdate[1],
                                Birthdate[0],
                                year,
                                new Uri((string)friend["pic_square"], UriKind.RelativeOrAbsolute),
                                "https://www.facebook.com/" + friend["uid"].ToString()
                            ));
                        }

                    }

                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return Friends;
        }
    }
}
