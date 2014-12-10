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
        internal static string FacebookId = String.Empty;
        private static bool isConnected = false;
        public static bool IsAuthenticated = false;

        private static readonly IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        public static string AccessToken
        {
            get
            {
                try
                {
                    if (appSettings.Contains("facebook_accessToken"))
                    {
                        accessToken = (string)appSettings["facebook_accessToken"];
                    }
                    else
                    {
                        accessToken = "";
                    }
                    
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
                    if (appSettings.Contains("facebook_isOnline"))
                    {
                        isConnected = (bool)appSettings["facebook_isOnline"];
                    }
                    else
                    {
                        isConnected = false;
                    }
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
            List<Friend> Friends = new List<Friend>();

            FacebookClient fb = new FacebookClient(FacebookAccount.AccessToken);

            try
            {
                object fb_data = await fb.GetTaskAsync("fql",
                    new
                    {
                        q = string.Format("SELECT uid, name, birthday, pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=me())")
                        //q = string.Format("SELECT uid, name, birthday, pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=me()) AND strpos(birthday, '{0} {1}') >= 0", month, day)

                    });

                var result = fb_data as IDictionary<string, object>;
                var data = (IEnumerable<object>)result["data"];

                foreach (var item in data)
                {
                    try
                    {
                        string[] Birthdate = { "", "" };
                        string year = "";

                        var friend = (IDictionary<string, object>)item;
                        if (friend.ContainsKey("birthday"))
                        {
                        
                            var friend_birthday = friend["birthday"];
                            if (friend_birthday != null)
                            {
                                Birthdate = ((string)friend["birthday"]).Split(new char[] { ' ', ',' });
                                year = Birthdate.Length == 3 ? Birthdate[2] : "";    
                            }

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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
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
