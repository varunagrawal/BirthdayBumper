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
using BirthdayBumper.ViewModels;

namespace BirthdayBumper.Views
{
    public partial class Birthdays : PhoneApplicationPage
    {
        public Birthdays()
        {
            InitializeComponent();
        }


        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while(NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
            
            Load_Notify.Visibility = System.Windows.Visibility.Visible;
            getFriendsBirthdays();
        }


        private void getFriendsBirthdays()
        {
            var fb = new FacebookClient(BB_Facebook.AccessToken);

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
                    var FBData = new FacebookDataModel();

                    foreach (var item in data)
                    {
                        var friend = (IDictionary<string, object>)item;
                        if (friend.ContainsKey("birthday"))
                        {
                            string[] Birthdate = ((string)friend["birthday"]).Split(' ');

                            FBData.Friends.Add(new FB_Friend
                            (
                                friend["uid"].ToString(),
                                (string)friend["name"],
                                Birthdate[1],
                                Birthdate[0],
                                new Uri((string)friend["pic_square"], UriKind.RelativeOrAbsolute)
                            ));

                        }

                    }

                    Load_Notify.Visibility = System.Windows.Visibility.Collapsed;

                    BirthdaysList.DataContext = FBData.Friends;

                    if (FBData.Friends.Count <= 0)
                        MessageBox.Show("No Birthdays Today");

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

            //MessageBox.Show("Yay! You wished " + (BirthdaysToday.SelectedItem as FBFriend).Name);
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/Views/WishFriend.xaml?selectedItem=" + (BirthdaysList.SelectedItem as FB_Friend).Id, UriKind.RelativeOrAbsolute));

            // Reset selected item to null (no selection)
            BirthdaysList.SelectedItem = null;
        }
    }
}