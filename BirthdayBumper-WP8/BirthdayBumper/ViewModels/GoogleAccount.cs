using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Http;

using Google.Contacts;
using Google.GData.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BirthdayBumper.Models;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace BirthdayBumper.ViewModels
{
    class GoogleAccount
    {
        private static readonly IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        #region OAuth Params
        public static string AuthUri = "https://accounts.google.com/o/oauth2/auth";
        public static string ClientId = "737182336924-ov8miiljhdaoce3h8pinjfudfvd0503r.apps.googleusercontent.com";
        public static string ClientSecret = "KXiEw8h4MsuBB1GSxYnc4geM";
        public static string TokenUri = "https://accounts.google.com/o/oauth2/token";
        public static string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
        public static string Auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs";
        public static string Scope = "https://www.google.com/m8/feeds";
        #endregion

        private static string accessToken;
        public static string AccessToken
        {
            get
            {
                try
                {
                    accessToken = (string)appSettings["google_accessToken"];
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
                    appSettings.Remove("google_accessToken");
                else
                {
                    if (!appSettings.Contains("google_accessToken"))
                    {
                        appSettings.Remove("google_accessToken");
                        appSettings.Add("google_accessToken", accessToken);
                    }
                    else 
                    {
                        appSettings["google_accessToken"] = accessToken;
                    }
                }
            }
        }

        private static string refreshToken;
        public static string RefreshToken
        {
            get
            {
                try
                {
                    refreshToken = (string)appSettings["google_refreshToken"];
                }
                catch (KeyNotFoundException e)
                {
                    var error = e.Message;
                    refreshToken = "";
                }
                return refreshToken;
            }

            set
            {
                refreshToken = value;
                if (refreshToken.Equals(""))
                    appSettings.Remove("google_refreshToken");
                else
                {
                    if (!appSettings.Contains("google_refreshToken"))
                    {
                        appSettings.Remove("google_refreshToken");
                        appSettings.Add("google_refreshToken", refreshToken);
                    }
                    else 
                    {
                        appSettings["google_refreshToken"] = refreshToken;
                    }
                }
            }
        }

        private static int expiry;
        public static int Expiry
        {
            get
            {
                try
                {
                    expiry = (int)appSettings["google_expiry"];
                }
                catch (KeyNotFoundException e)
                {
                    var error = e.Message;
                    expiry = -1;
                }
                return expiry;
            }

            set
            {
                expiry = value;
                if (!appSettings.Contains("google_expiry"))
                {
                    appSettings.Add("google_expiry", expiry);
                }
                else
                {
                    appSettings["google_expiry"] = expiry;
                }
            }
        }

        private static bool isConnected;
        public static bool IsConnected
        {
            get
            {
                try
                {
                    if (appSettings.Contains("google_isOnline"))
                    {
                        isConnected = (bool)appSettings["google_isOnline"];
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

                if (!appSettings.Contains("google_isOnline"))
                {
                    appSettings.Add("google_isOnline", isConnected);
                }
                else
                {
                    appSettings["google_isOnline"] = isConnected;
                }
            }
        }


        public static async Task<bool> Authorize(string AuthCode)
        {
            IsConnected = false;

            if (!string.IsNullOrEmpty(AccessToken))
            {
                RefreshAccessToken();

                return IsConnected;
            }

            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", AuthCode),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            };
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(TokenUri, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            SetAuthorization(responseString, false);

            return IsConnected;
        }

        public static async void RefreshAccessToken()
        {
            IsConnected = false;

            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("refresh_token", RefreshToken),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            };

            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(TokenUri, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                SetAuthorization(responseString, true);
            }
            catch (Exception)
            {
            }
        }

        public static void SetAuthorization(string tokenString, bool refresh)
        {
            string json = tokenString.Replace("\n", "").Replace("{", "").Replace("}", "").Replace("\"", "");
            List<string> list = json.Split(new char[] { ',' }).ToList<String>();

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string kvp in list)
            {
                string[] pair = kvp.Split(new char[] { ':' });
                dict.Add(pair[0].Trim(), pair[1].Trim());
            }

            AccessToken = dict["access_token"];
            
            if(!refresh)
                RefreshToken = dict["refresh_token"];

            Expiry = int.Parse(dict["expires_in"]);
            IsConnected = true;
        }

        /// <summary>
        /// Get Birthdays of Google Contacts
        /// </summary>
        public static async Task<List<Friend>> GetGoogleBirthdays()
        {
            List<Friend> Friends = new List<Friend>();

            RefreshAccessToken();

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri("https://www.google.com/m8/feeds/contacts/default/full?access_token=" + GoogleAccount.AccessToken + "&v=3.0&max-results=10000"));
            response.EnsureSuccessStatusCode();
            var gContacts = await response.Content.ReadAsStringAsync();

            //Extract XML and put in GoogleFriend objects.
            Feed feed;
            using (TextReader reader = new StringReader(gContacts))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Feed));

                feed = (Feed)ser.Deserialize(reader);
            }

            //feed.Entries = feed.Entries.Where(x => x.Name != null && x.Name.FullName != null && x.Name.FullName.ToLower().Contains("birthday")).ToList<Entry>();
            feed.Entries = feed.Entries.Where(x => x.Birthday != null).ToList<Entry>();

            foreach (Entry e in feed.Entries)
            {    
                if (e.Birthday != null && !string.IsNullOrEmpty(e.Birthday.Date))
                {
                    string[] birthday = e.Birthday.Date.Split(new char[] { '-' });

                    string day = birthday[birthday.Length - 1];
                    string month = birthday[birthday.Length - 2];
                    
                    string year = "";
                    if (birthday.Length == 3)
                        year = (birthday[0]);

                    string image = e.Links.Where(x => x.Type.Contains("image")).First<Link>().Image + "&access_token=" + GoogleAccount.AccessToken;

                    string site = "";
                    if (e.Websites != null && e.Websites.Count > 0)
                        site = e.Websites.First().Url;

                    Friends.Add(new GoogleFriend
                        (
                            e.Id,
                            e.Name.FullName,
                            day.ToString(),
                            month.ToString(),
                            year,
                            new Uri(image, UriKind.RelativeOrAbsolute),
                            site
                        ));

                }
            }

            return Friends;
        }
    }
}
