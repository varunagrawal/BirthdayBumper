using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;

using Facebook;
using BirthdayBumper.Models;
using BirthdayBumper.ViewModels;

namespace BirthdayBumper.Views
{
    public partial class Birthdays : PhoneApplicationPage
    {
        bool BirthdaysLoaded = false;
        FriendDataModel FriendData;

        public Birthdays()
        {
            InitializeComponent();
        }


        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while(NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            if (!BirthdaysLoaded)
            {
                BirthdaysLoaded = true;
                GetFriendsBirthdays();

            }
            
        }

        private void GetFriendsBirthdays()
        {
            //FacebookFriendsBirthdays();
            FriendData.FacebookBirthdays();

            Load_Notify.Visibility = System.Windows.Visibility.Collapsed;

            BirthdaysList.DataContext = FriendData.Friends;

            if (FriendData.Friends.Count <= 0)
                MessageBox.Show("No Birthdays Today");

        }


        private void ContactsFriendsBirthdays()
        { 
            
        }

        private void FacebookFriendsBirthdays()
        {
            var fb = new FacebookClient(BBFacebook.AccessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                var data = (IEnumerable<object>)result["data"];


                Dispatcher.BeginInvoke(() =>
                {
                    FriendData = new FriendDataModel();

                    foreach (var item in data)
                    {
                        var friend = (IDictionary<string, object>)item;
                        if (friend.ContainsKey("birthday"))
                        {
                            string[] Birthdate = ((string)friend["birthday"]).Split(' ');

                            FriendData.Friends.Add(new FacebookFriend
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
                });

        }



        // Handle selection changed on LongListSelector
        private void BirthdayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (BirthdaysList.SelectedItem == null)
                return;

            FacebookFriend f = BirthdaysList.SelectedItem as FacebookFriend;

            // Reset selected item to null (no selection)
            BirthdaysList.SelectedItem = null;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/Views/WishFriend.xaml?selectedItem=" + f.Id, UriKind.RelativeOrAbsolute));


            /*
            if (!f.Wished)
            {
                f.Wished = true;

                
            }
            else
            {
                MessageBox.Show("You already wished " + f.Name);
            }
            */
        }


    }
}