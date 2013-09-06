using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using BirthdayBumper.ViewModels;
using BirthdayBumper.Models;
using Facebook;
using Facebook.Client;

namespace BirthdayBumper.Views
{
    public partial class Birthdays : PhoneApplicationPage
    {
        public Birthdays()
        {
            
            InitializeComponent();

        }

        

        private void getFriendsBirthdays()
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
                            var FBData = new FacebookDataModel();

                            foreach (var item in data)
                            {
                                var friend = (IDictionary<string, object>)item;
                                if (friend.ContainsKey("birthday"))
                                {
                                    string[] Birthdate = ((string)friend["birthday"]).Split(' ');
                                    
                                    FBData.Friends.Add(new FBFriend
                                    {
                                        Id = friend["uid"].ToString(),
                                        Name = (string)friend["name"],
                                        Day = Birthdate[1],
                                        Month = Birthdate[0],
                                        PictureUri = new Uri((string)friend["pic_square"], UriKind.RelativeOrAbsolute)
                                    });
                            
                                }

                            }

                            BirthdaysToday.DataContext = FBData.Friends;
                            Notify.Text = "Tap Friend To Wish Them";
                        });

                };

            string month = DateTime.Now.ToString("MMMM");
            int day = DateTime.Now.Day;

            fb.GetTaskAsync("fql", 
                new
                {
                    q = string.Format("SELECT uid, name, birthday, pic_square FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=me()) AND strpos(birthday, '{0} {1}') >= 0", month, day)
                });


            FacebookWebDialog dialog = new FacebookWebDialog();
            
        }


        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            getFriendsBirthdays();
        }


      
        // Handle selection changed on LongListSelector
        private void BirthdaysToday_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (BirthdaysToday.SelectedItem == null)
                return;

            MessageBox.Show("Yay! You wished " + (BirthdaysToday.SelectedItem as FBFriend).Name);
            // Navigate to the new page
            //NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + (BirthdaysToday.SelectedItem as FBFriend).Id, UriKind.Relative));

            // Reset selected item to null (no selection)
            BirthdaysToday.SelectedItem = null;
        }
      
    }
}