using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Phone.UserData;
using System.Windows.Media.Imaging;
using Facebook;

using BirthdayBumper.Models;

namespace BirthdayBumper.ViewModels
{
    class FriendDataModel
    {
        public ObservableCollection<Friend> Friends = new ObservableCollection<Friend>();


        public void FacebookBirthdays()
        {
            var fb = new FacebookClient(BBFacebook.AccessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    throw e.Error;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                var data = (IEnumerable<object>)result["data"];

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {

                    foreach (var item in data)
                    {
                        var friend = (IDictionary<string, object>)item;
                        if (friend.ContainsKey("birthday"))
                        {                            
                            string[] Birthdate = ((string)friend["birthday"]).Split(new char[] { ' ', ',' });
                            string m = DateTime.Now.ToString("MMMM");
                            string d = DateTime.Now.Day.ToString();

                            Friends.Add(new FacebookFriend
                            (
                                friend["uid"].ToString(),
                                (string)friend["name"],
                                Birthdate[1],
                                Birthdate[0],
                                new Uri((string)friend["pic_square"], UriKind.RelativeOrAbsolute)
                            ));

                        }

                    }


                });

            };

            string month = DateTime.Now.ToString("MMMM");
            int day = DateTime.Now.Day;

            fb.GetTaskAsync("fql",
                new
                {
                    q = string.Format("SELECT uid, name, birthday, pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=me()) AND strpos(birthday, '{0} {1}') >= 0", month, day)
                    //q = string.Format("SELECT uid, name, birthday, pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=me()) >= 0")
                });

        }



        public void GetContacts()
        {
            Contacts cons = new Contacts();

            cons.SearchCompleted += cons_SearchCompleted;

            cons.SearchAsync(String.Empty, FilterKind.None , "Contacts Sync");
        }

        void cons_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            foreach (var c in e.Results)
            {
                DateTime d = c.Birthdays.First();

                if (d.Date.Equals(DateTime.Today.Date))
                {
                    BitmapImage img = new BitmapImage();
                    img.SetSource(c.GetPicture());

                    ContactFriend f = new ContactFriend(
                        //c.GetHashCode().ToString(),
                        c.PhoneNumbers.First().ToString(),
                        c.CompleteName.ToString(),
                        d.Day.ToString(),
                        d.Month.ToString(),
                        d.Year.ToString(),
                        img);

                    Friends.Add(f);
                }
                
            }

        }

    }
}
